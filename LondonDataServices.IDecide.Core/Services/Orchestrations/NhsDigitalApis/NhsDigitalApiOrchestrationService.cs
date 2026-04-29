// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Services.Foundations.NhsDigitalApis;
using LondonDataServices.IDecide.Core.Services.Foundations.Users;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationService : INhsDigitalApiOrchestrationService
    {
        private readonly INhsDigitalApiService nhsDigitalApiService;
        private readonly IUserService userService;
        private readonly ILoggingBroker loggingBroker;

        public NhsDigitalApiOrchestrationService(
            INhsDigitalApiService nhsDigitalApiService,
            IUserService userService,
            ILoggingBroker loggingBroker)
        {
            this.nhsDigitalApiService = nhsDigitalApiService;
            this.userService = userService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask ProcessCallbackAsync(string code, string state, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}
