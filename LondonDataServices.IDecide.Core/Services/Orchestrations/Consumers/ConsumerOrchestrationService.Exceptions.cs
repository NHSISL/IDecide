// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Models.Foundations.ConsumerAdoptions.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Consumers.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Notifications.Exceptions;
using LondonDataServices.IDecide.Core.Models.Foundations.Patients.Exceptions;
using LondonDataServices.IDecide.Core.Models.Orchestrations.Consumers.Exceptions;
using Xeptions;

namespace LondonDataServices.IDecide.Core.Services.Orchestrations.Consumers
{
    public partial class ConsumerOrchestrationService
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (InvalidDecisionsException invalidDecisionsException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidDecisionsException);
            }
            catch (ConsumerValidationException consumerValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(consumerValidationException);
            }
            catch (ConsumerDependencyValidationException consumerDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(consumerDependencyValidationException);
            }
            catch (ConsumerAdoptionValidationException consumerAdoptionValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(consumerAdoptionValidationException);
            }
            catch (ConsumerAdoptionDependencyValidationException consumerAdoptionDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    consumerAdoptionDependencyValidationException);
            }
            catch (PatientValidationException patientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(patientValidationException);
            }
            catch (PatientDependencyValidationException patientDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(patientDependencyValidationException);
            }
            catch (NotificationValidationException notificationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(notificationValidationException);
            }
            catch (NotificationDependencyValidationException notificationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(notificationDependencyValidationException);
            }
        }

        private async ValueTask<ConsumerOrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var consumerOrchestrationValidationException =
                new ConsumerOrchestrationValidationException(
                    message: "Consumer orchestration validation error occurred, " +
                             "please fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerOrchestrationValidationException);

            return consumerOrchestrationValidationException;
        }

        private async ValueTask<ConsumerOrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var consumerOrchestrationDependencyValidationException =
                new ConsumerOrchestrationDependencyValidationException(
                    message: "Consumer orchestration dependency validation error occurred, " +
                             "please fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(consumerOrchestrationDependencyValidationException);

            return consumerOrchestrationDependencyValidationException;
        }
    }
}
