using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace Badminton.Common
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

        public static void CopyValues<T>(this T target, T source)
        {
            if (target == null || source == null)
            {
                throw new ArgumentNullException("Target or Source cannot be null");
            }

            foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanWrite)
                {
                    var value = property.GetValue(source);
                    property.SetValue(target, value);
                }
            }
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

    }
}
