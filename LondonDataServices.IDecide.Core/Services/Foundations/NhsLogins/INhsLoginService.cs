// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsLogins;

namespace LondonDataServices.IDecide.Core.Services.Foundations.NhsLogins
{
    public interface INhsLoginService
    {
        ValueTask<NhsLoginUserInfo> NhsLoginAsync();
    }
}