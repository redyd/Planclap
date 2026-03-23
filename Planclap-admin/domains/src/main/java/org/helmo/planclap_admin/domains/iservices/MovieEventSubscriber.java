package org.helmo.planclap_admin.domains.iservices;

public interface MovieEventSubscriber {

    /**
     * Méthode de réception de la notification
     */
    void onMovieAdded();
}
