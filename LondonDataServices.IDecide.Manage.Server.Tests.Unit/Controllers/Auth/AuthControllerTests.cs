// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions;
using LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis;
using LondonDataServices.IDecide.Manage.Server.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Unit.Controllers.Auth
{
    public partial class AuthControllerTests : RESTFulController
    {
        private readonly Mock<INhsDigitalApiOrchestrationService> nhsDigitalApiOrchestrationServiceMock;
        private readonly Mock<ILogger<AuthController>> loggerMock;
        private readonly AuthController authController;

        public AuthControllerTests()
        {
            this.nhsDigitalApiOrchestrationServiceMock =
                new Mock<INhsDigitalApiOrchestrationService>();

            this.loggerMock = new Mock<ILogger<AuthController>>();

            this.authController = new AuthController(
                this.nhsDigitalApiOrchestrationServiceMock.Object,
                this.loggerMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new NhsDigitalApiOrchestrationValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new NhsDigitalApiOrchestrationDependencyValidationException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        public static TheoryData<Xeption> ServerExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new NhsDigitalApiOrchestrationDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new NhsDigitalApiOrchestrationServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();
    }
}
