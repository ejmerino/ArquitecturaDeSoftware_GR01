package com.example.eurekabank_restjava.ec.edu.monster.model

object Repo {
    suspend fun movimientos(cuenta: String) =
        Api.service.movimientos(cuenta)

    // === PRUEBA PRIMERO ESTA (A: FORM) ===
    suspend fun deposito(cuenta: String, importe: Double) =
        Api.service.depositoForm(cuenta, importe)

    suspend fun retiro(cuenta: String, importe: Double) =
        Api.service.retiroForm(cuenta, importe)

    suspend fun transferencia(origen: String, destino: String, importe: Double) =
        Api.service.transferenciaForm(origen, destino, importe)

    // === SI SIGUE 500, COMENTA LAS 3 ANTERIORES Y USA ESTAS (B: PATH) ===
    // suspend fun deposito(cuenta: String, importe: Double) =
    //     Api.service.depositoPath(cuenta, importe)
    //
    // suspend fun retiro(cuenta: String, importe: Double) =
    //     Api.service.retiroPath(cuenta, importe)
    //
    // suspend fun transferencia(origen: String, destino: String, importe: Double) =
    //     Api.service.transferenciaPath(origen, destino, importe)
}
