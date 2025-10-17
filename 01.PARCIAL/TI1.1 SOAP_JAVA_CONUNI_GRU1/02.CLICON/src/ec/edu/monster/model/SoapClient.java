/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ec.edu.monster.model;

/**
 *
 * @author ednan
 */
import javax.xml.soap.*;
import java.net.URL;

public class SoapClient {

    private static final String ENDPOINT = "http://localhost:8080/WS_ConvUni_SOAPJAVA_GR01/ConversorUnidadesService";

    public double convertir(String metodo, double valor) throws Exception {
        // Crear conexión SOAP
        SOAPConnectionFactory soapConnectionFactory = SOAPConnectionFactory.newInstance();
        SOAPConnection connection = soapConnectionFactory.createConnection();

        // Crear mensaje SOAP
        MessageFactory messageFactory = MessageFactory.newInstance();
        SOAPMessage message = messageFactory.createMessage();

        SOAPPart soapPart = message.getSOAPPart();
        SOAPEnvelope envelope = soapPart.getEnvelope();
        envelope.addNamespaceDeclaration("ser", "http://service.monster.edu.ec/");

        SOAPBody body = envelope.getBody();
        SOAPElement bodyElement = body.addChildElement(metodo, "ser");

        String paramName = getParamName(metodo);
        bodyElement.addChildElement(paramName).addTextNode(String.valueOf(valor));

        message.saveChanges();

        // Enviar mensaje
        SOAPMessage response = connection.call(message, ENDPOINT);

        // Leer respuesta
        SOAPBody responseBody = response.getSOAPBody();
        String resultStr = responseBody.getFirstChild().getFirstChild().getTextContent();

        connection.close();

        return Double.parseDouble(resultStr);
    }

    private String getParamName(String metodo){
        switch(metodo){
            case "metros_a_kilometros": return "metros";
            case "kilometros_a_metros": return "kilometros";
            case "centimetros_a_metros": return "centimetros";
            case "gramos_a_kilogramos": return "gramos";
            case "kilogramos_a_gramos": return "kilogramos";
            case "libras_a_kilogramos": return "libras";
            case "celsius_a_fahrenheit": return "celsius";
            case "fahrenheit_a_celsius": return "fahrenheit";
            case "celsius_a_kelvin": return "celsius";
            default: return "valor";
        }
    }
}
