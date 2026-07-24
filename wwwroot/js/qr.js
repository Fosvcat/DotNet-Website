// Renders the TOTP setup QR code on the "Configure authenticator app"
// page using the qrcode.js library (wwwroot/lib/qrcode.js/qrcode.min.js).
// The authenticator URI is provided by the server via a data attribute
// on #qrCodeData, populated from EnableAuthenticatorModel.AuthenticatorUri.
window.addEventListener("load", function () {
    var dataElement = document.getElementById("qrCodeData");
    if (!dataElement) {
        return;
    }

    var uri = dataElement.getAttribute("data-url");
    if (!uri) {
        return;
    }

    new QRCode(document.getElementById("qrCode"), {
        text: uri,
        width: 160,
        height: 160
    });

    // Hide the "enable QR code generation" help notice now that the
    // QR code is actually rendering.
    var notice = document.getElementById("qrCodeNotice");
    if (notice) {
        notice.style.display = "none";
    }
});
