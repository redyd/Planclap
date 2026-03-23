package org.helmo.planclap_admin.domains.core;

/**
 * Classe représentant un symbole CineCheck.
 * Voir: {@link <a href="https://www.cinecheck.be/">www.cinecheck.be</a>}
 */
public enum CineCheck {
    VIOLENCE("Violence"), FEAR("Peur"), SEX("Sexe"), RUDE("Paroles grossieres"), DISCRIMINATION("Discrimination"), DRUGS("Drogues, alcool et fumer");

    private final String description;

    CineCheck(String description) {
        this.description = description;
    }

    public String getDescription() {
        return description;
    }

    /**
     * Définis l'égalité avec un String.
     *
     * @param value valeur à vérifier
     * @return {@code true} si la valeur est égale au nom ou à la description du CineCheck
     */
    public boolean equalsValue(String value) {
        if (value == null || value.isEmpty()) {
            return false;
        }
        return value.equalsIgnoreCase(this.toString()) || value.equalsIgnoreCase(this.getDescription());
    }
}
