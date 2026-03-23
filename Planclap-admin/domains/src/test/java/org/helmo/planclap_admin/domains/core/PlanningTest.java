package org.helmo.planclap_admin.domains.core;

import org.junit.jupiter.api.*;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.List;

import static org.helmo.planclap_admin.domains.providers.TestProvider.movie;
import static org.helmo.planclap_admin.domains.providers.TestProvider.movies;
import static org.junit.jupiter.api.Assertions.*;

/**
 * Utilisation minim de l'IA suite à refractor
 */
class PlanningTest {

    private final LocalDate date = LocalDate.of(2025, 10, 20); // un lundi
    private Planning planning;

    @BeforeEach
    void setUp() {
        var manager = new PlanningTimeManager(date);
        planning = new Planning(manager);
    }

    private void addMovie(int duration, boolean forChildren, WeekDay day) {
        planning.add(day, movie(duration, forChildren));
    }

    private void addMovies(int size) {
        int i = 0;
        for (var movie : movies(size)) {
            planning.add(WeekDay.values()[i % 7], movie);
            i++;
        }
    }

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_be_equals() {
            Planning planning1 = new Planning(new PlanningTimeManager(date));
            Planning planning2 = new Planning(new PlanningTimeManager(date));

            var movie = movie();

            planning1.add(WeekDay.MONDAY, movie);
            planning2.add(WeekDay.MONDAY, movie);

            assertEquals(planning1, planning2);
        }

        @Test
        void should_have_same_hashcode_when_same_planning() {
            var plan1 =  new Planning(new PlanningTimeManager(date));

            plan1.add(WeekDay.SUNDAY, movie());
            plan1.add(WeekDay.MONDAY, movie());
            plan1.add(WeekDay.WEDNESDAY, movie());

            var plan2 = plan1.copyOf();

            assertEquals(plan1.hashCode(), plan2.hashCode());
        }

        @Test
        void should_not_be_equals_when_different_movie() {
            Planning planning1 = new Planning(new PlanningTimeManager(date));
            Planning planning2 = new Planning(new PlanningTimeManager(date));

            planning1.add(WeekDay.MONDAY, movie());
            planning2.add(WeekDay.MONDAY, movie());

            assertNotEquals(planning1, planning2);
        }

        @Test
        void should_not_be_equals_when_different_movie_and_day() {
            Planning planning1 = new Planning(new PlanningTimeManager(date));
            Planning planning2 = new Planning(new PlanningTimeManager(date));

            planning1.add(WeekDay.MONDAY, movie());
            planning2.add(WeekDay.THURSDAY, movie());

            assertNotEquals(planning1, planning2);
        }

        @Test
        void should_init_with_every_day() {
            for (WeekDay day : WeekDay.values()) {
                assertEquals(List.of(), planning.getAt(day));
            }
            assertEquals(0, planning.size());
        }

        @Test
        void should_add_planned_if_planning_is_empty() {
            var movie = movie(90, false); // 12:00 -> 13:30
            assertTrue(planning.add(WeekDay.MONDAY, movie));

            var monday = planning.getAt(WeekDay.MONDAY);
            assertEquals(1, monday.size());
            assertEquals(LocalDateTime.of(date, LocalTime.of(12, 0)), monday.getFirst().getStartTime());
            assertEquals(LocalDateTime.of(date, LocalTime.of(13, 30)), monday.getFirst().getEndTime());
        }

        @Test
        void should_add_at_next_quarter_when_time_overflows_perfect_quarter() {
            addMovie(94, true, WeekDay.MONDAY); // 12:00 -> 13:34
            addMovie(60, true, WeekDay.MONDAY); // 13:45 -> 14:45

            var monday = planning.getAt(WeekDay.MONDAY);

            assertEquals(LocalDateTime.of(date, LocalTime.of(13, 34)), monday.getFirst().getEndTime());
            assertEquals(LocalDateTime.of(date, LocalTime.of(13, 45)), monday.getLast().getStartTime());
        }

        @Test
        void should_add_at_next_quarter_when_time_hits_perfect_quarter() {
            addMovie(90, true, WeekDay.MONDAY); // 12:00 -> 13:30
            addMovie(60, true, WeekDay.MONDAY); // 13:45 -> 14:45

            var monday = planning.getAt(WeekDay.MONDAY);

            assertEquals(LocalDateTime.of(date, LocalTime.of(13, 30)), monday.getFirst().getEndTime());
            assertEquals(LocalDateTime.of(date, LocalTime.of(13, 45)), monday.getLast().getStartTime());
        }

        @Test
        void should_permute_two_movies_when_same_day_but_different_schedule() {
            addMovie(90, true, WeekDay.MONDAY); // 12:00 -> 13:30
            addMovie(60, false, WeekDay.MONDAY); // 13:45 -> 14:45

            assertTrue(planning.permute(WeekDay.MONDAY, 0, WeekDay.MONDAY, 1));
            var monday = planning.getAt(WeekDay.MONDAY);

            assertEquals(60, monday.getFirst().getDuration());
            assertEquals(90, monday.getLast().getDuration());
        }

        @Test
        void should_permute_two_movies_when_different_day_and_differents_schedule() {
            addMovies(26);

            PlannedMovie p1 = planning.getAt(WeekDay.MONDAY).get(3);
            PlannedMovie p2 = planning.getAt(WeekDay.FRIDAY).get(1);

            long duration1 = p1.getDuration();
            long duration2 = p2.getDuration();

            assertTrue(planning.permute(WeekDay.MONDAY, 3, WeekDay.FRIDAY, 1));

            var monday = planning.getAt(WeekDay.MONDAY);
            var friday = planning.getAt(WeekDay.FRIDAY);

            assertEquals(duration1, friday.get(1).getDuration());
            assertEquals(duration2, monday.get(3).getDuration());
        }

