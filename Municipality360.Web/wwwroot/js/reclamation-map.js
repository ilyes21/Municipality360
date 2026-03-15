// ═══════════════════════════════════════════════════════════════════
//  reclamation-map.js
//  wwwroot/js/reclamation-map.js
//
//  Fonctions Leaflet pour l'interopérabilité Blazor (JSRuntime)
//  Appels depuis ReclamationForm.razor :
//    await JS.InvokeVoidAsync("recMap.init", containerId, lat, lng, dotNetRef)
//    await JS.InvokeVoidAsync("recMap.setMarker", lat, lng)
//    var coords = await JS.InvokeAsync<double[]>("recMap.getLocation")
//
//  ⚠️ Ajouter dans index.html (wwwroot) AVANT </body> :
//    <script src="https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/leaflet.min.js"></script>
//    <script src="js/reclamation-map.js"></script>
//  ⚠️ Ajouter dans index.html dans <head> :
//    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/leaflet.min.css" />
// ═══════════════════════════════════════════════════════════════════

window.recMap = (() => {
    let _map     = null;
    let _marker  = null;
    let _dotNet  = null;

    // ── Icône personnalisée ───────────────────────────────────────
    function makeIcon(color = '#f97316') {
        const svg = `
            <svg xmlns="http://www.w3.org/2000/svg" width="32" height="42" viewBox="0 0 32 42">
              <path d="M16 0C7.163 0 0 7.163 0 16c0 10.575 14 26 16 26S32 26.575 32 16C32 7.163 24.837 0 16 0z"
                    fill="${color}" stroke="white" stroke-width="2"/>
              <circle cx="16" cy="16" r="6" fill="white"/>
            </svg>`;
        return L.divIcon({
            html: svg,
            iconSize:   [32, 42],
            iconAnchor: [16, 42],
            popupAnchor:[0, -42],
            className: ''
        });
    }

    // ── Initialiser la carte ──────────────────────────────────────
    function init(containerId, lat, lng, dotNetRef) {
        _dotNet = dotNetRef;

        // Détruire la carte précédente si elle existe
        if (_map) {
            _map.remove();
            _map = null;
            _marker = null;
        }

        const el = document.getElementById(containerId);
        if (!el) { console.warn('recMap.init: element #' + containerId + ' not found'); return; }

        _map = L.map(containerId, { zoomControl: true }).setView([lat, lng], 13);

        // Tuiles OpenStreetMap
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
            maxZoom: 19
        }).addTo(_map);

        // Marqueur initial si coordonnées valides
        if (lat !== 0 && lng !== 0) {
            _setMarkerInternal(lat, lng);
        }

        // Clic sur la carte
        _map.on('click', function (e) {
            const { lat, lng } = e.latlng;
            _setMarkerInternal(lat, lng);
            if (_dotNet) {
                _dotNet.invokeMethodAsync('OnMapClick',
                    Math.round(lat * 1e6) / 1e6,
                    Math.round(lng * 1e6) / 1e6);
            }
        });

        // Invalidate size après rendu (Blazor peut déclencher avant que le DOM soit stable)
        setTimeout(() => { if (_map) _map.invalidateSize(); }, 200);
    }

    // ── Placer / déplacer le marqueur ─────────────────────────────
    function _setMarkerInternal(lat, lng) {
        if (!_map) return;
        if (_marker) {
            _marker.setLatLng([lat, lng]);
        } else {
            _marker = L.marker([lat, lng], {
                icon: makeIcon(),
                draggable: true
            }).addTo(_map)
              .bindPopup('<div style="font-family:Tajawal,sans-serif;text-align:center;font-size:.84rem;padding:.25rem">'
                       + '<strong>موقع الشكوى</strong><br/>'
                       + 'يمكن سحب هذه العلامة لتعديل الموقع</div>');

            // Drag end → notifier Blazor
            _marker.on('dragend', function (e) {
                const pos = e.target.getLatLng();
                if (_dotNet) {
                    _dotNet.invokeMethodAsync('OnMapClick',
                        Math.round(pos.lat * 1e6) / 1e6,
                        Math.round(pos.lng * 1e6) / 1e6);
                }
            });
        }
        _map.setView([lat, lng], Math.max(_map.getZoom(), 15));
    }

    // ── API publique : setMarker (appelée depuis Blazor) ──────────
    function setMarker(lat, lng) {
        _setMarkerInternal(lat, lng);
    }

    // ── Géolocalisation du navigateur ─────────────────────────────
    function getLocation() {
        return new Promise((resolve, reject) => {
            if (!navigator.geolocation) {
                reject(new Error('Géolocalisation non disponible'));
                return;
            }
            navigator.geolocation.getCurrentPosition(
                (pos) => {
                    const coords = [pos.coords.latitude, pos.coords.longitude];
                    // Centrer la carte sur la position actuelle
                    if (_map) {
                        _setMarkerInternal(coords[0], coords[1]);
                    }
                    resolve(coords);
                },
                (err) => reject(err),
                { enableHighAccuracy: true, timeout: 10000, maximumAge: 60000 }
            );
        });
    }

    // ── Détruire la carte ─────────────────────────────────────────
    function destroy() {
        if (_map) { _map.remove(); _map = null; _marker = null; _dotNet = null; }
    }

    return { init, setMarker, getLocation, destroy };
})();
