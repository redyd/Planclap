package org.helmo.planclap_admin.infrastructures.helper;

import org.helmo.planclap_admin.domains.exceptions.SqlWrapperException;
import org.helmo.planclap_admin.domains.iservices.ResultSetMapper;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.MockedStatic;

import java.sql.*;
import java.util.List;
import java.util.Optional;
import java.util.Properties;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

/**
 * Test généré par IA
 */
class SqlWrapperTest {

    private Connection mockConnection;
    private PreparedStatement mockStatement;
    private ResultSet mockResultSet;
    private MockedStatic<DriverManager> driverManagerMock;

    @BeforeEach
    void setUp() throws SQLException {
        mockConnection = mock(Connection.class);
        mockStatement = mock(PreparedStatement.class);
        mockResultSet = mock(ResultSet.class);

        driverManagerMock = mockStatic(DriverManager.class);
        driverManagerMock.when(() -> DriverManager.getConnection(anyString(), any(Properties.class)))
                .thenReturn(mockConnection);

        when(mockConnection.prepareStatement(anyString())).thenReturn(mockStatement);
        when(mockConnection.prepareStatement(anyString(), anyInt())).thenReturn(mockStatement);
    }

    @AfterEach
    void tearDown() {
        driverManagerMock.close();
    }

    @Test
    void testWithAutoCommit_Success() {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        assertNotNull(wrapper);
    }

    @Test
    void testWithTransaction_Success() throws SQLException {
        SqlWrapper wrapper = SqlWrapper.withTransaction("jdbc:test", new Properties());
        assertNotNull(wrapper);
        verify(mockConnection).setAutoCommit(false);
    }

    @Test
    void testWithTransaction_SetAutoCommitFails() throws SQLException {
        doThrow(new SQLException("Failed")).when(mockConnection).setAutoCommit(false);

        assertThrows(SqlWrapperException.class, () ->
                SqlWrapper.withTransaction("jdbc:test", new Properties())
        );
    }

    @Test
    void testNewRead_Success() {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        SqlWrapper result = wrapper.newRead("SELECT * FROM test");
        assertNotNull(result);
        assertSame(wrapper, result);
    }

    @Test
    void testNewRead_StatementAlreadyExists() {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newRead("SELECT * FROM test");

        assertThrows(IllegalStateException.class, () ->
                wrapper.newRead("SELECT * FROM test2")
        );
    }

    @Test
    void testNewWrite_WithGeneratedKeysTrue() throws SQLException {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        SqlWrapper result = wrapper.newWrite("INSERT INTO test VALUES (?)", true);

        assertNotNull(result);
        verify(mockConnection).prepareStatement(anyString(), eq(Statement.RETURN_GENERATED_KEYS));
    }

    @Test
    void testNewWrite_WithGeneratedKeysFalse() throws SQLException {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        SqlWrapper result = wrapper.newWrite("UPDATE test SET x=?", false);

        assertNotNull(result);
        verify(mockConnection).prepareStatement(anyString(), eq(Statement.NO_GENERATED_KEYS));
    }

    @Test
    void testNewWrite_DefaultGeneratedKeys() throws SQLException {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newWrite("INSERT INTO test VALUES (?)");

        verify(mockConnection).prepareStatement(anyString(), eq(Statement.RETURN_GENERATED_KEYS));
    }

    @Test
    void testNewWrite_StatementAlreadyExists() {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newWrite("INSERT INTO test VALUES (?)");

        assertThrows(IllegalStateException.class, () ->
                wrapper.newWrite("INSERT INTO test2 VALUES (?)")
        );
    }

    @Test
    void testWithParam_Long() throws SQLException {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newRead("SELECT * FROM test WHERE id = ?");
        SqlWrapper result = wrapper.withParam(1, 123L);

        assertSame(wrapper, result);
        verify(mockStatement).setObject(1, 123L);
    }

    @Test
    void testWithParam_LongSetObjectFails() throws SQLException {
        doThrow(new SQLException("Set failed")).when(mockStatement).setObject(anyInt(), any());

        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newRead("SELECT * FROM test WHERE id = ?");

        assertThrows(SqlWrapperException.class, () ->
                wrapper.withParam(1, 123L)
        );
    }

    @Test
    void testWithParam_LongNoStatement() {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());

