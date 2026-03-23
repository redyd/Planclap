package org.helmo.planclap_admin.domains.core;

import java.util.*;

/**
 * Classe utilitaire pour la recherche intelligente de slugs
 */
public class SlugSearcher {

    private final static int SEARCH_TOLERANCE = 3;

    private final Set<MovieSessions> movieSessions;
    private String slug;

    public SlugSearcher() {
        this.movieSessions = new HashSet<>();
    }

    public void AddMovieSessions(MovieSessions movieSessions) {
        this.movieSessions.add(movieSessions);
    }

    public void setSlug(Name slug) {
        this.slug = slug.toString();
    }

    /**
     * Effectue une recherche intelligente grâce à la distance de Levenshtein.
     * Retourne le film le plus proche du slug donné en paramètre, si la distance
     * est inférieure ou égale à la tolérance définie.
     *
     * @param slug Le slug à chercher
     * @return le {@code movieSessions} le plus proche du slug, sinon rien
     */
    public Optional<MovieSessions> search(Name slug) {
        setSlug(slug);

        Map<Integer, List<MovieSessions>> list = new TreeMap<>();

        for (MovieSessions movieSession : movieSessions) {
            int distance = distance(movieSession.movie().getSlug().toString());
            if (!(distance > SEARCH_TOLERANCE)) {
                addInLinkedListMap(list, movieSession, distance);
            }
        }

        List<MovieSessions> ordered = flatList(list);
        return getClosest(ordered);
    }

    private Optional<MovieSessions> getClosest(List<MovieSessions> list) {
        if (!list.isEmpty()) {
            return Optional.of(list.getFirst());
        }
        return Optional.empty();
    }

    private List<MovieSessions> flatList(Map<Integer, List<MovieSessions>> list) {
        List<MovieSessions> flat = new LinkedList<>();

        for (Map.Entry<Integer, List<MovieSessions>> entry : list.entrySet()) {
            flat.addAll(entry.getValue());
        }

        return flat;
    }

    private void addInLinkedListMap(Map<Integer, List<MovieSessions>> map, MovieSessions movieSession, int distance) {
        if (!map.containsKey(distance)) {
            map.put(distance, new LinkedList<>());
        }
        map.get(distance).add(movieSession);
    }

    private int distance(String other) {
        int[][] D = new int[slug.length() + 1][other.length() + 1];
        int i;
        int j;
        int cost;

        for (i = 0; i <= slug.length(); i++) {
            D[i][0] = i;
        }
        for (j = 0; j <= other.length(); j++) {
            D[0][j] = j;
        }

        for (i = 1; i <= slug.length(); i++) {
            for (j = 1; j <= other.length(); j++) {
                cost = slug.charAt(i - 1) == other.charAt(j - 1) ? 0 : 1;
                D[i][j] = Math.min(
                        Math.min(
                                D[i - 1][j] + 1,
                                D[i][j - 1] + 1
                        ),
                        D[i - 1][j - 1] + cost
                );
            }
        }

        return D[slug.length()][other.length()];
    }
}
