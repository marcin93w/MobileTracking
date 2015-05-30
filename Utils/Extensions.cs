using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Utils
{
    public static class Extensions
    {
        public static string ToSqlString(this List<int> list)
        {
            var str = new StringBuilder();
            foreach (var item in list)
            {
                str.Append(item + ",");
            }
            str.Remove(str.Length - 1, 1);
            return "(" + str.ToString() + ")";
        }
    }
}
