// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------



using System;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Portal.Tests.Integration.Models.UserAccesses;

namespace LondonDataServices.IDecide.Portals.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string userAccessesRelativeUrl = "api/useraccesses";

        public async ValueTask<UserAccess> PostUserAccessAsync(UserAccess userAccess) =>
            await this.apiFactoryClient.PostContentAsync(userAccessesRelativeUrl, userAccess);

        public async ValueTask<UserAccess> DeleteUserAccessByIdAsync(Guid userAccessId) =>
            await this.apiFactoryClient.DeleteContentAsync<UserAccess>($"{userAccessesRelativeUrl}/{userAccessId}");
    }
}
