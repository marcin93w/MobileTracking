using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class LocationDTO
    {
        public int UserId { set; get; }
        public DateTime Time { set; get; }
        public double Lon { set; get; }
        public double Lat { set; get; }
    }
}