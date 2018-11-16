﻿using Microsoft.AspNetCore.Identity;
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
            //var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            //var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRol>>();
            //string[] roleNames = { "Super", "Admin", "User" };
            ////await context.Database.EnsureDeletedAsync();
            ////await context.Database.MigrateAsync();

            //IdentityResult roleResult;

            //foreach (var roleName in roleNames)
            //{
            //    //creating the roles and seeding them to the database
            //    var roleExist = await roleManager.RoleExistsAsync(roleName);
            //    if (!roleExist)
            //    {
            //        roleResult = await roleManager.CreateAsync(new ApplicationRol(roleName));
            //    }
            //}

            //if (!context.Users.Any())
            //{
            //    ApplicationUser user1 = new ApplicationUser()
            //    {
            //        Email = "otero.ortiz404@gmail.com",
            //        SecurityStamp = Guid.NewGuid().ToString(),
            //        UserName = "Julio"
            //    };
            //    ApplicationUser user2 = new ApplicationUser()
            //    {
            //        Email = "chucho@gmail.com",
            //        SecurityStamp = Guid.NewGuid().ToString(),
            //        UserName = "Jesus"
            //    };
            //    ApplicationUser user3 = new ApplicationUser()
            //    {
            //        Email = "kmontiel@gmail.com",
            //        SecurityStamp = Guid.NewGuid().ToString(),
            //        UserName = "Karla"
            //    };
            //    await userManager.CreateAsync(user1, "@Trigger@*2018");
            //    await userManager.AddToRoleAsync(user1, "Admin");
            //    await userManager.CreateAsync(user2, "@Trigger@*2018");
            //    await userManager.AddToRoleAsync(user2, "User");
            //    await userManager.CreateAsync(user3, "@Trigger@*2018");
            //    await userManager.AddToRoleAsync(user3, "User");
            //}

        }
    }
}
