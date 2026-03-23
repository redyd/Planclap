package org.helmo.planclap_admin.infrastructures.scripts;

public enum MovieToPlanScript {

    /**
     * Get every movie to plan based on the timestamp of the beginning of a week (monday midnight).
     * <h4>Param</h4>
     * <ul>
     *     <li><code>week</code>: a timestamp</li>
     * </ul>
     */
    GET_ALL("""
            SELECT m.slug                    as `slug`,
                   m.title                   as `title`,
                   m.description             as `description`,
                   m.poster                  as `poster`,
                   m.duration                as `duration`,
                   mp.show_count             as `seances`,
                   SUBSTRING(mp.week, 1, 10) as 'week',
                   GROUP_CONCAT(mc.label)    as `cinechecks`
            FROM movie m
                     JOIN movie_to_plan mp ON m.movie_id = mp.movie_id AND mp.week = ?
                     JOIN movie_cinecheck mc ON m.movie_id = mc.movie_id
            GROUP BY mp.movie_id, mp.week
            """),

    /**
     * Insert a new movie.
     * <h4>Params</h4>
     * <ul>
     *     <li><code>slug</code></li>
     *     <li><code>title</code></li>
     *     <li><code>description</code></li>
     *     <li><code>poster</code></li>
     *     <li><code>duration</code></li>
     * </ul>
     */
    INSERT_MOVIE("""
            INSERT INTO movie (slug, title, description, poster, duration)
            VALUES (?, ?, ?, ?, ?);
            """),

    /**
     * Insert a new cinecheck for a movie.
     * <h4>Params</h4>
     * <ul>
     *     <li><code>slug</code>: the slug of the wanted movie</li>
     *     <li><code>label</code></li>
     * </ul>
     */
    INSERT_CINECHECKS("""
            INSERT INTO movie_cinecheck (movie_id, label)
            VALUES ((SELECT movie_id FROM movie WHERE slug = ?), ?);
            """),

    /**
     * Insert a new movie to plan.
     * <h4>Params</h4>
     * <ul>
     *     <li><code>slug</code>: the slug of the wanted movie</li>
     *     <li><code>week</code>: a timestamp</li>
     *     <li><code>show_count</code>: the amount</li>
     * </ul>
     */
    INSERT_MOVIE_TO_PLAN("""
            INSERT INTO movie_to_plan (movie_id, week, show_count)
            VALUES ((SELECT m.movie_id FROM movie m WHERE slug = ?), ?, ?);
            """),

    /**
     * Check if a movie is already planned based on its slug.
     * If true, return 1. Otherwise, return nothing.
     * <h4>Params</h4>
     * <ul>
     *     <li><code>week</code>: a timestamp</li>
     *     <li><code>slug</code>: the slug of the searched movie</li>
     * </ul>
     */
    DOES_SLUG_EXISTS("""
            SELECT 1
            FROM movie_to_plan
            WHERE week = ?
            AND movie_id = (
                SELECT
                movie_id
                FROM movie
                WHERE slug = ?
            )
            """),

    /**
     * Fetch only one movie based on its slug.
     * <h4>Params</h4>
     * <ul>
     *     <li><code>slug</code>: the slug of the searched movie</li>
     * </ul>
     */
    GET_ONLY_MOVIE("""
            SELECT m.slug                    as `slug`,
                   m.title                   as `title`,
                   m.description             as `description`,
                   m.poster                  as `poster`,
                   m.duration                as `duration`,
                   GROUP_CONCAT(mc.label)    as `cinechecks`
            FROM movie m
                     JOIN movie_cinecheck mc ON m.movie_id = mc.movie_id
            WHERE m.slug = ?
            GROUP BY m.slug
            """),

    /**
     * Check if a movie is already defined in the database.
     * <h4>Params</h4>
     * <ul>
     *     <li><code>slug</code>: the slug of the searched movie</li>
     * </ul>
     */
    DOES_MOVIE_IS_ALREADY_DEFINED("""
            SELECT 1
            FROM movie m
            WHERE m.slug = ?
            """);

    private final String sql;

    MovieToPlanScript(String sql) {
        this.sql = sql;
    }

    public String get() {
        return sql;
    }
}
