package ec.edu.monster.filter;

import javax.servlet.*;
import javax.servlet.annotation.WebFilter;
import javax.servlet.http.HttpServletResponse;
import java.io.IOException;

@WebFilter(filterName="CacheControlFilter", urlPatterns={"/*"})
public class CacheControlFilter implements Filter {
  @Override public void init(FilterConfig filterConfig) {}
  @Override public void destroy() {}

  @Override
  public void doFilter(ServletRequest req, ServletResponse res, FilterChain chain)
      throws IOException, ServletException {
    HttpServletResponse resp = (HttpServletResponse) res;
    resp.setHeader("Cache-Control", "no-store, no-cache, must-revalidate, max-age=0");
    resp.setHeader("Pragma", "no-cache");
    resp.setDateHeader("Expires", 0);
    chain.doFilter(req, res);
  }
}
