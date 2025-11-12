package ec.edu.monster.filter;

import javax.servlet.*;
import javax.servlet.annotation.WebFilter;
import javax.servlet.http.*;
import java.io.IOException;

@WebFilter(filterName="AuthFilter", urlPatterns={"/dashboard", "/operar", "/login"})
public class AuthFilter implements Filter {
    @Override public void init(FilterConfig filterConfig) {}
    @Override public void destroy() {}

    @Override
    public void doFilter(ServletRequest req, ServletResponse res, FilterChain chain)
        throws IOException, ServletException {
        HttpServletRequest r = (HttpServletRequest) req;
        HttpServletResponse s = (HttpServletResponse) res;
        HttpSession ses = r.getSession(false);
        boolean logged = (ses != null && ses.getAttribute("user") != null);

        String path = r.getRequestURI().substring(r.getContextPath().length());
        if (!logged && (path.equals("/dashboard") || path.equals("/operar"))) {
            s.sendRedirect(r.getContextPath() + "/login");
            return;
        }
        if (logged && path.equals("/login")) {
            s.sendRedirect(r.getContextPath() + "/dashboard");
            return;
        }
        chain.doFilter(req, res);
    }
}
