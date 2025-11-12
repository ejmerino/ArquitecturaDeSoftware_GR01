package ec.edu.monster.controller;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.*;
import java.io.IOException;

@WebServlet(name="LogoutServlet", urlPatterns={"/logout"})
public class LogoutServlet extends HttpServlet {
    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws IOException {
        HttpSession ses = req.getSession(false);
        if (ses != null) ses.invalidate();
        resp.sendRedirect(req.getContextPath() + "/login");
    }
}
