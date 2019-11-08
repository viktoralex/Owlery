using System;

namespace Owlery.Services
{
    public interface IByteConversionService
    {
         byte[] ConvertToByteArray(object returned);
         object ConvertFromByteArray(byte[] arr, Type type);
    }
}