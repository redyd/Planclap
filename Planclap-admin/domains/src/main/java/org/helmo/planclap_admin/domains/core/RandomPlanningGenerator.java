package org.helmo.planclap_admin.domains.core;

import java.util.Random;

/**
 * Générateur de planning aléatoire.
 * Le principe de cet algorithme est de prendre deux journées et deux positions du planning aléatoirement et de les permuter.
 * Si le planning de cette permutation a un meilleur score que le planning précédent, on garde ce planning. Si ce planning a
 * un meilleur score que le meilleur planning, il devient le meilleur planning.
 * Par contre, si le planning de cette permutation a un score moins bon que le précédent, on réutilise le planning précédent
 * pour continuer la recherche du meilleur planning.
 */
public class RandomPlanningGenerator implements PlanningGenerator {

    private static final int MAX_DEPTH = 100;

    private final Random random = new Random();

    @Override
    public Planning generate(MoviesToPlan plan) {
        if (plan.belowPlanLimit()) {
            throw new IllegalArgumentException("Plan invalide: le nombre d'heure est en dessous du minimum");
        }

        PlanningTimeManager manager = new PlanningTimeManager(plan.getDateForPlan());
        Planning planning = new Planning(manager);
        fillPlan(plan, planning);

        return generatePlanning(planning, planning.copyOf(), planning.copyOf(), 0);
    }

    private Planning generatePlanning(Planning current, Planning previous, Planning best, int depth) {
        if (depth >= MAX_DEPTH) {
            return best;
        }

        int scoreCurrent;
        boolean permuted;

        do {
            permuted = current.permute(randomDay(), random.nextInt(), randomDay(), random.nextInt());
        } while (!permuted);

        scoreCurrent = current.score();

        if (scoreCurrent < previous.score()) {
            // si le score est plus faible que le précédent, on retourne en arrière
            return generatePlanning(previous.copyOf(), previous.copyOf(), best, depth + 1);
        } else {
            // mise à jour du précédent pour l'itération suivante
            Planning updatedPrevious = current.copyOf();

            // s'il a un meilleur score que best, on l'utilise
            Planning updatedBest = getBest(current, best, scoreCurrent);

            // continuer la recherche
            return generatePlanning(current, updatedPrevious, updatedBest, depth + 1);
        }
    }

    private static Planning getBest(Planning current, Planning best, int scoreCurrent) {
        Planning updatedBest;
        if (scoreCurrent > best.score()) {
            updatedBest = current.copyOf();
        } else {
            updatedBest = best;
        }
        return updatedBest;
    }

    private WeekDay randomDay() {
        return WeekDay.from(random.nextInt(WeekDay.values().length));
    }

    private static void fillPlan(MoviesToPlan plan, Planning planning) {
        int cursor = 0;
        for (MovieSessions sessions : plan.get()) {
            int amount = sessions.sessionsAmount().toInt();

            for (int i = 0; i < amount; i++) {
                planning.add(WeekDay.from(cursor), sessions.movie());
                cursor = (cursor + 1) % WeekDay.values().length;
            }
        }
    }

}
