// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using LondonDataServices.IDecide.Core.Extensions.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Tests.Unit.Extensions
{
    public partial class ExceptionDetailExtensionTests
    {
        [Fact]
        public void ShouldReturnEmptyValidationSummaryIfNoDataItemsFoundOnExceptionOrInnerException()
        {
            // given
            string randomMessage = GetRandomString();
            Exception randomException = new Exception(message: randomMessage);
            string expectedResult = string.Empty;

            // when
            string actualResult = randomException.GetValidationSummary();

            // then
            output.WriteLine($"Actual:");
            output.WriteLine(actualResult);
            output.WriteLine("");
            output.WriteLine($"Expected:");
            output.WriteLine(expectedResult);

            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void ShouldReturnValidationSummaryIfExceptionHasDataItems()
        {
            // given
            string randomMessage = GetRandomString();
            Xeption randomXeption = new Xeption(message: randomMessage);
            randomXeption.Data.Add("Key1", new List<string> { "Value1" });
            randomXeption.Data.Add("Key2", new List<string> { "Value2", "Value3" });

            string expectedResult = $"{randomXeption.GetType().Name} Errors:  " +
                $"Key1 => Value1;  Key2 => Value2, Value3;  " + Environment.NewLine;

            // when
            string actualResult = randomXeption.GetValidationSummary();

            // then
            output.WriteLine($"Actual:");
            output.WriteLine(actualResult);
            output.WriteLine("");
            output.WriteLine($"Expected:");
            output.WriteLine(expectedResult);

            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void ShouldReturnValidationSummaryIfInnerExceptionHasDataItems()
        {
            // given
            string randomMessage = GetRandomString();
            string randomInnerExceptionMessage = GetRandomString();
            Xeption randomInnerXeption = new Xeption(message: randomInnerExceptionMessage);
            randomInnerXeption.Data.Add("Key1", new List<string> { "Value1" });
            randomInnerXeption.Data.Add("Key2", new List<string> { "Value2", "Value3" });
            Xeption randomXeption = new Xeption(randomMessage, randomInnerXeption);

            string expectedResult = $"{randomInnerXeption.GetType().Name} Errors:  " +
                $"Key1 => Value1;  Key2 => Value2, Value3;  " + Environment.NewLine;

            // when
            string actualResult = randomXeption.GetValidationSummary();

            // then
            output.WriteLine($"Actual:");
            output.WriteLine(actualResult);
            output.WriteLine("");
            output.WriteLine($"Expected:");
            output.WriteLine(expectedResult);

            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void ShouldReturnValidationSummaryIfExceptionAndInnerExceptionHasDataItems()
        {
            // given
            string randomMessage = GetRandomString();
            string randomInnerExceptionMessage = GetRandomString();
            Xeption randomInnerXeption =
                new Xeption(message: randomInnerExceptionMessage);

            randomInnerXeption.Data.Add("Key3", new List<string> { "Value4" });
            randomInnerXeption.Data.Add("Key4", new List<string> { "Value5", "Value6" });

            Xeption randomXeption =
                new Xeption(randomMessage, randomInnerXeption);

            randomXeption.Data.Add("Key1", new List<string> { "Value1" });
            randomXeption.Data.Add("Key2", new List<string> { "Value2", "Value3" });

            string expectedResult =
                $"{randomXeption.GetType().Name} Errors:  " +
                $"Key1 => Value1;  Key2 => Value2, Value3;  " + Environment.NewLine +
                $"{randomInnerXeption.GetType().Name} Errors:  " +
                $"Key3 => Value4;  Key4 => Value5, Value6;  " + Environment.NewLine;

            // when
            string actualResult = randomXeption.GetValidationSummary();

            // then
            output.WriteLine($"Actual:");
            output.WriteLine(actualResult);
            output.WriteLine("");
            output.WriteLine($"Expected:");
            output.WriteLine(expectedResult);

            actualResult.Should().BeEquivalentTo(expectedResult);
        }
    }
}
