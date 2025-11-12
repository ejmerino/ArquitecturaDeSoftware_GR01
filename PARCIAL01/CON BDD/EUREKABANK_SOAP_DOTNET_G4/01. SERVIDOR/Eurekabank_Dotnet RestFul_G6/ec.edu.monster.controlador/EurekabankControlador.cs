using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;

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

        private string EncriptarContrasena(string contrasena)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasena));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                // Asegurarnos de que el hash no exceda 50 caracteres
                return builder.ToString().Substring(0, 50);
            }
        }


        // Método para consultar usuarios por correo y contraseña
        [HttpGet]
        [Route("Usuarios/PorCorreo")]
        public IActionResult ConsultarUsuarioPorCorreo(string correo, string contrasena)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();

                    // Consulta SQL para buscar por correo y validar la contraseña
                    string sql = "SELECT * FROM USUARIOS WHERE USU_CORREO = @Correo AND USU_CONTRASENA = @Contrasena";
                    var cmd = new SqlCommand(sql, conexion);
                    cmd.Parameters.AddWithValue("@Correo", correo);

                    // Encriptar la contraseña antes de compararla
                    string contrasenaEncriptada = EncriptarContrasena(contrasena);
                    cmd.Parameters.AddWithValue("@Contrasena", contrasenaEncriptada);

                    Usuario usuario = null;
                    using (var rd = cmd.ExecuteReader())
                    {
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
                                UsuRol = Convert.ToString(rd["USU_ROL"]),
                                UsuContrasena = Convert.ToString(rd["USU_CONTRASENA"])
                            };
                        }
                    }

                    if (usuario != null)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = usuario });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Usuario no encontrado o contraseña incorrecta." });
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message });
            }
        }



        [HttpGet]
        [Route("Movimientos")]
        public IActionResult Movimientos(string numeroCuenta)
        {
            List<Movimiento> movimientos = new List<Movimiento>();

            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();

                    string sql = "SELECT * FROM MOVIMIENTOS M " +
                                 "INNER JOIN CUENTAS C ON M.CU_ID = C.CU_ID " +
                                 "WHERE C.CU_NUMERO = @NumeroCuenta";

                    var cmd = new SqlCommand(sql, conexion);
                    cmd.Parameters.AddWithValue("@NumeroCuenta", numeroCuenta);

                    using (var rd = cmd.ExecuteReader())
                    {
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
                    }
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = movimientos });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message, response = movimientos });
            }
        }


        [HttpGet]
        [Route("Cuentas")]
        public IActionResult Cuentas(string? numeroCuenta)
        {
            List<Cuenta> cuentas = new List<Cuenta>();

            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();

                    string sql = "SELECT CU_ID, CLI_ID, CU_NUMERO, CU_TIPO, CU_SALDO " +
                                 "FROM CUENTAS";

                    if (!string.IsNullOrEmpty(numeroCuenta))
                    {
                        sql += " WHERE CU_NUMERO = @NumeroCuenta";
                    }

                    var cmd = new SqlCommand(sql, conexion);

                    if (!string.IsNullOrEmpty(numeroCuenta))
                    {
                        cmd.Parameters.AddWithValue("@NumeroCuenta", numeroCuenta);
                    }

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            cuentas.Add(new Cuenta()
                            {
                                CuId = Convert.ToInt32(rd["CU_ID"]),
                                CliId = Convert.ToInt32(rd["CLI_ID"]),
                                CuNumero = Convert.ToString(rd["CU_NUMERO"]),
                                CuTipo = Convert.ToString(rd["CU_TIPO"]),
                                CuSaldo = Convert.ToSingle(rd["CU_SALDO"]),
                            });
                        }
                    }
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = cuentas });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message, response = cuentas });
            }
        }

        [HttpPost]
        [Route("Transferencia")]
        public IActionResult Transferencia(string cuentaOrigen, string cuentaDestino, float valor)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var transaction = conexion.BeginTransaction();

                    try
                    {
                        string sqlSaldoOrigen = "SELECT CU_SALDO FROM CUENTAS WHERE CU_NUMERO = @CuentaOrigenNumero";
                        var cmdSaldoOrigen = new SqlCommand(sqlSaldoOrigen, conexion, transaction);
                        cmdSaldoOrigen.Parameters.AddWithValue("@CuentaOrigenNumero", cuentaOrigen);
                        float saldoOrigen = Convert.ToSingle(cmdSaldoOrigen.ExecuteScalar());

                        if (saldoOrigen < valor)
                        {
                            throw new Exception("Saldo insuficiente en la cuenta de origen.");
                        }

                        string sqlSaldoDestino = "SELECT CU_SALDO FROM CUENTAS WHERE CU_NUMERO = @CuentaDestinoNumero";
                        var cmdSaldoDestino = new SqlCommand(sqlSaldoDestino, conexion, transaction);
                        cmdSaldoDestino.Parameters.AddWithValue("@CuentaDestinoNumero", cuentaDestino);
                        float saldoDestino = Convert.ToSingle(cmdSaldoDestino.ExecuteScalar());

                        string sqlActualizarSaldoOrigen = "UPDATE CUENTAS SET CU_SALDO = CU_SALDO - @Valor WHERE CU_NUMERO = @CuentaOrigenNumero";
                        var cmdActualizarSaldoOrigen = new SqlCommand(sqlActualizarSaldoOrigen, conexion, transaction);
                        cmdActualizarSaldoOrigen.Parameters.AddWithValue("@Valor", valor);
                        cmdActualizarSaldoOrigen.Parameters.AddWithValue("@CuentaOrigenNumero", cuentaOrigen);
                        cmdActualizarSaldoOrigen.ExecuteNonQuery();

                        string sqlActualizarSaldoDestino = "UPDATE CUENTAS SET CU_SALDO = CU_SALDO + @Valor WHERE CU_NUMERO = @CuentaDestinoNumero";
                        var cmdActualizarSaldoDestino = new SqlCommand(sqlActualizarSaldoDestino, conexion, transaction);
                        cmdActualizarSaldoDestino.Parameters.AddWithValue("@Valor", valor);
                        cmdActualizarSaldoDestino.Parameters.AddWithValue("@CuentaDestinoNumero", cuentaDestino);
                        cmdActualizarSaldoDestino.ExecuteNonQuery();

                        string sqlRegistrarMovimiento = "INSERT INTO MOVIMIENTOS (CU_ID, MOV_FECHA, MOV_TIPO, MOV_VALOR, MOV_SALDO_FINAL) " +
                                                        "VALUES ((SELECT CU_ID FROM CUENTAS WHERE CU_NUMERO = @CuentaOrigenNumero), GETDATE(), 'TRANSFERENCIA', @Valor * -1, @NuevoSaldoOrigen), " +
                                                        "((SELECT CU_ID FROM CUENTAS WHERE CU_NUMERO = @CuentaDestinoNumero), GETDATE(), 'TRANSFERENCIA', @Valor, @NuevoSaldoDestino)";
                        var cmdRegistrarMovimiento = new SqlCommand(sqlRegistrarMovimiento, conexion, transaction);
                        cmdRegistrarMovimiento.Parameters.AddWithValue("@CuentaOrigenNumero", cuentaOrigen);
                        cmdRegistrarMovimiento.Parameters.AddWithValue("@CuentaDestinoNumero", cuentaDestino);
                        cmdRegistrarMovimiento.Parameters.AddWithValue("@Valor", valor);
                        cmdRegistrarMovimiento.Parameters.AddWithValue("@NuevoSaldoOrigen", saldoOrigen - valor);
                        cmdRegistrarMovimiento.Parameters.AddWithValue("@NuevoSaldoDestino", saldoDestino + valor);
                        cmdRegistrarMovimiento.ExecuteNonQuery();

                        transaction.Commit();

                        return StatusCode(StatusCodes.Status200OK, new { mensaje = "Transferencia realizada con éxito." });
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw e;
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message });
            }
        }

        // Método para ingresar usuarios
        [HttpPost]
        [Route("Usuarios")]
        public IActionResult AgregarUsuario(string nombre, string apellido, string correo, string telefono, string cedula, string rol, string contrasena)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();

                    // Inserta el nuevo usuario
                    string sqlInsert = "INSERT INTO USUARIOS (USU_NOMBRE, USU_APELLIDO, USU_CORREO, USU_TELEFONO, USU_CEDULA, USU_ROL, USU_CONTRASENA) " +
                                       "VALUES (@Nombre, @Apellido, @Correo, @Telefono, @Cedula, @Rol, @Contrasena); " +
                                       "SELECT CAST(scope_identity() AS int);";
                    var cmdInsert = new SqlCommand(sqlInsert, conexion);
                    cmdInsert.Parameters.AddWithValue("@Nombre", nombre);
                    cmdInsert.Parameters.AddWithValue("@Apellido", apellido);
                    cmdInsert.Parameters.AddWithValue("@Correo", correo);
                    cmdInsert.Parameters.AddWithValue("@Telefono", telefono);
                    cmdInsert.Parameters.AddWithValue("@Cedula", cedula);
                    cmdInsert.Parameters.AddWithValue("@Rol", rol);

                    // Encriptar la contraseña antes de enviarla
                    string contrasenaEncriptada = EncriptarContrasena(contrasena);
                    cmdInsert.Parameters.AddWithValue("@Contrasena", contrasenaEncriptada); // Asegúrate de encriptar esta contraseña antes de insertarla.

                    int usuarioId = (int)cmdInsert.ExecuteScalar();

                    // Verifica si el usuario es cliente y actualiza el campo CLI_USUARIO_ID
                    string sqlUpdateCliente = "UPDATE CLIENTES SET USU_ID = @UsuarioId WHERE CLI_CEDULA = @Cedula";
                    var cmdUpdateCliente = new SqlCommand(sqlUpdateCliente, conexion);
                    cmdUpdateCliente.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    cmdUpdateCliente.Parameters.AddWithValue("@Cedula", cedula);

                    int filasAfectadas = cmdUpdateCliente.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return StatusCode(StatusCodes.Status201Created, new { mensaje = "Usuario creado y cliente actualizado exitosamente." });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status201Created, new { mensaje = "Usuario creado exitosamente, pero no se encontró un cliente con la misma cédula." });
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message });
            }
        }

        // Método para consultar, borrar o modificar usuarios (solo administradores)
        [HttpGet]
        [Route("Usuarios/{id}")]
        public IActionResult ConsultarUsuario(int id)
        {
            Usuario usuario = null;

            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();

                    string sql = "SELECT * FROM USUARIOS WHERE USU_ID = @Id";
                    var cmd = new SqlCommand(sql, conexion);
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var rd = cmd.ExecuteReader())
                    {
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
                                UsuRol = Convert.ToString(rd["USU_ROL"]),
                                UsuContrasena = Convert.ToString(rd["USU_CONTRASENA"])
                            };
                        }
                    }
                }

                if (usuario != null)
                {
                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = usuario });
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Usuario no encontrado." });
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message });
            }
        }

        [HttpDelete]
        [Route("Usuarios/{id}")]
        public IActionResult EliminarUsuario(int id)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();

                    string sql = "DELETE FROM USUARIOS WHERE USU_ID = @Id";
                    var cmd = new SqlCommand(sql, conexion);
                    cmd.Parameters.AddWithValue("@Id", id);

                    var filasAfectadas = cmd.ExecuteNonQuery();
                    if (filasAfectadas > 0)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { mensaje = "Usuario eliminado exitosamente." });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Usuario no encontrado." });
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message });
            }
        }
        // Método para modificar usuarios (solo administradores)
        [HttpPut]
        [Route("Usuarios/{id}")]
        public IActionResult ModificarUsuario(int id, string nombre, string apellido, string correo, string telefono, string cedula, string rol, string contrasena)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();

                    // Encriptar la contraseña
                    string contrasenaEncriptada = EncriptarContrasena(contrasena);

                    // Actualizar el usuario
                    string sqlUpdateUsuario = "UPDATE USUARIOS SET USU_NOMBRE = @Nombre, USU_APELLIDO = @Apellido, USU_CORREO = @Correo, " +
                                              "USU_TELEFONO = @Telefono, USU_CEDULA = @Cedula, USU_ROL = @Rol, USU_CONTRASENA = @Contrasena " +
                                              "WHERE USU_ID = @Id";
                    var cmdUpdateUsuario = new SqlCommand(sqlUpdateUsuario, conexion);
                    cmdUpdateUsuario.Parameters.AddWithValue("@Id", id);
                    cmdUpdateUsuario.Parameters.AddWithValue("@Nombre", nombre);
                    cmdUpdateUsuario.Parameters.AddWithValue("@Apellido", apellido);
                    cmdUpdateUsuario.Parameters.AddWithValue("@Correo", correo);
                    cmdUpdateUsuario.Parameters.AddWithValue("@Telefono", telefono);
                    cmdUpdateUsuario.Parameters.AddWithValue("@Cedula", cedula);
                    cmdUpdateUsuario.Parameters.AddWithValue("@Rol", rol);
                    cmdUpdateUsuario.Parameters.AddWithValue("@Contrasena", contrasenaEncriptada);

                    var filasAfectadas = cmdUpdateUsuario.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        // Verificar si la cédula del usuario coincide con la cédula de algún cliente
                        string sqlCheckCliente = "SELECT COUNT(*) FROM CLIENTES WHERE CLI_CEDULA = @Cedula";
                        var cmdCheckCliente = new SqlCommand(sqlCheckCliente, conexion);
                        cmdCheckCliente.Parameters.AddWithValue("@Cedula", cedula);

                        int countClientes = (int)cmdCheckCliente.ExecuteScalar();

                        if (countClientes > 0)
                        {
                            // Actualizar el cliente con el ID del usuario
                            string sqlUpdateCliente = "UPDATE CLIENTES SET USU_ID = @Id WHERE CLI_CEDULA = @Cedula";
                            var cmdUpdateCliente = new SqlCommand(sqlUpdateCliente, conexion);
                            cmdUpdateCliente.Parameters.AddWithValue("@Id", id);
                            cmdUpdateCliente.Parameters.AddWithValue("@Cedula", cedula);

                            int filasClienteActualizadas = cmdUpdateCliente.ExecuteNonQuery();

                            if (filasClienteActualizadas > 0)
                            {
                                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Usuario y cliente actualizados exitosamente." });
                            }
                            else
                            {
                                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Usuario actualizado exitosamente, pero no se pudo actualizar el cliente." });
                            }
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status200OK, new { mensaje = "Usuario actualizado exitosamente, pero no se encontró un cliente con la misma cédula." });
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Usuario no encontrado." });
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message });
            }
        }


    }
}
