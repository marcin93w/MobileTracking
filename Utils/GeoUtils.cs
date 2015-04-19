using System;
using System.Collections.Generic;
using System.Data.Spatial;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WebApplication1.Utils
{
    public class GeoUtils
    {
        public static DbGeography CreatePoint(double lat, double lon, int srid)
        {
            string wkt = String.Format(CultureInfo.InvariantCulture, "POINT({0} {1})", lon, lat);

            return DbGeography.PointFromText(wkt, srid);
        }
    }
}