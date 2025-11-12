package com.example.eurekabank_restjava.ec.edu.monster.vista

import android.content.Intent
import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.example.eurekabank_restjava.databinding.ActivityLoginBinding
import com.google.android.material.snackbar.Snackbar

class LoginActivity : AppCompatActivity() {
    private lateinit var b: ActivityLoginBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        b = ActivityLoginBinding.inflate(layoutInflater)
        setContentView(b.root)

        b.btnLogin.setOnClickListener {
            val u = b.txtUser.text.toString().trim()
            val p = b.txtPass.text.toString().trim()
            if (u == "MONSTER" && p == "MONSTER9") {
                startActivity(Intent(this, MenuActivity::class.java))
                finish()
            } else {
                Snackbar.make(b.root, "Credenciales incorrectas", Snackbar.LENGTH_LONG).show()
            }
        }
    }
}
