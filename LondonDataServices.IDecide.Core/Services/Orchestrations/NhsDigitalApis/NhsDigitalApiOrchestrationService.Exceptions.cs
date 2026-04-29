// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.NhsDigitalApis.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Users.Exceptions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.NhsDigitalApis.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.NhsDigitalApis
{
    public partial class NhsDigitalApiOrchestrationService
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (InvalidNhsDigitalApiOrchestrationArgumentException
                invalidNhsDigitalApiOrchestrationArgumentException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidNhsDigitalApiOrchestrationArgumentException);
            }
            catch (NhsDigitalApiValidationException nhsDigitalApiValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    nhsDigitalApiValidationException);
            }
            catch (NhsDigitalApiDependencyValidationException nhsDigitalApiDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    nhsDigitalApiDependencyValidationException);
            }
            catch (UserValidationException userValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(userValidationException);
            }
            catch (UserDependencyValidationException userDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(userDependencyValidationException);
            }
            catch (NhsDigitalApiDependencyException nhsDigitalApiDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(nhsDigitalApiDependencyException);
            }
            catch (NhsDigitalApiServiceException nhsDigitalApiServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(nhsDigitalApiServiceException);
            }
            catch (UserDependencyException userDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(userDependencyException);
            }
            catch (UserServiceException userServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(userServiceException);
            }
            catch (System.Exception exception)
            {
                var failedNhsDigitalApiOrchestrationServiceException =
                    new FailedNhsDigitalApiOrchestrationServiceException(
                        message: "Failed NhsDigitalApi orchestration service error occurred, " +
                            "please contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(
                    failedNhsDigitalApiOrchestrationServiceException);
            }
        }

        private async ValueTask<NhsDigitalApiOrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var nhsDigitalApiOrchestrationValidationException =
                new NhsDigitalApiOrchestrationValidationException(
                    message: "NhsDigitalApi orchestration validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(nhsDigitalApiOrchestrationValidationException);

            return nhsDigitalApiOrchestrationValidationException;
        }

        private async ValueTask<NhsDigitalApiOrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var nhsDigitalApiOrchestrationDependencyValidationException =
                new NhsDigitalApiOrchestrationDependencyValidationException(
                    message: "NhsDigitalApi orchestration dependency validation error occurred, " +
                        "please fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(nhsDigitalApiOrchestrationDependencyValidationException);

            return nhsDigitalApiOrchestrationDependencyValidationException;
        }

        private async ValueTask<NhsDigitalApiOrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var nhsDigitalApiOrchestrationDependencyException =
                new NhsDigitalApiOrchestrationDependencyException(
                    message: "NhsDigitalApi orchestration dependency error occurred, " +
                        "please fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(nhsDigitalApiOrchestrationDependencyException);

            return nhsDigitalApiOrchestrationDependencyException;
        }

        private async ValueTask<NhsDigitalApiOrchestrationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var nhsDigitalApiOrchestrationServiceException =
                new NhsDigitalApiOrchestrationServiceException(
                    message: "NhsDigitalApi orchestration service error occurred, please contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(nhsDigitalApiOrchestrationServiceException);

            return nhsDigitalApiOrchestrationServiceException;
        }
    }
}
