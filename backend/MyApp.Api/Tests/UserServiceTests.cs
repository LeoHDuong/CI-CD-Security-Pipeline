using Microsoft.EntityFrameworkCore;
using MyApp.Api.Data;
using MyApp.Api.Models;
using MyApp.Api.Services;
using Xunit;

namespace MyApp.Api.Tests;

public class UserServiceTests
{
    private AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetAll_ReturnsAllUsers()
    {
        using var context = CreateContext(nameof(GetAll_ReturnsAllUsers));
        context.Users.AddRange(
            new User { Id = Guid.NewGuid(), Name = "Alice", Email = "alice@example.com" },
            new User { Id = Guid.NewGuid(), Name = "Bob", Email = "bob@example.com" }
        );
        await context.SaveChangesAsync();

        var service = new UserService(context);
        var result = await service.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoUsers()
    {
        using var context = CreateContext(nameof(GetAll_ReturnsEmptyList_WhenNoUsers));
        var service = new UserService(context);

        var result = await service.GetAll();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetById_ReturnsUser_WhenExists()
    {
        using var context = CreateContext(nameof(GetById_ReturnsUser_WhenExists));
        var userId = Guid.NewGuid();
        context.Users.Add(new User { Id = userId, Name = "Alice", Email = "alice@example.com" });
        await context.SaveChangesAsync();

        var service = new UserService(context);
        var result = await service.GetById(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("Alice", result.Name);
        Assert.Equal("alice@example.com", result.Email);
    }

    [Fact]
    public async Task GetById_ReturnsNull_WhenNotFound()
    {
        using var context = CreateContext(nameof(GetById_ReturnsNull_WhenNotFound));
        var service = new UserService(context);

        var result = await service.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task Create_ReturnsUserWithGeneratedId()
    {
        using var context = CreateContext(nameof(Create_ReturnsUserWithGeneratedId));
        var service = new UserService(context);

        var result = await service.Create("Alice", "alice@example.com");

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Alice", result.Name);
        Assert.Equal("alice@example.com", result.Email);
    }

    [Fact]
    public async Task Create_PersistsUserToDatabase()
    {
        using var context = CreateContext(nameof(Create_PersistsUserToDatabase));
        var service = new UserService(context);

        var created = await service.Create("Alice", "alice@example.com");

        var stored = await context.Users.FindAsync(created.Id);
        Assert.NotNull(stored);
        Assert.Equal("Alice", stored.Name);
        Assert.Equal("alice@example.com", stored.Email);
    }

    [Fact]
    public async Task Update_ReturnsUpdatedUser_WhenExists()
    {
        using var context = CreateContext(nameof(Update_ReturnsUpdatedUser_WhenExists));
        var userId = Guid.NewGuid();
        context.Users.Add(new User { Id = userId, Name = "Alice", Email = "alice@example.com" });
        await context.SaveChangesAsync();

        var service = new UserService(context);
        var result = await service.Update(userId, "Alice Updated", "alice.updated@example.com");

        Assert.NotNull(result);
        Assert.Equal("Alice Updated", result.Name);
        Assert.Equal("alice.updated@example.com", result.Email);
    }

    [Fact]
    public async Task Update_PersistsChangesToDatabase()
    {
        using var context = CreateContext(nameof(Update_PersistsChangesToDatabase));
        var userId = Guid.NewGuid();
        context.Users.Add(new User { Id = userId, Name = "Alice", Email = "alice@example.com" });
        await context.SaveChangesAsync();

        var service = new UserService(context);
        await service.Update(userId, "Alice Updated", "alice.updated@example.com");

        var stored = await context.Users.FindAsync(userId);
        Assert.NotNull(stored);
        Assert.Equal("Alice Updated", stored.Name);
        Assert.Equal("alice.updated@example.com", stored.Email);
    }

    [Fact]
    public async Task Update_ReturnsNull_WhenUserNotFound()
    {
        using var context = CreateContext(nameof(Update_ReturnsNull_WhenUserNotFound));
        var service = new UserService(context);

        var result = await service.Update(Guid.NewGuid(), "Alice", "alice@example.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_ReturnsTrue_WhenUserExists()
    {
        using var context = CreateContext(nameof(Delete_ReturnsTrue_WhenUserExists));
        var userId = Guid.NewGuid();
        context.Users.Add(new User { Id = userId, Name = "Alice", Email = "alice@example.com" });
        await context.SaveChangesAsync();

        var service = new UserService(context);
        var result = await service.Delete(userId);

        Assert.True(result);
    }

    [Fact]
    public async Task Delete_RemovesUserFromDatabase()
    {
        using var context = CreateContext(nameof(Delete_RemovesUserFromDatabase));
        var userId = Guid.NewGuid();
        context.Users.Add(new User { Id = userId, Name = "Alice", Email = "alice@example.com" });
        await context.SaveChangesAsync();

        var service = new UserService(context);
        await service.Delete(userId);

        var stored = await context.Users.FindAsync(userId);
        Assert.Null(stored);
    }

    [Fact]
    public async Task Delete_ReturnsFalse_WhenUserNotFound()
    {
        using var context = CreateContext(nameof(Delete_ReturnsFalse_WhenUserNotFound));
        var service = new UserService(context);

        var result = await service.Delete(Guid.NewGuid());

        Assert.False(result);
    }
}
