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
    public class LocationsDTO
    {
        public List<RouteDTO> Routes { get; set; }

        public Envelope Boundary { set; get; }

        public double TotalDistance { set; get; }

        public double AverageSpeed { set; get; }

        public int LocationsCount { set; get; }
    }
}