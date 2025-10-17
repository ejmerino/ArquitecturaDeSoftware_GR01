// Devuelve el nombre del parámetro para cada método
function getParamName(tipo) {
  switch(tipo) {
    // Longitud
    case "metros_a_kilometros": return "metros";
    case "kilometros_a_metros": return "kilometros";
    case "centimetros_a_metros": return "centimetros";
    // Masa
    case "gramos_a_kilogramos": return "gramos";
    case "kilogramos_a_gramos": return "kilogramos";
    case "libras_a_kilogramos": return "libras";
    // Temperatura
    case "celsius_a_fahrenheit":
    case "celsius_a_kelvin": return "celsius";
    case "fahrenheit_a_celsius": return "fahrenheit";
    default: return "valor";
  }
}

document.getElementById("convertir").addEventListener("click", () => {
  const tipo = document.getElementById("tipo").value;
  const valor = document.getElementById("valor").value;
  const resultadoDiv = document.getElementById("resultado");

  if (!valor) {
    resultadoDiv.textContent = "⚠️ Ingrese un valor para convertir.";
    return;
  }

  const url = "http://localhost:8080/WS_ConvUni_SOAPJAVA_GR01/ConversorUnidadesService";
  const param = getParamName(tipo);

  // Envelope SOAP correcto con prefijo ser:
  const xml = `<?xml version="1.0" encoding="utf-8"?>
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://service.monster.edu.ec/">
  <soapenv:Header/>
  <soapenv:Body>
    <ser:${tipo}>
      <${param}>${valor}</${param}>
    </ser:${tipo}>
  </soapenv:Body>
</soapenv:Envelope>`;

  fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "text/xml; charset=utf-8",
      "SOAPAction": ""
    },
    body: xml
  })
  .then(response => response.text())
  .then(text => {
    const parser = new DOMParser();
    const xmlDoc = parser.parseFromString(text, "text/xml");

    // Aquí leemos directamente el <return> de la respuesta
    const value = xmlDoc.getElementsByTagName("return")[0]?.textContent;

    if (value !== undefined) {
      resultadoDiv.textContent = `Resultado: ${value}`;
    } else {
      resultadoDiv.textContent = "❌ Error: no se recibió respuesta válida del servidor.";
      console.error("Respuesta SOAP completa:", text);
    }
  })
  .catch(err => {
    resultadoDiv.textContent = "❌ Error al conectar con el servidor SOAP.";
    console.error(err);
  });
});
