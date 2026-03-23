package org.helmo.planclap_admin.presentations.iview;

import org.helmo.planclap_admin.presentations.view_models.CineCheckAgeViewModel;
import org.helmo.planclap_admin.presentations.view_models.CineCheckViewModel;

import java.util.List;

public interface AddMovieToPlanPresenterView {
    String getTitle();
    String getDescription();
    String getPosterURI();

    int getDuration();
    int getSessionQuantity();

    String getCineCheckAge(CineCheckAgeViewModel choices);
    List<String> getCineChecks(CineCheckViewModel choices);

    void displayErrorMessage(String message);
    void displaySuccessMessage(String message);
}
