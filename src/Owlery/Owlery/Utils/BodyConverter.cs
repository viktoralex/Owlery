using System;
using System.Text;
using Newtonsoft.Json;

namespace Owlery.Utils
{
    public static class BodyConverter
    {
        public static byte[] ConvertToByteArray(object returned)
        {
            if (returned.GetType() == typeof(byte[]))
            {
                return (byte[])returned;
            }
            else if (returned.GetType() == typeof(string))
            {
                return Encoding.UTF8.GetBytes((string)returned);
            }

            // TODO: other converters
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(returned));
        }

        public static object ConvertFromByteArray(byte[] arr, Type type)
        {
            if (type == typeof(byte[]))
            {
                return arr;
            }
            else if (type == typeof(string))
            {
                return Encoding.UTF8.GetString(arr);
            }

            // TODO: other converters
            return JsonConvert.DeserializeObject(
                Encoding.UTF8.GetString(arr),
                type);
        }
    }
}