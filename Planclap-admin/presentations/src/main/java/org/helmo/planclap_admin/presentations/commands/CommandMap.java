package org.helmo.planclap_admin.presentations.commands;

import org.helmo.planclap_admin.domains.iservices.Executable;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.PrintStream;
import java.util.ArrayList;
import java.util.List;

public class CommandMap implements Executable {

    private final BufferedReader cin;
    private final PrintStream cout;
    private final List<String> labels;
    private final List<Executable> commands;

    public CommandMap(BufferedReader cin, PrintStream cout) {
        this.cin = cin;
        this.cout = cout;
        this.labels = new ArrayList<String>();
        this.commands = new ArrayList<Executable>();
    }

    public void add(String label, Executable command) {
        this.labels.add(label);
        this.commands.add(command);
    }

    @Override
    public void execute() {
        boolean quitRequested = false;
        while (!quitRequested) {
            displayItems();
            var userChoice = readChoice();

            if (userChoice == labels.size() + 1) {
                quitRequested = true;
            } else if (userChoice <= labels.size()) {
                commands.get(userChoice - 1).execute();
            } else {
                cout.println("Entrée inconnue");
            }
        }
    }

    private void displayItems() {
        for (int i = 0; i < labels.size(); i++) {
            cout.printf("%d. %s\n", i + 1, labels.get(i));
        }
        cout.printf("%d. Quitter\n", labels.size() + 1);
        cout.println();
    }

    private int readChoice() {
        var rawChoice = "";
        var integerFound = false;
        while (!integerFound) {
            try {
                cout.print("Votre choix : ");
                rawChoice = cin.readLine().strip();
                integerFound = rawChoice.matches("[0-9]+");
            } catch (IOException e) {
                rawChoice = "";
            }
        }

        return Integer.parseInt(rawChoice);
    }

}
