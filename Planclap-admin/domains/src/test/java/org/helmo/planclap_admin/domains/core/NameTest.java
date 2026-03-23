package org.helmo.planclap_admin.domains.core;

import org.helmo.planclap_admin.domains.core.Name;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

class NameTest {

    @Nested
    @DisplayName("Happy Path")
    class HappyPath {

        @Test
        void should_create_name_when_given_valid_string() {
            Name name = Name.of("ValidName");
            assertEquals("ValidName", name.toString());
        }

        @Test
        void should_return_minimal_name_when_given_uppercase_and_spaces() {
            Name name = Name.of("Mon Film Génial!");
            Name minimal = name.toMinimal();
            assertEquals("mon-film-genial", minimal.toString());
        }

        @Test
        void should_return_minimal_name_when_given_accents_and_punctuation() {
            Name name = Name.of("Été à Paris, C'est cool!");
            Name minimal = name.toMinimal();
            assertEquals("ete-a-paris-c-est-cool", minimal.toString());
        }

        @Test
        void should_return_same_value_for_equals_when_given_same_name() {
            Name n1 = Name.of("SameName");
            Name n2 = Name.of("SameName");
            assertEquals(n1, n2);
            assertEquals(n1.hashCode(), n2.hashCode());
        }

        @Test
        void should_return_string_value_when_toString_called() {
            Name n = Name.of("MonNom");
            assertEquals("MonNom", n.toString());
        }

        @Test
        void should_not_be_equals_when_null_or_obj_given() {
            Name name = Name.of("ValidName");

            assertNotEquals(name, null);
            assertNotEquals(name, "");
        }
    }

    @Nested
    @DisplayName("Error Cases")
    class ErrorCases {

        @Test
        void should_throw_exception_when_given_null_string() {
            assertThrows(IllegalArgumentException.class, () -> Name.of(null));
        }

        @Test
        void should_throw_exception_when_given_empty_string() {
            assertThrows(IllegalArgumentException.class, () -> Name.of(""));
        }

        @Test
        void should_not_equal_when_given_different_names() {
            Name n1 = Name.of("Name1");
            Name n2 = Name.of("Name2");
            assertNotEquals(n1, n2);
        }

        @Test
        void should_not_equal_when_given_null_or_different_class() {
            Name n = Name.of("Name");
            assertNotEquals(null, n);
            assertNotEquals("string", n);
        }
    }
}
