package org.helmo.planclap_admin.infrastructures.mapper;

import org.helmo.planclap_admin.domains.core.CineCheck;
import org.helmo.planclap_admin.domains.core.CineCheckAge;
import org.helmo.planclap_admin.domains.core.CineCheckGroup;

import java.util.*;

public class CineCheckGroupMapper {

    private CineCheckGroupMapper() {
    }

    public static String[] mapToDTO(CineCheckGroup cineCheckGroup) {
        List<String> array = new ArrayList<>();
        array.add(cineCheckGroup.getAge().toLiteralString());

        cineCheckGroup.getCineCheck().forEach(cc -> array.add(cc.getDescription()));

        return array.toArray(new String[0]);
    }

    public static CineCheckGroup map(Collection<String> cinechecks) {
        List<CineCheckAge> ages = new ArrayList<>(3);
        Set<CineCheck> checks = new HashSet<>();

        for (String cc : cinechecks) {
            var age = mapAge(cc);
            if (added(cc, age, ages, checks)) {
                continue;
            }

            throw new IllegalArgumentException("CineCheck invalide: " + cc);
        }

        final int expected = 1;
        if (ages.size() != expected) {
            throw new IllegalArgumentException("Il doit y avoir exactement un âge CineCheck.");
        }

        return CineCheckGroup.of(ages.getFirst(), checks);
    }

    private static boolean added(String cc, CineCheckAge age, List<CineCheckAge> ages, Set<CineCheck> checks) {
        if (age != null) {
            ages.add(age);
            return true;
        }

        var check = mapCheck(cc);
        if (check != null) {
            checks.add(check);
            return true;
        }
        return false;
    }

    public static CineCheckAge mapAge(String value) {
        for (CineCheckAge cc : CineCheckAge.values()) {
            if (cc.toLiteralString().equals(value)) {
                return cc;
            }
        }
        return null;
    }

    public static CineCheck mapCheck(String value) {
        for (CineCheck cc : CineCheck.values()) {
            if (cc.getDescription().equals(value) || cc.toString().equals(value)) {
                return cc;
            }
        }
        return null;
    }

}
