// =====================================================================
// === Configuración y Elementos DOM ===
// =====================================================================
const SOAP_URL = "http://localhost:3000/soap-proxy"; // Apunta al proxy de Express
const NAMESPACE = "http://service.monster.edu.ec/";

// Elementos del login
const loginContainer = document.getElementById('login-container');
const usernameInput = document.getElementById('username');
const passwordInput = document.getElementById('password');
const loginErrorMessageElement = document.getElementById('login-error-message');

// Elementos del conversor
const converterApp = document.getElementById('converter-app');
const soapErrorMessageElement = document.getElementById('soap-error-message');

// Credenciales quemadas
const VALID_USERNAME = "MONSTER";
const VALID_PASSWORD = "MONSTER9";

// Mapa de conversiones disponibles en el servicio SOAP
const CONVERSION_MAP = {
    temp: {
        'celsius_kelvin': { method: 'celsius_a_kelvin', param: 'celsius' },
        'celsius_fahrenheit': { method: 'celsius_a_fahrenheit', param: 'celsius' },
        'fahrenheit_celsius': { method: 'fahrenheit_a_celsius', param: 'fahrenheit' }
    },
    length: {
        'centimetros_metros': { method: 'centimetros_a_metros', param: 'centimetros' },
        'metros_kilometros': { method: 'metros_a_kilometros', param: 'metros' },
        'kilometros_metros': { method: 'kilometros_a_metros', param: 'kilometros' }
    },
    mass: {
        'kilogramos_gramos': { method: 'kilogramos_a_gramos', param: 'kilogramos' },
        'gramos_kilogramos': { method: 'gramos_a_kilogramos', param: 'gramos' },
        'libras_kilogramos': { method: 'libras_a_kilogramos', param: 'libras' }
    }
};

// =====================================================================
// === Funciones de UI y Mensajes (CORREGIDAS) ===
// =====================================================================

function showElement(element) {
    element.classList.remove('hidden');
}

function hideElement(element) {
    element.classList.add('hidden');
}

function displayMessage(element, message) {
    element.textContent = message;
    if (element.classList.contains('validation-message')) {
        element.style.display = 'block';
    } else {
        showElement(element);
    }
}

function clearMessage(element) {
    element.textContent = '';
    if (element.classList.contains('validation-message')) {
        element.style.display = 'none';
    } else {
        hideElement(element);
    }
}

// =====================================================================
// === Lógica de Autenticación ===
// =====================================================================

function handleLogin() {
    clearMessage(loginErrorMessageElement);
    if (usernameInput.value === VALID_USERNAME && passwordInput.value === VALID_PASSWORD) {
        sessionStorage.setItem('isLoggedIn', 'true');
        renderApp();
    } else {
        displayMessage(loginErrorMessageElement, "Usuario o contraseña incorrectos.");
    }
}

function handleLogout() {
    sessionStorage.removeItem('isLoggedIn');
    renderApp();
}

function renderApp() {
    const isLoggedIn = sessionStorage.getItem('isLoggedIn') === 'true';
    if (isLoggedIn) {
        hideElement(loginContainer);
        showElement(converterApp);
        document.body.style.alignItems = 'flex-start'; // Ajustar para que el conversor se vea bien
        clearMessage(loginErrorMessageElement);
        clearMessage(soapErrorMessageElement);
        ['temp', 'length', 'mass'].forEach(validateConversion);
    } else {
        showElement(loginContainer);
        hideElement(converterApp);
        document.body.style.alignItems = 'center'; // Centrar el login
        usernameInput.value = '';
        passwordInput.value = '';
        clearMessage(loginErrorMessageElement);
    }
}

// =====================================================================
// === Lógica del Cliente SOAP ===
// =====================================================================
async function callSoapService(methodName, parameterName, parameterValue) {
    clearMessage(soapErrorMessageElement);
    const soapRequest = `<?xml version="1.0" encoding="UTF-8"?>
        <S:Envelope xmlns:S="http://schemas.xmlsoap.org/soap/envelope/" xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/">
            <SOAP-ENV:Header/><S:Body>
                <ns2:${methodName} xmlns:ns2="${NAMESPACE}"><${parameterName}>${parameterValue}</${parameterName}></ns2:${methodName}>
            </S:Body></S:Envelope>`;
    try {
        const response = await fetch(SOAP_URL, { method: 'POST', headers: { 'Content-Type': 'text/xml; charset=utf-8', 'SOAPAction': `${NAMESPACE}${methodName}` }, body: soapRequest, mode: 'cors' });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}.`);
        const responseText = await response.text();
        const parser = new DOMParser();
        const xmlDoc = parser.parseFromString(responseText, "text/xml");
        const resultNode = xmlDoc.querySelector(`${methodName}Response return`) || xmlDoc.querySelector('return');
        if (resultNode) return parseFloat(resultNode.textContent);
        throw new Error("No se encontró el resultado en la respuesta SOAP.");
    } catch (error) {
        console.error("Error al llamar al servicio SOAP:", error);
        displayMessage(soapErrorMessageElement, `Problema de conexión con el servicio. Detalles: ${error.message}`);
        return null;
    }
}

// =====================================================================
// === Lógica de Validación y Conversión ===
// =====================================================================
function validateConversion(category) {
    const unitFrom = document.getElementById(`${category}UnitFrom`).value;
    const unitTo = document.getElementById(`${category}UnitTo`).value;
    const convertBtn = document.getElementById(`${category}ConvertBtn`);
    const validationMessageElement = document.getElementById(`${category}ValidationMessage`);
    const resultElement = document.getElementById(`${category}Result`);
    clearMessage(validationMessageElement);
    resultElement.textContent = '--';
    if (unitFrom === unitTo) {
        convertBtn.disabled = false;
        return true;
    }
    const conversionKey = `${unitFrom}_${unitTo}`;
    if (CONVERSION_MAP[category]?.[conversionKey]) {
        convertBtn.disabled = false;
        return true;
    } else {
        convertBtn.disabled = true;
        displayMessage(validationMessageElement, `Conversión de ${unitFrom} a ${unitTo} no disponible.`);
        return false;
    }
}

async function performConversion(category) {
    const valueInput = document.getElementById(`${category}Value`);
    const unitFrom = document.getElementById(`${category}UnitFrom`).value;
    const unitTo = document.getElementById(`${category}UnitTo`).value;
    const resultElement = document.getElementById(`${category}Result`);
    resultElement.textContent = '--';
    const value = parseFloat(valueInput.value);
    if (isNaN(value)) {
        displayMessage(soapErrorMessageElement, "Por favor, introduce un número válido.");
        return;
    }
    if (!validateConversion(category)) return;
    if (unitFrom === unitTo) {
        resultElement.textContent = value.toFixed(3);
        clearMessage(soapErrorMessageElement);
        return;
    }
    const conversionInfo = CONVERSION_MAP[category][`${unitFrom}_${unitTo}`];
    if (conversionInfo) {
        const result = await callSoapService(conversionInfo.method, conversionInfo.param, value);
        if (result !== null) {
            resultElement.textContent = result.toFixed(3);
        } else {
            resultElement.textContent = 'Error';
        }
    }
}
function convertTemperature() { performConversion('temp'); }
function convertLength() { performConversion('length'); }
function convertMass() { performConversion('mass'); }
// =====================================================================
// === Inicialización ===
// =====================================================================
document.addEventListener('DOMContentLoaded', renderApp);