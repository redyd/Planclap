package org.helmo.planclap_admin.presentations.commands;

import org.helmo.planclap_admin.domains.iservices.Executable;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.PrintStream;

import static org.mockito.Mockito.*;

/**
 * Généré en partie avec IA
 * Correction manuelle
 */
public class CommandMapTest {

    private BufferedReader cin;
    private PrintStream cout;
    private Executable command1;
    private Executable command2;
    private Executable command3;
    private CommandMap commandMap;

    @BeforeEach
    void setUp() {
        cin = mock(BufferedReader.class);
        cout = mock(PrintStream.class);
        command1 = mock(Executable.class);
        command2 = mock(Executable.class);
        command3 = mock(Executable.class);
        commandMap = new CommandMap(cin, cout);
    }

    @Nested
    @DisplayName("Happy path")
    class HappyPath {

        @Test
        void should_quit_immediately_when_given_quit_option() throws IOException {
            commandMap.add("Option 1", command1);
            commandMap.add("Option 2", command2);

            when(cin.readLine()).thenReturn("3");

            commandMap.execute();

            verify(cout, atLeastOnce()).printf("%d. %s\n", 1, "Option 1");
            verify(cout, atLeastOnce()).printf("%d. %s\n", 2, "Option 2");
            verify(cout, atLeastOnce()).printf("%d. Quitter\n", 3);
            verify(cin, times(1)).readLine();
            verify(command1, never()).execute();
            verify(command2, never()).execute();
        }

        @Test
        void should_execute_first_command_when_given_first_choice() throws IOException {
            commandMap.add("Option 1", command1);
            commandMap.add("Option 2", command2);

            when(cin.readLine()).thenReturn("1", "3");

            commandMap.execute();

            verify(command1, times(1)).execute();
            verify(command2, never()).execute();
        }

        @Test
        void should_execute_second_command_when_given_second_choice() throws IOException {
            commandMap.add("Option 1", command1);
            commandMap.add("Option 2", command2);

            when(cin.readLine()).thenReturn("2", "3");

            commandMap.execute();

            verify(command1, never()).execute();
            verify(command2, times(1)).execute();
        }

        @Test
        void should_execute_multiple_commands_when_given_multiple_choices_before_quitting() throws IOException {
            commandMap.add("Option 1", command1);
            commandMap.add("Option 2", command2);
            commandMap.add("Option 3", command3);

            when(cin.readLine()).thenReturn("1", "2", "3", "4");

            commandMap.execute();

            verify(command1, times(1)).execute();
            verify(command2, times(1)).execute();
            verify(command3, times(1)).execute();
        }

        @Test
        void should_execute_command_when_given_input_with_whitespace() throws IOException {
            commandMap.add("Option 1", command1);

            when(cin.readLine()).thenReturn("  1  ", "2");

            commandMap.execute();

            verify(command1, times(1)).execute();
        }

        @Test
        void should_display_all_labels_with_numbers_when_given_multiple_commands() throws IOException {
            commandMap.add("Premier choix", command1);
            commandMap.add("Deuxième choix", command2);
            commandMap.add("Troisième choix", command3);

            when(cin.readLine()).thenReturn("4");

            commandMap.execute();

            verify(cout, atLeastOnce()).printf("%d. %s\n", 1, "Premier choix");
            verify(cout, atLeastOnce()).printf("%d. %s\n", 2, "Deuxième choix");
            verify(cout, atLeastOnce()).printf("%d. %s\n", 3, "Troisième choix");
            verify(cout, atLeastOnce()).printf("%d. Quitter\n", 4);
        }

        @Test
        void should_display_prompt_for_each_iteration_when_given_multiple_iterations() throws IOException {
            commandMap.add("Option 1", command1);

            when(cin.readLine()).thenReturn("1", "2");

            commandMap.execute();

            verify(cout, times(2)).print("Votre choix : ");
        }

        @Test
        void should_execute_correct_command_when_given_multiple_added_commands() throws IOException {
            commandMap.add("First", command1);
            commandMap.add("Second", command2);
            commandMap.add("Third", command3);

            when(cin.readLine()).thenReturn("2", "4");

            commandMap.execute();

            verify(command2, times(1)).execute();
            verify(command1, never()).execute();
            verify(command3, never()).execute();
        }

    }

    @Nested
    @DisplayName("Error cases")
    class ErrorCases {

        @Test
        void should_display_error_message_when_given_invalid_choice() throws IOException {
            commandMap.add("Option 1", command1);
            commandMap.add("Option 2", command2);

            when(cin.readLine()).thenReturn("5", "3");

            commandMap.execute();

            verify(cout, times(1)).println("Entrée inconnue");
            verify(command1, never()).execute();
            verify(command2, never()).execute();
        }

        @Test
        void should_retry_reading_input_when_given_non_numeric_input() throws IOException {
            commandMap.add("Option 1", command1);

            when(cin.readLine()).thenReturn("abc", "xyz", "1", "2");

            commandMap.execute();

            verify(cin, times(4)).readLine();
            verify(command1, times(1)).execute();
        }

        @Test
        void should_retry_reading_input_when_given_io_exception() throws IOException {
            commandMap.add("Option 1", command1);

            when(cin.readLine())
                    .thenThrow(new IOException("Test exception"))
                    .thenReturn("2");

            commandMap.execute();

            verify(cin, times(2)).readLine();
            verify(command1, never()).execute();
        }

        @Test
        void should_display_only_quit_option_when_given_empty_menu() throws IOException {
            when(cin.readLine()).thenReturn("1");

            commandMap.execute();

            verify(cout, atLeastOnce()).printf("%d. Quitter\n", 1);
            verify(cout, atLeastOnce()).println();
        }

        @Test
        void should_retry_multiple_times_when_given_multiple_io_exceptions() throws IOException {
            commandMap.add("Option 1", command1);

            when(cin.readLine())
                    .thenThrow(new IOException())
                    .thenThrow(new IOException())
                    .thenReturn("2");

            commandMap.execute();

            verify(cin, times(3)).readLine();
        }

        @Test
        void should_execute_command_multiple_times_when_given_same_choice_multiple_times() throws IOException {
            commandMap.add("Option 1", command1);

            doNothing().when(command1).execute();
            when(cin.readLine()).thenReturn("1", "1", "2");

            commandMap.execute();

            verify(command1, times(2)).execute();
        }

        @Test
        void should_retry_reading_input_when_given_negative_number() throws IOException {
            commandMap.add("Option 1", command1);

            when(cin.readLine()).thenReturn("-1", "2");

            commandMap.execute();

            verify(cin, times(2)).readLine();
        }

        @Test
        void should_retry_reading_input_when_given_empty_input() throws IOException {
            commandMap.add("Option 1", command1);

            when(cin.readLine()).thenReturn("", "   ", "2");

            commandMap.execute();

            verify(cin, times(3)).readLine();
        }

    }
}