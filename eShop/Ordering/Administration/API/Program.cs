using eShop.Ordering.Administration.API.Extensions;
using eShop.Ordering.Administration.Service.BuildAllStateViews;
using eShop.Ordering.Administration.Service.BuildStateViewInstance;
using eShop.Ordering.Administration.Service.BuildStateViewInstances;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Service Registrations
builder.Services.Configure(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
//app.Urls.Add("http://*:{port}");

app.MapPostCommand<BuildAllStateViewsCommand>("/stateViews/all");
app.MapPostCommand<BuildStateViewInstancesCommand>("/stateViews");
app.MapPostCommand<BuildStateViewInstanceCommand>("/stateView");

app.Run();
