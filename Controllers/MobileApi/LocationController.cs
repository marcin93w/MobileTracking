using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Npgsql;
using WebApplication1.Models;
using WebApplication1.Utils;

namespace WebApplication1.Controllers
{
    public class LocationController : ApiController
    {
        private string AddLocationQuery = @"INSERT INTO public.""Locations""(""RouteId"", ""Datetime"", ""Point"")
                VALUES (@routeId, @date, ST_GeomFromText('POINT(' || @lon || ' ' || @lat || ')'))";

        [HttpPost]
        public IHttpActionResult UpdateLocation([FromBody]LocationDTO l)
        {
            using (var locationsEntity = new postgresEntities1())
            {
                var route = locationsEntity.Routes.FirstOrDefault(r => r.UserId == l.UserId);
                if (route == null)
                {
                    route = locationsEntity.Routes.Add(new Route { UserId = l.UserId });
                }

                locationsEntity.SaveChanges();

                locationsEntity.Database.ExecuteSqlCommand(AddLocationQuery,
                    new NpgsqlParameter("routeId", route.Id),
                    new NpgsqlParameter("date", DateTime.Now),
                    new NpgsqlParameter("lon", l.Lon),
                    new NpgsqlParameter("lat", l.Lat));
            }
            return Ok(l);
        }
    }
}