        assertThrows(NullPointerException.class, () ->
                wrapper.withParam(1, 123L)
        );
    }

    @Test
    void testWithParam_String() throws SQLException {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newRead("SELECT * FROM test WHERE name = ?");
        SqlWrapper result = wrapper.withParam(1, "test");

        assertSame(wrapper, result);
        verify(mockStatement).setString(1, "test");
    }

    @Test
    void testWithParam_StringNull() throws SQLException {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newRead("SELECT * FROM test WHERE name = ?");
        wrapper.withParam(1, null);

        verify(mockStatement).setString(1, null);
    }

    @Test
    void testWithParam_StringSetStringFails() throws SQLException {
        doThrow(new SQLException("Set failed")).when(mockStatement).setString(anyInt(), anyString());

        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newRead("SELECT * FROM test WHERE name = ?");

        assertThrows(SqlWrapperException.class, () ->
                wrapper.withParam(1, "test")
        );
    }

    @Test
    void testWithParam_StringNoStatement() {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());

        assertThrows(NullPointerException.class, () ->
                wrapper.withParam(1, "test")
        );
    }

    @Test
    void testExecuteSelect_MultipleRows() throws SQLException {
        when(mockStatement.executeQuery()).thenReturn(mockResultSet);
        when(mockResultSet.next()).thenReturn(true, true, true, false);
        when(mockResultSet.getString("name")).thenReturn("Alice", "Bob", "Charlie");

        ResultSetMapper<String> mapper = rs -> rs.getString("name");

        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newRead("SELECT name FROM users");
        List<String> results = wrapper.executeSelect(mapper);

        assertEquals(3, results.size());
        assertEquals("Alice", results.get(0));
        assertEquals("Bob", results.get(1));
        assertEquals("Charlie", results.get(2));
        verify(mockStatement).close();
    }

    @Test
    void testExecuteSelect_NoRows() throws SQLException {
        when(mockStatement.executeQuery()).thenReturn(mockResultSet);
        when(mockResultSet.next()).thenReturn(false);

        ResultSetMapper<String> mapper = rs -> rs.getString("name");

        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newRead("SELECT name FROM users");
        List<String> results = wrapper.executeSelect(mapper);

        assertTrue(results.isEmpty());
        verify(mockStatement).close();
    }

    @Test
    void testExecuteSelect_NoStatement() {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());

        assertThrows(NullPointerException.class, () ->
                wrapper.executeSelect(rs -> rs.getString("name"))
        );
    }

    @Test
    void testExecuteSelectOne_Found() throws SQLException {
        when(mockStatement.executeQuery()).thenReturn(mockResultSet);
        when(mockResultSet.next()).thenReturn(true);
        when(mockResultSet.getString("name")).thenReturn("Alice");

        ResultSetMapper<String> mapper = rs -> rs.getString("name");

        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newRead("SELECT name FROM users WHERE id = 1");
        Optional<String> result = wrapper.executeSelectOne(mapper);

        assertTrue(result.isPresent());
        assertEquals("Alice", result.get());
        verify(mockStatement).close();
    }

    @Test
    void testExecuteSelectOne_NotFound() throws SQLException {
        when(mockStatement.executeQuery()).thenReturn(mockResultSet);
        when(mockResultSet.next()).thenReturn(false);

        ResultSetMapper<String> mapper = rs -> rs.getString("name");

        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newRead("SELECT name FROM users WHERE id = 999");
        Optional<String> result = wrapper.executeSelectOne(mapper);

        assertFalse(result.isPresent());
        verify(mockStatement).close();
    }

    @Test
    void testExecuteSelectOne_NoStatement() {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());

        assertThrows(NullPointerException.class, () ->
                wrapper.executeSelectOne(rs -> rs.getString("name"))
        );
    }

    @Test
    void testExecuteExists_True() throws SQLException {
        when(mockStatement.executeQuery()).thenReturn(mockResultSet);
        when(mockResultSet.next()).thenReturn(true);

        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newRead("SELECT 1 FROM users WHERE id = 1");
        boolean exists = wrapper.executeExists();

        assertTrue(exists);
        verify(mockStatement).close();
    }

    @Test
    void testExecuteExists_False() throws SQLException {
        when(mockStatement.executeQuery()).thenReturn(mockResultSet);
        when(mockResultSet.next()).thenReturn(false);

        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newRead("SELECT 1 FROM users WHERE id = 999");
        boolean exists = wrapper.executeExists();

        assertFalse(exists);
        verify(mockStatement).close();
    }

    @Test
    void testExecuteExists_NoStatement() {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());

        assertThrows(NullPointerException.class, wrapper::executeExists);
    }

    @Test
    void testExecute_Success() throws SQLException {
        when(mockStatement.executeUpdate()).thenReturn(3);

        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newWrite("DELETE FROM users WHERE active = false");
        int affected = wrapper.execute();

        assertEquals(3, affected);
        verify(mockStatement).close();
    }

    @Test
    void testExecute_NoStatement() {
        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());

        assertThrows(NullPointerException.class, wrapper::execute);
    }

    @Test
    void testCommit_InTransaction() throws SQLException {
        when(mockConnection.getAutoCommit()).thenReturn(false);

        SqlWrapper wrapper = SqlWrapper.withTransaction("jdbc:test", new Properties());
        wrapper.commit();

        verify(mockConnection).commit();
        verify(mockConnection).setAutoCommit(true);
    }

    @Test
    void testCommit_AlreadyAutoCommit() throws SQLException {
        when(mockConnection.getAutoCommit()).thenReturn(true);

        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.commit();

        verify(mockConnection, never()).commit();
        verify(mockConnection, never()).setAutoCommit(true);
    }

    @Test
    void testCommit_CommitFails() throws SQLException {
        when(mockConnection.getAutoCommit()).thenReturn(false);
        doThrow(new SQLException("Commit failed")).when(mockConnection).commit();

        SqlWrapper wrapper = SqlWrapper.withTransaction("jdbc:test", new Properties());

        assertThrows(SqlWrapperException.class, wrapper::commit);
    }

    @Test
    void testClose_WithAutoCommit() throws Exception {
        when(mockConnection.getAutoCommit()).thenReturn(true);

        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.close();

        verify(mockConnection, never()).rollback();
        verify(mockConnection).close();
    }

    @Test
    void testClose_WithTransaction_DoesRollback() throws Exception {
        when(mockConnection.getAutoCommit()).thenReturn(false);

        SqlWrapper wrapper = SqlWrapper.withTransaction("jdbc:test", new Properties());
        wrapper.close();

        verify(mockConnection).rollback();
        verify(mockConnection).close();
    }

    @Test
    void testCloseStatement_CloseFails() throws SQLException {
        doThrow(new SQLException("Close failed")).when(mockStatement).close();

        SqlWrapper wrapper = SqlWrapper.withAutoCommit("jdbc:test", new Properties());
        wrapper.newRead("SELECT * FROM test");

        when(mockStatement.executeQuery()).thenReturn(mockResultSet);
        when(mockResultSet.next()).thenReturn(false);

        assertThrows(SqlWrapperException.class, () ->
                wrapper.executeSelect(rs -> rs.getString("name"))
        );
    }
}