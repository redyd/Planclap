plugins {
    id("helmo-common-conventions")
    id("jacoco-report-aggregation")
    application
}


repositories {
    // Use Maven Central for resolving dependencies.
    mavenCentral()
}

dependencies {
    implementation(project(":presentations"))
    implementation(project(":infrastructures"))
    implementation(project(":views"))
    implementation(project(":domains"))

    // https://mvnrepository.com/artifact/net.sf.jopt-simple/jopt-simple
    implementation(libs.jopt)

    // use to read args
    testImplementation(libs.arch)
}

application {
    // Define the main class for the application.
    mainClass = "org.helmo.planclap_admin.app.Program"
}

// Apply a specific Java toolchain to ease working on different environments.
java {
    toolchain {
        languageVersion = JavaLanguageVersion.of(21)
    }
}

tasks.register<AggregatePmdReportsTask>("aggregatePmdReports") {
    projectsFiles = files(
        "../app/build/reports/pmd/main.xml",
        "../domains/build/reports/pmd/main.xml",
        "../presentations/build/reports/pmd/main.xml",
        "../infrastructures/build/reports/pmd/main.xml",
        "../views/build/reports/pmd/main.xml"
    )

    reportFile = file("build/reports/pmd/agregatedPmdViolations.html")

    dependsOn(":app:pmdMain",":domains:build",":presentations:build",":infrastructures:build",":views:build")
}

tasks.named("check") {
    dependsOn(tasks.named("testCodeCoverageReport", JacocoReport::class.java))
    dependsOn(tasks.named("aggregatePmdReports", AggregatePmdReportsTask::class.java))
}

tasks.run.configure {
    standardInput = System.`in`
}

tasks.named<Test>("test") {
    // Use JUnit Platform for unit tests.
    useJUnitPlatform()
}
