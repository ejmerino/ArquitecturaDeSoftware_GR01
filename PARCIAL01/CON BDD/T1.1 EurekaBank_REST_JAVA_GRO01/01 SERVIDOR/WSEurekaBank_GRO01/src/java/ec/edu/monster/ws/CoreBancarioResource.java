package ec.edu.monster.ws;

import ec.edu.monster.modelo.Movimiento;
import ec.edu.monster.servicio.EurekaService;

import javax.ws.rs.*;
import javax.ws.rs.core.*;
import java.util.List;

@Path("coreBancario")
public class CoreBancarioResource {

    @Context
    private UriInfo context;

    /** GET: Lista movimientos */
    @GET
    @Path("/movimientos/{cuenta}")
    @Produces(MediaType.APPLICATION_JSON)
    public Response getMovimientos(@PathParam("cuenta") String cuenta) {
        try {
            EurekaService service = new EurekaService();
            List<Movimiento> lista = service.leerMovimientos(cuenta);
            if (lista.isEmpty()) {
                return Response.status(Response.Status.NOT_FOUND)
                        .entity("{\"error\":\"No se encontraron movimientos para la cuenta " + cuenta + "\"}")
                        .build();
            }
            String json = service.convertirMovimientosAJSON(lista);
            return Response.ok(json).build();
        } catch (Exception e) {
            return Response.status(Response.Status.INTERNAL_SERVER_ERROR)
                    .entity("{\"error\":\"" + e.getMessage() + "\"}").build();
        }
    }

    /** POST: Depósito */
    @POST
    @Path("/deposito")
    @Consumes(MediaType.APPLICATION_FORM_URLENCODED)
    @Produces(MediaType.APPLICATION_JSON)
    public Response registrarDeposito(@FormParam("cuenta") String cuenta,
                                      @FormParam("importe") double importe) {
        try {
            EurekaService service = new EurekaService();
            service.registrarDeposito(cuenta, importe, "0001");
            return Response.ok("{\"estado\":1}").build();
        } catch (Exception e) {
            return Response.status(Response.Status.INTERNAL_SERVER_ERROR)
                    .entity("{\"error\":\"" + e.getMessage() + "\"}").build();
        }
    }

    /** POST: Retiro (importe positivo; tipo 002) */
    @POST
    @Path("/retiro")
    @Consumes(MediaType.APPLICATION_FORM_URLENCODED)
    @Produces(MediaType.APPLICATION_JSON)
    public Response registrarRetiro(@FormParam("cuenta") String cuenta,
                                    @FormParam("importe") double importe) {
        try {
            EurekaService service = new EurekaService();
            service.registrarRetiro(cuenta, importe, "0001");
            return Response.ok("{\"estado\":1}").build();
        } catch (Exception e) {
            return Response.status(Response.Status.INTERNAL_SERVER_ERROR)
                    .entity("{\"error\":\"" + e.getMessage() + "\"}").build();
        }
    }

    /** POST: Transferencia (retiro + depósito) */
    @POST
    @Path("/transferencia")
    @Consumes(MediaType.APPLICATION_FORM_URLENCODED)
    @Produces(MediaType.APPLICATION_JSON)
    public Response registrarTransferencia(@FormParam("origen") String origen,
                                           @FormParam("destino") String destino,
                                           @FormParam("importe") double importe) {
        try {
            EurekaService service = new EurekaService();
            service.registrarTransferencia(origen, destino, importe, "0001");
            return Response.ok("{\"estado\":1}").build();
        } catch (Exception e) {
            return Response.status(Response.Status.INTERNAL_SERVER_ERROR)
                    .entity("{\"error\":\"" + e.getMessage() + "\"}").build();
        }
    }
}
