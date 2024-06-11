using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NOS.Engineering.Challenge.API.Extensions;
using NOS.Engineering.Challenge.Database;

var builder = WebApplication.CreateBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureServices(services =>
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        });
    })
    .RegisterServices();

var app = builder.Build();

app.MapControllers();
app.UseSwagger()
   .UseSwaggerUI();

app.Run();
