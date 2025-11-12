// ===============================
// CONFIGURACIÓN GLOBAL
// ===============================
const HOST = location.hostname;
const PORT = location.port || "8080";

// ⚠️ AJUSTA el nombre del WAR del servicio REST si es diferente:
const APP_CONTEXT = "WSEurekaBank_GRO01";

// Base del API REST (JAX-RS)
const BASE = `http://${HOST}:${PORT}/${APP_CONTEXT}/webresources/coreBancario`;

// ===============================
// FUNCIONES DEL SERVICIO
// ===============================
async function apiGetMovimientos(cuenta) {
  const url = `${BASE}/movimientos/${encodeURIComponent(cuenta)}`;
  const r = await fetch(url, { headers: { "Accept": "application/json" } });
  if (!r.ok) throw new Error(`Error GET movimientos: ${r.status}`);
  return r.json();
}

async function apiDeposito(cuenta, importe) {
  const url = `${BASE}/deposito`;
  const body = new URLSearchParams({ cuenta, importe: String(importe) });
  const r = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/x-www-form-urlencoded",
      "Accept": "application/json"
    },
    body
  });
  if (!r.ok) throw new Error(`Error POST deposito: ${r.status}`);
  return r.json();
}

async function apiRetiro(cuenta, importe) {
  const url = `${BASE}/retiro`;
  const body = new URLSearchParams({ cuenta, importe: String(importe) });
  const r = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/x-www-form-urlencoded",
      "Accept": "application/json"
    },
    body
  });
  if (!r.ok) throw new Error(`Error POST retiro: ${r.status}`);
  return r.json();
}

async function apiTransferencia(origen, destino, importe) {
  const url = `${BASE}/transferencia`;
  const body = new URLSearchParams({
    origen,
    destino,
    importe: String(importe)
  });
  const r = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/x-www-form-urlencoded",
      "Accept": "application/json"
    },
    body
  });
  if (!r.ok) throw new Error(`Error POST transferencia: ${r.status}`);
  return r.json();
}
