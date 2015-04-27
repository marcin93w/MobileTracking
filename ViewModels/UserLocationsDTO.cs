using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class UserLocationsDTO
    {
        public int UserId { set; get; }
        public List<LocationDTO> Locations { set; get; }
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
    }
}