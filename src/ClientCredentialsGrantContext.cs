using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore
{
    public abstract class IdentityClientCredentialsContext : IdentityClientCredentialsContext<IdentityUser>
    {
        public IdentityClientCredentialsContext(DbContextOptions options) : base(options)
        {

        }
    }

    public class IdentityClientCredentialsContext<TUser> : IdentityClientCredentialsContext<ClientCredential, TUser, IdentityRole> 
        where TUser : IdentityUser
    {
        public IdentityClientCredentialsContext(DbContextOptions options) : base(options)
        {

        }
    }


    public abstract class IdentityClientCredentialsContext<TClient, TUser, TRole> : IdentityDbContext<TUser, TRole, string>
        where TClient : ClientCredential
        where TUser : IdentityUser
        where TRole : IdentityRole
    {
        public IdentityClientCredentialsContext(DbContextOptions options)
            : base(options)
        {
      
        }


        public virtual DbSet<ClientCredential> ClientCredential { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<ClientCredential>(entity =>
            {
                entity.ToTable("AspNetClientCredential");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450)
                    .ValueGeneratedNever();


                entity.Property(e => e.ClientSecret)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                //entity.HasMany<TUser>().WithOne().HasForeignKey(ur => ur.Id).IsRequired();

                entity.HasOne<TUser>().WithMany().HasForeignKey(ur => ur.UserId).IsRequired();


            });


        }

    }
}