// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins.Exceptions;
using LondonDataServices.IDecide.Core.Services.Foundations.NhsLogins;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Services.Foundations.NhsLogins
{
    public partial class NhsLoginServiceTests
    {
        private readonly Mock<ISecurityBroker> securityBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly NhsLoginService nhsLoginService;

        public NhsLoginServiceTests()
        {
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();


            this.nhsLoginService = new NhsLoginService(
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static NhsLoginServiceDependencyException GetNhsLoginServiceDependencyException()
        {
            string randomMessage = GetRandomString();
            var innerException = new Xeption(randomMessage);

            return new NhsLoginServiceDependencyException(
                message: "NHS Login dependency error occurred, please contact support.",
                innerException: innerException);
        }

        private static NhsLoginServiceException GetNhsLoginServiceException()
        {
            string randomMessage = GetRandomString();
            var innerException = new Xeption(randomMessage);

            return new NhsLoginServiceException(
                message: "NHS Login service error occurred, please contact support.",
                innerException: innerException);
        }

        public static TheoryData<Exception> DependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Exception(exceptionMessage);

            return new TheoryData<Exception>
            {
                new HttpRequestException(
                    message: randomMessage,
                    inner: innerException,
                    statusCode: HttpStatusCode.BadRequest),

                new OperationCanceledException(
                    message: randomMessage,
                    innerException),
            };
        }

        public static TheoryData<Exception> DependencyExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Exception(exceptionMessage);

            return new TheoryData<Exception>
            {
                 new HttpRequestException(
                    message: randomMessage,
                    inner: null,
                    statusCode: HttpStatusCode.InternalServerError),
            };
        }


        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();


        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();
    }
}