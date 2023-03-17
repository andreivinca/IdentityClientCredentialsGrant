using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IdentityClientCredentialsGrant.Store
{
    public class ClientCredentialStore<TUser, TContext>
        where TUser : IdentityUser
        where TContext: DbContext
    {
        private readonly TContext _dbContext;


        private DbSet<ClientCredential> ClientCredential { get { return _dbContext.Set<ClientCredential>(); } }
        private DbSet<TUser> User { get { return _dbContext.Set<TUser>(); } }
        public bool AutoSaveChanges { get; set; } = true;

        public ClientCredentialStore(TContext dbContext)
        {
       
            _dbContext = dbContext;
        }


        public async Task<IdentityResult> CreateAsync(ClientCredential clientCredential, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if(clientCredential is null)
            {
                throw new ArgumentNullException(nameof(clientCredential));
            }

            this.ClientCredential.Add(clientCredential);
            await this.SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<SignInResult> IsSecretValid(TUser user, string secret, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await this.FindByUserAndSecret(user, secret, cancellationToken);

            if(result is null)
            {
                return SignInResult.Failed;
            }

            if(result.ExpireDate < DateTimeOffset.UtcNow)
            {
                return SignInResult.Failed;
            }

            return SignInResult.Success;
        }

        public async Task<ClientCredential?> FindByUserAndSecret(TUser user, string secret, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await this.ClientCredential.FirstOrDefaultAsync(i => i.UserId.Equals(user.Id) && i.ClientSecret == secret);
            return result;
        }

        protected Task SaveChanges(CancellationToken cancellationToken)
        {
            return AutoSaveChanges ? _dbContext.SaveChangesAsync(cancellationToken) : Task.CompletedTask;
        }
























        //public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        //{
        //    // Create user in User table

        //    await _dbContext.Users.AddAsync(user, cancellationToken);
        //    await _dbContext.SaveChangesAsync(cancellationToken);

        //    // Create UserProfile record

        //    var userProfile = new AspNetClientCredential
        //    {
        //        UserId = user.Id
        //    };

        //    await _dbContext.AspNetClientCredentials.AddAsync(userProfile, cancellationToken);
        //    await _dbContext.SaveChangesAsync(cancellationToken);

        //    return IdentityResult.Success;
        //}

        //public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        //{
        //    // Delete user from User table

        //    _dbContext.Users.Remove(user);
        //    await _dbContext.SaveChangesAsync(cancellationToken);

        //    // Delete UserProfile record

        //    var userProfile = await _dbContext.AspNetClientCredentials.FirstOrDefaultAsync(up => up.UserId == user.Id, cancellationToken);

        //    if (userProfile != null)
        //    {
        //        _dbContext.AspNetClientCredentials.Remove(userProfile);
        //        await _dbContext.SaveChangesAsync(cancellationToken);
        //    }

        //    return IdentityResult.Success;
        //}

        //public void Dispose()
        //{
        //    // Dispose any resources if needed
        //}

        //public async Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        //{
        //    // Find user in User table

        //    return await _dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        //}

        //public async Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        //{
        //    // Find user in User table

        //    return await _dbContext.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
        //}

        //public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        //{
        //    return Task.FromResult(user.NormalizedUserName);
        //}

        //public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        //{
        //    return Task.FromResult(user.Id);
        //}

        //public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        //{
        //    return Task.FromResult(user.UserName);
        //}

        //public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
        //{
        //    user.NormalizedUserName = normalizedName;
        //    return Task.CompletedTask;
        //}

        //public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
        //{
        //    user.UserName = userName;
        //    return Task.CompletedTask;
        //}

        //public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        //{
        //    // Update user in User table

        //    _dbContext.Users.Update(user);
        //    await _dbContext.SaveChangesAsync(cancellationToken);

        //    // Update UserProfile record

        //    var userProfile = await _dbContext.AspNetClientCredentials.FirstOrDefaultAsync(up => up.UserId == user.Id, cancellationToken);

        //    if (userProfile != null)
        //    {
        //        // Update UserProfile properties as needed

        //        //userProfile.FirstName = "John";
        //        //userProfile.LastName = "Doe";

        //        _dbContext.AspNetClientCredentials.Update(userProfile);
        //        await _dbContext.SaveChangesAsync(cancellationToken);
        //    }

        //    return IdentityResult.Success;
        //}
    }
}
