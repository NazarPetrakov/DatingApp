using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public static class Seed
{
    public static async Task SeedUsersAsync(UserManager<AppUser> userManager, 
        RoleManager<AppRole> roleManager)
    {
        if(await userManager.Users.AnyAsync()) return ;

        var userData = await File.ReadAllTextAsync("Data/UserSeedData");

        var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};

        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        if(users == null) return ;

        var roles = new List<AppRole> 
        {
            new() {Name = "Member"},
            new() {Name = "Admin"},
            new() {Name = "Moderator"}
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        foreach (var item in users)
        {
            item.UserName = item.UserName!.ToLower();
            await userManager.CreateAsync(item, "Pa$$w0rd");
            await userManager.AddToRoleAsync(item, "Member");
        }

        var admin = new AppUser
        {
            UserName = "Admin",
            KnownAs = "Admin",
            Gender = "",
            City = "",
            Country = ""
        };

        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);
    }
}
