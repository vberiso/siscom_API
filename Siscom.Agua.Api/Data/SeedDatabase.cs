using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Siscom.Agua.DAL;
using Siscom.Agua.DAL.Models;
using System;
using System.Linq;

namespace Siscom.Agua.Api.Data
{
    public class SeedDatabase
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            //var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            ////var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            //var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRol>>();
            //string[] roleNames = { "Super", "Admin", "User", "Isabi", "Transito", "Supervisor","GenerarOrden" };
            ////string[] roleNames = { "Super", "Admin", "User", "Supervisor" };
            ////////    //await context.Database.EnsureDeletedAsync();
            ////////    //await context.Database.MigrateAsync();

            //IdentityResult roleResult;
            ////IdentityResult result;

            //foreach (var roleName in roleNames)
            //{
            //    //creating the roles and seeding them to the database
            //    var roleExist = await roleManager.RoleExistsAsync(roleName);
            //    if (!roleExist)
            //    {
            //        roleResult = await roleManager.CreateAsync(new ApplicationRol(roleName));
            //    }
            //}

            //    if (!context.Users.Any())
            //    {
            //        ApplicationUser user1 = new ApplicationUser()
            //        {
            //            Email = "otero.ortiz404@gmail.com",
            //            SecurityStamp = Guid.NewGuid().ToString(),
            //            UserName = "Julio Cesar",
            //            LastName = "Otero",
            //            SecondLastName = "Ortiz"
            //        };
            //        ApplicationUser user2 = new ApplicationUser()
            //        {
            //            Email = "chucho@gmail.com",
            //            SecurityStamp = Guid.NewGuid().ToString(),
            //            UserName = "Jesus",
            //            LastName = "Gonzalez",
            //            SecondLastName = "Romero"
            //        };
            //        ApplicationUser user3 = new ApplicationUser()
            //        {
            //            Email = "kmontiel@gmail.com",
            //            SecurityStamp = Guid.NewGuid().ToString(),
            //            UserName = "Karla",
            //            LastName = "Montiel",
            //            SecondLastName = "Morales"
            //        };
            //        result = await userManager.CreateAsync(user1, "@Trigger@*2018");
            //        await userManager.AddToRoleAsync(user1, "Admin");
            //        result = await userManager.CreateAsync(user2, "@Trigger@*2018");
            //        await userManager.AddToRoleAsync(user2, "User");
            //        result = await userManager.CreateAsync(user3, "@Trigger@*2018");
            //        await userManager.AddToRoleAsync(user3, "User");
            //    }
            //    if (context.Users.Where(u => u.UserName == "Julio" && u.UserName == "Jesus" && u.UserName == "Karla") != null)
            //    {
            //        ApplicationUser user1 = new ApplicationUser()
            //        {
            //            Email = "uriel@gmail.com",
            //            SecurityStamp = Guid.NewGuid().ToString(),
            //            UserName = "Uriel",
            //            Name = "Uriel",
            //            LastName = "Romero",
            //            SecondLastName = "Lopez"
            //        };
            //        ApplicationUser user2 = new ApplicationUser()
            //        {
            //            Email = "victor@gmail.com",
            //            SecurityStamp = Guid.NewGuid().ToString(),
            //            UserName = "Victor",
            //            Name = "Victor",
            //            LastName = "Garcia",
            //            SecondLastName = "Romero"
            //        };
            //        ApplicationUser user3 = new ApplicationUser()
            //        {
            //            Email = "batch@gmail.com",
            //            SecurityStamp = Guid.NewGuid().ToString(),
            //            UserName = "Batch",
            //            Name = "Sistemas",
            //            LastName = "Servicios",
            //            SecondLastName = "Comerciales"
            //        };
            //        result = await userManager.CreateAsync(user1, "@Trigger@*2018");
            //        await userManager.AddToRoleAsync(user1, "User");
            //        result = await userManager.CreateAsync(user2, "@Trigger@*2018");
            //        await userManager.AddToRoleAsync(user2, "User");
            //        result = await userManager.CreateAsync(user3, "@Trigger@*2018");
            //        await userManager.AddToRoleAsync(user3, "Admin");
            //    }

            //ApplicationUser user2 = new ApplicationUser()
            //{
            //    Email = "LLarios@gmail.com",
            //    SecurityStamp = Guid.NewGuid().ToString(),
            //    UserName = "LLarios",
            //    Name = "Linda",
            //    LastName = "Larios",
            //    SecondLastName = "Castillo",
            //    DivitionId = 1
            //};
            //result = await userManager.CreateAsync(user2, "Presidencia2019");
            //var x  = await userManager.AddToRoleAsync(user2, "User");

            //ApplicationUser user2 = new ApplicationUser()
            //{
            //    Email = "IGil@gmail.com",
            //    SecurityStamp = Guid.NewGuid().ToString(),
            //    UserName = "LCuautle",
            //    Name = "Leticia",
            //    LastName = "Cuautle",
            //    SecondLastName = ".",
            //    DivitionId = 1
            //};
            //result = await userManager.CreateAsync(user2, "Presidencia2019");
            //await userManager.AddToRoleAsync(user2, "User");

            //ApplicationUser user3 = new ApplicationUser()
            //{
            //    Email = "LOlvera@gmail.com",
            //    SecurityStamp = Guid.NewGuid().ToString(),
            //    UserName = "LOlvera",
            //    Name = "Laura ",
            //    LastName = "Olvera  ",
            //    SecondLastName = ".",
            //    DivitionId = 1
            //};
            //result = await userManager.CreateAsync(user3, "Presidencia2019");
            //await userManager.AddToRoleAsync(user3, "User");

            //ApplicationUser user4 = new ApplicationUser()
            //{
            //    Email = "ACandida@gmail.com",
            //    SecurityStamp = Guid.NewGuid().ToString(),
            //    UserName = "ACandida",
            //    Name = "Alicia",
            //    LastName = "Candida",
            //    SecondLastName = ".",
            //    DivitionId = 1
            //};
            //result = await userManager.CreateAsync(user4, "Presidencia2019");
            //await userManager.AddToRoleAsync(user4, "User");

            //ApplicationUser user5 = new ApplicationUser()
            //{
            //    Email = "AGarcia@gmail.com",
            //    SecurityStamp = Guid.NewGuid().ToString(),
            //    UserName = "AGarcia",
            //    Name = "Anahi ",
            //    LastName = "Garcia",
            //    SecondLastName = ".",
            //    DivitionId = 1
            //};
            //result = await userManager.CreateAsync(user5, "Presidencia2019");
            //await userManager.AddToRoleAsync(user5, "User");


            //ApplicationUser user6 = new ApplicationUser()
            //{
            //    Email = "MRojas@gmail.com",
            //    SecurityStamp = Guid.NewGuid().ToString(),
            //    UserName = "MRojas",
            //    Name = "Martha",
            //    LastName = "Rojas",
            //    SecondLastName = ".",
            //    DivitionId = 1
            //};
            //result = await userManager.CreateAsync(user6, "Presidencia2019");
            //await userManager.AddToRoleAsync(user6, "User");

            //ApplicationUser user7 = new ApplicationUser()
            //{
            //    Email = "JSarmiento@gmail.com",
            //    SecurityStamp = Guid.NewGuid().ToString(),
            //    UserName = "JSarmiento",
            //    Name = "Javier",
            //    LastName = "Sarmiento",
            //    SecondLastName = ".",
            //    DivitionId = 1
            //};
            //result = await userManager.CreateAsync(user7, "Presidencia2019");
            //await userManager.AddToRoleAsync(user7, "User");
        }
    }
}
