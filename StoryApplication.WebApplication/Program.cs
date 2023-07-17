using Microsoft.OpenApi.Models;
using Santander.Collections.Generic;
using Santander.StoryApplication.Story.Query;
using Santander.StoryApplication.Update.Query;
using Santander.StoryApplication.WebApplication.Options;
using Santander.StoryApplication.WebApplication.Story.BackgroundTask;
using System.Reflection;
using System.Runtime.CompilerServices;
using SSA = Santander.StoryApplication;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Santander.StoryApplication.WebApplication.TestProject")]

AppDomain.CurrentDomain.Load(assemblyString: "Santander.StoryApplication");

var webApplicationBuilder = WebApplication.CreateBuilder(args);

var serviceCollection = webApplicationBuilder.Services;
serviceCollection.AddControllers();
serviceCollection
    .AddEndpointsApiExplorer()
    .AddHostedService<PeriodicRefreshBackgroundService>()
    .AddHostedService<PeriodicUpdateBackgroundService>()
    .AddMediatR(msc => msc.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()))
    .AddSingleton<BufferedMemoryCache<uint, SSA.ViewModel.Story>>()
    .AddSingleton<ConcurrentMemoryCache<uint, SSA.ViewModel.Story>>()
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

serviceCollection.AddHttpClient(name: nameof(UpdateHandler), hc => hc.BaseAddress = new Uri(uriString: "https://hacker-news.firebaseio.com/"));
serviceCollection.AddHttpClient(name: nameof(StoryHandler), hc => hc.BaseAddress = new Uri(uriString: "https://hacker-news.firebaseio.com/"));

serviceCollection.AddOptions<PeriodicTimerOptions>(name: nameof(PeriodicRefreshBackgroundService)).BindConfiguration(configSectionPath: "PeriodicRefreshTimerOptions").ValidateOnStart();
serviceCollection.AddOptions<PeriodicTimerOptions>(name: nameof(PeriodicUpdateBackgroundService)).BindConfiguration(configSectionPath: "PeriodicUpdateTimerOptions").ValidateOnStart();

var webApplication = webApplicationBuilder.Build();
if (webApplication.Environment.IsDevelopment()) {
    webApplication.UseSwagger().UseSwaggerUI(suio => {
        suio.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Santander");
    });
}

webApplication.UseHttpsRedirection().UseAuthorization();
webApplication.MapControllers();
webApplication.Run();
