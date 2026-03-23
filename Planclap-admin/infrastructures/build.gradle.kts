plugins {
    id("helmo-common-conventions")
    `java-library`
    alias(libs.plugins.freefair.aspectj)
}

dependencies {
    aspect(project(":domains"))

    implementation(libs.gson)
    implementation(libs.mysql)

    testImplementation(libs.sqlite)
    testImplementation(libs.mockito)
}
