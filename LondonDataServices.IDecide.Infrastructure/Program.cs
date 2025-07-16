// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Infrastructure.Services;

namespace LondonDataServices.IDecide.Infrastructure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var scriptGenerationService = new ScriptGenerationService();

            scriptGenerationService.GenerateBuildScript(
                branchName: "main",
                projectName: "LondonDataServices.IDecide.Core",
                dotNetVersion: "9.0.100");

            scriptGenerationService.GeneratePrLintScript(branchName: "main");
        }
    }
}
