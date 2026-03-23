package org.helmo.planclap_admin.domains.services;

import org.helmo.planclap_admin.domains.iservices.*;

import java.util.*;

public class ServicesNotifier implements PublishMovieEvents {

    private final Set<MovieEventSubscriber> subscribers;

    public ServicesNotifier() {
        this.subscribers = new HashSet<>();
    }

    @Override
    public void subscribe(MovieEventSubscriber subscriber) {
        subscribers.add(subscriber);
    }

    @Override
    public void notifySubscribers() {
        for (final MovieEventSubscriber subscriber : subscribers) {
            if (subscriber != null) {
                subscriber.onMovieAdded();
            }
        }
    }

}
