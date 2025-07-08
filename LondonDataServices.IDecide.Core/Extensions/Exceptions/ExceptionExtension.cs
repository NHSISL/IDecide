// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LondonDataServices.IDecide.Core.Extensions.Exceptions
{
    public static class ExceptionExtension
    {
        public static string GetValidationSummary(this Exception exception)
        {
            if ((exception == null || exception.Data.Count == 0)
                && (exception?.InnerException == null || exception.InnerException.Data.Count == 0))
            {
                return string.Empty;
            }

            StringBuilder validationSummary = new StringBuilder();
            AppendErrorSummary(exception, validationSummary);

            if (exception.InnerException != null)
            {
                AppendInnerErrorSummary(exception.InnerException, validationSummary);
            }

            return validationSummary.ToString();
        }

        private static void AppendErrorSummary(Exception exception, StringBuilder validationSummary)
        {
            if (exception.Data.Count > 0)
            {
                validationSummary.Append($"{exception.GetType().Name} Errors:  ");

                foreach (DictionaryEntry entry in exception.Data)
                {
                    string errorSummary = ((List<string>)entry.Value)
                        .Select((value) => value)
                        .Aggregate((current, next) => current + ", " + next);

                    string line = $"{entry.Key} => {errorSummary};  ";

                    if (!validationSummary.ToString().Contains(line))
                    {
                        validationSummary.Append(line);
                    }
                }

                validationSummary.AppendLine();
            }
        }

        private static void AppendInnerErrorSummary(Exception exception, StringBuilder validationSummary)
        {
            if (exception != null)
            {
                AppendErrorSummary(exception, validationSummary);

                if (exception.InnerException != null)
                {
                    AppendInnerErrorSummary(exception.InnerException, validationSummary);
                }
            }
        }

        public static bool IsSameExceptionAs(this Exception exception, Exception otherException)
        {
            // If both exceptions are null, they are considered equal
            if (exception == null && otherException == null)
                return true;

            // If only one of them is null, they are not equal
            if (exception == null || otherException == null)
                return false;

            // Compare exception types, messages, and data
            if (exception.GetType().FullName != otherException.GetType().FullName ||
                exception.Message != otherException.Message ||
                !DataEquals(exception.Data, otherException.Data))
            {
                return false;
            }

            // Recursively compare inner exceptions
            return exception.InnerException.IsSameExceptionAs(otherException.InnerException);
        }

        private static bool DataEquals(IDictionary data1, IDictionary data2)
        {
            // Custom logic for comparing data dictionaries, implement as needed
            // Example implementation:
            if (data1 == null && data2 == null)
                return true;

            if (data1 == null || data2 == null || data1.Count != data2.Count)
                return false;

            foreach (var key in data1.Keys)
            {
                if (!data2.Contains(key) || !Equals(data1[key], data2[key]))
                    return false;
            }

            return true;
        }
    }
}
