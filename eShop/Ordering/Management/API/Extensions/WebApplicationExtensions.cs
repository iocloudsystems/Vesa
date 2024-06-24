using Microsoft.AspNetCore.Mvc;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Management.API.Extensions;

public static class WebApplicationExtensions
{
    public static void MapPostCommand<TCommand>(this WebApplication app, string url)
           where TCommand : ICommand
    {
        app.MapPost(url, async ([FromBody] TCommand command) =>
        {
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var commandHandler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
                var events = await commandHandler.HandleAsync(command, new CancellationToken());
                return Results.Ok(events);
            }
        });
    }

    public static void MapPostCommand<TCommand, TStateView>(this WebApplication app, string url)
       where TCommand : ICommand
       where TStateView : class, IStateView, new()
    {
        app.MapPost(url, async ([FromBody] TCommand command) =>
        {
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var commandHandler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TStateView>>();
                var events = await commandHandler.HandleAsync(command, new CancellationToken());
                return Results.Ok(events);
            }
        });
    }

    public static void MapDeleteCommand<TCommand, TStateView>(this WebApplication app, string url)
        where TCommand : ICommand
        where TStateView : class, IStateView, new()
    {
        app.MapDelete(url, async ([FromBody] TCommand command) =>
        {
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var commandHandler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TStateView>>();
                await commandHandler.HandleAsync(command, new CancellationToken());
            }
        });
    }
}
