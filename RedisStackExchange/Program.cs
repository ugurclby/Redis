using RedisStackExchange.Services;
using StackExchange.Redis;
 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<RedisServices>();



//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//                 ConnectionMultiplexer.Connect(new ConfigurationOptions
//                 {
//                  EndPoints = { $"{builder.Configuration.GetValue<string>("Redis:Host")}:{builder.Configuration.GetValue<int>("Redis:Port")}" },
//                  AbortOnConnectFail = false,
//               })); 
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

app.UseAuthorization();

app.Services.GetService<RedisServices>().Connect();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
