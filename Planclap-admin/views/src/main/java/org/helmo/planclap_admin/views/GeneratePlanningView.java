package org.helmo.planclap_admin.views;

import org.helmo.planclap_admin.presentations.iview.GeneratePlanningPresenterView;

import java.io.BufferedReader;
import java.io.PrintStream;

public class GeneratePlanningView extends AbstractCliView implements GeneratePlanningPresenterView {

    public GeneratePlanningView(BufferedReader cin, PrintStream cout) {
        super(cin, cout);
    }

    @Override
    public void displaySuccess(String message) {
        addSuccessMessage(message);
    }

    @Override
    public void displayError(String message) {
        addErrorMessage(message);
    }
}
