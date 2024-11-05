using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OverblikPlus;
using OverblikPlus.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddHttpClient<ITaskService, TaskService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5101");
});

builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5102");
});

builder.Services.AddAutoMapper(typeof(MappingProfile));

await builder.Build().RunAsync();