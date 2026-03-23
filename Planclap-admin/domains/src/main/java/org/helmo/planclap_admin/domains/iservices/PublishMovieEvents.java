package org.helmo.planclap_admin.domains.iservices;

public interface PublishMovieEvents {

    /**
     * Permet à un {@code MovieEventSubscriber} de s'abonner
     * @param subscriber le demandeur d'abonnement
     */
    void subscribe(MovieEventSubscriber subscriber);

    /**
     * Notifie tous les abonnés
     */
    void notifySubscribers();
}
