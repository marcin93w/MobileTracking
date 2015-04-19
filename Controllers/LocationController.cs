using System;
using System.Collections.Generic;
using System.Data.Spatial;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;
using WebApplication1.Utils;

namespace WebApplication1.Controllers
{
    public class LocationController : ApiController
    {
        [HttpPost]
        public IHttpActionResult UpdateLocation([FromBody]LocationViewModel l)
        {
            using(var locationsEntity = new LocationEntities())
            {
                var route = locationsEntity.Routes.FirstOrDefault(r => r.UserId == l.UserId);
                if(route == null)
                {
                    route = locationsEntity.Routes.Add(new Route { UserId = l.UserId });
                }

                locationsEntity.Locations.Add(new Location()
                    {
                        Route = route,
                        Point = GeoUtils.CreatePoint(l.Lat, l.Lon, DbGeography.DefaultCoordinateSystemId),
                        Datetime = DateTime.Now
                    });

                locationsEntity.SaveChanges();
            }
            return Ok(l);
        }
    }
}
