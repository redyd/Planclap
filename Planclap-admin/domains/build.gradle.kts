plugins {
    id("helmo-common-conventions")
    `java-library`
    alias(libs.plugins.freefair.aspectj)
}

dependencies {
    testImplementation(libs.mockito)
    api(libs.aspectj)
}