        @Test
        void should_give_positive_score_when_schedule_is_coherent() {
            addMovie(90, true, WeekDay.MONDAY);
            addMovie(120, false, WeekDay.MONDAY);

            var score = planning.score();
            assertTrue(score > 0);
        }

        @Test
        void should_give_zero_score_when_no_movies() {
            var score = planning.score();
            assertEquals(0, score);
        }

        @Test
        void should_create_deep_copy_with_same_content() {
            addMovie(90, false, WeekDay.MONDAY);
            addMovie(120, true, WeekDay.TUESDAY);

            Planning copy = planning.copyOf();

            assertEquals(planning.size(), copy.size());

            for (WeekDay day : WeekDay.values()) {
                assertEquals(planning.getAt(day).size(), copy.getAt(day).size());
            }
        }

        @Test
        void should_not_reference_same_objects_in_copy() {
            addMovie(90, false, WeekDay.MONDAY);
            addMovie(120, true, WeekDay.MONDAY);

            Planning copy = planning.copyOf();

            var originalList = planning.getAt(WeekDay.MONDAY);
            var copiedList = copy.getAt(WeekDay.MONDAY);

            for (int i = 0; i < originalList.size(); i++) {
                assertNotSame(originalList.get(i), copiedList.get(i));
                assertEquals(originalList.get(i).getSlug(), copiedList.get(i).getSlug());
                assertEquals(originalList.get(i).getStartTime(), copiedList.get(i).getStartTime());
                assertEquals(originalList.get(i).getEndTime(), copiedList.get(i).getEndTime());
            }
        }

        @Test
        void should_not_affect_original_when_modifying_copy() {
            addMovie(90, false, WeekDay.MONDAY);
            addMovie(60, true, WeekDay.MONDAY);

            Planning copy = planning.copyOf();
            copy.permute(WeekDay.MONDAY, 0, WeekDay.MONDAY, 1);

            var original = planning.getAt(WeekDay.MONDAY);
            var modified = copy.getAt(WeekDay.MONDAY);

            assertNotEquals(original.getFirst().getSlug(), modified.getFirst().getSlug());
            assertEquals(original.getFirst().getSlug(), planning.getAt(WeekDay.MONDAY).getFirst().getSlug());
        }

        @Test
        void should_allow_movie_that_ends_after_midnight_when_starts_before_23_and_duration_is_valid() {
            // Arrange
            addMovie(90, false, WeekDay.MONDAY);   // 12:00 → 13:30
            addMovie(120, false, WeekDay.MONDAY);  // 13:45 → 15:45
            addMovie(150, false, WeekDay.MONDAY);  // 16:00 → 18:30
            addMovie(180, false, WeekDay.MONDAY);  // 18:45 → 21:45

            // Ce film commencerait à 22:00 → finirait à 02:00 le lendemain (autorisé)
            var lateMovie = movie(240, false);

            // Act
            boolean added = planning.add(WeekDay.MONDAY, lateMovie);

            // Assert
            assertTrue(added, "Le film doit être accepté s’il commence avant 23h00 et dure ≤ 240 min");
            var monday = planning.getAt(WeekDay.MONDAY);
            assertEquals(5, monday.size(), "Le film doit avoir été ajouté à la journée");
            assertEquals(LocalDateTime.of(date, LocalTime.of(22, 0)), monday.getLast().getStartTime());
            assertEquals(LocalDateTime.of(date.plusDays(1), LocalTime.of(2, 0)), monday.getLast().getEndTime());
        }
    }

    @Nested
    @DisplayName("Error cases")
    class ErrorCases {

        @Test
        void should_throw_exceptions_when_not_monday_given() {
            assertThrows(IllegalArgumentException.class, () -> new Planning(new PlanningTimeManager(date.plusDays(1))));
        }

        @Test
        void should_not_add_when_movie_day_is_full() {
            addMovie(240, false, WeekDay.MONDAY); // 12:00 -> 16:00
            addMovie(240, false, WeekDay.MONDAY); // 16:15 -> 20:15
            addMovie(150, false, WeekDay.MONDAY); // 20:30 -> 23:00

            // Le prochain film commencerait à 23:15 -> refusé
            assertFalse(planning.add(WeekDay.MONDAY, movie(240, false)));
            assertEquals(3, planning.getAt(WeekDay.MONDAY).size());
        }

        @Test
        void should_not_allow_movie_that_starts_at_or_after_23() {
            // Arrange
            addMovie(120, false, WeekDay.MONDAY);  // 12:00 → 14:00
            addMovie(180, false, WeekDay.MONDAY);  // 14:15 → 17:15
            addMovie(180, false, WeekDay.MONDAY);  // 17:30 → 20:30
            addMovie(120, false, WeekDay.MONDAY);  // 20:45 → 22:45

            // Ce film commencerait à 23:00 → interdit
            var tooLate = movie(90, false);

            // Act
            boolean added = planning.add(WeekDay.MONDAY, tooLate);

            // Assert
            assertFalse(added, "Un film ne doit pas être ajouté s’il commence à ou après 23h00");
            assertEquals(4, planning.getAt(WeekDay.MONDAY).size(),
                    "Le nombre de films doit rester inchangé");
        }

        @Test
        void two_differents_obj_should_not_be_equals() {
            var plan = new Planning(new PlanningTimeManager(date));

            assertNotEquals(plan, date);
        }

        @Test
        void should_not_be_equals_when_differents_date_given() {
            var plan = new Planning(new PlanningTimeManager(date));
            var plan2 = new Planning(new PlanningTimeManager(date.plusDays(7)));

            assertNotEquals(plan, plan2);
        }
    }
}
