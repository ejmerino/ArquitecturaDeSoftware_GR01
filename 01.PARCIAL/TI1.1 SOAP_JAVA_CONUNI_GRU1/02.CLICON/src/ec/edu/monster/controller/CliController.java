package ec.edu.monster.controller;

import ec.edu.monster.model.User;
import ec.edu.monster.model.SoapClient;
import ec.edu.monster.view.ConsoleView;

import java.util.HashMap;
import java.util.Map;

public class CliController {
    private ConsoleView view;
    private User user;
    private SoapClient client;

    private String[] opciones = {
        "Metros a Kilómetros",
        "Kilómetros a Metros",
        "Centímetros a Metros",
        "Gramos a Kilogramos",
        "Kilogramos a Gramos",
        "Libras a Kilogramos",
        "Celsius a Fahrenheit",
        "Fahrenheit a Celsius",
        "Celsius a Kelvin"
    };

    private Map<String,String> metodoMap;
    private Map<String,String> unidadMap;

    public CliController(ConsoleView view){
        this.view = view;
        user = new User("MONSTER","MONSTER9");
        client = new SoapClient();
        inicializarMapas();
    }

    private void inicializarMapas(){
        metodoMap = new HashMap<>();
        unidadMap = new HashMap<>();

        metodoMap.put("Metros a Kilómetros","metros_a_kilometros");
        metodoMap.put("Kilómetros a Metros","kilometros_a_metros");
        metodoMap.put("Centímetros a Metros","centimetros_a_metros");
        metodoMap.put("Gramos a Kilogramos","gramos_a_kilogramos");
        metodoMap.put("Kilogramos a Gramos","kilogramos_a_gramos");
        metodoMap.put("Libras a Kilogramos","libras_a_kilogramos");
        metodoMap.put("Celsius a Fahrenheit","celsius_a_fahrenheit");
        metodoMap.put("Fahrenheit a Celsius","fahrenheit_a_celsius");
        metodoMap.put("Celsius a Kelvin","celsius_a_kelvin");

        unidadMap.put("metros_a_kilometros","km");
        unidadMap.put("kilometros_a_metros","m");
        unidadMap.put("centimetros_a_metros","m");
        unidadMap.put("gramos_a_kilogramos","kg");
        unidadMap.put("kilogramos_a_gramos","g");
        unidadMap.put("libras_a_kilogramos","kg");
        unidadMap.put("celsius_a_fahrenheit","°F");
        unidadMap.put("fahrenheit_a_celsius","°C");
        unidadMap.put("celsius_a_kelvin","K");
    }

    public void iniciar(){
        boolean loginOK = false;
        while(!loginOK){
            view.mostrarMensaje("\n--- LOGIN ---");
            String usuario = view.pedirTexto("Usuario");
            String clave = view.pedirPassword("Contraseña"); // ahora se oculta con *****
            if(user.validar(usuario, clave)){
                loginOK = true;
                view.mostrarMensaje("✅ ¡Login correcto!\n");
            } else {
                view.mostrarMensaje("❌ Usuario o contraseña incorrectos. Intente de nuevo.");
            }
        }
        menuPrincipal();
    }

    private void menuPrincipal(){
        boolean seguir = true;
        while(seguir){
            view.mostrarMenu(opciones);
            int opcion = view.pedirOpcion(opciones.length);
            if(opcion==0){
                view.mostrarMensaje("👋 Saliendo...");
                break;
            }

            String seleccion = opciones[opcion-1];
            double valor = view.pedirNumero("Ingrese valor a convertir");
            String metodo = metodoMap.get(seleccion);

            try{
                double resultado = client.convertir(metodo, valor);
                view.mostrarResultado(resultado, unidadMap.get(metodo));
            } catch(Exception e){
                view.mostrarMensaje("⚠ Error al conectar con el servidor SOAP: " + e.getMessage());
            }

            // Pregunta si quiere hacer otra conversión
            seguir = view.confirmarOtraConversion("¿Deseas hacer otra conversión? (S/N)");
        }
    }
}
