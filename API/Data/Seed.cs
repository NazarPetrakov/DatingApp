using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public static class Seed
{
    public static async Task SeedUsersAsync(DataContext context)
    {
        if(await context.Users.AnyAsync()) return ;

        var userData = await File.ReadAllTextAsync("Data/UserSeedData");

        var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};

        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        if(users == null) return ;

        foreach (var item in users)
        {
            using var hmac = new HMACSHA512();

            item.UserName = item.UserName.ToLower();
            item.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
            item.PasswordSalt = hmac.Key;

            context.Users.Add(item);
        }

        await context.SaveChangesAsync();
    }
}