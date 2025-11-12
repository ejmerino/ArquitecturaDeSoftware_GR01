document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("frmLogin");
  if (!form) return;

  form.addEventListener("submit", (e) => {
    e.preventDefault();
    const user = (document.getElementById("usuario").value || "").trim();
    const pass = (document.getElementById("clave").value || "").trim();

    if (!user || !pass) {
      alert("Ingresa usuario y clave.");
      return;
    }
    // Login ficticio solo para ejemplo:
    sessionStorage.setItem("eb_user", user);
    location.href = "eurekabank.html";
  });
});
