package org.helmo.planclap_admin.infrastructures.helper;

import org.helmo.planclap_admin.domains.core.MoviesToPlan;
import org.helmo.planclap_admin.infrastructures.dto.MoviesToPlanDto;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import java.io.Reader;
import java.io.StringReader;
import java.io.StringWriter;
import java.time.LocalDate;

import static org.junit.jupiter.api.Assertions.*;

/**
 * Classe de test générée en partie par IA
 * Correction manuelle
 */
class JsonHandlerTest {

    private final JsonHandler handler = new JsonHandler();

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_return_moviesToPlan_when_json_is_valid() {
            LocalDate date = LocalDate.now();
            String json = """
                {
                  "movies": []
                }
                """;
            Reader reader = new StringReader(json);

            MoviesToPlan result = handler.readAndParse(reader, date);
            assertNotNull(result);
        }

        @Test
        void should_return_empty_moviesToPlan_when_json_is_null() {
            LocalDate date = LocalDate.now();
            Reader reader = new StringReader("null");

            MoviesToPlan result = handler.readAndParse(reader, date);
            assertNotNull(result);
        }

        @Test
        void should_return_dto_when_json_is_valid() {
            String json = """
                {
                  "movies": []
                }
                """;
            Reader reader = new StringReader(json);

            MoviesToPlanDto dto = handler.read(reader);
            assertNotNull(dto);
        }

        @Test
        void should_return_empty_dto_when_json_is_null() {
            Reader reader = new StringReader("null");

            MoviesToPlanDto dto = handler.read(reader);
            assertNotNull(dto);
        }

        @Test
        void should_write_json_to_writer_when_valid_dto_given() {
            StringWriter writer = new StringWriter();
            MoviesToPlanDto dto = MoviesToPlanDto.empty();

            handler.write(writer, dto);

            String jsonOutput = writer.toString();
            assertTrue(jsonOutput.contains("{"));
            assertTrue(jsonOutput.contains("}"));
        }
    }
}
