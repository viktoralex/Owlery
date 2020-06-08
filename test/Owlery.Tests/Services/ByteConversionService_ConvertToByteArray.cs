using System.Text;
using Owlery.Services;
using Xunit;

namespace Owlery.Tests.Services
{
    public class ByteConversionService_ConvertToByteArray
    {
        private readonly ByteConversionService byteConversionService;

        public ByteConversionService_ConvertToByteArray()
        {
            this.byteConversionService = new ByteConversionService();
        }

        [Fact]
        public void ShouldNotChangeByteArray()
        {
            var input = Encoding.UTF8.GetBytes("This will be an array of bytes");

            var result = this.byteConversionService.ConvertToByteArray(input);

            Assert.Equal(input, result);
            Assert.Equal(input.GetType(), result.GetType());
        }

        [Fact]
        public void ShouldConvertTextFromUtf8ToByteArray()
        {
            var input = "This is a string";

            var result = this.byteConversionService.ConvertToByteArray(input);

            Assert.Equal(Encoding.UTF8.GetBytes(input), result);
            Assert.Equal(typeof(byte[]), result.GetType());
        }

        [Fact]
        public void ShouldConvertObjectToJsonAndThenToByteArray()
        {
            var input = new Thing { Property = 99 };

            var result = this.byteConversionService.ConvertToByteArray(input);

            Assert.Equal(@"{""Property"":99}", Encoding.UTF8.GetString(result));
            Assert.Equal(typeof(byte[]), result.GetType());
        }

        private class Thing {
            public int Property { get; set; }
        }
    }
}