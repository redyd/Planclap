package org.helmo.planclap_admin.domains.annotations;

import org.helmo.planclap_admin.domains.core.LogLevel;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * Annotation de log
 */
@Retention(RetentionPolicy.RUNTIME)
@Target({ElementType.METHOD, ElementType.CONSTRUCTOR})
public @interface Log {
    LogLevel level() default LogLevel.INFO;
}
