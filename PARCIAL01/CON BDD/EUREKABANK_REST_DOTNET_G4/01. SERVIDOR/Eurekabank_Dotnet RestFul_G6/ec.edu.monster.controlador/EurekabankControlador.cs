using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;

using Eurekabank_Dotnet_RestFul_G6.ec.edu.monster.modelo;

namespace Eurekabank_Dotnet_RestFul_G6.ec.edu.monster.controlador
{
    [Route("api/[controller]")]
    [ApiController]
    public class EurekabankControlador : ControllerBase
    {
        private readonly string cadenaSQL;

        public EurekabankControlador(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }

        // ===== HASH SHA-256 (64 HEX en MAYÚSCULAS) =====
        private static string EncriptarContrasena(string contrasena)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasena ?? string.Empty));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString().ToUpperInvariant();
        }

        // ====== LOGIN por NOMBRE + CONTRASEÑA ======
        public class LoginByNameDto { public string Nombre { get; set; } public string Contrasena { get; set; } }

        [HttpPost]
        [Route("Usuarios/Entrar")]
        public IActionResult EntrarPorNombre([FromBody] LoginByNameDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Contrasena))
                return BadRequest(new { mensaje = "Nombre y contraseña son requeridos." });

            try
            {
                using var conexion = new SqlConnection(cadenaSQL);
                conexion.Open();

                const string sql = @"
                    SELECT TOP 1 *
                    FROM USUARIOS
                    WHERE USU_NOMBRE = @Nombre
                      AND USU_CONTRASENA = @Contrasena;";

                using var cmd = new SqlCommand(sql, conexion);
                cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = dto.Nombre.Trim();
                cmd.Parameters.Add("@Contrasena", SqlDbType.NVarChar, 64).Value = EncriptarContrasena(dto.Contrasena);

                Usuario usuario = null;
                using var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    usuario = new Usuario
                    {
                        UsuId = Convert.ToInt32(rd["USU_ID"]),
                        UsuNombre = Convert.ToString(rd["USU_NOMBRE"]),
                        UsuApellido = Convert.ToString(rd["USU_APELLIDO"]),
                        UsuCorreo = Convert.ToString(rd["USU_CORREO"]),
                        UsuTelefono = Convert.ToString(rd["USU_TELEFONO"]),
                        UsuCedula = Convert.ToString(rd["USU_CEDULA"]),
                        UsuRol = Convert.ToString(rd["USU_ROL"])
                    };
                }

                return usuario != null
                    ? Ok(new { mensaje = "ok", response = usuario })
                    : NotFound(new { mensaje = "Usuario no encontrado o contraseña incorrecta." });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { mensaje = e.Message });
            }
        }

        // ===== MOVIMIENTOS (listar por número de cuenta) =====
        [HttpGet]
        [Route("Movimientos")]
        public IActionResult Movimientos(string numeroCuenta)
        {
            var movimientos = new List<Movimiento>();
            try
            {
                using var conexion = new SqlConnection(cadenaSQL);
                conexion.Open();

                string sql = "SELECT M.MOV_ID, M.CU_ID, M.MOV_FECHA, M.MOV_TIPO, M.MOV_VALOR, M.MOV_SALDO_FINAL " +
                             "FROM MOVIMIENTOS M INNER JOIN CUENTAS C ON M.CU_ID = C.CU_ID " +
                             "WHERE C.CU_NUMERO = @NumeroCuenta";

                using var cmd = new SqlCommand(sql, conexion);
                cmd.Parameters.Add("@NumeroCuenta", SqlDbType.NVarChar, 50).Value = (numeroCuenta ?? string.Empty).Trim();

                using var rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    movimientos.Add(new Movimiento()
                    {
                        MovId = Convert.ToInt32(rd["MOV_ID"]),
                        CuId = Convert.ToInt32(rd["CU_ID"]),
                        MovFecha = Convert.ToDateTime(rd["MOV_FECHA"]),
                        MovTipo = Convert.ToString(rd["MOV_TIPO"]),
                        MovValor = Convert.ToSingle(rd["MOV_VALOR"]),
                        MovSaldoFinal = Convert.ToSingle(rd["MOV_SALDO_FINAL"])
                    });
                }
                return Ok(new { mensaje = "ok", response = movimientos });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { mensaje = e.Message, response = movimientos });
            }
        }

        // ===== CUENTAS (listar o consultar por número) =====
        [HttpGet]
        [Route("Cuentas")]
        public IActionResult Cuentas(string? numeroCuenta)
        {
            var cuentas = new List<Cuenta>();
            try
            {
                using var conexion = new SqlConnection(cadenaSQL);
                conexion.Open();

                string sql = "SELECT CU_ID, CLI_ID, CU_NUMERO, CU_TIPO, CU_SALDO FROM CUENTAS";
                if (!string.IsNullOrEmpty(numeroCuenta)) sql += " WHERE CU_NUMERO = @NumeroCuenta";

                using var cmd = new SqlCommand(sql, conexion);
                if (!string.IsNullOrEmpty(numeroCuenta))
                    cmd.Parameters.Add("@NumeroCuenta", SqlDbType.NVarChar, 50).Value = numeroCuenta.Trim();

                using var rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    cuentas.Add(new Cuenta()
                    {
                        CuId = Convert.ToInt32(rd["CU_ID"]),
                        CliId = Convert.ToInt32(rd["CLI_ID"]),
                        CuNumero = Convert.ToString(rd["CU_NUMERO"]),
                        CuTipo = Convert.ToString(rd["CU_TIPO"]),
                        CuSaldo = Convert.ToSingle(rd["CU_SALDO"])
                    });
                }
                return Ok(new { mensaje = "ok", response = cuentas });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { mensaje = e.Message, response = cuentas });
            }
        }

        // ===== TRANSFERENCIA entre cuentas =====
        [HttpPost]
        [Route("Transferencia")]
        public IActionResult Transferencia(string cuentaOrigen, string cuentaDestino, float valor)
        {
            if (string.IsNullOrWhiteSpace(cuentaOrigen) || string.IsNullOrWhiteSpace(cuentaDestino) || valor <= 0)
                return BadRequest(new { mensaje = "Cuentas válidas y valor > 0 son obligatorios." });

            try
            {
                using var conexion = new SqlConnection(cadenaSQL);
                conexion.Open();
                using var tx = conexion.BeginTransaction();

                try
                {
                    // Saldo origen
                    var cmdSaldoOrigen = new SqlCommand("SELECT CU_ID, CU_SALDO FROM CUENTAS WHERE CU_NUMERO = @n", conexion, tx);
                    cmdSaldoOrigen.Parameters.Add("@n", SqlDbType.NVarChar, 50).Value = cuentaOrigen.Trim();
                    int cuIdOrigen; float saldoOrigen;
                    using (var rd = cmdSaldoOrigen.ExecuteReader())
                    {
                        if (!rd.Read()) throw new Exception("Cuenta origen no encontrada.");
                        cuIdOrigen = Convert.ToInt32(rd["CU_ID"]);
                        saldoOrigen = Convert.ToSingle(rd["CU_SALDO"]);
                    }
                    if (saldoOrigen < valor) throw new Exception("Saldo insuficiente en la cuenta de origen.");

                    // Saldo destino
                    var cmdSaldoDestino = new SqlCommand("SELECT CU_ID, CU_SALDO FROM CUENTAS WHERE CU_NUMERO = @n", conexion, tx);
                    cmdSaldoDestino.Parameters.Add("@n", SqlDbType.NVarChar, 50).Value = cuentaDestino.Trim();
                    int cuIdDestino; float saldoDestino;
                    using (var rd = cmdSaldoDestino.ExecuteReader())
                    {
                        if (!rd.Read()) throw new Exception("Cuenta destino no encontrada.");
                        cuIdDestino = Convert.ToInt32(rd["CU_ID"]);
                        saldoDestino = Convert.ToSingle(rd["CU_SALDO"]);
                    }

                    // Actualizar saldos
                    var updOrigen = new SqlCommand("UPDATE CUENTAS SET CU_SALDO = CU_SALDO - @v WHERE CU_ID = @id", conexion, tx);
                    updOrigen.Parameters.Add("@v", SqlDbType.Float).Value = valor;
                    updOrigen.Parameters.Add("@id", SqlDbType.Int).Value = cuIdOrigen;
                    updOrigen.ExecuteNonQuery();

                    var updDestino = new SqlCommand("UPDATE CUENTAS SET CU_SALDO = CU_SALDO + @v WHERE CU_ID = @id", conexion, tx);
                    updDestino.Parameters.Add("@v", SqlDbType.Float).Value = valor;
                    updDestino.Parameters.Add("@id", SqlDbType.Int).Value = cuIdDestino;
                    updDestino.ExecuteNonQuery();

                    var nuevoSaldoOrigen = saldoOrigen - valor;
                    var nuevoSaldoDestino = saldoDestino + valor;

                    // Registrar movimientos
                    var ins = new SqlCommand(@"
                        INSERT INTO MOVIMIENTOS (CU_ID, MOV_FECHA, MOV_TIPO, MOV_VALOR, MOV_SALDO_FINAL)
                        VALUES (@idor, GETDATE(), 'TRANSFERENCIA', @valNeg, @sor);
                        INSERT INTO MOVIMIENTOS (CU_ID, MOV_FECHA, MOV_TIPO, MOV_VALOR, MOV_SALDO_FINAL)
                        VALUES (@idds, GETDATE(), 'TRANSFERENCIA', @valPos, @sds);", conexion, tx);
                    ins.Parameters.Add("@idor", SqlDbType.Int).Value = cuIdOrigen;
                    ins.Parameters.Add("@valNeg", SqlDbType.Float).Value = -valor;
                    ins.Parameters.Add("@sor", SqlDbType.Float).Value = nuevoSaldoOrigen;
                    ins.Parameters.Add("@idds", SqlDbType.Int).Value = cuIdDestino;
                    ins.Parameters.Add("@valPos", SqlDbType.Float).Value = valor;
                    ins.Parameters.Add("@sds", SqlDbType.Float).Value = nuevoSaldoDestino;
                    ins.ExecuteNonQuery();

                    tx.Commit();
                    return Ok(new { mensaje = "Transferencia realizada con éxito." });
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    return StatusCode(500, new { mensaje = ex.Message });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { mensaje = e.Message });
            }
        }

        // ===== DEPÓSITO =====
        [HttpPost]
        [Route("Movimientos/Deposito")]
        public IActionResult Deposito(string numeroCuenta, float valor)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta) || valor <= 0)
                return BadRequest(new { mensaje = "Cuenta y valor > 0 son obligatorios." });

            try
            {
                using var cn = new SqlConnection(cadenaSQL);
                cn.Open();
                using var tx = cn.BeginTransaction();

                try
                {
                    var getSaldo = new SqlCommand("SELECT CU_ID, CU_SALDO FROM CUENTAS WHERE CU_NUMERO = @n", cn, tx);
                    getSaldo.Parameters.Add("@n", SqlDbType.NVarChar, 50).Value = numeroCuenta.Trim();

                    int cuId; float saldo;
                    using (var rd = getSaldo.ExecuteReader())
                    {
                        if (!rd.Read()) throw new Exception("Cuenta no encontrada.");
                        cuId = Convert.ToInt32(rd["CU_ID"]);
                        saldo = Convert.ToSingle(rd["CU_SALDO"]);
                    }

                    var upd = new SqlCommand("UPDATE CUENTAS SET CU_SALDO = CU_SALDO + @v WHERE CU_ID = @id", cn, tx);
                    upd.Parameters.Add("@v", SqlDbType.Float).Value = valor;
                    upd.Parameters.Add("@id", SqlDbType.Int).Value = cuId;
                    upd.ExecuteNonQuery();

                    var nuevoSaldo = saldo + valor;

                    var ins = new SqlCommand(@"
                        INSERT INTO MOVIMIENTOS (CU_ID, MOV_FECHA, MOV_TIPO, MOV_VALOR, MOV_SALDO_FINAL)
                        VALUES (@id, GETDATE(), 'DEPOSITO', @v, @sf)", cn, tx);
                    ins.Parameters.Add("@id", SqlDbType.Int).Value = cuId;
                    ins.Parameters.Add("@v", SqlDbType.Float).Value = valor;
                    ins.Parameters.Add("@sf", SqlDbType.Float).Value = nuevoSaldo;
                    ins.ExecuteNonQuery();

                    tx.Commit();
                    return Ok(new { mensaje = "Depósito realizado.", saldo = nuevoSaldo });
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    return StatusCode(500, new { mensaje = ex.Message });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { mensaje = e.Message });
            }
        }

        // ===== RETIRO =====
        [HttpPost]
        [Route("Movimientos/Retiro")]
        public IActionResult Retiro(string numeroCuenta, float valor)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta) || valor <= 0)
                return BadRequest(new { mensaje = "Cuenta y valor > 0 son obligatorios." });

            try
            {
                using var cn = new SqlConnection(cadenaSQL);
                cn.Open();
                using var tx = cn.BeginTransaction();

                try
                {
                    var getSaldo = new SqlCommand("SELECT CU_ID, CU_SALDO FROM CUENTAS WHERE CU_NUMERO = @n", cn, tx);
                    getSaldo.Parameters.Add("@n", SqlDbType.NVarChar, 50).Value = numeroCuenta.Trim();

                    int cuId; float saldo;
                    using (var rd = getSaldo.ExecuteReader())
                    {
                        if (!rd.Read()) throw new Exception("Cuenta no encontrada.");
                        cuId = Convert.ToInt32(rd["CU_ID"]);
                        saldo = Convert.ToSingle(rd["CU_SALDO"]);
                    }

                    if (saldo < valor) return BadRequest(new { mensaje = "Saldo insuficiente." });

                    var upd = new SqlCommand("UPDATE CUENTAS SET CU_SALDO = CU_SALDO - @v WHERE CU_ID = @id", cn, tx);
                    upd.Parameters.Add("@v", SqlDbType.Float).Value = valor;
                    upd.Parameters.Add("@id", SqlDbType.Int).Value = cuId;
                    upd.ExecuteNonQuery();

                    var nuevoSaldo = saldo - valor;

                    var ins = new SqlCommand(@"
                        INSERT INTO MOVIMIENTOS (CU_ID, MOV_FECHA, MOV_TIPO, MOV_VALOR, MOV_SALDO_FINAL)
                        VALUES (@id, GETDATE(), 'RETIRO', @v, @sf)", cn, tx);
                    ins.Parameters.Add("@id", SqlDbType.Int).Value = cuId;
                    ins.Parameters.Add("@v", SqlDbType.Float).Value = valor;
                    ins.Parameters.Add("@sf", SqlDbType.Float).Value = nuevoSaldo;
                    ins.ExecuteNonQuery();

                    tx.Commit();
                    return Ok(new { mensaje = "Retiro realizado.", saldo = nuevoSaldo });
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    return StatusCode(500, new { mensaje = ex.Message });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { mensaje = e.Message });
            }
        }

        // ===== USUARIOS: Crear =====
        [HttpPost]
        [Route("Usuarios")]
        public IActionResult AgregarUsuario(string nombre, string apellido, string correo, string telefono, string cedula, string rol, string contrasena)
        {
            try
            {
                using var conexion = new SqlConnection(cadenaSQL);
                conexion.Open();

                string sqlInsert = @"
                    INSERT INTO USUARIOS (USU_NOMBRE, USU_APELLIDO, USU_CORREO, USU_TELEFONO, USU_CEDULA, USU_ROL, USU_CONTRASENA)
                    VALUES (@Nombre, @Apellido, @Correo, @Telefono, @Cedula, @Rol, @Contrasena);
                    SELECT CAST(SCOPE_IDENTITY() AS int);";

                using var cmdInsert = new SqlCommand(sqlInsert, conexion);
                cmdInsert.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = (nombre ?? string.Empty).Trim();
                cmdInsert.Parameters.Add("@Apellido", SqlDbType.NVarChar, 100).Value = (apellido ?? string.Empty).Trim();
                cmdInsert.Parameters.Add("@Correo", SqlDbType.NVarChar, 255).Value = (correo ?? string.Empty).Trim();
                cmdInsert.Parameters.Add("@Telefono", SqlDbType.NVarChar, 20).Value = (telefono ?? string.Empty).Trim();
                cmdInsert.Parameters.Add("@Cedula", SqlDbType.NVarChar, 20).Value = (cedula ?? string.Empty).Trim();
                cmdInsert.Parameters.Add("@Rol", SqlDbType.NVarChar, 20).Value = (rol ?? string.Empty).Trim();

                string contrasenaEncriptada = EncriptarContrasena(contrasena);
                cmdInsert.Parameters.Add("@Contrasena", SqlDbType.NVarChar, 64).Value = contrasenaEncriptada;

                int usuarioId = (int)cmdInsert.ExecuteScalar();

                string sqlUpdateCliente = "UPDATE CLIENTES SET USU_ID = @UsuarioId WHERE CLI_CEDULA = @Cedula";
                using var cmdUpdateCliente = new SqlCommand(sqlUpdateCliente, conexion);
                cmdUpdateCliente.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                cmdUpdateCliente.Parameters.Add("@Cedula", SqlDbType.NVarChar, 20).Value = (cedula ?? string.Empty).Trim();

                int filasAfectadas = cmdUpdateCliente.ExecuteNonQuery();

                if (filasAfectadas > 0)
                    return StatusCode(201, new { mensaje = "Usuario creado y cliente actualizado exitosamente." });
                else
                    return StatusCode(201, new { mensaje = "Usuario creado exitosamente, pero no se encontró un cliente con la misma cédula." });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { mensaje = e.Message });
            }
        }

        // ===== USUARIOS: Consultar por ID =====
        [HttpGet]
        [Route("Usuarios/{id}")]
        public IActionResult ConsultarUsuario(int id)
        {
            Usuario usuario = null;

            try
            {
                using var conexion = new SqlConnection(cadenaSQL);
                conexion.Open();

                string sql = "SELECT * FROM USUARIOS WHERE USU_ID = @Id";
                using var cmd = new SqlCommand(sql, conexion);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                using var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    usuario = new Usuario()
                    {
                        UsuId = Convert.ToInt32(rd["USU_ID"]),
                        UsuNombre = Convert.ToString(rd["USU_NOMBRE"]),
                        UsuApellido = Convert.ToString(rd["USU_APELLIDO"]),
                        UsuCorreo = Convert.ToString(rd["USU_CORREO"]),
                        UsuTelefono = Convert.ToString(rd["USU_TELEFONO"]),
                        UsuCedula = Convert.ToString(rd["USU_CEDULA"]),
                        UsuRol = Convert.ToString(rd["USU_ROL"])
                    };
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { mensaje = e.Message });
            }

            return usuario != null
                ? Ok(new { mensaje = "ok", response = usuario })
                : NotFound(new { mensaje = "Usuario no encontrado." });
        }

        // ===== USUARIOS: Eliminar =====
        [HttpDelete]
        [Route("Usuarios/{id}")]
        public IActionResult EliminarUsuario(int id)
        {
            try
            {
                using var conexion = new SqlConnection(cadenaSQL);
                conexion.Open();

                string sql = "DELETE FROM USUARIOS WHERE USU_ID = @Id";
                using var cmd = new SqlCommand(sql, conexion);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                var filasAfectadas = cmd.ExecuteNonQuery();
                return filasAfectadas > 0
                    ? Ok(new { mensaje = "Usuario eliminado exitosamente." })
                    : NotFound(new { mensaje = "Usuario no encontrado." });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { mensaje = e.Message });
            }
        }

        // ===== USUARIOS: Modificar =====
        [HttpPut]
        [Route("Usuarios/{id}")]
        public IActionResult ModificarUsuario(int id, string nombre, string apellido, string correo, string telefono, string cedula, string rol, string contrasena)
        {
            try
            {
                using var conexion = new SqlConnection(cadenaSQL);
                conexion.Open();

                string contrasenaEncriptada = EncriptarContrasena(contrasena);

                string sqlUpdateUsuario = @"
                    UPDATE USUARIOS
                    SET USU_NOMBRE = @Nombre,
                        USU_APELLIDO = @Apellido,
                        USU_CORREO = @Correo,
                        USU_TELEFONO = @Telefono,
                        USU_CEDULA = @Cedula,
                        USU_ROL = @Rol,
                        USU_CONTRASENA = @Contrasena
                    WHERE USU_ID = @Id";

                using var cmdUpdateUsuario = new SqlCommand(sqlUpdateUsuario, conexion);
                cmdUpdateUsuario.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmdUpdateUsuario.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = (nombre ?? string.Empty).Trim();
                cmdUpdateUsuario.Parameters.Add("@Apellido", SqlDbType.NVarChar, 100).Value = (apellido ?? string.Empty).Trim();
                cmdUpdateUsuario.Parameters.Add("@Correo", SqlDbType.NVarChar, 255).Value = (correo ?? string.Empty).Trim();
                cmdUpdateUsuario.Parameters.Add("@Telefono", SqlDbType.NVarChar, 20).Value = (telefono ?? string.Empty).Trim();
                cmdUpdateUsuario.Parameters.Add("@Cedula", SqlDbType.NVarChar, 20).Value = (cedula ?? string.Empty).Trim();
                cmdUpdateUsuario.Parameters.Add("@Rol", SqlDbType.NVarChar, 20).Value = (rol ?? string.Empty).Trim();
                cmdUpdateUsuario.Parameters.Add("@Contrasena", SqlDbType.NVarChar, 64).Value = contrasenaEncriptada;

                var filasAfectadas = cmdUpdateUsuario.ExecuteNonQuery();

                if (filasAfectadas > 0)
                {
                    string sqlCheckCliente = "SELECT COUNT(*) FROM CLIENTES WHERE CLI_CEDULA = @Cedula";
                    using var cmdCheckCliente = new SqlCommand(sqlCheckCliente, conexion);
                    cmdCheckCliente.Parameters.Add("@Cedula", SqlDbType.NVarChar, 20).Value = (cedula ?? string.Empty).Trim();

                    int countClientes = (int)cmdCheckCliente.ExecuteScalar();

                    if (countClientes > 0)
                    {
                        string sqlUpdateCliente = "UPDATE CLIENTES SET USU_ID = @Id WHERE CLI_CEDULA = @Cedula";
                        using var cmdUpdateCliente = new SqlCommand(sqlUpdateCliente, conexion);
                        cmdUpdateCliente.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                        cmdUpdateCliente.Parameters.Add("@Cedula", SqlDbType.NVarChar, 20).Value = (cedula ?? string.Empty).Trim();

                        int filasClienteActualizadas = cmdUpdateCliente.ExecuteNonQuery();

                        return filasClienteActualizadas > 0
                            ? Ok(new { mensaje = "Usuario y cliente actualizados exitosamente." })
                            : Ok(new { mensaje = "Usuario actualizado, pero no se pudo actualizar el cliente." });
                    }
                    else
                    {
                        return Ok(new { mensaje = "Usuario actualizado, pero no se encontró un cliente con la misma cédula." });
                    }
                }
                else
                {
                    return NotFound(new { mensaje = "Usuario no encontrado." });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { mensaje = e.Message });
            }
        }
    }
}
