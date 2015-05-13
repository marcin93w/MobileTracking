﻿var apiUrl = "http://ec2-54-187-122-224.us-west-2.compute.amazonaws.com/api";
var map = null;
var mapMarkers = [];

$( document ).ready(function() {
    initialize();
});

function initialize() {
	var mapOptions = {
		center: { lat: 50, lng: 20},
		zoom: 8
	};
	map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
	
	getUsersData();
	$('#load').click(function () {
	    getLocationData($('#users').val(), $('#start-date').val(), $('#end-date').val());
	});
}

function getLocationData(userId, startTime, endTime) {
    var params = "userId=" + userId + "&start=" + startTime + "&end=" + endTime;
	var result = $.ajax( apiUrl + "/LocationData/GetLocations?" + params )
	  .done(function(data) {
		populateMap(data);
	  })
	  .fail(function() {
		alert( "Błąd podczas pobierania danych z serwera" );
	  });
}

function populateMap(data) {
    mapMarkers.map(function(marker) {
        marker.setMap(null);
    });
    mapMarkers = [];

    map.fitBounds(new google.maps.LatLngBounds(
        new google.maps.LatLng(data.Boundary._miny, data.Boundary._minx),
        new google.maps.LatLng(data.Boundary._maxy, data.Boundary._maxx)));

    if (data.Locations.length > 0) {
        var points = [data.Locations[0], data.Locations[data.Locations.length - 1]];
        points.forEach(function(point) {
            mapMarkers.push(new google.maps.Marker({
                position: new google.maps.LatLng(point.Lat, point.Lon),
                map: map,
                title: point.Time
            }));
        });

        $('#start-time').text(formatDateTime(points[0].Time));
        $('#end-time').text(formatDateTime(points[1].Time));
    }

    mapMarkers.push(new google.maps.Polyline({
        path: data.Locations.map(function (point) {
            return new google.maps.LatLng(point.Lat, point.Lon);
        }),
        geodesic: true,
        map: map,
        //strokeColor: '#FF0000',
        //strokeOpacity: 1.0,
        //strokeWeight: 2
    }));

    $('#points-count').text(data.Locations.length);
    $('#max-speed').text(data.MaxSpeed);
}

function formatDateTime(dateTime) {
    return dateTime.replace('T', ' ').split('.')[0];
}

function getUsersData() {
    var result = $.ajax(apiUrl + "/Users/GetAll")
	  .done(function (data) {
	      populateUsersSelect(data);
	  })
	  .fail(function () {
	      alert("Błąd podczas pobierania danych z serwera");
	  });
}

function populateUsersSelect(data) {
    $.each(data, function (i, user) {
        $('#users').append($('<option>', { 
            value: user.Id,
            text : user.Name 
        }));
    });
}