<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GoogleMap.ascx.cs" Inherits="Sitecore.SharedSource.GoogleMaps.Sublayouts.GoogleMap" %>

<% if (CurrentMap != null) { %>

<div id="<%= CurrentMap.CanvasID %>" style="width:<%= CurrentMap.Width %>px; height:<%= CurrentMap.Height %>px"></div>

<script type="text/javascript">
    
    // initialize the two global arrays for all maps on the page
    if(mapData == undefined) {
        var mapData = new Array(); // array holding all seralized Sitecore content
        var map = new Array(); // array holding all Google Maps objects
    }
    
    mapData.push(<%= Sitecore.SharedSource.GoogleMaps.Utilities.JSON(CurrentMap) %>);   

    // initializes the Google Maps by translating the serialized Sitecore 
    // items into Google Maps objects
    function initializeGMap() {
        // extend the google maps object to add a pointer to the current info window
        // and a function to close the current info window
        google.maps.Map.prototype.latestInfoWindow;
        google.maps.Map.prototype.closeInfoWindow = function() {
            if(this.latestInfoWindow != undefined)
                this.latestInfoWindow.close();
        };

        // initialize all maps on the page
        for(var mapCount=0; mapCount < mapData.length; mapCount++)
        {
            // figure out the initial map type (first one in the list or default to a roadmap if nothing's defined)
            var allowedMapTypes = new Array();
            if (mapData[mapCount].MapTypes.length > 0) {
                var initialMapType = eval(mapData[mapCount].MapTypes[0]);                 
                for(var i=0; i<mapData[mapCount].MapTypes.length; i++) 
                {
                    allowedMapTypes.push(eval(mapData[mapCount].MapTypes[i]));
                }
            } else {
                var initialMapType = google.maps.MapTypeId.ROADMAP;
            }

            // initial map options
            var mapOptions = {            
                center: new google.maps.LatLng(mapData[mapCount].Center.Latitude, mapData[mapCount].Center.Longitude),
                disableDefaultUI: mapData[mapCount].DisableAllDefaultUIElements,
                disableDoubleClickZoom: !mapData[mapCount].EnableDoubleClickZoom,
                draggable: mapData[mapCount].EnableDragging,
                draggableCursor: mapData[mapCount].DraggableCursor,
                draggingCursor: mapData[mapCount].DraggingCursor,
                keyboardShortcuts: mapData[mapCount].EnableKeyboardFunctionality,
                mapTypeControlOptions: {
                    mapTypeIds: allowedMapTypes
                },
                mapTypeId: initialMapType,
                maxZoom: mapData[mapCount].MaxZoomLevel,
                minZoom: mapData[mapCount].MinZoomLevel,
                overviewMapControl: mapData[mapCount].EnableOverview,
                overviewMapControlOptions: {
                    opened: mapData[mapCount].EnableOverview
                },
                panControl: mapData[mapCount].EnablePanControl,
                scaleControl: mapData[mapCount].EnableScaleControl,
                scrollwheel: mapData[mapCount].EnableScrollWheelZoom,
                streetViewControl: mapData[mapCount].EnableStreetViewControl,
                zoom: mapData[mapCount].Zoom,
                zoomControl: mapData[mapCount].EnableZoomControl
            };

            // initialize the map
            map.push(new google.maps.Map(document.getElementById(mapData[mapCount].CanvasID), mapOptions));
            
            // add marker, lines and polygons as arrays on the map object to support multiple maps per page
            map[mapCount].markers = new Array();
            map[mapCount].lines = new Array();
            map[mapCount].polygons = new Array();

        
            // close any open info windows when clicking on the map
            google.maps.event.addListener(map[mapCount], 'click', function(){
                this.closeInfoWindow();
            });
            
            // initialize all markers        
            for(var i=0; i<mapData[mapCount].Markers.length; i++) {
                var m = new google.maps.Marker({
                    position: new google.maps.LatLng(mapData[mapCount].Markers[i].Position.Latitude, mapData[mapCount].Markers[i].Position.Longitude), 
                    map: map[mapCount], 
                    title: mapData[mapCount].Markers[i].Title
                });
            
                m.infowindow = new google.maps.InfoWindow({
                        content: mapData[mapCount].Markers[i].InfoWindow                    
                });

                map[mapCount].markers.push(m);            

                if(mapData[mapCount].Markers[i].InfoWindow.length > 0)
                {
                    google.maps.event.addListener(m, 'click', function() {
                        this.map.closeInfoWindow();
                        this.infowindow.open(this.map, this);
                        this.map.latestInfoWindow = this.infowindow;
                    });
                }
                if(mapData[mapCount].Markers[i].CustomIcon != undefined)
                {
                    m.icon = new google.maps.MarkerImage(mapData[mapCount].Markers[i].CustomIcon.ImageURL,
                                new google.maps.Size(mapData[mapCount].Markers[i].CustomIcon.ImageDimensions.Width, mapData[mapCount].Markers[i].CustomIcon.ImageDimensions.Height),
                                new google.maps.Point(0,0),
                                new google.maps.Point(mapData[mapCount].Markers[i].CustomIcon.Anchor.X, mapData[mapCount].Markers[i].CustomIcon.Anchor.Y));
                
                    if(mapData[mapCount].Markers[i].CustomIcon.ShadowURL != undefined)
                    {
                        m.shadow = new google.maps.MarkerImage(mapData[mapCount].Markers[i].CustomIcon.ShadowURL,
                                new google.maps.Size(mapData[mapCount].Markers[i].CustomIcon.ShadowDimensions.Width, mapData[mapCount].Markers[i].CustomIcon.ShadowDimensions.Height),
                                new google.maps.Point(0,0),
                                new google.maps.Point(mapData[mapCount].Markers[i].CustomIcon.ShadowAnchor.X, mapData[mapCount].Markers[i].CustomIcon.ShadowAnchor.Y));
                    }
                    if(mapData[mapCount].Markers[i].CustomIcon.ClickablePolygon != undefined && mapData[mapCount].Markers[i].CustomIcon.ClickablePolygon.length > 0)
                    {
                        var coords = "[" + mapData[mapCount].Markers[i].CustomIcon.ClickablePolygon + "]"; 
                        m.shape  =  
                        {
                            coord: eval(coords),
                            type: 'poly'
                        };
                    }
                }
            }
            
            // initialize all lines
            for(var i=0; i<mapData[mapCount].Lines.length; i++) {
                var lineOptions = {
                    path: new Array(),
                    strokeColor: mapData[mapCount].Lines[i].StrokeColor,
                    strokeOpacity: mapData[mapCount].Lines[i].StrokeOpacity,
                    strokeWeight: mapData[mapCount].Lines[i].StrokeWeight,
                    clickable: false,
                    map: map[mapCount]
                }
                for(var j=0; j<mapData[mapCount].Lines[i].Points.length; j++) {
                    lineOptions.path.push(new google.maps.LatLng(mapData[mapCount].Lines[i].Points[j].Latitude, mapData[mapCount].Lines[i].Points[j].Longitude));
                }
                var line = new google.maps.Polyline(lineOptions);
                map[mapCount].lines.push(line);
            }

            // initialize all polygons
            for(var i=0; i<mapData[mapCount].Polygons.length; i++) {
                var polyOptions = {
                    paths: new Array(),
                    strokeColor: mapData[mapCount].Polygons[i].StrokeColor,
                    strokeOpacity: mapData[mapCount].Polygons[i].StrokeOpacity,
                    strokeWeight: mapData[mapCount].Polygons[i].StrokeWeight,
                    fillColor: mapData[mapCount].Polygons[i].FillColor,
                    fillOpacity: mapData[mapCount].Polygons[i].FillOpacity,
                    clickable: mapData[mapCount].Polygons[i].Clickable,
                    map: map[mapCount]
                }
                for(var j=0; j<mapData[mapCount].Polygons[i].Points.length; j++) {
                    polyOptions.paths.push(new google.maps.LatLng(mapData[mapCount].Polygons[i].Points[j].Latitude, mapData[mapCount].Polygons[i].Points[j].Longitude));
                }
                var poly = new google.maps.Polygon(polyOptions);
                map[mapCount].polygons.push(poly);
            
                // add info window
                if(polyOptions.clickable && mapData[mapCount].Polygons[i].InfoWindow.length > 0) {
                    poly.infowindow = new google.maps.InfoWindow({
                        content: mapData[mapCount].Polygons[i].InfoWindow
                    });
                    google.maps.event.addListener(map[mapCount].polygons[i], 'click', function(event) {
                        this.map.closeInfoWindow();
                        this.infowindow.setPosition(event.latLng);
                        this.infowindow.open(this.map);
                        this.map.latestInfoWindow = this.infowindow;
                    });
                }
            }
        }                
    }

    function loadGMapScript() {
        var script = document.createElement("script");
        script.type = "text/javascript";
        script.src = "http://maps.googleapis.com/maps/api/js?sensor=false&callback=initializeGMap";
        document.body.appendChild(script);
    }
    
    if (window.addEventListener && !GMapLoadedAlready) {
        window.addEventListener("load", loadGMapScript, false);
        var GMapLoadedAlready = true;
    } else if (window.attachEvent && !GMapLoadedAlready) {
        window.attachEvent("onload", loadGMapScript);
        var GMapLoadedAlready = true;
    }
</script>
<% } else { %>
    <div style="width: 250px; height: 250px; border: 1px solid black; background-color: #CCC;"><b>Your Google Map is almost ready to use.</b><br /> Please set the data source in the component properties / presentation settings.</div>
<% } %>