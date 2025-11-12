package com.example.eurekabank_restjava.ec.edu.monster.model

// Lo que llega del servicio REST
data class MovimientoDto(
    val fecha: String,   // ejemplo: "2025-11-10 00:00:00.0" o "2025-11-10"
    val tipo: String,    // "Deposito", "Retiro", "Cancelar Cuenta", "Apertura de Cuenta"
    val importe: Double  // monto
)
