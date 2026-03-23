package org.helmo.planclap_admin.domains.core;

import java.util.Locale;
import java.util.Objects;

/**
 * Représente un nom avec des fonctionnalités de normalisation.
 */
public final class Name {
    private final String value;

    private Name(final String name) {
        this.value = name;
    }

    /**
     * Convertit le nom en une version minimale normalisée (slug).
     *
     * @return Le nom minimal.
     */
    public Name toMinimal() {
        String minimal = StripUtils.stripAccents(this.value)
                .toLowerCase(Locale.ROOT)
                .replaceAll("\\s|\\p{Punct}|\\W", "-")
                .replaceAll("-{2,}", "-")
                .replaceAll("^-|-$", "");

        return new Name(minimal);
    }

    /**
     * Crée une instance de Name après validation.
     *
     * @param name Le nom
     * @return L'instance de Name.
     * @throws IllegalArgumentException si le Name est invalide
     */
    public static Name of(final String name) {
        if (name == null || name.isEmpty()) {
            throw new IllegalArgumentException("Un nom ne peut pas être nul ou vide.");
        }

        return new Name(name);
    }

    @Override
    public boolean equals(Object o) {
        if (o == null || getClass() != o.getClass()) {
            return false;
        }
        Name name1 = (Name) o;
        return value.equals(name1.value);
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(value);
    }

    @Override
    public String toString() {
        return this.value;
    }
}
