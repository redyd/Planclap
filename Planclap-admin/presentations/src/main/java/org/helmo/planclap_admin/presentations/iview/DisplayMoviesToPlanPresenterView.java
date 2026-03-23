package org.helmo.planclap_admin.presentations.iview;

import org.helmo.planclap_admin.presentations.view_models.MoviesToPlanViewModel;

public interface DisplayMoviesToPlanPresenterView {
    void displayMoviesToPlan(MoviesToPlanViewModel moviesToPlan);
    void displayTotalDuration(String totalDuration);
    void displayErrorMessage(String message);
}
