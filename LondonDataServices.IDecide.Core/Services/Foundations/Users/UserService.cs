// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using LondonDataServices.IDecide.Core.Brokers.DateTimes;
using LondonDataServices.IDecide.Core.Brokers.Loggings;
using LondonDataServices.IDecide.Core.Brokers.Securities;
using LondonDataServices.IDecide.Core.Brokers.Storages.Sql;
using LondonDataServices.IDecide.Core.Models.Foundations.Users;

namespace LondonDataServices.IDecide.Core.Services.Foundations.Users
{
    public partial class UserService : IUserService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityAuditBroker securityAuditBroker;
        private readonly ILoggingBroker loggingBroker;

        public UserService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityAuditBroker securityAuditBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityAuditBroker = securityAuditBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<User> AddUserAsync(User user) =>
            TryCatch(async () =>
            {
                user = await this.securityAuditBroker.ApplyAddAuditValuesAsync(user);
                await ValidateUserOnAdd(user);

                return await this.storageBroker.InsertUserAsync(user);
            });

        public ValueTask<IQueryable<User>> RetrieveAllUsersAsync() =>
            TryCatch(async () => await this.storageBroker.SelectAllUsersAsync());

        public ValueTask<User> RetrieveUserByIdAsync(Guid userId) =>
            TryCatch(async () =>
            {
                ValidateUserId(userId);

                User maybeUser = await this.storageBroker.SelectUserByIdAsync(userId);

                ValidateStorageUser(maybeUser, userId);

                return maybeUser;
            });

        public ValueTask<User> ModifyUserAsync(User user) =>
            TryCatch(async () =>
            {
                user = await this.securityAuditBroker.ApplyModifyAuditValuesAsync(user);
                await ValidateUserOnModify(user);

                User maybeUser = await this.storageBroker.SelectUserByIdAsync(user.Id);

                ValidateStorageUser(maybeUser, user.Id);

                user = await this.securityAuditBroker
                    .EnsureAddAuditValuesRemainsUnchangedOnModifyAsync(user, maybeUser);

                ValidateAgainstStorageUserOnModify(
                    inputUser: user,
                    storageUser: maybeUser);

                return await this.storageBroker.UpdateUserAsync(user);
            });

        public ValueTask<User> RemoveUserByIdAsync(Guid userId) =>
            TryCatch(async () =>
            {
                ValidateUserId(userId);

                User maybeUser = await this.storageBroker.SelectUserByIdAsync(userId);

                ValidateStorageUser(maybeUser, userId);

                return await this.storageBroker.DeleteUserAsync(maybeUser);
            });
    }
}
