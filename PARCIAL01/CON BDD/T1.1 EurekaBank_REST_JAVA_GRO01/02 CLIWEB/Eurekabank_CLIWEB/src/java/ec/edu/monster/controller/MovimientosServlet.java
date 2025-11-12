package ec.edu.monster.controller;

import ec.edu.monster.model.Movimiento;
import ec.edu.monster.service.EurekaApiClient;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.*;
import java.io.IOException;
import java.util.List;

@WebServlet(name="MovimientosServlet", urlPatterns={"/dashboard"})
public class MovimientosServlet extends HttpServlet {

    private final EurekaApiClient api = new EurekaApiClient();

    @Override
    protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        req.getRequestDispatcher("/WEB-INF/jsp/dashboard.jsp").forward(req, resp);
    }

    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        String cuenta = req.getParameter("cuentaMov");
        if (cuenta == null || cuenta.trim().isEmpty()) {
            req.setAttribute("msgMov", "Ingrese una cuenta.");
            req.getRequestDispatcher("/WEB-INF/jsp/dashboard.jsp").forward(req, resp);
            return;
        }
        try {
            List<Movimiento> lista = api.movimientos(cuenta.trim());
            double saldo = 0;
            for (Movimiento m : lista) {
                if ("INGRESO".equalsIgnoreCase(m.getAccion())) saldo += m.getImporte();
                else saldo -= m.getImporte();
            }
            req.setAttribute("cuentaMov", cuenta.trim());
            req.setAttribute("movs", lista);
            req.setAttribute("saldo", saldo);
        } catch (Exception e) {
            req.setAttribute("msgMov", "Error: " + e.getMessage());
        }
        req.getRequestDispatcher("/WEB-INF/jsp/dashboard.jsp").forward(req, resp);
    }
}
