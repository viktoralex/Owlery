using System.Text;
using Newtonsoft.Json;
using Owlery.Utils;
using Xunit;

namespace Owlery.Tests.Utils
{
    public class BodyConverterTests_ConvertToByteArray
    {
        [Fact]
        public void ShouldNotChangeByteArray()
        {
            var input = Encoding.UTF8.GetBytes("This will be an array of bytes");

            var result = BodyConverter.ConvertToByteArray(input);

            Assert.Equal(input, result);
            Assert.Equal(input.GetType(), result.GetType());
        }

        [Fact]
        public void ShouldConvertTextFromUtf8ToByteArray()
        {
            var input = "This is a string";

            var result = BodyConverter.ConvertToByteArray(input);

            Assert.Equal(Encoding.UTF8.GetBytes(input), result);
            Assert.Equal(typeof(byte[]), result.GetType());
        }

        [Fact]
        public void ShouldConvertObjectToJsonAndThenToByteArray()
        {
            var input = new Thing { Property = 99 };

            var result = BodyConverter.ConvertToByteArray(input);

            Assert.Equal(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(input)), result);
            Assert.Equal(typeof(byte[]), result.GetType());
        }

        private class Thing {
            public int Property { get; set; }
        }
    }
}