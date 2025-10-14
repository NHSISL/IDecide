// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.PatientDecisions
{
    public partial class PatientDecisionControllerTests
    {
        [Fact]
        public void GetShouldHaveRoleAttributeWithRoles()
        {
            // given
            var controllerType = typeof(PatientDecisionController);
            var methodInfo = controllerType.GetMethod("GetPatientDecisionsAsync");
            Type attributeType = typeof(AuthorizeAttribute);
            string attributeProperty = "Roles";

            List<string> expectedAttributeValues = new List<string>
            {
                "LondonDataServices.IDecide.Manage.Server.Administrators",
                "LondonDataServices.IDecide.Manage.Server.Agents"
            };

            // when
            var methodAttribute = methodInfo?
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            var controllerAttribute = controllerType
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            var attribute = methodAttribute ?? controllerAttribute;

            // then
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
