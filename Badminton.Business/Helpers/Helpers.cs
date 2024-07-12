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

        public static bool GetValueFromSession<T>(string key, out T value, HttpContext context)
        {
            value = JsonConvert.DeserializeObject<T>("");
            context.Session.TryGetValue(key, out byte[] o);
            if (o == null)
            {
                return false;
            }
            value = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(o));
            return true;
        }

        public static void SetValueToSession(string key, object value, HttpContext context)
        {
            context.Session.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        }

    }
}
