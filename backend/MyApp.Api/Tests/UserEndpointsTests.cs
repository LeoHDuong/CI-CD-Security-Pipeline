using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Api.Data;
using MyApp.Api.DTOs;
using MyApp.Api.Models;
using Xunit;

namespace MyApp.Api.Tests;

public class UserEndpointsTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UserEndpointsTests()
    {
        var dbName = "EndpointTestDb_" + Guid.NewGuid();
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(opts =>
                        opts.UseInMemoryDatabase(dbName));
                });
            });
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private async Task SeedUser(User user)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetUsers_ReturnsOkWithList()
    {
        var response = await _client.GetAsync("/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var users = await response.Content.ReadFromJsonAsync<List<User>>();
        Assert.NotNull(users);
    }

    [Fact]
    public async Task GetUser_ReturnsOk_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        await SeedUser(new User { Id = userId, Name = "Alice", Email = "alice@example.com" });

        var response = await _client.GetAsync($"/users/{userId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(user);
        Assert.Equal("Alice", user.Name);
        Assert.Equal("alice@example.com", user.Email);
    }

    [Fact]
    public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var response = await _client.GetAsync($"/users/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreatedWithUser()
    {
        var dto = new UserCreateDto("Alice", "alice@example.com");

        var response = await _client.PostAsJsonAsync("/users", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(user);
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("Alice", user.Name);
        Assert.Equal("alice@example.com", user.Email);
    }

    [Fact]
    public async Task CreateUser_SetsLocationHeader()
    {
        var dto = new UserCreateDto("Alice", "alice@example.com");

        var response = await _client.PostAsJsonAsync("/users", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task UpdateUser_ReturnsOkWithUpdatedUser_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        await SeedUser(new User { Id = userId, Name = "Alice", Email = "alice@example.com" });

        var dto = new UserUpdateDto("Alice Updated", "alice.updated@example.com");
        var response = await _client.PutAsJsonAsync($"/users/{userId}", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(user);
        Assert.Equal("Alice Updated", user.Name);
        Assert.Equal("alice.updated@example.com", user.Email);
    }

    [Fact]
    public async Task UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var dto = new UserUpdateDto("Alice", "alice@example.com");

        var response = await _client.PutAsJsonAsync($"/users/{Guid.NewGuid()}", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        await SeedUser(new User { Id = userId, Name = "Alice", Email = "alice@example.com" });

        var response = await _client.DeleteAsync($"/users/{userId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var response = await _client.DeleteAsync($"/users/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
