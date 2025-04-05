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

window.maps = window.maps || {};

window.maps.initLocationPicker = function (mapId, dotNetHelper) {
    const map = L.map(mapId).setView([23.8103, 90.4125], 13); // Default center

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    let marker;

    map.on('click', async function (e) {
        const { lat, lng } = e.latlng;

        if (marker) {
            marker.setLatLng([lat, lng]);
        } else {
            marker = L.marker([lat, lng], { draggable: true }).addTo(map);
            marker.on('dragend', function (e) {
                const pos = e.target.getLatLng();
                reverseGeocode(pos.lat, pos.lng, dotNetHelper);
            });
        }

        reverseGeocode(lat, lng, dotNetHelper);
    });

    window.maps[mapId] = {
        map: map,
        marker: marker
    };
};

async function reverseGeocode(lat, lon, dotNetHelper) {
    const url = `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lat}&lon=${lon}`;
    const response = await fetch(url);
    const data = await response.json();

    if (data && data.display_name) {
        await dotNetHelper.invokeMethodAsync('OnAddressSelected', data.display_name);
    }
}


window.initMap = function (mapId, lat, lng) {
    const map = L.map(mapId).setView([lat, lng], 13);

    // Add tile layer
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    // Save to window so we can update it later
    window.driverMaps = window.driverMaps || {};
    window.driverMaps[mapId] = {
        map: map,
        marker: L.marker([lat, lng]).addTo(map)
    };
};

window.updateDriverLocation = function (mapId, lat, lng) {
    const mapObj = window.driverMaps?.[mapId];
    if (!mapObj) return;

    mapObj.marker.setLatLng([lat, lng]);
    mapObj.map.setView([lat, lng], 13);
};



