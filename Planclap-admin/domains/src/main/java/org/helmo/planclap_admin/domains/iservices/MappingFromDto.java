package org.helmo.planclap_admin.domains.iservices;

@FunctionalInterface
public interface MappingFromDto<R, T> {
    R mapFromDto(T t);
}
