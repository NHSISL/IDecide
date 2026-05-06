// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using Attrify.Attributes;
using FluentAssertions;
using LondonDataServices.IDecide.Manage.Server.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Auth
{
    public partial class AuthControllerTests
    {
        [Fact]
        public void LogoutShouldHaveAuthorizeAttribute()
        {
            // given
            var controllerType = typeof(AuthController);
            var methodInfo = controllerType.GetMethod("Logout");
            Type attributeType = typeof(AuthorizeAttribute);

            // when
            var attribute = methodInfo?
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            // then
            attribute.Should().NotBeNull();
        }

        [Fact]
        public void LogoutShouldNotHaveInvisibleApiAttribute()
        {
            // given
            var controllerType = typeof(AuthController);
            var methodInfo = controllerType.GetMethod("Logout");
            Type attributeType = typeof(InvisibleApiAttribute);

            // when
            var attribute = methodInfo?
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            // then
            attribute.Should().BeNull();
        }
    }
}
