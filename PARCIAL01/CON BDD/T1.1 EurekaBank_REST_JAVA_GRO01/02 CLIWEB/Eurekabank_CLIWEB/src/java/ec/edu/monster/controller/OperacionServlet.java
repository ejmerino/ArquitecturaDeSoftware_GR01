package ec.edu.monster.controller;

import ec.edu.monster.service.EurekaApiClient;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.*;
import java.io.IOException;

@WebServlet(name="OperacionServlet", urlPatterns={"/operar"})
public class OperacionServlet extends HttpServlet {

    private final EurekaApiClient api = new EurekaApiClient();

    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        String tipo = req.getParameter("tipo"); // dep | ret | tra
        try {
            switch (tipo) {
                case "dep": {
                    String c = req.getParameter("depCuenta");
                    double imp = Double.parseDouble(req.getParameter("depImporte"));
                    if (c == null || c.trim().isEmpty() || imp <= 0) throw new RuntimeException("Datos inválidos.");
                    api.deposito(c.trim(), imp);
                    req.setAttribute("msgDep", "Depósito OK.");
                    break;
                }
                case "ret": {
                    String c = req.getParameter("retCuenta");
                    double imp = Double.parseDouble(req.getParameter("retImporte"));
                    if (c == null || c.trim().isEmpty() || imp <= 0) throw new RuntimeException("Datos inválidos.");
                    api.retiro(c.trim(), imp);
                    req.setAttribute("msgRet", "Retiro OK.");
                    break;
                }
                case "tra": {
                    String o = req.getParameter("traOrigen");
                    String d = req.getParameter("traDestino");
                    double imp = Double.parseDouble(req.getParameter("traImporte"));
                    if (o == null || d == null || o.trim().isEmpty() || d.trim().isEmpty() || o.equals(d) || imp <= 0)
                        throw new RuntimeException("Datos inválidos.");
                    api.transferencia(o.trim(), d.trim(), imp);
                    req.setAttribute("msgTra", "Transferencia OK.");
                    break;
                }
                default: throw new RuntimeException("Operación desconocida.");
            }
        } catch (Exception e) {
            if ("dep".equals(tipo)) req.setAttribute("msgDep", "Error: " + e.getMessage());
            if ("ret".equals(tipo)) req.setAttribute("msgRet", "Error: " + e.getMessage());
            if ("tra".equals(tipo)) req.setAttribute("msgTra", "Error: " + e.getMessage());
        }
        req.getRequestDispatcher("/WEB-INF/jsp/dashboard.jsp").forward(req, resp);
    }
}
