var apiUrl = "http://ec2-54-187-122-224.us-west-2.compute.amazonaws.com/api";
var map = null;
var mapMarkers = [];
var users = [];

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
	    var users = $('#users').val();
	    if (users != null && users.length > 0) {
	        getLocationData(users, $('#start-date').val(), $('#end-date').val());
	    } else {
	        alert('Musisz wybrać przynajmniej jednego użytkownika');
	    }
	});
}

function getLocationData(userIds, startTime, endTime) {
    var post = {
        "UserIds": userIds,
        "Start": startTime,
        "End": endTime
    };
    $.ajax({
        type: "POST",
        url: apiUrl + "/LocationData/GetLocations",
        data: JSON.stringify(post),
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    })
	.done(function(data) {
	    populateMap(data, userIds);
	})
	.fail(function() {
	    alert( "Błąd podczas pobierania danych z serwera" );
	});
}

function populateMap(data, userIds) {
    mapMarkers.map(function(marker) {
        marker.setMap(null);
    });
    mapMarkers = [];

    map.fitBounds(new google.maps.LatLngBounds(
        new google.maps.LatLng(data.Boundary._miny, data.Boundary._minx),
        new google.maps.LatLng(data.Boundary._maxy, data.Boundary._maxx)));

    var colors = [];

    if (userIds.length == 1) {
        colors = randomColors(data.Routes.length);
    } else {
        var userColors = randomColors(userIds.length);
        for (var j=0; j<data.Routes.length; j++) {
            var userIdx = userIds.indexOf(data.Routes[j].UserId + '');
            colors.push(userColors[userIdx]);
        }
        populateLegend(userIds, userColors);
    }

    for (var i in data.Routes) {
        var route = data.Routes[i];
        if (route.Locations.length > 0) {
            var points = [route.Locations[0], route.Locations[route.Locations.length - 1]];
            if ($('#markers').is(':checked')) {
                points.forEach(function(point) {
                    mapMarkers.push(new google.maps.Marker({
                        position: new google.maps.LatLng(point.Lat, point.Lon),
                        map: map,
                        title: point.Time
                    }));
                });
            }

            $('#start-time').text(formatDateTime(points[0].Time));
            $('#end-time').text(formatDateTime(points[1].Time));
        }

        mapMarkers.push(new google.maps.Polyline({
            path: route.Locations.map(function (point) {
                return new google.maps.LatLng(point.Lat, point.Lon);
            }),
            geodesic: true,
            map: map,
            strokeColor: colors[i],
            strokeOpacity: 0.7,
            strokeWeight: 5
        }));
    }

    $('#points-count').text(data.LocationsCount);
    $('#routes-count').text(data.Routes.length);
    $('#total-distance').text(data.TotalDistance);
    $('#average-speed').text(data.AverageSpeed);
}

function formatDateTime(dateTime) {
    return dateTime.replace('T', ' ').split('.')[0];
}

function populateLegend(userIds, usersColors) {
    $('#users-legend').empty();
    for (var i = 0; i < userIds.length; i++) {
        var user = jQuery.grep(users, function(u, j) { return u.Id == userIds[i]; })[0];
        $('#users-legend').append('<p style="color: ' + usersColors[i] + '">' + user.Name + '</p>');
    }
}

function getUsersData() {
    var result = $.ajax(apiUrl + "/Users/GetAll")
	  .done(function (data) {
            users = data;
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
