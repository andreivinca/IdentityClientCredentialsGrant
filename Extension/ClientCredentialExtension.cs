using IdentityClientCredentialsGrant.Manager;
using IdentityClientCredentialsGrant.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace IdentityClientCredentialsGrant.Extension
{
    public static class ClientCredentialExtension
    {
        public static IdentityBuilder AddClientCredentialIdentity<TContext, TUser>(this IdentityBuilder builder)
             where TUser : IdentityUser
             where TContext : DbContext
        {
            builder.AddClientCredentialIdentity<ClientCredential, TContext, TUser>();
            return builder;
        }


        public static IdentityBuilder AddClientCredentialIdentity<TClient, TContext, TUser>(this IdentityBuilder builder)
             where TUser : IdentityUser
             where TContext : DbContext
        {
            AddStores(builder.Services, builder.UserType, typeof(TClient), typeof(TContext));

            builder.Services.TryAddScoped<ClientCredentialManager<TUser, TContext>>();

            return builder;
        }


        private static void AddStores(IServiceCollection services, Type userType, Type clientCredential,  Type contextType)
        {
            var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<>));
            if (identityUserType == null)
            {
                throw new InvalidOperationException();
            }

            //var keyType = identityUserType.GenericTypeArguments[0];


            //var identityContext = FindGenericBaseType(contextType, typeof(IdentityClientCredentialsContext<,>));

            //var identityClientCredential = typeof(ClientCredential<>).MakeGenericType(userType);

            var clientCredentialStore = typeof(ClientCredentialStore<,>).MakeGenericType(userType, contextType);

            services.TryAddScoped(clientCredentialStore);
            services.TryAddScoped<ClientCredentialStore<IdentityUser, DbContext>>(i => (ClientCredentialStore<IdentityUser, DbContext>)i.GetRequiredService(clientCredentialStore));


            var manager = typeof(ClientCredentialManager<,>).MakeGenericType(userType, contextType);
            services.TryAddScoped(manager);
        }

        private static Type FindGenericBaseType(Type currentType, Type genericBaseType)
        {
            var type = currentType;
            while (type != null)
            {
                var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
                if (genericType != null && genericType == genericBaseType)
                {
                    return type;
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}
