package org.helmo.planclap_admin.domains.core;

/**
 * Classe représentant un âge CineCheck <br/>
 * Voir: {@link <a href="https://www.cinecheck.be/">www.cinecheck.be</a>}
 */
public enum CineCheckAge {
    AL(0), SIX(6), NINE(9), TWELVE(12), FOURTEEN(14), SIXTEEN(16), EIGHTEEN(18);

    private final int age;

    CineCheckAge(int age) {
        this.age = age;
    }

    public int getAge() {
        return age;
    }

    /**
     * Retranscrit l'âge en String (ou AL pour tout public).
     *
     * @return la représentation de l'âge
     */
    public String toLiteralString() {
        return this == AL ? "AL" : String.valueOf(age);
    }

    /**
     * Définis l'égalité entre un string et le CineCheck
     *
     * @param value valeur à vérifier
     * @return {@code true} si égal, sinon {@code false}
     */
    public boolean equalsValue(String value) {
        if (value == null || value.isEmpty()) {
            return false;
        }
        return value.equalsIgnoreCase(this.toString()) || value.equalsIgnoreCase(toLiteralString());
    }
}
