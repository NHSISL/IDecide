// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Attrify.Attributes;
using FluentAssertions;
using LondonDataServices.IDecide.Portal.Server.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.Patients
{
    public partial class PatientsControllerTests
    {
        [Fact]
        public void DeleteShouldHaveInvisibleApiAttribute()
        {
            // Given
            var controllerType = typeof(PatientsController);
            var methodInfo = controllerType.GetMethod("DeletePatientByIdAsync");
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
            attribute.Should().NotBeNull();
        }
    }
}
