using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.EnterpriseServices;
using System.Net.Sockets;
using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Mono.Security.Cryptography;
using Newtonsoft.Json;
using Npgsql;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class LocationDataController : ApiController
    {
        private const string GetLocationsQuery = 
          @"SELECT ""Routes"".""UserId"", ""Datetime"",  ST_X(""Point"") as ""Lon"", ST_Y(""Point"") as ""Lat""
            FROM ""Locations""
            JOIN ""Routes"" ON (""Locations"".""RouteId"" = ""Routes"".""Id"")
            WHERE ""Routes"".""UserId"" = @p0
                AND (@p1 IS NULL OR ""Locations"".""Datetime"" >= @p1)
                AND (@p2 IS NULL OR ""Locations"".""Datetime"" <= @p2)
            ORDER BY ""Locations"".""Datetime""";

        [HttpGet]
        public UserLocationsDTO GetLocations([FromUri] LocationsSettings settings)
        {

            using (var locationsEntity = new postgresEntities1())
            {
                locationsEntity.Database.Connection.Open();
                var cmd = locationsEntity.Database.Connection.CreateCommand();
                cmd.CommandText = GetLocationsQuery;
                cmd.Parameters.Add(new NpgsqlParameter("p0", settings.UserId));
                cmd.Parameters.Add(new NpgsqlParameter("p1", settings.Start));
                cmd.Parameters.Add(new NpgsqlParameter("p2", settings.End));
                DbDataReader r = cmd.ExecuteReader();
                var locations = from IDataRecord row in r
                                select new LocationDTO
                                {
                                    UserId =Int32.Parse(row["UserId"].ToString()),
                                    Time = DateTime.Parse(row["Datetime"].ToString()),
                                    Lon = Double.Parse(row["Lon"].ToString()),
                                    Lat = Double.Parse(row["Lat"].ToString())
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
