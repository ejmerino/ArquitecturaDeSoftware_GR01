package com.example.eurekabank_restjava.ec.edu.monster.vista

import android.os.Bundle
import android.widget.Button
import android.widget.EditText
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.eurekabank_restjava.R
import com.example.eurekabank_restjava.ec.edu.monster.model.Repo
import kotlinx.coroutines.*

class RetiroActivity : AppCompatActivity() {

    private lateinit var txtCuenta: EditText
    private lateinit var txtImporte: EditText
    private lateinit var btnOk: Button

    private val job = Job()
    private val ui = CoroutineScope(Dispatchers.Main + job)

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_retiro)

        txtCuenta = findViewById(R.id.txtCuenta)
        txtImporte = findViewById(R.id.txtImporte)
        btnOk = findViewById(R.id.btnOk)

        btnOk.setOnClickListener { hacerRetiro() }
    }

    private fun hacerRetiro() {
        val cuenta = txtCuenta.text.toString().trim()
        val importeStr = txtImporte.text.toString().trim()

        if (cuenta.isEmpty() || importeStr.isEmpty()) {
            Toast.makeText(this, "Ingresa cuenta e importe", Toast.LENGTH_SHORT).show()
            return
        }
        val importe = importeStr.toDoubleOrNull()
        if (importe == null || importe <= 0.0) {
            Toast.makeText(this, "Importe inválido", Toast.LENGTH_SHORT).show()
            return
        }

        ui.launch {
            try {
                val resp = withContext(Dispatchers.IO) {
                    Repo.retiro(cuenta, importe)
                }
                if (resp.isSuccessful) {
                    Toast.makeText(this@RetiroActivity, "Retiro realizado", Toast.LENGTH_LONG).show()
                    txtImporte.setText("")
                } else {
                    Toast.makeText(
                        this@RetiroActivity,
                        "Error ${resp.code()}: ${resp.message()}",
                        Toast.LENGTH_LONG
                    ).show()
                }
            } catch (e: Exception) {
                Toast.makeText(this@RetiroActivity, "Fallo: ${e.message}", Toast.LENGTH_LONG).show()
                e.printStackTrace()
            }
        }
    }

    override fun onDestroy() {
        super.onDestroy()
        job.cancel()
    }
}
