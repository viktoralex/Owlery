using System;
using System.Text;
using System.Text.Json;

namespace Owlery.Services
{
    public class ByteConversionService : IByteConversionService
    {
        public object ConvertFromByteArray(byte[] arr, Type type)
        {
            if (arr == null)
                return arr;

            if (type == typeof(byte[]))
                return arr;

            else if (type == typeof(string))
                return Encoding.UTF8.GetString(arr);

            return JsonSerializer.Deserialize(arr, type);
        }

        public byte[] ConvertToByteArray(object returned)
        {
            if (returned == null)
                return null;

            if (returned.GetType() == typeof(byte[]))
                return (byte[])returned;

            if (returned.GetType() == typeof(string))
                return Encoding.UTF8.GetBytes((string)returned);

            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(returned));
        }
    }
}