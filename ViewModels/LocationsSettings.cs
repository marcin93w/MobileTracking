using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.ViewModels
{
    public class LocationsSettings
    {
        public List<int> UserIds { set; get; }
        public DateTime Start { set; get; }
        public DateTime End { set; get; }
    }
}
