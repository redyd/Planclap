package org.helmo.planclap_admin.domains.services;

import org.helmo.planclap_admin.domains.iservices.MovieEventSubscriber;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import static org.mockito.Mockito.*;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
class ServicesNotifierTest {

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_notify_all_subscribers_when_notifySubscribers_called() {
            var notifier = new ServicesNotifier();
            var sub1 = mock(MovieEventSubscriber.class);
            var sub2 = mock(MovieEventSubscriber.class);

            notifier.subscribe(sub1);
            notifier.subscribe(sub2);

            notifier.notifySubscribers();

            verify(sub1, times(1)).onMovieAdded();
            verify(sub2, times(1)).onMovieAdded();
        }

        @Test
        void should_handle_duplicate_subscribers_gracefully() {
            var notifier = new ServicesNotifier();
            var sub = mock(MovieEventSubscriber.class);

            notifier.subscribe(sub);
            notifier.subscribe(sub); // doublon

            notifier.notifySubscribers();

            verify(sub, times(1)).onMovieAdded(); // appelé une seule fois
        }

        @Test
        void should_do_nothing_when_no_subscribers() {
            var notifier = new ServicesNotifier();
            // Aucun abonné : ne doit rien faire
            notifier.notifySubscribers();
        }

        @Test
        void should_add_subscriber_when_subscribe_called() {
            var notifier = new ServicesNotifier();
            var sub = mock(MovieEventSubscriber.class);

            notifier.subscribe(sub);
            notifier.notifySubscribers();

            verify(sub, times(1)).onMovieAdded();
        }
    }

    @Nested
    @DisplayName("Edge cases")
    class EdgeCases {

        @Test
        void should_not_throw_when_subscriber_is_null() {
            var notifier = new ServicesNotifier();

            notifier.subscribe(null);
            notifier.notifySubscribers();
        }
    }
}
