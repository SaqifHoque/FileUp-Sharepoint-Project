using DocUploader.Shared.AuthModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DocUploader.Server.Data
{
    public partial class ApplicationDbContext : IdentityDbContext<ApiUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
                
        }

        public DbSet<ApiUser> ApiUsers { get; set; }
        public DbSet<Shared.Models.Client> Clients { get; set; }  
        public DbSet<ClientsRequest> ClientsRequests { get; set; }  
        public DbSet<Request> Requests { get; set; }  
        public DbSet<RequestDocuments> RequestDocuments { get; set; }
        public DbSet<UploadedDocuments> UploadedDocuments { get; set; }
        public DbSet<TableModel> TableModels { get; set; }
        public DbSet<GenericModel> GenericModels { get; set; }
        public DbSet<TwoFactorAuth> TwoFactorAuths { get; set; }
        public DbSet<DocumentCategory> DocumentCategories { get; set; }
        public DbSet<ClientDocumentCategory> ClientDocumentCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            //Create Role and Seed data to DB Start

            //Role Adding Start
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    Id = "ACD89-BAF03-ZKR88-99MN"
                },

                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                    Id = "3BGTS-ISBNX-89KRK-YYZZ"
                }

              );
            //Role Adding End

            //Add User with Seed Data Start
            var hasher = new PasswordHasher<ApiUser>();

            modelBuilder.Entity<ApiUser>().HasData(

                new ApiUser
                {
                    Id = "KRMN8-SHDA3-BGMNP-MP88",
                    Email = "admin@ltarsde.com",
                    NormalizedEmail = "ADMIN@LTARSDE.COM",
                    UserName = "admin@ltarsde.com",
                    NormalizedUserName = "ADMIN@LTARSDE.COM",
                    ClientName = "Admin",
                    Last4Digits = "1234",
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                },

                new ApiUser
                {
                    Id = "SHDRE-COM2T-FF99Z-GM55",
                    Email = "user@ltarsde.com",
                    NormalizedEmail = "USER@LTARSDE.COM",
                    UserName = "user@ltarsde.com",
                    NormalizedUserName = "USER@LTARSDE.COM",
                    ClientName = "Admin",
                    Last4Digits = "1234",
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                }

             );
            //Add User with Seed Data End

            //Mapping or Adding Role with ApiUser Start
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(

                new IdentityUserRole<string>
                {
                    RoleId = "ACD89-BAF03-ZKR88-99MN",
                    UserId = "KRMN8-SHDA3-BGMNP-MP88"
                },

                new IdentityUserRole<string>
                {
                    RoleId = "3BGTS-ISBNX-89KRK-YYZZ",
                    UserId = "SHDRE-COM2T-FF99Z-GM55"
                }
              );
            //Mapping or Adding Role with ApiUser End

            //Create Role and Seed data to DB End


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
