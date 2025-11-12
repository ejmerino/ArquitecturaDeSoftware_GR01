package com.example.eurekabank_restjava.ec.edu.monster.vista

import android.os.Bundle
import android.widget.Button
import android.widget.EditText
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.eurekabank_restjava.R
import com.example.eurekabank_restjava.ec.edu.monster.model.Repo
import kotlinx.coroutines.*

class TransferenciaActivity : AppCompatActivity() {

    private lateinit var txtOrigen: EditText
    private lateinit var txtDestino: EditText
    private lateinit var txtImporte: EditText
    private lateinit var btnOk: Button

    private val job = Job()
    private val ui = CoroutineScope(Dispatchers.Main + job)

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_transferencia)

        txtOrigen = findViewById(R.id.txtOrigen)
        txtDestino = findViewById(R.id.txtDestino)
        txtImporte = findViewById(R.id.txtImporte)
        btnOk = findViewById(R.id.btnOk)

        btnOk.setOnClickListener { hacerTransferencia() }
    }

    private fun hacerTransferencia() {
        val origen = txtOrigen.text.toString().trim()
        val destino = txtDestino.text.toString().trim()
        val importeStr = txtImporte.text.toString().trim()

        if (origen.isEmpty() || destino.isEmpty() || importeStr.isEmpty()) {
            Toast.makeText(this, "Ingresa origen, destino e importe", Toast.LENGTH_SHORT).show()
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
                    Repo.transferencia(origen, destino, importe)
                }
                if (resp.isSuccessful) {
                    Toast.makeText(this@TransferenciaActivity, "Transferencia realizada", Toast.LENGTH_LONG).show()
                    txtImporte.setText("")
                } else {
                    Toast.makeText(
                        this@TransferenciaActivity,
                        "Error ${resp.code()}: ${resp.message()}",
                        Toast.LENGTH_LONG
                    ).show()
                }
            } catch (e: Exception) {
                Toast.makeText(this@TransferenciaActivity, "Fallo: ${e.message}", Toast.LENGTH_LONG).show()
                e.printStackTrace()
            }
        }
    }

    override fun onDestroy() {
        super.onDestroy()
        job.cancel()
    }
}
