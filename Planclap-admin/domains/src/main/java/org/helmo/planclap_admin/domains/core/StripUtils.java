package org.helmo.planclap_admin.domains.core;

import java.text.Normalizer;
import java.util.regex.Pattern;

/**
 * Classe utilitaire pour supprimer les accents des chaînes de caractères.
 */
public final class StripUtils {

    private static final Pattern MARKS = Pattern.compile("\\p{M}");

    private StripUtils() {}

    /**
     * Enlève les accents d'un string.
     *
     * @param input le string à modifier
     * @return le string sans accent
     */
    public static String stripAccents(String input) {
        if (input == null || input.isEmpty()) {
            return "";
        }
        String normalized = Normalizer.normalize(input, Normalizer.Form.NFD);
        return MARKS.matcher(normalized).replaceAll("");
    }
}
