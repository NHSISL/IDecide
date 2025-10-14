// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Controllers.ConsumerStatuses;
using Microsoft.AspNetCore.Authorization;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.ConsumerStatuses
{
    public partial class ConsumerStatusControllerTests
    {
        [Fact]
        public void PostShouldHaveRoleAttributeWithRoles()
        {
            // Given
            var controllerType = typeof(ConsumerStatusController);
            var methodInfo = controllerType.GetMethod("AdoptPatientDecisionsAsync");
            Type attributeType = typeof(AuthorizeAttribute);
            string attributeProperty = "Roles";

            List<string> expectedAttributeValues = new List<string>
            {
                "LondonDataServices.IDecide.Manage.Server.Administrators",
                "LondonDataServices.IDecide.Manage.Server.Agents"
            };

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

            var actualAttributeValue = attributeType
                .GetProperty(attributeProperty)?
                .GetValue(attribute) as string ?? string.Empty;

            var actualAttributeValues = actualAttributeValue?
                .Split(',')
                .Select(role => role.Trim())
                .Where(role => !string.IsNullOrEmpty(role))
                .ToList();

            actualAttributeValues.Should().BeEquivalentTo(expectedAttributeValues);
        }

    }
}
