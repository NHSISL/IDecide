// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using LondonDataServices.IDecide.Portal.Server.Models;
using LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Brokers;
using Tynamix.ObjectFiller;

namespace LondonDataServices.IDecide.Portal.Server.Tests.Acceptance.Apis.PatientCodes
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PatientCodeTests
    {
        private readonly ApiBroker apiBroker;

        public PatientCodeTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static PatientCodeRequest CreateRandomPatientCodeRequest(string nhsNumber)
        {
            PatientCodeRequest patientCodeRequest = new PatientCodeRequest
            {
                NhsNumber = nhsNumber,
                VerificationCode = GetRandomStringWithLengthOf(5),
                NotificationPreference = "Email",
                GenerateNewCode = false
            };

            return patientCodeRequest;
        }
    }
}
