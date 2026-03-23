package org.helmo.planclap_admin.views;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.PrintStream;

public abstract class AbstractCliView {
    private final BufferedReader cin;
    private final PrintStream cout;

    public AbstractCliView(BufferedReader cin, PrintStream cout) {
        this.cin = cin;
        this.cout = cout;
    }

    String readString(String message) {
        printf(message);
        try {
            return cin.readLine();
        } catch (IOException e) {
            return readString(message);
        }
    }

    int readInt(String message) {
        var rawInt = readString(message);

        while(!rawInt.matches("[+-]?[0-9]+")) {
            rawInt = readString(message);
        }

        return Integer.parseInt(rawInt);
    }

    public void addErrorMessage(String message) {
        cout.printf("[!] %s\n", message);
    }

    public void addSuccessMessage(String message) {
        cout.printf("[:)] %s\n", message);
    }

    public void addWarningMessage(String message) {
        cout.printf("[:/] %s\n", message);
    }

    public void printf(String fmt, Object... args) {
        cout.printf(fmt, args);
    }

}
