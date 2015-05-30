using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Device.Location;
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
using NpgsqlTypes;
using WebApplication1.Models;
using WebApplication1.Utils;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class LocationDataController : ApiController
    {
        private const int MaxSecondsBetweenPoints = 120;

        private const string GetLocationsQuery = 
          @"SELECT ""Routes"".""UserId"", ""Datetime"",  ST_X(""Point"") as ""Lon"", ST_Y(""Point"") as ""Lat""
            FROM ""Locations""
            JOIN ""Routes"" ON (""Locations"".""RouteId"" = ""Routes"".""Id"")
            WHERE ""Routes"".""UserId"" IN @p0
                AND (@p1 IS NULL OR ""Locations"".""Datetime"" >= @p1)
                AND (@p2 IS NULL OR ""Locations"".""Datetime"" <= @p2)
            ORDER BY ""Locations"".""Datetime""";

        [HttpPost]
        public LocationsDTO GetLocations([FromBody] LocationsSettings settings)
        {
            using (var locationsEntity = new postgresEntities1())
            {
                locationsEntity.Database.Connection.Open();
                var cmd = locationsEntity.Database.Connection.CreateCommand();
                var query = GetLocationsQuery.Replace("@p0", settings.UserIds.ToSqlString());
                cmd.CommandText = query;
                cmd.Parameters.Add(new NpgsqlParameter("p1", settings.Start));
                cmd.Parameters.Add(new NpgsqlParameter("p2", settings.End));
                DbDataReader r = cmd.ExecuteReader();
                var locations = (from IDataRecord row in r
                                select new LocationDTO
                                {
                                    UserId =Int32.Parse(row["UserId"].ToString()),
                                    Time = DateTime.Parse(row["Datetime"].ToString()),
                                    Lon = Double.Parse(row["Lon"].ToString()),
                                    Lat = Double.Parse(row["Lat"].ToString())
                                }).ToList();

                var data = new LocationsDTO
                {
                    Routes = GenerateRoutesFromLocations(locations),
                    Boundary = CalculateBoundary(locations),
                    LocationsCount = locations.Count
                };

                var totalDistance = CalculateTotalLength(data.Routes);
                data.TotalDistance = Math.Round(totalDistance / 1000, 2);
                data.AverageSpeed = Math.Round(CalculateAverageSpeed(totalDistance, data.Routes), 2);

                return data;
            }
        }

        private List<RouteDTO> GenerateRoutesFromLocations(List<LocationDTO> locations)
        {
            if(locations.Count == 0)
                return null;
            
            var routes = new List<RouteDTO>();
            var route = new RouteDTO();
            route.UserId = locations.First().UserId;
            route.Locations.Add(locations.First());

            foreach (var location in locations.Skip(1))
            {
                if ((location.Time - route.Locations.Last().Time).TotalSeconds > MaxSecondsBetweenPoints)
                {
                    routes.Add(route);
                    route = new RouteDTO();
                    route.UserId = location.UserId;
                }
                route.Locations.Add(location);
            }
            routes.Add(route);

            return routes;
        }

        private Envelope CalculateBoundary(IEnumerable<LocationDTO> locations)
        {
            var envelope = new Envelope();
            foreach (var location in locations)
            {
                envelope.ExpandToInclude(location.Lon, location.Lat);
            }

            return envelope;
        }

        private double CalculateTotalLength(List<RouteDTO> routes)
        {
            foreach (var route in routes)
            {
                if (route.Locations.Count < 2)
                {
                    route.Distance = 0;
                }
                else
                {
                    route.Distance = route.Locations.Zip(route.Locations.Skip(1), CalculateDistanceBetweenLocations).Sum();
                }
            }

            return routes.Sum(r => r.Distance);
        }

        private double CalculateAverageSpeed(double totalDistance, List<RouteDTO> routes)
        {
            foreach (var route in routes)
            {
                if (route.Locations.Count < 2)
                {
                    route.AverageSpeed = 0;
                }
                else
                {
                    route.AverageSpeed = (route.Distance / (route.Locations.Last().Time - route.Locations.First().Time).TotalSeconds) * 3.6;
                }
            }

            var totalTravelTime = routes
                .Where(r => r.Locations.Count > 1)
                .Sum(route => (route.Locations.Last().Time - route.Locations.First().Time).TotalSeconds);

            return (totalDistance/totalTravelTime)*3.6;
        }

        private double CalculateDistanceBetweenLocations(LocationDTO location1, LocationDTO location2)
        {
            if ((location2.Time - location1.Time).TotalSeconds <= 0)
                return 0;

            var coord1 = new GeoCoordinate(location1.Lat, location1.Lon);
            var coord2 = new GeoCoordinate(location2.Lat, location2.Lon);

            return coord1.GetDistanceTo(coord2);
        }

        //private double CalculateSpeedBetweenLocations(LocationDTO location1, LocationDTO location2)
        //{
        //    if ((location2.Time - location1.Time).TotalSeconds <= 0)
        //        return 0;

        //    var coord1 = new GeoCoordinate(location1.Lat, location1.Lon);
        //    var coord2 = new GeoCoordinate(location2.Lat, location2.Lon);

        //    var maxSpeed = ((coord1.GetDistanceTo(coord2) / (location2.Time - location1.Time).TotalSeconds)) * 3.6;
        //    return Math.Round(maxSpeed, 2);
        //}
    }
}
