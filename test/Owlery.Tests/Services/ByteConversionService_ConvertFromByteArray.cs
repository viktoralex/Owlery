using System.Text;
using Owlery.Services;
using Xunit;

namespace Owlery.Tests.Services
{
    public class ByteConversionService_ConvertFromByteArray
    {
        private readonly ByteConversionService byteConversionService;

        public ByteConversionService_ConvertFromByteArray()
        {
            this.byteConversionService = new ByteConversionService();
        }

        [Fact]
        public void ShouldNotChangeByteArray()
        {
            var input = Encoding.UTF8.GetBytes("This will be an array of bytes");

            var result = this.byteConversionService.ConvertFromByteArray(input, typeof(byte[]));

            Assert.Equal(input, result);
            Assert.Equal(input.GetType(), result.GetType());
        }

        [Fact]
        public void ShouldConvertTextFromUtf8ToByteArray()
        {
            var original = "This is a string";
            var input = Encoding.UTF8.GetBytes(original);

            var result = this.byteConversionService.ConvertFromByteArray(input, typeof(string));

            Assert.Equal(original, result);
            Assert.Equal(typeof(string), result.GetType());
        }

        [Fact]
        public void ShouldConvertObjectToJsonAndThenToByteArray()
        {
            var original = new Thing { Property = 99 };
            var input = Encoding.UTF8.GetBytes(@"{""Property"":99}");

            var result = this.byteConversionService.ConvertFromByteArray(input, typeof(Thing));

            Assert.Equal(original, result);
            Assert.Equal(typeof(Thing), result.GetType());
        }

        private class Thing {
            public int Property { get; set; }

            public override bool Equals(object obj)
            {
                if (obj.GetType() != typeof(Thing))
                {
                    return false;
                }

                return this.Property == (obj as Thing).Property;
            }

            public override int GetHashCode()
            {
                return this.Property;
            }
        }
    }
}