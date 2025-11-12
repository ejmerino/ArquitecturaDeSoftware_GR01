package com.example.eurekabank_restjava.ec.edu.monster.vista

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import androidx.appcompat.app.AppCompatActivity
import com.example.eurekabank_restjava.R

class MenuActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_menu)

        findViewById<Button>(R.id.btnMovimientos).setOnClickListener {
            startActivity(Intent(this, MovimientosActivity::class.java))
        }

        findViewById<Button>(R.id.btnDeposito).setOnClickListener {
            startActivity(Intent(this, DepositoActivity::class.java))
        }

        findViewById<Button>(R.id.btnRetiro).setOnClickListener {
            startActivity(Intent(this, RetiroActivity::class.java))
        }

        findViewById<Button>(R.id.btnTransferencia).setOnClickListener {
            startActivity(Intent(this, TransferenciaActivity::class.java))
        }

        findViewById<Button>(R.id.btnSalir).setOnClickListener {
            // Cierra todas las activities de la app
            finishAffinity()
        }
    }
}
