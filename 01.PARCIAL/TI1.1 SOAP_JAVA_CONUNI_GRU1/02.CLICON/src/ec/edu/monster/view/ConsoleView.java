package ec.edu.monster.view;

import java.util.Scanner;

public class ConsoleView {
    private Scanner sc;

    public ConsoleView() {
        sc = new Scanner(System.in);
    }

    // Muestra un mensaje simple
    public void mostrarMensaje(String msg) {
        System.out.println(msg);
    }

    // Pide texto al usuario
    public String pedirTexto(String prompt) {
        System.out.print(prompt + ": ");
        return sc.nextLine();
    }

    // Pide contraseña con *****
    public String pedirPassword(String prompt) {
        System.out.print(prompt + ": ");
        // Para simplificar en consola, usamos texto normal
        // Para terminales que soporten ocultar, se podría usar Console.readPassword()
        String password;
        try {
            if (System.console() != null) {
                char[] passArray = System.console().readPassword();
                password = new String(passArray);
            } else {
                // Si no hay console (por ejemplo en IDE), se muestra normalmente
                password = sc.nextLine();
            }
        } catch (Exception e) {
            password = sc.nextLine();
        }
        return password;
    }

    // Muestra el menú de opciones
    public void mostrarMenu(String[] opciones) {
        System.out.println("\n--- MENÚ PRINCIPAL ---");
        for (int i = 0; i < opciones.length; i++) {
            System.out.println((i + 1) + ". " + opciones[i]);
        }
        System.out.println("0. Salir");
    }

    // Pide opción y valida que sea número dentro del rango
    public int pedirOpcion(int max) {
        int opcion = -1;
        while (true) {
            System.out.print("Selecciona una opción (0-" + max + "): ");
            try {
                opcion = Integer.parseInt(sc.nextLine());
                if (opcion >= 0 && opcion <= max) break;
                else System.out.println("⚠ Opción inválida, inténtalo de nuevo.");
            } catch (NumberFormatException e) {
                System.out.println("⚠ Debes ingresar un número válido.");
            }
        }
        return opcion;
    }

    // Pide un número y valida que sea un double
    public double pedirNumero(String prompt) {
        double valor = 0;
        while (true) {
            System.out.print(prompt + ": ");
            try {
                valor = Double.parseDouble(sc.nextLine());
                break;
            } catch (NumberFormatException e) {
                System.out.println("⚠ Debes ingresar un número válido.");
            }
        }
        return valor;
    }

    // Muestra el resultado con unidad
    public void mostrarResultado(double resultado, String unidad) {
        System.out.println("✅ Resultado: " + resultado + " " + unidad);
    }

    // Pregunta si desea otra conversión
    public boolean confirmarOtraConversion(String msg) {
        String respuesta;
        while (true) {
            System.out.print(msg + " ");
            respuesta = sc.nextLine().trim().toLowerCase();
            if (respuesta.equals("s") || respuesta.equals("si")) return true;
            else if (respuesta.equals("n") || respuesta.equals("no")) return false;
            else System.out.println("⚠ Responde S (sí) o N (no).");
        }
    }
}
