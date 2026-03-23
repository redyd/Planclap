package org.helmo.planclap_admin.infrastructures.implementations;

import org.helmo.planclap_admin.domains.core.*;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.infrastructures.helper.SqlWrapper;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.LocalDate;
import java.util.Properties;

import static org.helmo.planclap_admin.infrastructures.provider.TestProvider.movie;
import static org.junit.jupiter.api.Assertions.*;

/**
 * Les 3 derniers tests ont été générés par IA
 */
public class SqlPlanningRepositoryTest {

    public static final Properties PROPS = new Properties();
    public String jdbcUrl = "";
    private SqlPlanningRepository repo;

    @BeforeEach
    void setUp(@TempDir Path temp) throws IOException {
        var filePath = temp.resolve("planclap-test.sqlite");
        Files.copy(Path.of("src", "test", "resources", "db", "planclap-test.sqlite"), filePath);
        jdbcUrl = "jdbc:sqlite:%s".formatted(filePath);
        repo = new SqlPlanningRepository(jdbcUrl, PROPS);
    }

    @Test
    void should_save_with_success_even_if_planning_is_already_defined() {
        var date = LocalDate.of(2025, 11, 24);
        Planning planning = new Planning(new PlanningTimeManager(date));
        planning.add(WeekDay.MONDAY, movie(100, true));

        assertDoesNotThrow(() -> repo.encode(planning, date));
    }

    @Test
    void should_clear_existing_shows_before_inserting_new_planning() throws RepositoryException {
        var date = LocalDate.of(2025, 11, 24);

        // Premier planning avec un film le lundi
        Planning firstPlanning = new Planning(new PlanningTimeManager(date));
        firstPlanning.add(WeekDay.MONDAY, movie(100, true));
        repo.encode(firstPlanning, date);

        // Compter les séances après le premier insert
        int countAfterFirst = countShowsInPeriod(firstPlanning);
        assertTrue(countAfterFirst > 0, "Des séances devraient être présentes après le premier insert");

        // Deuxième planning avec un film différent le mardi (même semaine)
        Planning secondPlanning = new Planning(new PlanningTimeManager(date));
        secondPlanning.add(WeekDay.TUESDAY, movie(166, false));
        repo.encode(secondPlanning, date);

        // Vérification : le nombre de séances devrait correspondre uniquement au deuxième planning
        int countAfterSecond = countShowsInPeriod(secondPlanning);
        assertEquals(1, countAfterSecond,
                "Une seule séance (celle du mardi) devrait être présente après le second insert");
    }

    @Test
    void should_insert_multiple_shows_for_multiple_days() throws RepositoryException {
        var date = LocalDate.of(2025, 11, 24);
        Planning planning = new Planning(new PlanningTimeManager(date));

        // Ajout de films sur plusieurs jours
        planning.add(WeekDay.MONDAY, movie(100, true));
        planning.add(WeekDay.WEDNESDAY, movie(166, false));
        planning.add(WeekDay.FRIDAY, movie(94, true));

        repo.encode(planning, date);

        // Vérification : toutes les séances doivent être présentes
        int showCount = countShowsInPeriod(planning);
        assertEquals(3, showCount, "3 séances devraient être enregistrées");
    }

    @Test
    void should_handle_empty_planning() throws RepositoryException {
        var date = LocalDate.of(2025, 11, 24);

        // D'abord, insérer un planning avec des séances
        Planning planningWithShows = new Planning(new PlanningTimeManager(date));
        planningWithShows.add(WeekDay.MONDAY, movie(100, true));
        planningWithShows.add(WeekDay.TUESDAY, movie(166, false));
        repo.encode(planningWithShows, date);

        int countBefore = countShowsInPeriod(planningWithShows);
        assertTrue(countBefore > 0, "Des séances devraient exister avant");

        // Ensuite, encoder un planning vide (devrait tout supprimer).
        Planning emptyPlanning = new Planning(new PlanningTimeManager(date));
        repo.encode(emptyPlanning, date);

        // Vérification : toutes les séances devraient avoir été supprimées
        int countAfter = countShowsInPeriod(emptyPlanning);
        assertEquals(0, countAfter, "Aucune séance ne devrait subsister après un planning vide");
    }

    // Méthode utilitaire pour compter les séances dans une période
    private int countShowsInPeriod(Planning planning) {
        try (var wrapper = SqlWrapper.withAutoCommit(jdbcUrl, PROPS)) {
            return wrapper
                    .newRead("SELECT COUNT(*) as count FROM movie_show WHERE show_start >= ? AND show_start < ?")
                    .withParam(1, planning.epochOfBeginning())
                    .withParam(2, planning.epochOfEnd())
                    .executeSelectOne(rs -> rs.getInt("count"))
                    .orElse(0);
        } catch (Exception e) {
            fail("Erreur lors du comptage des séances : " + e.getMessage());
            return -1;
        }
    }

}
