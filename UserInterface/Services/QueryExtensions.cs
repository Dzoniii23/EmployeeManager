using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface.Services
{
    public static class QueryExtensions
    {
        public static string ToQueryString(this object source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var properties = from property in source.GetType().GetProperties()
                             where property.GetValue(source, null) != null
                             select property.Name + "=" + property.GetValue(source, null);

            return "?" + string.Join("&", properties.ToArray());
        }
    }
}
