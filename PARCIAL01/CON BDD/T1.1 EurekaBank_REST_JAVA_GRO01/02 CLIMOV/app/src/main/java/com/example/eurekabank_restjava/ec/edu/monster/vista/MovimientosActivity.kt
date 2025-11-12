package com.example.eurekabank_restjava.ec.edu.monster.vista

import android.os.Bundle
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.eurekabank_restjava.R
import com.example.eurekabank_restjava.ec.edu.monster.model.MovimientoDto
import com.example.eurekabank_restjava.ec.edu.monster.model.Repo
import kotlinx.coroutines.*
import java.text.NumberFormat
import java.text.ParsePosition
import java.text.SimpleDateFormat
import java.util.*

data class MovimientoUI(
    val fecha: Date,
    val fechaStr: String,
    val tipo: String,
    val importe: Double,
    val saldo: Double,
    val seq: Int                // para desempatar por orden original
)

class MovimientosActivity : AppCompatActivity() {

    private lateinit var edtCuenta: EditText
    private lateinit var btnBuscar: Button
    private lateinit var lblSaldoActual: TextView
    private lateinit var lblTotal: TextView
    private lateinit var rv: RecyclerView
    private lateinit var adapter: MovimientosAdapter

    private val nf = NumberFormat.getCurrencyInstance(Locale("es", "EC"))
    private val uiScope = CoroutineScope(Dispatchers.Main + SupervisorJob())

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_movimientos)

        edtCuenta = findViewById(R.id.txtCuenta)
        btnBuscar = findViewById(R.id.btnBuscar)
        lblSaldoActual = findViewById(R.id.lblSaldoActual)
        lblTotal = findViewById(R.id.lblTotal)
        rv = findViewById(R.id.recycler)

        rv.layoutManager = LinearLayoutManager(this)
        rv.setHasFixedSize(true)
        adapter = MovimientosAdapter(nf)
        rv.adapter = adapter

        btnBuscar.setOnClickListener { cargar() }
    }

    private fun cargar() {
        val cuenta = edtCuenta.text.toString().trim()
        if (cuenta.isEmpty()) {
            Toast.makeText(this, "Ingrese una cuenta", Toast.LENGTH_SHORT).show()
            return
        }

        btnBuscar.isEnabled = false
        lblSaldoActual.text = "Saldo de la cuenta: —"
        lblTotal.text = "Número de movimientos: 0"
        adapter.submit(emptyList())

        uiScope.launch {
            try {
                // 1) Traer movimientos (TODOS)
                val raw = Repo.movimientos(cuenta)   // List<MovimientoDto>

                // 2) Mapear, conservar índice original (seq) para order estable
                val mappedAsc = raw.mapIndexedNotNull { idx, it -> toUI(it, idx) }
                    .sortedWith(compareBy<MovimientoUI> { it.fecha }.thenBy { it.seq }) // ASC estable

                // 3) Calcular saldos acumulados en ASC (apertura->último)
                val withSaldoAsc = calcularSaldos(mappedAsc)

                // 4) Saldo actual
                val saldoActual = withSaldoAsc.lastOrNull()?.saldo ?: 0.0
                lblSaldoActual.text = "Saldo de la cuenta: ${nf.format(saldoActual)}"

                // 5) Total
                lblTotal.text = "Número de movimientos: ${withSaldoAsc.size}"

                // 6) Mostrar en DESC (más reciente primero). Para empates por misma fecha,
                //    usamos seq DESC para que lo último añadido quede arriba.
                val desc = withSaldoAsc.sortedWith(
                    compareByDescending<MovimientoUI> { it.fecha }.thenByDescending { it.seq }
                )

                adapter.submit(desc)

            } catch (ex: Exception) {
                Toast.makeText(this@MovimientosActivity, "Error: ${ex.message}", Toast.LENGTH_LONG).show()
            } finally {
                btnBuscar.isEnabled = true
            }
        }
    }

    private fun calcularSaldos(asc: List<MovimientoUI>): List<MovimientoUI> {
        var saldo = 0.0
        val out = ArrayList<MovimientoUI>(asc.size)
        for (m in asc) {
            val delta = when (m.tipo.trim().lowercase(Locale.ROOT)) {
                "deposito", "apertura de cuenta" -> m.importe
                "retiro", "cancelar cuenta" -> -m.importe
                else -> 0.0
            }
            saldo += delta
            out.add(m.copy(saldo = saldo))
        }
        return out
    }

    private fun parseFecha(s: String): Date? {
        val patterns = arrayOf(
            "yyyy-MM-dd HH:mm:ss.S",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd"
        )
        for (p in patterns) {
            val sdf = SimpleDateFormat(p, Locale.US).apply {
                timeZone = TimeZone.getDefault()
            }
            val d = sdf.parse(s, ParsePosition(0))
            if (d != null) return d
        }
        return null
    }

    private fun toUI(dto: MovimientoDto, seq: Int): MovimientoUI? {
        val d = parseFecha(dto.fecha) ?: return null
        val fechaLegible = SimpleDateFormat("yyyy-MM-dd", Locale.US).format(d)
        return MovimientoUI(
            fecha = d,
            fechaStr = fechaLegible,
            tipo = dto.tipo,
            importe = dto.importe,
            saldo = 0.0,
            seq = seq
        )
    }
}
