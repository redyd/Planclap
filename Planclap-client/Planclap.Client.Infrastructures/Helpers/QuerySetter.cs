using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Domains.Services;
using Planclap.Client.Infrastructures.Extentions;
using Planclap.Client.Infrastructures.IHelpers;

namespace Planclap.Client.Infrastructures.Helpers;

public class QuerySetter(ITimeService time) : IQuerySetter
{
    private const string FetchAllBySlugTemplate = """
                                                  SELECT m.slug, m.title, m.poster, m.description, :GROUP_CONCAT: as cinechecks
                                                  FROM movie_show ms
                                                  JOIN movie m ON ms.movie_id = m.movie_id
                                                  JOIN movie_cinecheck mc ON m.movie_id = mc.movie_id
                                                  WHERE m.slug IN (:SLUGS:)
                                                  AND ms.show_start >= @begin AND ms.show_start < @end
                                                  GROUP BY m.movie_id
                                                  """;

    private const string GetReservationsInfoByDate = """
                                                     SELECT m.slug,
                                                            m.duration,
                                                            ms.show_start,
                                                            :GROUP_CONCAT: as tickets
                                                     FROM movie_show ms
                                                              JOIN movie m ON m.movie_id = ms.movie_id
                                                              LEFT JOIN reservation r ON r.movie_id = ms.movie_id AND r.show_start = ms.show_start
                                                              LEFT JOIN reservation_seat rs ON r.reservation_id = rs.reservation_id
                                                     WHERE ms.show_start >= @begin AND ms.show_start < @end
                                                     GROUP BY m.slug, m.duration, ms.show_start
                                                     ORDER BY ms.show_start, m.slug;
                                                     """;

    private const string ReadReservationId = """
                                             SELECT r.reservation_id as id
                                             FROM reservation r
                                             WHERE r.show_start = @show_start
                                               AND r.movie_id = (SELECT m.movie_id
                                                                FROM movie m
                                                                WHERE m.slug = @slug)
                                             """;

    private const string InsertNewReservation = """
                                                INSERT INTO reservation (movie_id, show_start)
                                                SELECT ms.movie_id, ms.show_start
                                                FROM movie_show ms
                                                WHERE ms.movie_id = (SELECT m.movie_id FROM movie m WHERE m.slug = @slug)
                                                  AND ms.show_start = @show_start
                                                """;

    private const string InsertNewReservationSeat = """
                                                    INSERT INTO reservation_seat (reservation_id, seat_id, customer_age)
                                                    VALUES (@reservation_id, @seat_id, @customer_age)
                                                    """;

    public ISqlWrapper ExecuteFetchBySlugQuery(ISqlWrapper db, ISet<MovieSlug> slugs)
    {
        var slugParams = Enumerable.Range(0, slugs.Count).Select(i => $"@slug{i}");

        var template = FetchAllBySlugTemplate
            .Replace(":GROUP_CONCAT:", db.QueryProvider.GroupConcat("mc.label", distinct: true))
            .Replace(":SLUGS:", string.Join(",", slugParams));

        var readQuery = db.NewRead(template)
            .WithParam("@begin", time.StartOfDay.ToTimeStamp())
            .WithParam("@end", time.EndOfDay.ToTimeStamp());

        var slugList = slugs.ToList();
        for (var i = 0; i < slugList.Count; i++)
        {
            readQuery.WithParam($"@slug{i}", slugList[i].ToString());
        }

        return db;
    }

    public ISqlWrapper ExecuteFetchAllForTodayQuery(ISqlWrapper db)
        => db
            .NewRead(GetReservationsInfoByDate.Replace(":GROUP_CONCAT:",
                db.QueryProvider.GroupConcat(
                    db.QueryProvider.Concat("rs.seat_id", "'='", "rs.customer_age"),
                    '|')))
            .WithParam("@begin", time.StartOfDay.ToTimeStamp())
            .WithParam("@end", time.EndOfDay.ToTimeStamp());

    public ISqlWrapper ExecuteUpdateReservation(ISqlWrapper db, IReservation reservation)
    {
        var titleSlug = MovieSlug.Slugify(reservation.Title.Value);
        var id = db
            .NewRead(ReadReservationId)
            .WithParam("@show_start", reservation.StartTime.ToTimeStamp())
            .WithParam("@slug", titleSlug)
            .ExecuteOneQuery(m => (long?)Convert.ToInt64(m["id"]));

        // if no, create it
        if (id is null)
        {
            db
                .NewWrite(InsertNewReservation, true)
                .WithParam("@slug", titleSlug)
                .WithParam("@show_start", reservation.StartTime.ToTimeStamp())
                .Execute();

            id = db
                .NewRead(db.QueryProvider.LastInsertIdQuery)
                .ExecuteOneQuery(m => (long?)Convert.ToInt64(m[0]));
        }

        // insert each tickets
        foreach (var ticket in reservation.Tickets)
        {
            db
                .NewWrite(InsertNewReservationSeat)
                .WithParam("@reservation_id", id)
                .WithParam("@seat_id", ticket.Seat.ToId)
                .WithParam("@customer_age", ticket.Age)
                .Execute();
        }

        return db;
    }
}
