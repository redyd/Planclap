package org.helmo.planclap_admin.infrastructures.scripts;

public enum PlanningScript {

    /**
     * Clear every planned movie for a specific range date
     * <h4>Param</h4>
     * <ul>
     *     <li><code>start</code></li>
     *     <li><code>end</code></li>
     * </ul>
     */
    DELETE_BETWEEN("""
            DELETE FROM movie_show
            WHERE show_start BETWEEN ? AND ?
            """),

    /**
     * Insert a new planned movie.
     * Should be used after doing {@code DELETE_BETWEEN}
     * <h4>Param</h4>
     * <ul>
     *     <li><code>slug</code></li>
     *     <li><code>show_start</code></li>
     * </ul>
     */
    INSERT_NEW("""
            INSERT INTO movie_show (movie_id, show_start)
            VALUES ((SELECT movie_id FROM movie WHERE slug = ?), ?)
            """);

    private final String sql;

    PlanningScript(String sql) {
        this.sql = sql;
    }

    public String get() {
        return sql;
    }

}
