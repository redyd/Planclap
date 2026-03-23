package org.helmo.planclap_admin.infrastructures.implementations;

import org.helmo.planclap_admin.domains.annotations.Log;
import org.helmo.planclap_admin.domains.core.*;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.exceptions.SqlWrapperException;
import org.helmo.planclap_admin.domains.repository.MoviesToPlanRepository;
import org.helmo.planclap_admin.infrastructures.dto.MovieDto;
import org.helmo.planclap_admin.infrastructures.dto.MovieSessionsDto;
import org.helmo.planclap_admin.infrastructures.dto.MoviesToPlanDto;
import org.helmo.planclap_admin.infrastructures.helper.SqlWrapper;
import org.helmo.planclap_admin.infrastructures.logged_operations.LoggedDbOperations;
import org.helmo.planclap_admin.infrastructures.mapper.MovieMapper;
import org.helmo.planclap_admin.infrastructures.mapper.MoviesToPlanMapper;
import org.helmo.planclap_admin.infrastructures.scripts.MovieToPlanScript;

import java.time.LocalDate;
import java.time.LocalTime;
import java.time.ZoneOffset;
import java.util.Optional;
import java.util.Properties;
import java.util.Set;

public class SqlMoviesToPlanRepository implements MoviesToPlanRepository {

    private final String url;
    private final Properties properties;

    public SqlMoviesToPlanRepository(String url, Properties properties) {
        this.url = url;
        this.properties = properties;
    }

    @Override
    @Log
    public MoviesToPlan getMoviesToPlan(LocalDate date) throws RepositoryException {
        long convertedDate = getConvertedDate(date);

        try (var wrapper = LoggedDbOperations.withAutoCommit(url, properties)) {
            var moviesToPlanDto = wrapper
                    .newRead(MovieToPlanScript.GET_ALL.get())
                    .withParam(1, convertedDate)
                    .executeSelect(rs -> new MovieSessionsDto(
                            rs.getString("slug"),
                            rs.getString("title"),
                            rs.getInt("duration"),
                            rs.getString("poster"),
                            rs.getString("description"),
                            rs.getString("cinechecks").split(","),
                            rs.getInt("seances")
                    ));

            var dto = new MoviesToPlanDto(Set.copyOf(moviesToPlanDto));

            return MoviesToPlanMapper.mapDTO(date, dto);
        } catch (SqlWrapperException e) {
            throw new RepositoryException("Error while fetching data");
        } catch (Exception e) {
            throw new RepositoryException("Try-with-resources exception during getMoviesToPlan");
        }
    }

    @Override
    @Log
    public void addMovieToPlan(MovieSessions movieSessions, LocalDate date) throws RepositoryException {
        try (var wrapper = LoggedDbOperations.withTransaction(url, properties)) {
            var slug = movieSessions.movie().getSlug().toString();
            var cinechecks = movieSessions.movie().getCineChecksGroup();

            boolean doesExits = wrapper
                    .newRead(MovieToPlanScript.DOES_MOVIE_IS_ALREADY_DEFINED.get())
                    .withParam(1, slug)
                    .executeExists();

            addIfDoesNotExist(movieSessions, doesExits, wrapper, slug, cinechecks);

            wrapper
                    .newWrite(MovieToPlanScript.INSERT_MOVIE_TO_PLAN.get())
                    .withParam(1, slug)
                    .withParam(2, getConvertedDate(date))
                    .withParam(3, movieSessions.sessionsAmount().toInt())
                    .execute();

            wrapper.commit();
        } catch (SqlWrapperException e) {
            throw new RepositoryException("Error while opening resource");
        } catch (Exception e) {
            throw new RepositoryException("Try-with-resources exception during adding a movie to plan");
        }
    }

    private static void addIfDoesNotExist(MovieSessions movieSessions, boolean doesExits, SqlWrapper wrapper, String slug, CineCheckGroup cinechecks) {
        if (!doesExits) {
            //addNewMovie(movieSessions, wrapper, slug, cinechecks);
            wrapper
                    .newWrite(MovieToPlanScript.INSERT_MOVIE.get(), true)
                    .withParam(1, slug)
                    .withParam(2, movieSessions.movie().getTitle().toString())
                    .withParam(3, movieSessions.movie().getDescription().toString())
                    .withParam(4, movieSessions.movie().getPoster().url().toString())
                    .withParam(5, movieSessions.movie().getDuration().get())
                    .execute();

            wrapper
                    .newWrite(MovieToPlanScript.INSERT_CINECHECKS.get())
                    .withParam(1, slug)
                    .withParam(2, cinechecks.getAge().toLiteralString())
                    .execute();

            for (var checks : cinechecks.getCineCheck()) {
                wrapper
                        .newWrite(MovieToPlanScript.INSERT_CINECHECKS.get())
                        .withParam(1, slug)
                        .withParam(2, checks.toString())
                        .execute();
            }
        }
    }

    @Override
    @Log
    public boolean isAlreadyPlanned(Name slug, LocalDate date) throws RepositoryException {
        long convertedDate = getConvertedDate(date);
        try (var wrapper = LoggedDbOperations.withAutoCommit(url, properties)) {
            return wrapper
                    .newRead(MovieToPlanScript.DOES_SLUG_EXISTS.get())
                    .withParam(1, convertedDate)
                    .withParam(2, slug.toString())
                    .executeExists();
        } catch (SqlWrapperException e) {
            throw new RepositoryException("Error while checking if this movie is already planned");
        } catch (Exception e) {
            throw new RepositoryException("Try-with-resources exception while checking if a movie is already planned");
        }
    }

    @Override
    @Log
    public Optional<Movie> getOnlyMovie(Name slug) throws RepositoryException {
        try (var wrapper = LoggedDbOperations.withAutoCommit(url, properties)) {
            var result = wrapper
                    .newRead(MovieToPlanScript.GET_ONLY_MOVIE.get())
                    .withParam(1, slug.toString())
                    .executeSelectOne(rs -> new MovieDto(
                            rs.getString("title"),
                            rs.getString("description"),
                            rs.getInt("duration"),
                            rs.getString("poster"),
                            rs.getString("cinechecks").split(",")));

            return result.map(MovieMapper::mapDto);
        } catch (SqlWrapperException e) {
            throw new RepositoryException("Error while fetching data: " + e.getMessage());
        } catch (Exception e) {
            throw new RepositoryException("Try-with-resources exception while getting a single movie");
        }
    }

    private static long getConvertedDate(LocalDate date) {
        return date.toEpochSecond(LocalTime.MIDNIGHT, ZoneOffset.UTC);
    }
}
