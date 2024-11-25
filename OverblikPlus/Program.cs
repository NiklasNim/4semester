using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OverblikPlus;
using OverblikPlus.Services;
using OverblikPlus.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddSingleton<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri("https://overblikplus-user-api.azurewebsites.net");
}).AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

builder.Services.AddScoped<JwtAuthorizationMessageHandler>(provider =>
    new JwtAuthorizationMessageHandler(provider.GetRequiredService<CustomAuthStateProvider>())
        .ConfigureHandler(authorizedUrls: new[]
        {
            "https://overblikplus-user-api.azurewebsites.net",
            "https://overblikplus-task-api.azurewebsites.net"
        }));

builder.Services.AddHttpClient<ITaskService, TaskService>(client =>
{
    client.BaseAddress = new Uri("https://overblikplus-task-api.azurewebsites.net");
}).AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

builder.Services.AddHttpClient<ITaskStepService, TaskStepService>(client =>
{
    client.BaseAddress = new Uri("https://overblikplus-task-api.azurewebsites.net");
}).AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri("https://overblikplus-user-api.azurewebsites.net");
}).AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();

