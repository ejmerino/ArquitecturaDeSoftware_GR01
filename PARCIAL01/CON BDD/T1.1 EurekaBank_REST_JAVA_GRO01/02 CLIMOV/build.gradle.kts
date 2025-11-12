// Archivo: 02 CLIMOV/build.gradle.kts
// ⚠️ ESTE es el build.gradle.kts raíz del proyecto (no el del módulo "app")

plugins {
    // Plugin Kotlin para que el módulo app pueda usar Kotlin Android
    id("org.jetbrains.kotlin.android") version "1.9.24" apply false
    id("com.android.application") version "8.4.1" apply false
}

tasks.register("clean", Delete::class) {
    delete(rootProject.buildDir)
}
