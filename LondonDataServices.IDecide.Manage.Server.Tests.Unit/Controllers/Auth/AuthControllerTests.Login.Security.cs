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
        public void LoginShouldNotHaveAuthorizeAttribute()
        {
            // given
            var controllerType = typeof(AuthController);
            var methodInfo = controllerType.GetMethod("Login");
            Type attributeType = typeof(AuthorizeAttribute);

            // when
            var attribute = methodInfo?
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            // then
            attribute.Should().BeNull();
        }

        [Fact]
        public void LoginShouldNotHaveInvisibleApiAttribute()
        {
            // given
            var controllerType = typeof(AuthController);
            var methodInfo = controllerType.GetMethod("Login");
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
