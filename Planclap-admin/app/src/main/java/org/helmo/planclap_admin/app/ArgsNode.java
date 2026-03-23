package org.helmo.planclap_admin.app;

import org.helmo.planclap_admin.domains.exceptions.InvalidArgsException;

public abstract class ArgsNode {

    private final ArgsNode nextNode;

    public ArgsNode(ArgsNode nextNode) {
        this.nextNode = nextNode;
    }

    ArgsNode next(String[] args) {
        if (nextNode != null) {
            return nextNode.test(args);
        }
        throw new InvalidArgsException("Argument not valid");
    }

    abstract ArgsNode test(String[] args);
}
