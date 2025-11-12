package com.example.eurekabank_restjava.ec.edu.monster.vista

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.eurekabank_restjava.R
import java.text.NumberFormat

class MovimientosAdapter(
    private val nf: NumberFormat
) : RecyclerView.Adapter<MovimientosAdapter.VH>() {

    private val data = ArrayList<MovimientoUI>()

    fun submit(list: List<MovimientoUI>) {
        data.clear()
        data.addAll(list)
        notifyDataSetChanged()
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): VH {
        val v = LayoutInflater.from(parent.context).inflate(R.layout.item_movimiento, parent, false)
        return VH(v)
    }

    override fun onBindViewHolder(holder: VH, position: Int) {
        holder.bind(data[position], nf)
    }

    override fun getItemCount() = data.size

    class VH(v: View) : RecyclerView.ViewHolder(v) {
        private val lblFecha: TextView = v.findViewById(R.id.lblFecha)
        private val lblTipo: TextView = v.findViewById(R.id.lblTipo)
        private val lblImporte: TextView = v.findViewById(R.id.lblImporte)
        private val lblSaldo: TextView = v.findViewById(R.id.lblSaldo)

        fun bind(m: MovimientoUI, nf: NumberFormat) {
            lblFecha.text = m.fechaStr
            lblTipo.text = m.tipo
            lblImporte.text = nf.format(m.importe)
            lblSaldo.text = nf.format(m.saldo)
        }
    }
}
