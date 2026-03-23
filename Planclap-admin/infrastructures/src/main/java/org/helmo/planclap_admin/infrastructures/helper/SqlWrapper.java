package org.helmo.planclap_admin.infrastructures.helper;

import org.helmo.planclap_admin.domains.exceptions.SqlWrapperException;
import org.helmo.planclap_admin.domains.iservices.ResultSetMapper;

import java.sql.*;
import java.util.*;

public class SqlWrapper implements AutoCloseable {

    public static final String STATEMENT_NULL_MSG = "Statement is null";
    public static final String CONNECTION_NULL_MSG = "Connection is null";

    private final Connection connection;
    private Optional<PreparedStatement> statement = Optional.empty();

    /**
     * Crée un nouveau wrapper avec une connexion à la base de données.
     *
     * @param dbUrl l'URL de connexion JDBC (ex: "jdbc:postgresql://localhost:5432/mydb")
     * @param props les propriétés de connexion (user, password, etc.)
     * @throws SqlWrapperException si la connexion échoue
     */
    private SqlWrapper(String dbUrl, Properties props) {
        try {
            this.connection = DriverManager.getConnection(dbUrl, props);
        } catch (SQLException ex) {
            throw new SqlWrapperException("Error while connecting", ex);
        }
    }

    /**
     * Crée un wrapper en mode auto-commit (chaque opération est immédiatement validée).
     *
     * <p>Utilisez cette méthode pour des opérations simples ne nécessitant pas de transaction.</p>
     *
     * @param dbUrl l'URL de connexion JDBC
     * @param props les propriétés de connexion
     * @return une nouvelle instance de SqlWrapper en mode auto-commit
     */
    public static SqlWrapper withAutoCommit(String dbUrl, Properties props) {
        return new SqlWrapper(dbUrl, props);
    }

    /**
     * Crée un wrapper en mode transactionnel (auto-commit désactivé).
     *
     * <p>En mode transactionnel, vous devez explicitement appeler {@link #commit()}
     * pour valider les modifications.
     * ou laissez {@link #close()} effectuer un rollback automatique.</p>
     *
     * @param url        l'URL de connexion JDBC
     * @param properties les propriétés de connexion
     * @return une nouvelle instance de SqlWrapper en mode transactionnel
     * @throws SqlWrapperException si la création de la transaction échoue
     */
    public static SqlWrapper withTransaction(String url, Properties properties) {
        try {
            var wrapper = new SqlWrapper(url, properties);
            wrapper.connection.setAutoCommit(false);
            return wrapper;
        } catch (SQLException e) {
            throw new SqlWrapperException("Error while creating transaction", e);
        }
    }

    /**
     * Crée un nouveau statement pour une requête de lecture (SELECT).
     *
     * <p>Cette méthode vérifie qu'aucun statement n'est déjà actif. Si c'est le cas,
     * une {@link IllegalStateException} est levée.</p>
     *
     * @param sqlTemplate la requête SQL avec des paramètres '?' (ex: "SELECT * FROM users WHERE id = ?")
     * @return cette instance pour chaînage fluide
     * @throws IllegalStateException si un statement est déjà actif
     * @throws SqlWrapperException   si la création du statement échoue
     */
    public SqlWrapper newRead(String sqlTemplate) {
        requireNonNullStatement();

        try {
            this.statement = Optional.of(connection.prepareStatement(sqlTemplate));
            return this;
        } catch (SQLException e) {
            throw new SqlWrapperException("Error while creating statement", e);
        }
    }

    /**
     * Crée un nouveau statement pour une opération d'écriture (INSERT, UPDATE, DELETE).
     *
     * @param sqlTemplate       la requête SQL avec des paramètres '?'
     * @param withGeneratedKeys true pour récupérer les clés auto-générées (utile pour INSERT)
     * @return cette instance pour chaînage fluide
     * @throws IllegalStateException si un statement est déjà actif
     * @throws SqlWrapperException   si la création du statement échoue
     */
    public SqlWrapper newWrite(String sqlTemplate, boolean withGeneratedKeys) {
        requireNonNullStatement();

        try {
            this.statement = Optional.of(connection.prepareStatement(sqlTemplate,
                    withGeneratedKeys ? Statement.RETURN_GENERATED_KEYS : Statement.NO_GENERATED_KEYS));
            return this;
        } catch (SQLException e) {
            throw new SqlWrapperException("Error while creating insert statement", e);
        }
    }

    /**
     * Vérifie qu'aucun statement n'est actuellement actif.
     *
     * @throws IllegalStateException si un statement existe déjà
     */
    private void requireNonNullStatement() {
        if (statement.isPresent()) {
            throw new IllegalStateException("Statement already created");
        }
    }

    /**
     * Crée un nouveau statement pour une opération d'écriture avec récupération des clés générées.
     *
     * <p>Équivalent à {@code newWrite(sqlTemplate, true)}.</p>
     *
     * @param sqlTemplate la requête SQL avec des paramètres '?'
     * @return cette instance pour chaînage fluide
     * @throws IllegalStateException si un statement est déjà actif
     * @throws SqlWrapperException   si la création du statement échoue
     */
    public SqlWrapper newWrite(String sqlTemplate) {
        return this.newWrite(sqlTemplate, true);
    }

    /**
     * Définit un paramètre de type long (ou int) dans le statement préparé.
     *
     * @param pos la position du paramètre (commence à 1)
     * @param arg la valeur du paramètre
     * @return cette instance pour chaînage fluide
     * @throws NullPointerException si aucun statement n'est actif
     * @throws SqlWrapperException  si la définition du paramètre échoue
     */
    public SqlWrapper withParam(int pos, long arg) {
        nonNullStatement();

        try {
            statement.get().setObject(pos, arg);
            return this;
        } catch (SQLException e) {
            throw new SqlWrapperException("Error while setting param at " + pos, e);
        }
    }

