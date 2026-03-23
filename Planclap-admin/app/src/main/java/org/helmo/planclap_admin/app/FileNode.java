package org.helmo.planclap_admin.app;

import joptsimple.OptionParser;
import joptsimple.OptionSet;
import org.helmo.planclap_admin.domains.exceptions.InvalidArgsException;

import java.nio.file.Files;
import java.nio.file.Path;

/**
 * This class parse a single line arguments for the file path
 */
public class FileNode extends ArgsNode {

    private static final String ARG_NAME = "dir";
    private Path path;

    public FileNode(ArgsNode next) {
        super(next);
    }

    public Path getPath() {
        return path;
    }

    @Override
    public ArgsNode test(String[] args) {
        // create the parser
        OptionParser parser = new OptionParser();
        parser.accepts(ARG_NAME).withOptionalArg().ofType(String.class);
        parser.allowsUnrecognizedOptions();

        // parse the option
        OptionSet option = parser.parse(args);

        if (!option.has(ARG_NAME)) {
            return next(args);
        }

        Path path = Path.of(option.valueOf(ARG_NAME).toString());

        // if the path is invalid, Program should know it
        if (!Files.exists(path)) {
            throw new InvalidArgsException("path is invalid");
        }

        this.path = path;

        return this;
    }
}
