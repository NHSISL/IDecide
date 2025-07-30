// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.Providers.PDS.Abstractions.Models.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds;
using LondonDataServices.IDecide.Core.Models.Foundations.Pds.Exceptions;
using System;
using System.Threading.Tasks;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Pds
{
    public partial class PdsService
    {
        private delegate ValueTask<PatientLookup> ReturningPatientLookupFunction();

        private async ValueTask<PatientLookup> TryCatch(ReturningPatientLookupFunction returningPatientLookupFunction)
        {
            try
            {
                return await returningPatientLookupFunction();
            }
            catch (PdsProviderValidationException pdsProviderValidationException)
            {
                ClientPdsException clientPdsException = new ClientPdsException(
                    message: "Client PDS error occurred, please contact support.",
                    innerException: pdsProviderValidationException,
                    data: pdsProviderValidationException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(clientPdsException);
            }
            catch (PdsProviderDependencyException pdsProviderDependencyException)
            {
                ServerPdsException serverPdsException = new ServerPdsException(
                    message: "Server PDS error occurred, please contact support.",
                    innerException: pdsProviderDependencyException,
                    data: pdsProviderDependencyException.Data);

                throw await CreateAndLogDependencyExceptionAsync(serverPdsException);
            }
            catch (PdsProviderServiceException pdsProviderServiceException)
            {
                ServerPdsException serverPdsException = new ServerPdsException(
                    message: "Server PDS error occurred, please contact support.",
                    innerException: pdsProviderServiceException,
                    data: pdsProviderServiceException.Data);

                throw await CreateAndLogDependencyExceptionAsync(serverPdsException);
            }
            catch (Exception exception)
            {
                var failedServicePdsException =
                    new FailedServicePdsException(
                        message: "Failed PDS service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedServicePdsException);
            }
        }

        private async ValueTask<PdsDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var pdsDependencyValidationException = new PdsDependencyValidationException(
                message: "PDS dependency validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(pdsDependencyValidationException);

            return pdsDependencyValidationException;
        }

        private async ValueTask<PdsDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var pdsDependencyException = new PdsDependencyException(
                message: "PDS dependency error occurred, please contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(pdsDependencyException);

            return pdsDependencyException;
        }

        private async ValueTask<PdsServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var pdsServiceException = new PdsServiceException(
                message: "PDS service error occurred, please contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(pdsServiceException);

            return pdsServiceException;
        }
    }
}
