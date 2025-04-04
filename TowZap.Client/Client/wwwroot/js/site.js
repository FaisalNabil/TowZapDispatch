window.toast = {
    success: function (message) {
        toastr.success(message);
    },
    error: function (message) {
        toastr.error(message);
    },
    info: function (message) {
        toastr.info(message);
    },
    warning: function (message) {
        toastr.warning(message);
    }
};

window.updateDriverMarker = function (lat, lng) {
    if (window.driverMarker) {
        window.driverMarker.setLatLng([lat, lng]);
    } else {
        window.driverMarker = L.marker([lat, lng], { title: "Driver" }).addTo(window.map);
    }
}

