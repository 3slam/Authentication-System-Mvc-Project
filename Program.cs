using LOGWITHGOOFLE.Models;
using LOGWITHGOOFLE.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LOGWITHGOOFLE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            }).AddCookie().AddGoogle(options =>
       {
           options.ClientId = builder.Configuration["GoogleAuth:ClientId"];
           options.ClientSecret = builder.Configuration["GoogleAuth:ClientSecret"];
       }).AddMicrosoftAccount(options =>
       {
           options.ClientId = builder.Configuration["MicrosoftAuth:ClientId"];
           options.ClientSecret = builder.Configuration["MicrosoftAuth:ClientSecret"];
       });


            builder.Services.AddDbContext<CompanyDbContext>(options =>
            {
                options.UseSqlServer("Data Source=DESKTOP-7NSIHE8\\SQLEXPRESS;Initial Catalog=AuthSysetm;Integrated Security=True;TrustServerCertificate=True");
            });

            builder.Services.AddTransient<ISenderEmail, EmailSender>();

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;

            }).AddEntityFrameworkStores<CompanyDbContext>().AddDefaultTokenProviders();


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
