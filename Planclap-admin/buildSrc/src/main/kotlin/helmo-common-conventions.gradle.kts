plugins {
    // Apply the java Plugin to add support for Java.
    java
    pmd
    jacoco
}

repositories {
    // Use Maven Central for resolving dependencies.
    mavenCentral()
}

dependencies {
    constraints {
        testImplementation("org.mockito:mockito-core:5.19.0")
    }

    implementation("org.apache.logging.log4j:log4j-core:2.25.1")
    implementation("org.apache.logging.log4j:log4j-api:2.25.1")

    implementation("org.apache.commons:commons-text:1.14.0")

    // Use JUnit Jupiter for testing.
    testImplementation("org.junit.jupiter:junit-jupiter:5.13.+")
    testImplementation("org.assertj:assertj-core:3.27.4")

    testRuntimeOnly("org.junit.platform:junit-platform-launcher")
}

// Apply a specific Java toolchain to ease working on different environments.
java {
    toolchain {
        languageVersion = JavaLanguageVersion.of(21)
    }
}

tasks.named<Test>("test") {
    // Use JUnit Platform for unit tests.
    useJUnitPlatform()
}

pmd {
    toolVersion = "7.16.0"
    ruleSets = listOf()
    ruleSetConfig = resources.text.fromFile("config/pmd/ue36-ruleset.xml")
    maxFailures = 15
    isConsoleOutput = true
    isIgnoreFailures = true
}

tasks.pmdTest {
    isEnabled = false
}