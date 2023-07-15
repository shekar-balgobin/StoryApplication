using Microsoft.OpenApi.Models;
using Santander.Collections.Generic;
using Santander.StoryApplication.WebApplication.Story.BackgroundTask;
using System.Reflection;
using SSA = Santander.StoryApplication;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

var serviceCollection = webApplicationBuilder.Services;
serviceCollection.AddControllers();
serviceCollection
    .AddEndpointsApiExplorer()
    .AddHostedService<PeriodicRefreshBackgroundService>()
    .AddHostedService<PeriodicUpdateBackgroundService>()
    .AddSingleton<BufferedMemoryCache<uint, SSA.ViewModel.Story>>()
    .AddSwaggerGen(sgo => {
        sgo.SwaggerDoc(name: "v1", new OpenApiInfo {
            Contact = new OpenApiContact {
                Email = "shekar.balgobin@gmail.com",
                Name = "Shekar Balgobin",
            },
            Description = "An API to get the details of the 'best stories' from the Hacker News API",
            Title = "Developer Coding Test",
            Version = "v1"
        });

        sgo.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    });

var webApplication = webApplicationBuilder.Build();
if (webApplication.Environment.IsDevelopment()) {
    webApplication.UseSwagger().UseSwaggerUI(suio => {
        suio.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Santander");
    });
}

webApplication.UseHttpsRedirection().UseAuthorization();
webApplication.MapControllers();
webApplication.Run();
