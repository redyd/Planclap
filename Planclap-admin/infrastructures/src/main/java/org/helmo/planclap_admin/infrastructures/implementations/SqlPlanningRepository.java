package org.helmo.planclap_admin.infrastructures.implementations;

import org.helmo.planclap_admin.domains.annotations.Log;
import org.helmo.planclap_admin.domains.core.Planning;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.exceptions.SqlWrapperException;
import org.helmo.planclap_admin.domains.repository.PlanningRepository;
import org.helmo.planclap_admin.infrastructures.logged_operations.LoggedDbOperations;
import org.helmo.planclap_admin.infrastructures.mapper.PlanningMapper;
import org.helmo.planclap_admin.infrastructures.scripts.PlanningScript;

import java.time.LocalDate;
import java.util.Properties;

public class SqlPlanningRepository implements PlanningRepository {

    private final String url;
    private final Properties props;

    public SqlPlanningRepository(String url, Properties props) {
        this.url = url;
        this.props = props;
    }

    @Override
    @Log
    public void encode(Planning planning, LocalDate date) throws RepositoryException {
        try (var wrapper = LoggedDbOperations.withTransaction(url, props)) {
            var dtos = PlanningMapper.mapToSqlDto(planning);
            // 1. clear existing values
            wrapper
                    .newWrite(PlanningScript.DELETE_BETWEEN.get())
                    .withParam(1, planning.epochOfBeginning())
                    .withParam(2, planning.epochOfEnd())
                    .execute();

            // 2. add the new ones
            for (var dto : dtos.plannedMovies()) {
                wrapper
                        .newWrite(PlanningScript.INSERT_NEW.get())
                        .withParam(1, dto.slug())
                        .withParam(2, dto.showStart())
                        .execute();
            }

            wrapper.commit();
        } catch (SqlWrapperException e) {
            throw new RepositoryException("Error while encoding the planning: " + e);
        } catch (Exception e) {
            throw new RepositoryException("Try-with-resources exception");
        }
    }
}
