package org.helmo.planclap_admin.infrastructures.logged_operations;

import org.helmo.planclap_admin.domains.annotations.Log;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;

public class LoggedFileOperations {

    @Log
    public static BufferedReader openForReading(Path path) throws IOException {
        return Files.newBufferedReader(path);
    }

    @Log
    public static BufferedWriter openForWriting(Path path) throws IOException {
        return Files.newBufferedWriter(path);
    }

}
