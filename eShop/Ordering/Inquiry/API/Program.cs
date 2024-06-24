using eShop.Ordering.Inquiry.API.Extensions;
using eShop.Ordering.Inquiry.Service.GetCustomerOrders;
using eShop.Ordering.Inquiry.Service.GetDailyOrders;
using eShop.Ordering.Inquiry.Service.GetOrder;
using eShop.Ordering.Inquiry.Service.GetStatusOrders;
using eShop.Ordering.Inquiry.StateViews;

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

app.MapGetQuery<GetCustomerOrdersQuery, CustomerOrdersStateView>("/customers/{customerNumber}/orders");
app.MapGetQuery<GetOrderQuery, OrderStateView>("/orders/{orderNumber}");
app.MapGetQuery<GetStatusOrdersQuery, StatusOrdersStateView>("/orders/status/{orderStatus}");
app.MapGetQuery<GetDailyOrdersQuery, DailyOrdersStateView>("/orders/{stateViewDate}");

app.Run();
