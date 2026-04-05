using MyApp.Api.DTOs;
using MyApp.Api.Services;
using Microsoft.EntityFrameworkCore;
using MyApp.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UserService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UserService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/users", async (UserService service) =>
{
    return Results.Ok(await service.GetAll());
});

app.MapGet("/users/{id}", async (Guid id, UserService service) =>
{
    var user = await service.GetById(id);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapPost("/users", async (UserCreateDto input, UserService service) =>
{
    var user = await service.Create(input.Name, input.Email);
    return Results.Created($"/users/{user.Id}", user);
});

app.MapPut("/users/{id}", async (Guid id, UserUpdateDto input, UserService service) =>
{
    var user = await service.Update(id, input.Name, input.Email);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapDelete("/users/{id}", async (Guid id, UserService service) =>
{
    var success = await service.Delete(id);
    return success ? Results.NoContent() : Results.NotFound();
});


app.Run();

public partial class Program { }