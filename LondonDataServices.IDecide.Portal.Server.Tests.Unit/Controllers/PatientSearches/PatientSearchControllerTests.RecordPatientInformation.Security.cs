// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using Attrify.Attributes;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Controllers;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.PatientSearches
{
    public partial class PatientSearchControllerTests
    {
        [Fact]
        public void RecordPatientInformationShouldNotHaveInvisibleApiAttribute()
        {
            // Given
            var controllerType = typeof(PatientSearchController);
            var methodInfo = controllerType.GetMethod("RecordPatientInformation");
            Type attributeType = typeof(InvisibleApiAttribute);

            // When
            var methodAttribute = methodInfo?
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            var controllerAttribute = controllerType
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            var attribute = methodAttribute ?? controllerAttribute;

            // Then
            attribute.Should().BeNull();
        }
    }
}
