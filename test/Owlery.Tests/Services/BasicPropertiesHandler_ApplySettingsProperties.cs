using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using Owlery.Models;
using Owlery.Models.Settings;
using Owlery.Services;
using RabbitMQ.Client;
using Xunit;

namespace Owlery.Tests.Services
{
    public class BasicPropertiesHandler_ApplySettingsProperties
    {
        [Fact]
        public void ShouldApplyNonNullSettings()
        {
            // GIVEN - a message to be applied to message properties
            var appId = "AnAppId";

            Mock<IBasicProperties> mockBasicProperties = new Mock<IBasicProperties>();

            var settings = Options.Create<OwlerySettings>(new OwlerySettings {
                AppId = appId,
            });

            var handler = new BasicPropertiesHandler(settings);

            // WHEN - ApplySettingsProperties is run with the mock basic properties
            handler.ApplySettingsProperties(mockBasicProperties.Object);

            // THEN - Each properties of the rabbit message should be applied
            mockBasicProperties.VerifySet(p => p.AppId = appId);
            mockBasicProperties.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldNotApplyNullProperties()
        {
            // GIVEN - a message to be applied to message properties
            Mock<IBasicProperties> mockBasicProperties = new Mock<IBasicProperties>();

            var settings = Options.Create<OwlerySettings>(new OwlerySettings {
                AppId = null,
            });

            var handler = new BasicPropertiesHandler(settings);

            // WHEN - ApplySettingsProperties is run with the mock basic properties
            handler.ApplySettingsProperties(mockBasicProperties.Object);

            // THEN - No properties of the rabbit message should be applied
            mockBasicProperties.VerifyNoOtherCalls();
        }
    }
}