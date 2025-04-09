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


window.maps.initLocationPickerWithSearch = function (mapId, dotNetHelper) {
    const mapContainer = document.getElementById(mapId);
    if (!mapContainer) {
        console.warn(`Map container with id '${mapId}' not found.`);
        return;
    }


    // Remove any old instance (in case of modal reopen)
    if (window.maps[mapId]?.map) {
        window.maps[mapId].map.remove();
    }

    const map = L.map(mapId).setView([23.8103, 90.4125], 13);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    let marker;

    map.on('click', function (e) {
        const { lat, lng } = e.latlng;

        if (marker) {
            marker.setLatLng([lat, lng]);
        } else {
            marker = L.marker([lat, lng], { draggable: true }).addTo(map);
            marker.on('dragend', function (event) {
                const pos = event.target.getLatLng();
                reverseGeocodeAndSend(pos.lat, pos.lng, dotNetHelper);
            });
        }

        reverseGeocodeAndSend(lat, lng, dotNetHelper);
    });

    window.maps[mapId] = {
        map: map,
        marker: marker,
        dotNetHelper: dotNetHelper
    };
};

window.maps.searchLocation = async function (mapId, query) {
    if (!query || query.length < 3) return;

    const url = `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(query)}`;
    const response = await fetch(url);
    const data = await response.json();

    if (data.length > 0) {
        const { lat, lon, display_name } = data[0];
        const mapObj = window.maps[mapId];

        mapObj.map.setView([lat, lon], 14);

        if (mapObj.marker) {
            mapObj.marker.setLatLng([lat, lon]);
        } else {
            mapObj.marker = L.marker([lat, lon], { draggable: true }).addTo(mapObj.map);
            mapObj.marker.on('dragend', function (event) {
                const pos = event.target.getLatLng();
                reverseGeocodeAndSend(pos.lat, pos.lng, mapObj.dotNetHelper);
            });
        }

        reverseGeocodeAndSend(lat, lon, mapObj.dotNetHelper);
    }
};

async function reverseGeocodeAndSend(lat, lon, dotNetHelper) {
    const url = `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lat}&lon=${lon}`;
    const response = await fetch(url);
    const data = await response.json();

    if (data?.display_name) {
        await dotNetHelper.invokeMethodAsync("OnLocationSelectedJS", data.display_name, parseFloat(lat), parseFloat(lon));
    }
}


