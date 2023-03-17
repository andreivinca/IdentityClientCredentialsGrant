using IdentityClientCredentialsGrant.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdentityClientCredentialsGrant.Manager
{
    public class ClientCredentialManager<TUser, TContext>
        where TUser : IdentityUser
        where TContext : DbContext
    {
        private readonly ClientCredentialStore<TUser, TContext> Store;

        public ClientCredentialManager(UserManager<TUser> userManager,
                                        ClientCredentialStore<TUser, TContext> userProfileStore,
                                         IOptions<IdentityOptions> optionsAccessor,
                                         IUserConfirmation<TUser> confirmation)
        {
            this.UserManager = userManager;
            Store = userProfileStore;
            Options = optionsAccessor?.Value ?? new IdentityOptions();

        }

        private readonly IUserConfirmation<TUser> _confirmation;

        public UserManager<TUser> UserManager { get; private set; }
        public IdentityOptions Options { get; set; }



        public virtual string GenerateSecretString()
        {
            // Generate a random secret of 32 bytes (256 bits)
            byte[] secretBytes = new byte[32];
            RandomNumberGenerator.Fill(secretBytes);

            // Convert the bytes to a string (e.g. for use as a password)
            string secretString = Convert.ToBase64String(secretBytes);
            return secretString;
        }

        public Task<IdentityResult> CreateAsync(TUser user, string secret, DateTimeOffset expirationDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var newClientCredential = new ClientCredential
            {
                UserId = user.Id,
                ClientSecret = secret,
                ExpireDate = expirationDate
            };

            return this.CreateAsync(newClientCredential);
        }

        public Task<IdentityResult> CreateAsync(ClientCredential clientCredential, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.Store.CreateAsync(clientCredential);
        }

        public virtual async Task<SignInResult> CheckSecretSignInAsync(string userName, string secret)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            return await CheckSecretSignInAsync(user, secret);
        }


        public virtual async Task<SignInResult> CheckSecretSignInAsync(TUser user, string secret)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var error = await PreSignInCheck(user);
            if (error != null)
            {
                return error;
            }

            var result = await this.Store.IsSecretValid(user, secret);

            return result;
        }


        protected virtual async Task<SignInResult> PreSignInCheck(TUser user)
        {
            if (!await CanSignInAsync(user))
            {
                return SignInResult.NotAllowed;
            }
            if (await IsLockedOut(user))
            {
                return await LockedOut(user);
            }
            return null;
        }

        protected virtual async Task<bool> IsLockedOut(TUser user)
        {
            return UserManager.SupportsUserLockout && await UserManager.IsLockedOutAsync(user);
        }
        protected virtual Task<SignInResult> LockedOut(TUser user)
        {
            return Task.FromResult(SignInResult.LockedOut);
        }

        public virtual async Task<bool> CanSignInAsync(TUser user)
        {
            if (Options.SignIn.RequireConfirmedEmail && !(await UserManager.IsEmailConfirmedAsync(user)))
            {
                return false;
            }
            if (Options.SignIn.RequireConfirmedPhoneNumber && !(await UserManager.IsPhoneNumberConfirmedAsync(user)))
            {
                return false;
            }
            if (Options.SignIn.RequireConfirmedAccount && !(await _confirmation.IsConfirmedAsync(UserManager, user)))
            {
                return false;
            }
            return true;
        }



        //public async Task<AspNetClientCredential> GetUserProfileAsync(IdentityUser user)
        //{
        //    // Get UserProfile record for user

        //    return Task.FromResult(user.UserName);
        //    //return await _userProfileStore.GetUserProfileAsync(user.Id);
        //}

        //public async Task<IdentityResult> UpdateUserProfileAsync(IdentityUser user, AspNetClientCredential userProfile)
        //{
        //    // Update UserProfile record

        //    userProfile.UserId = user.Id;

        //    return await _userProfileStore.UpdateUserProfileAsync(userProfile);
        //}
    }

}
