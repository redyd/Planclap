package org.helmo.planclap_admin.views;

import org.helmo.planclap_admin.presentations.iview.AddMovieToPlanPresenterView;
import org.helmo.planclap_admin.presentations.view_models.CineCheckAgeViewModel;
import org.helmo.planclap_admin.presentations.view_models.CineCheckViewModel;

import java.io.BufferedReader;
import java.io.PrintStream;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

public class AddMovieToPlanView extends AbstractCliView implements AddMovieToPlanPresenterView {

    public AddMovieToPlanView(BufferedReader cin, PrintStream cout) {
        super(cin, cout);
    }

    @Override
    public String getTitle() {
        return readString("Titre du film: ");
    }

    @Override
    public String getDescription() {
        return readString("Description du film: ");
    }

    @Override
    public String getPosterURI() {
        return readString("URL du poster: ");
    }

    @Override
    public int getDuration() {
        return readInt("Duree du film: ");
    }

    @Override
    public int getSessionQuantity() {
        return readInt("Nombre de seances a planifier: ");
    }

    @Override
    public String getCineCheckAge(CineCheckAgeViewModel choices) {
        for (String age : choices.values()) {
            printf("- %s\n", age);
        }

        return readString("Age minimum: ");
    }

    @Override
    public List<String> getCineChecks(CineCheckViewModel choices) {
        Set<String> choicesList = new HashSet<>();

        while (choicesList.size() < choices.values().size()) {
            displayCineChecks(choices);
            String input = readString("Numero du Cinecheck a ajouter (enter pour arreter): ");

            if (input.isBlank()) {
                break;
            }

            try {
                choicesList.add(choices.values().get(Integer.parseInt(input) - 1));
            } catch (NumberFormatException e) {
                addWarningMessage("Choix invalide");
            }
        }

        return List.copyOf(choicesList);
    }

    private void displayCineChecks(CineCheckViewModel choices) {
        int count = 1;
        for (String val: choices.values()) {
            printf("[%d] %s\n",  count++, val);
        }
    }

    @Override
    public void displayErrorMessage(String message) {
        addErrorMessage(message);
    }

    @Override
    public void displaySuccessMessage(String message) {
        addSuccessMessage(message);
    }
}
