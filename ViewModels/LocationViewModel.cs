using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class LocationViewModel
    {
        public int UserId { set; get; }
        public double Lon { set; get; }
        public double Lat { set; get; }
    }
}