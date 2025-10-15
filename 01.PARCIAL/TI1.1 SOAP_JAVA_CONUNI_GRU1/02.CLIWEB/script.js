// Función para obtener el nombre del parámetro correcto según el método
function getParamName(tipo) {
  switch(tipo) {
    case "metros_a_kilometros": return "metros";
    case "kilometros_a_metros": return "kilometros";
    case "centimetros_a_metros": return "centimetros";
    case "gramos_a_kilogramos": return "gramos";
    case "kilogramos_a_gramos": return "kilogramos";
    case "libras_a_kilogramos": return "libras";
    case "celsius_a_fahrenheit":
    case "celsius_a_kelvin": return "celsius";
    case "fahrenheit_a_celsius": return "fahrenheit";
    default: return "valor";
  }
}

// Evento del botón "Convertir"
document.getElementById("convertir").addEventListener("click", async () => {
  const tipo = document.getElementById("tipo").value;
  const valor = document.getElementById("valor").value;
  const resultadoDiv = document.getElementById("resultado");

  if (valor === "") {
    resultadoDiv.textContent = "⚠️ Ingrese un valor para convertir.";
    return;
  }

  // URL del servicio SOAP
  const url = "http://localhost:8080/WS_ConvUni_SOAPJAVA_GR01/ConversorUnidadesService";

  // Nombre del parámetro correcto
  const param = getParamName(tipo);

  // Construcción del Envelope SOAP
  const xml = `<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <${tipo} xmlns="http://service.monster.edu.ec/">
      <${param}>${valor}</${param}>
    </${tipo}>
  </soap:Body>
</soap:Envelope>`;

  try {
    const response = await fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "text/xml; charset=utf-8",
        "SOAPAction": ""
      },
      body: xml
    });

    const text = await response.text();

    // Parseamos el XML de respuesta
    const parser = new DOMParser();
    const xmlDoc = parser.parseFromString(text, "text/xml");

    // Extraemos el valor de la respuesta
    const value = xmlDoc.getElementsByTagName(`${tipo}Response`)[0]
      ?.children[0].textContent;

    if (value !== undefined) {
      resultadoDiv.textContent = `Resultado: ${value}`;
    } else {
      resultadoDiv.textContent = "❌ Error: no se recibió respuesta válida del servidor.";
      console.error("Respuesta SOAP completa:", text);
    }

  } catch (error) {
    resultadoDiv.textContent = "❌ Error al conectar con el servidor SOAP.";
    console.error(error);
  }
});
