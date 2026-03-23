package org.helmo.planclap_admin.presentations.presenters;

import org.helmo.planclap_admin.domains.exceptions.PlanningServiceException;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.iservices.GeneratePlanning;
import org.helmo.planclap_admin.domains.iservices.Executable;
import org.helmo.planclap_admin.presentations.iview.GeneratePlanningPresenterView;

public class GeneratePlanningPresenter implements Executable {

    private final GeneratePlanning service;
    private final GeneratePlanningPresenterView view;

    private GeneratePlanningPresenter(GeneratePlanning service, GeneratePlanningPresenterView view) {
        this.service = service;
        this.view = view;
    }

    public static GeneratePlanningPresenter of(GeneratePlanning service, GeneratePlanningPresenterView view) {
        return new GeneratePlanningPresenter(service, view);
    }

    @Override
    public void execute() {
        try {
            service.generatePlanning();
            view.displaySuccess("Planning genere avec succes.");
        } catch (PlanningServiceException e) {
            view.displayError("Films a planifier invalide: impossible de generer un planning");
        } catch (RepositoryException e) {
            view.displayError("Une erreur est survenue lors de la sauvegarde");
        }
    }
}
