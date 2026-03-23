package org.helmo.planclap_admin.domains.aspects;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.aspectj.lang.ProceedingJoinPoint;
import org.aspectj.lang.annotation.Around;
import org.aspectj.lang.annotation.Aspect;
import org.aspectj.lang.reflect.MethodSignature;
import org.helmo.planclap_admin.domains.annotations.Log;
import org.helmo.planclap_admin.domains.core.LogLevel;

import java.lang.reflect.Method;
import java.nio.file.Path;

/**
 * Classe d'auto logging grâce à l'annotation {@code @Log}
 */
@Aspect
public class AutoLogging {

    private final Logger logger = LogManager.getLogger(AutoLogging.class);

    private final static String READING_METHOD = "openForReading";
    private final static String WRITING_METHOD = "openForWriting";

    private final static String TRANSACTION_METHOD = "withTransaction";
    private final static String AUTOCOMMIT_METHOD = "withAutoCommit";

    /**
     * Log autour de chaque méthode/constructeur le début et la fin de l'exécution.
     * Indique également si l'appel a lancé une exception.
     *
     * @param joinPoint le point de jonction (méthode/constructeur avec l'annotation {@code @Log}
     * @return le résultat de jonction
     * @throws Throwable si le point de jonction lance une exception
     */
    @Around("@annotation(annotation) && execution(* *(..))")
    public Object log(ProceedingJoinPoint joinPoint, Log annotation) throws Throwable {
        MethodSignature methodSignature = (MethodSignature) joinPoint.getSignature();
        Method method = methodSignature.getMethod();
        Object[] args = joinPoint.getArgs();

        LogLevel level = annotation.level();
        String classMethod = method.getDeclaringClass().getSimpleName() + "." + method.getName();

        if (isFileOperation(method, args)) {
            return fileLogging(joinPoint, method, args, level);
        } else if (isDbOperation(method, args)) {
            return databaseLogging(joinPoint, method, args, level);
        }

        return execute(joinPoint, level, classMethod);
    }

    private Object execute(ProceedingJoinPoint joinPoint, LogLevel level, String classMethod) throws Throwable {
        log(level, "BEGIN %s".formatted(classMethod));
        Object result = basicExecute(joinPoint, classMethod);
        log(level, "END WITH SUCCESS %s".formatted(classMethod));

        return result;
    }

    /**
     * Permet de logger une ouverture de base de données.
     *
     * @param joinPoint le point de jonction
     * @param method la méthode appelée
     * @param args les arguments de la méthode
     * @param level le niveau de log
     * @return le résultat de la jonction
     * @throws Throwable si le point de jonction lance une exception
     */
    private Object databaseLogging(ProceedingJoinPoint joinPoint, Method method, Object[] args, LogLevel level) throws Throwable {
        String url = args.length > 0 ? args[0].toString() : "unknown url";
        String operation = AUTOCOMMIT_METHOD.equals(method.getName()) ? "AUTO COMMIT" : "TRANSACTION";

        log(level, "DATABASE %s OPENED WITH %s".formatted(url, operation));

        return fileExecute(joinPoint, operation, url);
    }

    /**
     * Permet de logger une ouverture de fichier.
     *
     * @param joinPoint le point de jonction
     * @param method la méthode appelée
     * @param args les arguments de la méthode
     * @param level le niveau de log
     * @return le résultat de la jonction
     * @throws Throwable si le point de jonction lance une exception
     */
    private Object fileLogging(ProceedingJoinPoint joinPoint, Method method, Object[] args, LogLevel level) throws Throwable {
        String filePath = args.length > 0 ? args[0].toString() : "unknown file";
        String operation = READING_METHOD.equals(method.getName()) ? "READING" : "WRITING";

        log(level, "%s FILE IN %s".formatted(operation, filePath));

        return fileExecute(joinPoint, operation, filePath);
    }

    private boolean isDbOperation(Method method, Object[] args) {
        String methodName = method.getName();
        return (TRANSACTION_METHOD.equals(methodName) || AUTOCOMMIT_METHOD.equals(methodName))
                && args.length > 0;
    }

    private boolean isFileOperation(Method method, Object[] args) {
        String methodName = method.getName();
        return (READING_METHOD.equals(methodName) || WRITING_METHOD.equals(methodName))
                && args.length > 0
                && args[0] instanceof Path;
    }

    private Object fileExecute(ProceedingJoinPoint joinPoint, String operation, String filePath) throws Throwable {
        Object result;
        try {
            result = joinPoint.proceed();
        } catch (Exception e) {
            log(LogLevel.ERROR, "%s FAILED AT FILE %s: %s".formatted(operation, filePath, e.getMessage()));
            throw e;
        }
        return result;
    }

    private Object basicExecute(ProceedingJoinPoint joinPoint, String classMethod) throws Throwable {
        Object result;
        try {
            result = joinPoint.proceed();
        } catch (Exception e) {
            log(LogLevel.ERROR, "Exception %s in %s".formatted(e.getClass().getSimpleName(), classMethod));
            throw e;
        }
        return result;
    }

    private void log(LogLevel level, String message) {
        switch (level) {
            case DEBUG -> logger.debug(message);
            case INFO -> logger.info(message);
            case WARN -> logger.warn(message);
            case ERROR -> logger.error(message);
            case FATAL -> logger.fatal(message);
        }
    }
}
