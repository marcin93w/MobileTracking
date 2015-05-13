using System.Device.Location;
using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetTopologySuite.Geometries;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class UserLocationsDTO
    {
        private List<LocationDTO> _locations;

        public List<LocationDTO> Locations
        {
            set
            {
                _locations = value;
                UpdateLocationsData();
            }
            get { return _locations; }
        }

        public int UserId { set; get; }

        public Envelope Boundary
        {
            get
            {
                var envelope = new Envelope();
                foreach(var location in Locations)
                {
                    envelope.ExpandToInclude(location.Lon, location.Lat);
                }

                return envelope;
            }
        }

        public double MaxSpeed { private set; get; }

        private void UpdateLocationsData()
        {
            MaxSpeed = Locations.Zip(Locations.Skip(1), CalculateMaxSpeedBetweenLocations).Max();
        }

        private double CalculateMaxSpeedBetweenLocations(LocationDTO location1, LocationDTO location2)
        {
            var coord1 = new GeoCoordinate(location1.Lat, location1.Lon);
            var coord2 = new GeoCoordinate(location2.Lat, location2.Lon);

            var maxSpeed = (coord1.GetDistanceTo(coord2) / (location2.Time - location1.Time).Milliseconds) * 36000;
            return Math.Round(maxSpeed, 2);
        }
    }
}