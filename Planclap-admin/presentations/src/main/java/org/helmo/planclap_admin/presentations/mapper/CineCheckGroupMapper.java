package org.helmo.planclap_admin.presentations.mapper;

import org.helmo.planclap_admin.domains.core.CineCheckGroup;

import java.util.*;

public class CineCheckGroupMapper {

    private CineCheckGroupMapper() {}

    public static List<String> mapGroupToVM(CineCheckGroup cineCheckGroup) {
        List<String> vm = new ArrayList<>();
        vm.add(cineCheckGroup.getAge().toLiteralString());

        cineCheckGroup.getCineCheck().forEach(cineCheck -> vm.add(cineCheck.getDescription()));

        return vm;
    }

}
