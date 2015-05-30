using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class RouteDTO
    {
        public int UserId { set; get; }
        public List<LocationDTO> Locations { get; set; }

        public double Distance { set; get; }
        public double AverageSpeed { set; get; }

        public RouteDTO()
        {
            Locations = new List<LocationDTO>();
        }
    }
}
