package org.helmo.planclap_admin.domains.core;

import java.util.*;

/**
 * Classe représentant un groupe de CineCheck <br/>
 * Un CineCheckGroup doit obligatoirement avoir un âge et 0:N symboles associés
 */
public class CineCheckGroup {

    private final CineCheckAge age;
    private final Set<CineCheck> cineChecks;

    private CineCheckGroup(CineCheckAge age, Set<CineCheck> cineChecks) {
        this.age = age;
        this.cineChecks = cineChecks;
    }

    public CineCheckAge getAge() {
        return age;
    }

    /**
     * Permet d'obtenir les CineChecks associés au groupe.
     *
     * @return un ensemble de CineCheck
     */
    public Set<CineCheck> getCineCheck() {
        return Set.copyOf(cineChecks);
    }

    /**
     * Détermine si le CineCheckGroup est destiné aux enfants.
     *
     * @return {@code true} si l'âge est en dessous ou égal à 12 ans, {@code false} sinon
     */
    public boolean isForChildren() {
        return
                age.equals(CineCheckAge.AL)
                        || age.equals(CineCheckAge.SIX)
                        || age.equals(CineCheckAge.NINE)
                        || age.equals(CineCheckAge.TWELVE);
    }

    /**
     * Recrée un CineCheckGroup à partir d'un âge et d'une collection de CineCheck.
     *
     * @param age Âge du groupe
     * @param checks CineChecks associés au groupe
     * @return un nouveau CineCheckGroup
     */
    public static CineCheckGroup of(CineCheckAge age, Collection<CineCheck> checks) {
        return new CineCheckGroup(age, new HashSet<>(checks));
    }

    @Override
    public boolean equals(Object o) {
        if (o == null || getClass() != o.getClass()) {
            return false;
        }
        CineCheckGroup that = (CineCheckGroup) o;
        return age == that.age && Objects.equals(cineChecks, that.cineChecks);
    }

    @Override
    public int hashCode() {
        return Objects.hash(age, cineChecks);
    }
}
