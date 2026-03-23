package org.helmo.planclap_admin.domains.core;

import java.net.URI;
import java.util.Objects;

/**
 * Représente l'URL d'une affiche de film
 */
public class PosterURI {

    private final URI uri;

    private PosterURI(URI uri) {
        this.uri = uri;
    }

    /**
     * Crée un nouveau poster.
     *
     * @param posterURI l'URL du poster
     * @return un nouveau poster
     * @throws IllegalArgumentException si l'URL est nulle
     */
    public static PosterURI of(String posterURI) {
        if (posterURI == null || posterURI.isBlank()) {
            throw new IllegalArgumentException("L'URL de l'affiche ne peut pas etre nulle.");
        }

        return new PosterURI(URI.create(posterURI));
    }

    /**
     * Retourne l'URL de l'affiche sous forme de chaîne de caractères.
     *
     * @return L'URL de l'affiche
     */
    public URI url() {
        return uri;
    }

    @Override
    public boolean equals(Object o) {
        if (o == null || getClass() != o.getClass()) {
            return false;
        }
        PosterURI posterURI1 = (PosterURI) o;
        return url().equals(posterURI1.url());
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(uri);
    }
}
