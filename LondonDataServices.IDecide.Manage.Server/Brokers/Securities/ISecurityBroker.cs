// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Manage.Server.Models.Securities;

namespace LondonDataServices.IDecide.Manage.Server.Brokers.Securities
{
    public interface ISecurityBroker
    {
        ValueTask<User> GetCurrentUserAsync();
    }
}
