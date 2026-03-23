package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import java.util.Collections;
import java.util.HashSet;
import java.util.Set;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.mock;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
class CineCheckGroupTest {

    @Nested
    class HappyPath {

        @Test
        void should_return_age_when_getAge_called() {
            CineCheckGroup group = CineCheckGroup.of(CineCheckAge.SIX, Collections.emptyList());
            assertEquals(CineCheckAge.SIX, group.getAge());
        }

        @Test
        void should_return_cinechecks_when_getCineCheck_called() {
            CineCheck check = mock(CineCheck.class);
            Set<CineCheck> checks = new HashSet<>();
            checks.add(check);

            CineCheckGroup group = CineCheckGroup.of(CineCheckAge.NINE, checks);

            Set<CineCheck> returned = group.getCineCheck();
            assertEquals(1, returned.size());
            assertTrue(returned.contains(check));

            // Vérifier que la collection retournée est immuable
            assertThrows(UnsupportedOperationException.class, () -> returned.add(mock(CineCheck.class)));
        }

        @Test
        void should_return_true_for_isForChildren_when_age_is_12_or_less() {
            assertTrue(CineCheckGroup.of(CineCheckAge.AL, Collections.emptyList()).isForChildren());
            assertTrue(CineCheckGroup.of(CineCheckAge.SIX, Collections.emptyList()).isForChildren());
            assertTrue(CineCheckGroup.of(CineCheckAge.NINE, Collections.emptyList()).isForChildren());
            assertTrue(CineCheckGroup.of(CineCheckAge.TWELVE, Collections.emptyList()).isForChildren());
        }

        @Test
        void should_return_false_for_isForChildren_when_age_greater_than_12() {
            assertFalse(CineCheckGroup.of(CineCheckAge.FOURTEEN, Collections.emptyList()).isForChildren());
            assertFalse(CineCheckGroup.of(CineCheckAge.EIGHTEEN, Collections.emptyList()).isForChildren());
        }

        @Test
        void should_respect_equals_and_hashCode_contract() {
            CineCheck check = mock(CineCheck.class);
            CineCheckGroup g1 = CineCheckGroup.of(CineCheckAge.NINE, Collections.singleton(check));
            CineCheckGroup g2 = CineCheckGroup.of(CineCheckAge.NINE, Collections.singleton(check));
            CineCheckGroup g3 = CineCheckGroup.of(CineCheckAge.SIX, Collections.emptySet());

            assertEquals(g1, g2);
            assertEquals(g1.hashCode(), g2.hashCode());
            assertNotEquals(g1, g3);
            assertNotEquals(null, g1);
        }

        @Test
        void should_not_be_equals_when_age_equals_but_not_checks() {
            CineCheckGroup g1 = CineCheckGroup.of(CineCheckAge.NINE, Set.of(CineCheck.RUDE));
            CineCheckGroup g2 = CineCheckGroup.of(CineCheckAge.NINE, Set.of(CineCheck.VIOLENCE));

            assertNotEquals(g1, g2);
        }

        @Test
        void should_not_be_equals_when_null_or_obj_given() {
            CineCheckGroup g = CineCheckGroup.of(CineCheckAge.NINE, Set.of(CineCheck.RUDE));

            assertNotEquals(g, null);
            assertNotEquals(g, new Object());
        }
    }

    @Nested
    class ErrorCases {

        @Test
        void should_create_group_with_empty_checks() {
            CineCheckGroup group = CineCheckGroup.of(CineCheckAge.AL, Collections.emptyList());
            assertTrue(group.getCineCheck().isEmpty());
        }
    }
}