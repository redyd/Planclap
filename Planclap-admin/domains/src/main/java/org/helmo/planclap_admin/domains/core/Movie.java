package org.helmo.planclap_admin.domains.core;

import java.util.Objects;

/**
 * Classe de film
 */
public class Movie {

    private final Name slug;
    private final Name title;
    private final MovieDescription description;
    private final MovieDuration duration;
    private final PosterURI poster;
    private final CineCheckGroup cineChecks;

    public Movie(Name title,
                 MovieDescription description,
                 MovieDuration duration,
                 PosterURI poster,
                 CineCheckGroup cineChecks) {

        this.slug = Objects.requireNonNull(title).toMinimal();
        this.title = Objects.requireNonNull(title);
        this.description = Objects.requireNonNull(description);
        this.duration = Objects.requireNonNull(duration);
        this.poster = Objects.requireNonNull(poster);
        this.cineChecks = Objects.requireNonNull(cineChecks);
    }

    public Name getSlug() {
        return slug;
    }

    public Name getTitle() {
        return title;
    }

    public MovieDescription getDescription() {
        return description;
    }

    public MovieDuration getDuration() {
        return duration;
    }

    public PosterURI getPoster() {
        return poster;
    }

    public CineCheckGroup getCineChecksGroup() {
        return cineChecks;
    }

    /**
     * Indique si le film est destiné aux enfants.
     *
     * @return {@code true} si le film est pour enfants, {@code false} sinon
     */
    public boolean isForChildren() {
        return cineChecks.isForChildren();
    }

    @Override
    public boolean equals(Object o) {
        if (o == null || getClass() != o.getClass()) {
            return false;
        }
        Movie movie = (Movie) o;
        return Objects.equals(slug, movie.slug)
                && Objects.equals(title, movie.title)
                && Objects.equals(poster, movie.poster)
                && Objects.equals(duration, movie.duration)
                && Objects.equals(cineChecks, movie.cineChecks)
                && Objects.equals(description, movie.description);
    }

    @Override
    public int hashCode() {
        return Objects.hash(slug, title, description, duration, poster, cineChecks);
    }
}
