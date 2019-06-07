<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Editor.aspx.cs" Inherits="Sitecore.SharedSource.GoogleMaps.Editor" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Set Location On Map</title>
    
    <!--
    /********************************************************************************************** 
    * Product: Google Maps Module
    * Author : IE Agency (http://www.ie.com.au) - Heiko Franz
    * Purpose: Google Maps to be controlled from within Sitecore
    * Status : Published
    *
    * This is a Sitecore shared source module with Sitecore and IE both not liable for the use
    * of this code, please refer to the license information:
    * http://sdn.sitecore.net/Resources/Shared%20Source/Shared%20Source%20License.aspx
    **********************************************************************************************/
    -->

    <base target="_self" />

    <style>
        body { margin: 0; padding: 5px; overflow: hidden; font-family: tahoma; font-size: 11px; font-style: normal; color: #000; background-color: #F0F1F2; } 
        form { padding: 0; margin: 0; }
        #wrap { width: 690px; height: 30px; overflow: hidden; }
        #wrap input { float: left; margin-right: 20px; }
        .hidden { display: none; }
        .btnWrap { padding-left: 40px;}
        #btnSave { font-weight: bold; }
    </style>

    <script type="text/javascript">
        var geocoder;
        var map;
        var marker;

        var markers = [];

        var polyLine;
        var tmpPolyLine;
        var vmarkers = [];
        
        /*********
         HELPER
         *********/

        function preventFormSubmit(event) {
            if((window.event && window.event.keyCode == 13) || (event.which == 13)) {
                codeAddress();
                return false;
            }
        }

        function codeAddress() {
            var address = document.getElementById("address").value;
            geocoder.geocode({ 'address': address }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    map.setCenter(results[0].geometry.location);
                    dropMarker(results[0].geometry.location);                     
                } else {
                    alert("Geocode was not successful for the following reason: " + status);
                }
            });
        }

        /*********
         INITIALIZER
         *********/

        function initializeGMap() {
            geocoder = new google.maps.Geocoder();
            var myLatlng = new google.maps.LatLng(<%= StartPoint.Latitude.ToString() %>, <%= StartPoint.Longitude.ToString() %>);
            var myOptions = {
                zoom: <%= CurrentZoom %>,
                center: myLatlng,
                panControl: false,
                streetViewControl: false,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                draggableCursor: 'auto',
                draggingCursor: 'move'
            }
            map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
            
            google.maps.event.addListener(map, 'click', function (event) {
                <% if(EditMultiplePoints) { %> 
                polyClick(event.latLng);
                <% } else { %>
                dropMarker(event.latLng);
                <% } %>
            });
            
            // initialize, don't show yet
            marker = new google.maps.Marker({ position: myLatlng });

            <% if(!EditMultiplePoints && !IsDefaultMap) { %> 
            marker.setMap(map);
            <% } %>

            <% if(EditMultiplePoints) { %> 
            initPolyline();
            <% } %>
        }

        /*********
         POLYLINE
         *********/

        function initPolyline() {
            var polyOptions = {
                strokeColor: "#3355FF",
                strokeOpacity: 0.8,
                strokeWeight: 4
            };
            var tmpPolyOptions = {
                strokeColor: "#3355FF",
                strokeOpacity: 0.4,
                strokeWeight: 4
            };
            polyLine = new google.maps.Polyline(polyOptions);
            polyLine.setMap(map);
            tmpPolyLine = new google.maps.Polyline(tmpPolyOptions);
            tmpPolyLine.setMap(map);

            // init existing polyline
            if(document.getElementById("<%=txtMultiple.ClientID %>").value.length > 0) {
                var boundsRect = new google.maps.LatLngBounds();
                var coordinates = document.getElementById("<%=txtMultiple.ClientID %>").value.split("|");
                for(var i=0;i<coordinates.length;i++) {
                    var parts = coordinates[i].split(",");
                    var pos = new google.maps.LatLng(parts[0], parts[1]);
                    boundsRect.extend(pos);
                    polyClick(pos);
                }
                map.setCenter(boundsRect.getCenter());
                map.fitBounds(boundsRect);
            }
        };

        var createMarkerForPoly = function(point) {
            var imageNormal = new google.maps.MarkerImage("square.png",
                new google.maps.Size(11, 11),
                new google.maps.Point(0, 0),
                new google.maps.Point(6, 6)
            );
            var imageHover = new google.maps.MarkerImage("square_over.png",
                new google.maps.Size(11, 11),
                new google.maps.Point(0, 0),
                new google.maps.Point(6, 6)
            );
            var marker = new google.maps.Marker({
                position: point,
                map: map,
                icon: imageNormal,
                draggable: true
            });
            google.maps.event.addListener(marker, "mouseover", function() {
                marker.setIcon(imageHover);
            });
            google.maps.event.addListener(marker, "mouseout", function() {
                marker.setIcon(imageNormal);
            });
            google.maps.event.addListener(marker, "drag", function() {
                for (var m = 0; m < markers.length; m++) {
                    if (markers[m] == marker) {
                        polyLine.getPath().setAt(m, marker.getPosition());
                        moveVMarker(m);
                        break;
                    }
                }
                m = null;
            });
            google.maps.event.addListener(marker, "click", function() {
                for (var m = 0; m < markers.length; m++) {
                    if (markers[m] == marker) {
                        marker.setMap(null);
                        markers.splice(m, 1);
                        polyLine.getPath().removeAt(m);
                        removeVMarkers(m);
                        break;
                    }
                }
                m = null;
            });
            return marker;
        };
 
        var createVMarkerForPoly = function(point) {
            var prevpoint = markers[markers.length-2].getPosition();
            var imageNormal = new google.maps.MarkerImage("square_transparent.png",
                new google.maps.Size(11, 11),
                new google.maps.Point(0, 0),
                new google.maps.Point(6, 6)
            );
            var imageHover = new google.maps.MarkerImage("square_transparent_over.png",
                new google.maps.Size(11, 11),
                new google.maps.Point(0, 0),
                new google.maps.Point(6, 6)
            );
            var marker = new google.maps.Marker({
                position: new google.maps.LatLng(
                    point.lat() - (0.5 * (point.lat() - prevpoint.lat())),
                    point.lng() - (0.5 * (point.lng() - prevpoint.lng()))
                ),
                map: map,
                icon: imageNormal,
                draggable: true
            });
            google.maps.event.addListener(marker, "mouseover", function() {
                marker.setIcon(imageHover);
            });
            google.maps.event.addListener(marker, "mouseout", function() {
                marker.setIcon(imageNormal);
            });
            google.maps.event.addListener(marker, "dragstart", function() {
                for (var m = 0; m < vmarkers.length; m++) {
                    if (vmarkers[m] == marker) {
                        var tmpPath = tmpPolyLine.getPath();
                        tmpPath.push(markers[m].getPosition());
                        tmpPath.push(vmarkers[m].getPosition());
                        tmpPath.push(markers[m+1].getPosition());
                        break;
                    }
                }
                m = null;
            });
            google.maps.event.addListener(marker, "drag", function() {
                for (var m = 0; m < vmarkers.length; m++) {
                    if (vmarkers[m] == marker) {
                        tmpPolyLine.getPath().setAt(1, marker.getPosition());
                        break;
                    }
                }
                m = null;
            });
            google.maps.event.addListener(marker, "dragend", function() {
                for (var m = 0; m < vmarkers.length; m++) {
                    if (vmarkers[m] == marker) {
                        var newpos = marker.getPosition();
                        var startMarkerPos = markers[m].getPosition();
                        var firstVPos = new google.maps.LatLng(
                            newpos.lat() - (0.5 * (newpos.lat() - startMarkerPos.lat())),
                            newpos.lng() - (0.5 * (newpos.lng() - startMarkerPos.lng()))
                        );
                        var endMarkerPos = markers[m+1].getPosition();
                        var secondVPos = new google.maps.LatLng(
                            newpos.lat() - (0.5 * (newpos.lat() - endMarkerPos.lat())),
                            newpos.lng() - (0.5 * (newpos.lng() - endMarkerPos.lng()))
                        );
                        var newVMarker = createVMarkerForPoly(secondVPos);
                        newVMarker.setPosition(secondVPos);//apply the correct position to the vmarker
                        var newMarker = createMarkerForPoly(newpos);
                        markers.splice(m+1, 0, newMarker);
                        polyLine.getPath().insertAt(m+1, newpos);
                        marker.setPosition(firstVPos);
                        vmarkers.splice(m+1, 0, newVMarker);
                        tmpPolyLine.getPath().removeAt(2);
                        tmpPolyLine.getPath().removeAt(1);
                        tmpPolyLine.getPath().removeAt(0);
                        newpos = null;
                        startMarkerPos = null;
                        firstVPos = null;
                        endMarkerPos = null;
                        secondVPos = null;
                        newVMarker = null;
                        newMarker = null;
                        break;
                    }
                }
            });
            return marker;
        };
 
        var moveVMarker = function(index) {
           var newpos = markers[index].getPosition();
            if (index != 0) {
                var prevpos = markers[index-1].getPosition();
                vmarkers[index-1].setPosition(new google.maps.LatLng(
                    newpos.lat() - (0.5 * (newpos.lat() - prevpos.lat())),
                    newpos.lng() - (0.5 * (newpos.lng() - prevpos.lng()))
                ));
                prevpos = null;
            }
            if (index != markers.length - 1) {
                var nextpos = markers[index+1].getPosition();
                vmarkers[index].setPosition(new google.maps.LatLng(
                    newpos.lat() - (0.5 * (newpos.lat() - nextpos.lat())),
                    newpos.lng() - (0.5 * (newpos.lng() - nextpos.lng()))
                ));
                nextpos = null;
            }
            newpos = null;
            index = null;
        };
 
        var removeVMarkers = function(index) {
            if (markers.length > 0) {//clicked marker has already been deleted
                if (index != markers.length) {
                    vmarkers[index].setMap(null);
                    vmarkers.splice(index, 1);
                } else {
                    vmarkers[index-1].setMap(null);
                    vmarkers.splice(index-1, 1);
                }
            }
            if (index != 0 && index != markers.length) {
                var prevpos = markers[index-1].getPosition();
                var newpos = markers[index].getPosition();
                vmarkers[index-1].setPosition(new google.maps.LatLng(
                    newpos.lat() - (0.5 * (newpos.lat() - prevpos.lat())),
                    newpos.lng() - (0.5 * (newpos.lng() - prevpos.lng()))
                ));
                prevpos = null;
                newpos = null;
            }
            index = null;
        };

        function polyClick(position) {
            var marker = createMarkerForPoly(position);
            markers.push(marker);
            if (markers.length != 1) {
                var vmarker = createVMarkerForPoly(position);
                vmarkers.push(vmarker);
                vmarker = null;
            }
            var path = polyLine.getPath();
            path.push(position);
            marker = null;
        }

        function serializeMarkers() {
            <% if(EditMultiplePoints) { %> 
                var strMarkers = "";
                for(var i=0;i<markers.length;i++) {
                    strMarkers += markers[i].position.lat() + "," + markers[i].position.lng();
                    if(i != markers.length - 1)
                        strMarkers += "|";
                }
                document.getElementById("<%=txtMultiple.ClientID %>").value = strMarkers;
            return true;
            <% } else { %> 
            return true;
            <% } %> 
        }

        /*********
         SINGLE MARKER
         *********/
        function dropMarker(position) {
            marker.setMap(null);
            marker.setPosition(position);
            marker.setMap(map);
            map.setCenter(position);
            
            document.getElementById("<%=txtLat.ClientID %>").value = position.lat();
            document.getElementById("<%=txtLong.ClientID %>").value = position.lng();            
            document.getElementById("<%=txtZoom.ClientID %>").value = map.getZoom();
        }


        /*********
         LOAD MAP
         *********/
        function loadGMapScript() {
            var script = document.createElement("script");
            script.type = "text/javascript";
            script.src = "http://maps.googleapis.com/maps/api/js?sensor=false&callback=initializeGMap";
            document.body.appendChild(script);
        }

        window.onload = loadGMapScript;
    </script>

</head>
<body>
    <form id="form1" runat="server">   
        <div id="wrap">
            <input id="address" type="text" value="" placeholder="search for a city / location" onkeydown="return preventFormSubmit(event)">

            <input type="button" value="search" onclick="codeAddress()">
            
            <asp:Panel runat="server" ID="foobar">
            <asp:TextBox ID="txtLong" runat="server" CssClass="hidden" />
            <asp:TextBox ID="txtLat" runat="server" CssClass="hidden" />
            <asp:TextBox ID="txtMultiple" runat="server" CssClass="hidden" />
            <asp:TextBox ID="txtZoom" runat="server" CssClass="hidden" />
            </asp:Panel>

            <div class="btnWrap">
                <asp:Button ID="btnClose" runat="server" Text="close" OnClientClick="javascript:window.close();"  />
                <asp:Button ID="btnSave" runat="server" Text="save" onclick="btnSave_Click" OnClientClick="return serializeMarkers();" />
            </div>

        </div>    
        <div id="map_canvas" style="width:690px; height:460px"></div>
    </form>
</body>
</html>
