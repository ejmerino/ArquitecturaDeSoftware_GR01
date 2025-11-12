package ec.edu.monster.service;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import ec.edu.monster.model.Movimiento;

import java.io.*;
import java.lang.reflect.Type;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.util.Collections;
import java.util.List;

public class EurekaApiClient {

    // ⚠️ Cambia host/IP si tu API REST está en otra máquina
    private static final String BASE =
        "http://10.40.31.127:8080/WSEurekaBank_GRO01/webresources/coreBancario";

    private final Gson gson = new Gson();

    public List<Movimiento> movimientos(String cuenta) throws Exception {
        String endpoint = BASE + "/movimientos/" + encode(cuenta);
        HttpURLConnection con = (HttpURLConnection) new URL(endpoint).openConnection();
        con.setRequestMethod("GET");
        con.setRequestProperty("Accept", "application/json");
        int code = con.getResponseCode();
        String body = read(con);
        if (code != 200) throw new RuntimeException(body);
        Type listType = new TypeToken<List<Movimiento>>(){}.getType();
        List<Movimiento> lista = gson.fromJson(body, listType);
        if (lista == null) return Collections.emptyList();
        lista.sort((a,b) -> b.getNromov() - a.getNromov());
        return lista;
    }

    public void deposito(String cuenta, double importe) throws Exception {
        postForm("/deposito", "cuenta="+encode(cuenta)+"&importe="+encode(String.valueOf(importe)));
    }

    public void retiro(String cuenta, double importe) throws Exception {
        postForm("/retiro", "cuenta="+encode(cuenta)+"&importe="+encode(String.valueOf(importe)));
    }

    public void transferencia(String origen, String destino, double importe) throws Exception {
        postForm("/transferencia",
                "origen="+encode(origen)+"&destino="+encode(destino)+"&importe="+encode(String.valueOf(importe)));
    }

    // ------- helpers -------
    private void postForm(String path, String params) throws Exception {
        HttpURLConnection con = (HttpURLConnection) new URL(BASE + path).openConnection();
        con.setRequestMethod("POST");
        con.setDoOutput(true);
        con.setRequestProperty("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
        try (OutputStream os = con.getOutputStream()) {
            os.write(params.getBytes(StandardCharsets.UTF_8));
        }
        int code = con.getResponseCode();
        String body = read(con);
        if (code != 200) throw new RuntimeException(body);
    }

    private static String read(HttpURLConnection con) throws Exception {
        InputStream stream = (con.getResponseCode() >= 400) ? con.getErrorStream() : con.getInputStream();
        try (BufferedReader br = new BufferedReader(new InputStreamReader(stream, StandardCharsets.UTF_8))) {
            StringBuilder sb = new StringBuilder();
            String line; while ((line = br.readLine()) != null) sb.append(line);
            return sb.toString();
        }
    }

    private static String encode(String s) throws UnsupportedEncodingException {
        return URLEncoder.encode(s, "UTF-8");
    }
}
