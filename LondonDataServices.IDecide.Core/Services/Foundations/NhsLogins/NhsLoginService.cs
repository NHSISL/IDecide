// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins;

namespace LondonDataServices.IDecide.Core.Services.Foundations.NhsLogins
{
    public partial class NhsLoginService : INhsLoginService
    {
        private readonly ISecurityAuditBroker securityAuditBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;

        public NhsLoginService(
            ISecurityBroker securityBroker,
            ILoggingBroker loggingBroker)
        {
            this.securityBroker = securityBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<NhsLoginUserInfo> NhsLoginAsync() =>
            TryCatch(async () =>
            {
                var accessToken =
                await this.securityBroker.GetNhsLoginAccessTokenAsync();

                ValidateAccessToken(accessToken);

                NhsLoginUserInfo userInfo =
                    await this.securityBroker.GetNhsLoginUserInfoAsync(accessToken);

                ValidateSuccessStatusCode(userInfo);

                return userInfo;
            });
    }
}
