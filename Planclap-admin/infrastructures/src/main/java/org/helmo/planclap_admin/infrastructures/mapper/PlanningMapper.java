package org.helmo.planclap_admin.infrastructures.mapper;

import org.helmo.planclap_admin.domains.core.PlannedMovie;
import org.helmo.planclap_admin.domains.core.Planning;
import org.helmo.planclap_admin.domains.core.WeekDay;
import org.helmo.planclap_admin.domains.iservices.MappingFromDto;
import org.helmo.planclap_admin.infrastructures.dto.CsvPlanningDto;
import org.helmo.planclap_admin.infrastructures.dto.SqlPlanningDto;

import java.util.ArrayList;
import java.util.List;

public class PlanningMapper {

    private PlanningMapper() {
    }

    public static CsvPlanningDto mapToCsvDto(Planning planning) {
        return new CsvPlanningDto(mapToDtoList(planning, PlannedMovieMapper::mapToCsvDto));
    }

    public static SqlPlanningDto mapToSqlDto(Planning planning) {
        return new SqlPlanningDto(mapToDtoList(planning, PlannedMovieMapper::mapToSqlDto));
    }

    /**
     * Méthode générique qui utilise une interface fonctionnelle pour mapper un planning en dto.
     *
     * @param planning le planning à mapper
     * @param mapper référence de méthode de mapping
     * @return une liste de dto de type R
     * @param <R> le type de dto à retourner
     * @see MappingFromDto l'interface fonctionnelle
     */
    private static <R> List<R> mapToDtoList(Planning planning, MappingFromDto<R, PlannedMovie> mapper) {
        List<R> result = new ArrayList<>();

        for (WeekDay day : WeekDay.values()) {
            for (PlannedMovie concreteValue : planning.getAt(day)) {
                result.add(mapper.mapFromDto(concreteValue));
            }
        }

        return result;
    }


}
