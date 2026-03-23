package org.helmo.planclap_admin.domains.core;

public interface PlanningGenerator {

    /**
     * Génère une planification de films pour une date donnée.
     *
     * @param plan Les films à planifier
     * @return La planification générée
     */
    Planning generate(MoviesToPlan plan);
}
