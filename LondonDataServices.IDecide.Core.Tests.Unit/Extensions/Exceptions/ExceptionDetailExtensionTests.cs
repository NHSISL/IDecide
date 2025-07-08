// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Tynamix.ObjectFiller;
using Xunit.Abstractions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Extensions
{
    public partial class ExceptionDetailExtensionTests
    {
        private readonly ITestOutputHelper output;

        public ExceptionDetailExtensionTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();
    }
}
