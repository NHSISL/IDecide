﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xunit.Sdk;

namespace LondonDataServices.IDecide.Manage.Server.Tests.Integration
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer(
        typeName: "LondonDataServices.IDecide.Portals.Server.Tests.Integration.ReIdentification.ReleaseCandidateTestCaseDiscoverer",
        assemblyName: "LondonDataServices.IDecide.Portals.Server.Tests.Integration.ReIdentification")]
    public class ReleaseCandidateFactAttribute : FactAttribute { }
}
