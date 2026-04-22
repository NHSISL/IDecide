// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Attrify.Attributes;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Controllers.ConsumerStatuses;
using Microsoft.AspNetCore.Authorization;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.ConsumerStatuses
{
    public partial class ConsumerStatusControllerTests
    {
        [Fact]
        public void PostShouldNotHaveInvisibleApiAttribute()
        {
            // Given
            var controllerType = typeof(ConsumerStatusController);
            var methodInfo = controllerType.GetMethod("AdoptPatientDecisionsAsync");
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
