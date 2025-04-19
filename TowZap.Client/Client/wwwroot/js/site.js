// ==================== TOAST NOTIFICATIONS ====================
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

// ==================== SIDEBAR CONTROLS ====================
function closeOffcanvasSidebar() {
    const sidebar = bootstrap.Offcanvas.getInstance(document.getElementById('sidebar'));
    if (sidebar) {
        sidebar.hide();
    }
}

// ==================== GLOBAL INITIALIZATION ====================
window.maps = window.maps || {};
window.driverMaps = window.driverMaps || {};


// ==================== REVERSE GEOCODING ====================
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

window.reverseGeocodeCache = {};

async function reverseGeocode(lat, lon, dotNetHelper) {
    const key = `${lat.toFixed(5)},${lon.toFixed(5)}`;

    if (window.reverseGeocodeCache[key]) {
        await dotNetHelper.invokeMethodAsync('OnAddressSelected', window.reverseGeocodeCache[key]);
        return;
    }

    const url = `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lat}&lon=${lon}`;
    const response = await fetch(url);
    const data = await response.json();

    if (data && data.display_name) {
        const address = data.display_name;
        window.reverseGeocodeCache[key] = address;
        await dotNetHelper.invokeMethodAsync('OnAddressSelected', address);
    }
}

async function reverseGeocodeAndSend(lat, lon, dotNetHelper) {
    const url = `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lat}&lon=${lon}`;
    const response = await fetch(url);
    const data = await response.json();

    if (data?.display_name) {
        await dotNetHelper.invokeMethodAsync("OnLocationSelectedJS", data.display_name, parseFloat(lat), parseFloat(lon));
    }
}

// ==================== ICONS ====================
const sourceIcon = L.icon({
    iconUrl: 'images/source-marker.png',
    iconSize: [30, 30],
    iconAnchor: [15, 30]
});

const destinationIcon = L.icon({
    iconUrl: 'images/destination-marker.png',
    iconSize: [30, 30],
    iconAnchor: [15, 30]
});

const towTruckIcon = L.icon({
    iconUrl: 'images/towtruck-icon.png',
    iconSize: [40, 40],
    iconAnchor: [20, 20]
});

// ==================== BASIC MAP INITIALIZATION ====================
window.initMap = function (mapId, lat, lng, fromLat, fromLng, toLat, toLng, towardSource) {
    const map = L.map(mapId).setView([lat, lng], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    const truckMarker = L.marker([lat, lng], { icon: towTruckIcon }).addTo(map);
    const sourceMarker = L.marker([fromLat, fromLng], { icon: sourceIcon }).addTo(map);
    const destinationMarker = L.marker([toLat, toLng], { icon: destinationIcon }).addTo(map);

    const initialTrail = towardSource
        ? [L.latLng(lat, lng), L.latLng(fromLat, fromLng)]
        : [L.latLng(lat, lng), L.latLng(toLat, toLng)];

    const trail = L.polyline(initialTrail, { color: 'blue' }).addTo(map);

    window.driverMaps = window.driverMaps || {};
    window.driverMaps[mapId] = {
        map,
        marker: truckMarker,
        trail,
        lastLatLng: L.latLng(lat, lng),
        source: L.latLng(fromLat, fromLng),
        destination: L.latLng(toLat, toLng),
        sourceMarker,
        destinationMarker,
        towardSource
    };
};

// ==================== DRIVER LOCATION UPDATES ====================
window.updateDriverLocation = function (mapId, lat, lng) {
    const mapObj = window.driverMaps?.[mapId];
    if (!mapObj) return;

    const newLatLng = L.latLng(lat, lng);
    const angle = getAngle(mapObj.lastLatLng, newLatLng);

    mapObj.marker.setLatLng(newLatLng);
    mapObj.marker.setRotationAngle?.(angle);
    mapObj.map.setView(newLatLng);

    const endPoint = mapObj.towardSource ? mapObj.source : mapObj.destination;

    console.log("Fetching optimal route from", newLatLng, "to", endPoint);

    fetchOptimalRoute(lat, lng, endPoint.lat, endPoint.lng).then(routeCoords => {
        if (routeCoords.length > 0) {
            console.log("✅ Route coords:", routeCoords);
            mapObj.trail.setLatLngs(routeCoords);
        } else {
            console.warn("❌ Route empty, falling back to straight line.");
            mapObj.trail.setLatLngs([newLatLng, endPoint]);
        }
    });

    mapObj.lastLatLng = newLatLng;
};


function getAngle(start, end) {
    const dx = end.lng - start.lng;
    const dy = end.lat - start.lat;
    const rad = Math.atan2(dy, dx);
    return rad * 180 / Math.PI;
}

// ==================== ROUTING: FETCH OPTIMAL ROUTE ====================
async function fetchOptimalRoute(fromLat, fromLng, toLat, toLng) {
    try {
        const url = `https://router.project-osrm.org/route/v1/driving/${fromLng},${fromLat};${toLng},${toLat}?overview=full&geometries=geojson`;
        const response = await fetch(url);
        const data = await response.json();

        if (data.routes?.length > 0) {
            const coords = data.routes[0].geometry.coordinates;
            // Convert [lng, lat] to Leaflet-friendly [lat, lng]
            return coords.map(coord => [coord[1], coord[0]]);
        }
    } catch (error) {
        console.error("Route fetch failed:", error);
    }

    return []; // fallback
}


// ==================== LOCATION PICKER ====================
window.maps.initLocationPicker = function (mapId, dotNetHelper) {
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

// ==================== LOCATION SEARCH ====================
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
