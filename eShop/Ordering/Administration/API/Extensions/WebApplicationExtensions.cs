using Microsoft.AspNetCore.Mvc;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Administration.API.Extensions;

public static class WebApplicationExtensions
{
    public static void MapGetQuery<TQuery, TStateView>(this WebApplication app, string url)
        where TQuery : IQuery<TStateView>
        where TStateView : class, IStateView, new()
    {
        app.MapGet(url, async (TQuery query) =>
        {
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var queryHandler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TStateView>>();
                var stateView = await queryHandler.HandleAsync(query);
                return Results.Ok(stateView);
            }
        });
    }

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
}
