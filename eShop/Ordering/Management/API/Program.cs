using eShop.Ordering.Inquiry.StateViews;
using eShop.Ordering.Management.API.Extensions;
using eShop.Ordering.Management.Service.CancelOrder;
using eShop.Ordering.Management.Service.PlaceOrder;
using eShop.Ordering.Management.Service.ReorderStock;
using eShop.Ordering.Management.Service.ReturnOrder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Service Registrations
builder.Services.Configure(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "eShop Order Management API", Version = "v1" });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint
    (
        "/swagger/v1/swagger.json",
        "eOM_API_v1"
    ));
}

app.UseHttpsRedirection();

//var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
//app.Urls.Add("http://*:{port}");


app.MapPostCommand<PlaceOrderCommand>("/orders");
app.MapPostCommand<CancelOrderCommand, OrderStateView>("/orders/cancellation");
app.MapPostCommand<ReturnOrderCommand, OrderStateView>("/orders/return");
app.MapPostCommand<PlaceOrderCommand>("/placeOrderCommand");
app.MapPostCommand<ReorderStockCommand>("/reorderStockCommand");

app.MapPostCommand<CancelOrderCommand, OrderStateView>("/cancelOrderCommand");
app.MapPostCommand<CancelOrderCommand, OrderStateView>("/returnOrderCommand");

app.Run();