package org.helmo.planclap_admin.presentations.mapper;

import org.helmo.planclap_admin.domains.core.CineCheck;
import org.helmo.planclap_admin.domains.core.CineCheckAge;
import org.helmo.planclap_admin.presentations.view_models.CineCheckAgeViewModel;
import org.helmo.planclap_admin.presentations.view_models.CineCheckViewModel;

import java.util.Arrays;
import java.util.Collection;
import java.util.HashSet;
import java.util.Set;
import java.util.stream.Collectors;

public class CineCheckMapper {

    private CineCheckMapper() {}

    public static CineCheckViewModel mapToVM() {
        return new CineCheckViewModel(
                Arrays.stream(CineCheck.values())
                        .map(CineCheck::getDescription)
                        .collect(Collectors.toList()));
    }

    public static Set<CineCheck> map(Collection<String> cinechecks) {
        Set<CineCheck> checks = new HashSet<>();

        for (String cc : cinechecks) {
            var check = mapCheck(cc);
            if (check != null) {
                checks.add(check);
                continue;
            }

            throw new IllegalArgumentException("CineCheck invalide: " + cc);
        }

        return checks;
    }

    public static CineCheck mapCheck(String value) {
        for (CineCheck cc : CineCheck.values()) {
            if (cc.getDescription().equals(value)) {
                return cc;
            }
        }
        return null;
    }

    public static CineCheckAgeViewModel mapAgeToVM() {
        return new CineCheckAgeViewModel(
                Arrays.stream(CineCheckAge.values())
                        .map(CineCheckAge::toLiteralString)
                        .collect(Collectors.toList()));
    }

    public static CineCheckAge map(String age) {
        for (CineCheckAge cine : CineCheckAge.values()) {
            if (cine.equalsValue(age)) {
                return cine;
            }
        }
        throw new IllegalArgumentException("L'age n'est pas valide");
    }
}
