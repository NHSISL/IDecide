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

namespace LondonDataServices.IDecide.Portal.Server.Tests.Unit.Controllers.DecisionTypes
{
    public partial class DecisionTypesControllerTests
    {
        [Fact]
        public void GetShouldHaveInvisibleApiAttribute()
        {
            // Given
            var controllerType = typeof(DecisionTypesController);
            var methodInfo = controllerType.GetMethod("GetDecisionTypeByIdAsync");
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
