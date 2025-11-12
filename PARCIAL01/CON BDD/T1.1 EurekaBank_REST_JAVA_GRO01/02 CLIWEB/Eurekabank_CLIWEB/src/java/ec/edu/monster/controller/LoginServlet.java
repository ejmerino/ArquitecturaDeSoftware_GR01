package ec.edu.monster.controller;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.*;
import java.io.IOException;

@WebServlet(name="LoginServlet", urlPatterns={"/login"})
public class LoginServlet extends HttpServlet {

    private static final String USER = "MONSTER";
    private static final String PASS = "MONSTER9";

    @Override
    protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws IOException, ServletException {
        // Si ya inició sesión, manda al dashboard
        HttpSession ses = req.getSession(false);
        if (ses != null && ses.getAttribute("user") != null) {
            resp.sendRedirect(req.getContextPath() + "/dashboard");
            return;
        }
        req.getRequestDispatcher("/WEB-INF/jsp/login.jsp").forward(req, resp);
    }

    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws IOException, ServletException {
        String u = req.getParameter("user");
        String p = req.getParameter("pass");

        if (u != null && p != null && USER.equalsIgnoreCase(u.trim()) && PASS.equals(p)) {
            HttpSession ses = req.getSession(true);
            ses.setAttribute("user", USER);
            resp.sendRedirect(req.getContextPath() + "/dashboard");
        } else {
            req.setAttribute("error", "Credenciales inválidas");
            req.getRequestDispatcher("/WEB-INF/jsp/login.jsp").forward(req, resp);
        }
    }
}