    /**
     * Définit un paramètre de type String dans le statement préparé.
     *
     * @param pos la position du paramètre (commence à 1)
     * @param arg la valeur du paramètre (peut être null)
     * @return cette instance pour chaînage fluide
     * @throws NullPointerException si aucun statement n'est actif
     * @throws SqlWrapperException  si la définition du paramètre échoue
     */
    public SqlWrapper withParam(int pos, String arg) {
        nonNullStatement();
        try {
            statement.get().setString(pos, arg);
            return this;
        } catch (SQLException e) {
            throw new SqlWrapperException("Error while setting param at " + pos, e);
        }
    }

    /**
     * Exécute une requête SELECT et mappe chaque ligne du résultat.
     *
     * <p>Le statement est automatiquement fermé après l'exécution, même en cas d'erreur.</p>
     *
     * @param <T>    le type des objets retournés
     * @param mapper fonction de mapping des lignes du ResultSet vers des objets T
     * @return une liste (possiblement vide) d'objets mappés
     * @throws NullPointerException si aucun statement n'est actif
     * @throws SqlWrapperException  si l'exécution de la requête échoue
     */
    public <T> List<T> executeSelect(ResultSetMapper<T> mapper) {
        nonNullStatement();

        var result = new ArrayList<T>();
        try (var resultSet = statement.get().executeQuery()) {
            while (resultSet.next()) {
                result.add(mapper.map(resultSet));
            }
            return result;
        } catch (SQLException e) {
            throw new SqlWrapperException("Error executing select", e);
        } finally {
            closeStatement();
        }
    }

    /**
     * Ferme le statement actuel et le remet à null.
     *
     * @throws NullPointerException si aucun statement n'est actif
     * @throws SqlWrapperException  si la fermeture échoue
     */
    private void closeStatement() {
        nonNullStatement();

        try {
            statement.get().close();
            statement = Optional.empty();
        } catch (SQLException e) {
            throw new SqlWrapperException("Error while closing statement", e);
        }
    }

    public boolean executeExists() {
        nonNullStatement();
        try (var resultSet = statement.get().executeQuery()) {
            return resultSet.next();
        } catch (SQLException e) {
            throw new SqlWrapperException("Error executing select", e);
        } finally {
            closeStatement();
        }
    }

    /**
     * Exécute une requête SELECT et retourne au plus un résultat.
     *
     * <p>Si la requête retourne plusieurs lignes, seule la première est prise en compte.
     * Le statement est automatiquement fermé après l'exécution.</p>
     *
     * @param <T>    le type de l'objet retourné
     * @param mapper fonction de mapping de la ligne du ResultSet vers un objet T
     * @return un Optional contenant le résultat, ou Optional.empty() si aucune ligne
     * @throws NullPointerException si aucun statement n'est actif
     * @throws SqlWrapperException  si l'exécution de la requête échoue
     */
    public <T> Optional<T> executeSelectOne(ResultSetMapper<T> mapper) {
        nonNullStatement();

        try (var resultSet = statement.get().executeQuery()) {
            return resultSet.next()
                    ? Optional.of(mapper.map(resultSet))
                    : Optional.empty();
        } catch (SQLException e) {
            throw new SqlWrapperException("Error executing select", e);
        } finally {
            closeStatement();
        }
    }

    /**
     * Exécute une opération d'écriture (INSERT, UPDATE, DELETE) sans récupérer de clé.
     *
     * <p>Le statement est automatiquement fermé après l'exécution.</p>
     *
     * @return le nombre de lignes affectées
     * @throws NullPointerException si aucun statement n'est actif
     * @throws SqlWrapperException  si l'exécution échoue
     */
    public int execute() {
        nonNullStatement();
        try {
            return statement.get().executeUpdate();
        } catch (SQLException e) {
            throw new SqlWrapperException("Error while executing update", e);
        } finally {
            closeStatement();
        }
    }

    /**
     * Valide la transaction en cours et repasse en mode auto-commit.
     *
     * <p>Cette méthode n'a d'effet que si la connexion est en mode transactionnel
     * (créée avec {@link #withTransaction(String, Properties)}).</p>
     *
     * @throws NullPointerException si la connexion est null
     * @throws SqlWrapperException  si le commit échoue
     */
    public void commit() {
        nonNullConnection();
        try {
            if (!connection.getAutoCommit()) {
                connection.commit();
                connection.setAutoCommit(true);
            }
        } catch (SQLException e) {
            throw new SqlWrapperException("Error while committing", e);
        }
    }

    private void nonNullConnection() {
        if (connection == null) {
            throw new NullPointerException(CONNECTION_NULL_MSG);
        }
    }

    private void nonNullStatement() {
        if (statement.isEmpty()) {
            throw new NullPointerException(STATEMENT_NULL_MSG);
        }
    }

    /**
     * Ferme la connexion et libère les ressources.
     *
     * <p>Si une transaction est en cours (pas de commit effectué),
     * un rollback automatique est effectué avant la fermeture.</p>
     *
     * <p><strong>Important :</strong> Utilisez toujours ce wrapper dans un
     * try-with-resources pour garantir la fermeture de la connexion.</p>
     *
     * @throws Exception si la fermeture échoue
     */
    @Override
    public void close() throws Exception {
        if (connection != null) {
            if (!connection.getAutoCommit()) {
                connection.rollback();
            }
            connection.close();
        }
    }
}