using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class LocationDataController : ApiController
    {
        [HttpGet]
        public UserLocationsDTO GetLocations([FromUri] LocationsSettings settings)
        {
            using (var locationsEntity = new LocationEntities())
            {
                var locations = 
                       from location in locationsEntity.Locations
                       where location.Route.UserId == settings.UserId
                        && (settings.Start == null || location.Datetime >= settings.Start)
                        && (settings.End == null || location.Datetime <= settings.End)
                       orderby location.Datetime
                       select new LocationDTO()
                       {
                           UserId = settings.UserId,
                           Time = location.Datetime,
                           Lon = location.Point.XCoordinate.Value,
                           Lat = location.Point.YCoordinate.Value
                       };

                return new UserLocationsDTO()
                {
                    UserId = settings.UserId,
                    Locations = locations.ToList()
                };
            }
        }
    }
}
