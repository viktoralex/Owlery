using System.Text;
using Newtonsoft.Json;
using Owlery.Utils;
using Xunit;

namespace Owlery.Tests.Utils
{
    public class BodyConverter_ConvertFromByteArray
    {
        [Fact]
        public void ShouldNotChangeByteArray()
        {
            var input = Encoding.UTF8.GetBytes("This will be an array of bytes");

            var result = BodyConverter.ConvertFromByteArray(input, typeof(byte[]));

            Assert.Equal(input, result);
            Assert.Equal(input.GetType(), result.GetType());
        }

        [Fact]
        public void ShouldConvertTextFromUtf8ToByteArray()
        {
            var original = "This is a string";
            var input = Encoding.UTF8.GetBytes(original);

            var result = BodyConverter.ConvertFromByteArray(input, typeof(string));

            Assert.Equal(original, result);
            Assert.Equal(typeof(string), result.GetType());
        }

        [Fact]
        public void ShouldConvertObjectToJsonAndThenToByteArray()
        {
            var original = new Thing { Property = 99 };
            var input = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(original));

            var result = BodyConverter.ConvertFromByteArray(input, typeof(Thing));

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