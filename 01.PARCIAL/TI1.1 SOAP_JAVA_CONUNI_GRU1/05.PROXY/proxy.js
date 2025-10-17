const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const fetch = require('node-fetch'); // Importamos node-fetch para hacer peticiones HTTP

const app = express();
const PORT = 3000; // El puerto en el que correrá tu servidor proxy

// URL de tu servicio SOAP Java
// ¡IMPORTANTE! Si tu servicio Payara NO está en localhost para el PROXY (ej. si el proxy está en otra máquina),
// DEBES cambiar "localhost" por la IP real de la máquina donde corre Payara.
const TARGET_SOAP_URL = "http://localhost:8080/WS_ConvUni_SOAPJAVA_GR01/ConversorUnidadesService";

// --- Configuración de CORS para tu cliente web ---
// Esto permite que tu cliente web (que puede estar en otro puerto o dominio)
// haga peticiones a este proxy sin problemas de CORS.
app.use(cors({
    origin: '*', // Permite peticiones desde cualquier origen para tu cliente web.
                  // Para producción, se recomienda especificar dominios específicos (ej. 'http://localhost:8081', 'https://tudominio.com')
    methods: ['GET', 'POST', 'OPTIONS'], // Permitimos GET, POST y OPTIONS (para preflight requests)
    allowedHeaders: ['Content-Type', 'SOAPAction', 'Accept', 'Authorization'] // Aseguramos que SOAPAction esté permitido
}));

// Middleware para parsear el cuerpo de la petición como texto (XML)
app.use(bodyParser.text({ type: 'text/xml' }));

// // Middleware para manejar peticiones OPTIONS (preflight requests de CORS)
// // El middleware 'cors' ya se encarga de esto en su mayor parte, pero lo dejamos explícito por si acaso.
// app.options('*', cors()); // <--- ESTA LÍNEA ES LA QUE DEBEMOS ELIMINAR O COMENTAR

// --- Ruta del Proxy SOAP ---
app.post('/soap-proxy', async (req, res) => {
    // Extraemos el SOAPAction del encabezado de la petición del cliente web
    const soapAction = req.headers['soapaction'] || '';

    console.log(`Proxy recibiendo petición para: ${TARGET_SOAP_URL}`);
    console.log(`SOAPAction: ${soapAction}`);
    // console.log(`Cuerpo de la petición: ${req.body}`); // Descomentar para depurar

    try {
        // Hacemos la petición a tu servicio SOAP Java
        const response = await fetch(TARGET_SOAP_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'text/xml; charset=utf-8',
                'SOAPAction': soapAction // Reenviamos el SOAPAction a tu servicio Java
            },
            body: req.body // El cuerpo XML de la petición del cliente web se envía a Java
        });

        // Verificamos si la respuesta del servicio SOAP fue exitosa
        if (!response.ok) {
            const errorText = await response.text();
            console.error(`Error del servicio SOAP: ${response.status} - ${errorText}`);
            // Reenviamos el error al cliente web
            return res.status(response.status).set('Content-Type', 'text/xml').send(errorText);
        }

        // Si la respuesta es exitosa, la reenviamos al cliente web
        const responseText = await response.text();
        res.status(response.status).set('Content-Type', 'text/xml').send(responseText);

    } catch (error) {
        console.error('Error en el proxy al comunicarse con el servicio SOAP:', error);
        // Enviamos una respuesta de error SOAP al cliente web
        res.status(500).set('Content-Type', 'text/xml').send(
            `<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
                <soap:Body>
                    <soap:Fault>
                        <faultcode>soap:Server</faultcode>
                        <faultstring>Error del proxy: ${error.message}</faultstring>
                    </soap:Fault>
                </soap:Body>
            </soap:Envelope>`
        );
    }
});

// Inicia el servidor proxy
app.listen(PORT, () => {
    console.log(`Servidor Proxy SOAP escuchando en http://localhost:${PORT}`);
    console.log(`Tu cliente web debe apuntar a http://localhost:${PORT}/soap-proxy`);
});