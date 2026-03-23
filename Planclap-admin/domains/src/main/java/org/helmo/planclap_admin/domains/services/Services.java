package org.helmo.planclap_admin.domains.services;

import java.time.DayOfWeek;
import java.time.LocalDate;

public abstract class Services {

    /**
     * Récupère le lundi prochain.
     *
     * @return le lundi prochain
     */
     LocalDate getNextMonday() {
        LocalDate today = LocalDate.now();
        int daysUntilMonday = (DayOfWeek.MONDAY.getValue() - today.getDayOfWeek().getValue() + 7) % 7;
        daysUntilMonday = daysUntilMonday == 0 ? 7 : daysUntilMonday;
        return today.plusDays(daysUntilMonday);
    }

}
