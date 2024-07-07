using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Badminton.Business.Helpers
{
    public static class Helpers
    {

        public static object GetValueFromSession(string key, HttpContext context)
        {
            context.Session.TryGetValue(key, out byte[] o);
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(o));
        }

        public static void SetValueToSession(string key, object value, HttpContext context)
        {
            context.Session.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        }

    }
}
