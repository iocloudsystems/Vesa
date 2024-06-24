using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using vesa.Core.Abstractions;
using vesa.Core.Helpers;
using vesa.Core.Infrastructure;
using ErrorEventArgs = TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs;

namespace vesa.SQL.Infrastructure;

public class SQLEventStoreListener : IEventStoreListener
{
    private CancellationToken _cancellationToken;
    private readonly string _connection;
    private readonly IEventProcessor _eventProcessor;
    private readonly ILogger<SQLEventStoreListener> _logger;
    private SqlTableDependency<EventJson> _eventJsonChangeFeed;

    public SQLEventStoreListener
    (
        string connection,
        IEventProcessor eventProcessor,
        ILogger<SQLEventStoreListener> logger
    )
    {
        _connection = connection;
        _eventProcessor = eventProcessor;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        var _eventJsonChangeFeed = new SqlTableDependency<EventJson>(_connection);
        _eventJsonChangeFeed.OnChanged += Changed;
        _eventJsonChangeFeed.Start();
    }

    public async Task StopAsync()
    {
        _eventJsonChangeFeed.Stop();
    }

    private void Changed(object sender, RecordChangedEventArgs<EventJson> e)
    {
        if (_cancellationToken.IsCancellationRequested)
        {
            StopAsync().ConfigureAwait(false);
        }
        if (e.ChangeType == ChangeType.Insert)
        {
            var newEventJson = e.Entity;
            var newEvent = (IEvent)JsonConvert.DeserializeObject(newEventJson.Json, TypeHelper.GetType(newEventJson.EventTypeName));
            _eventProcessor.ProcessAsync(newEvent, _cancellationToken).GetAwaiter().GetResult();
        }
    }

    private static void OnError(object sender, ErrorEventArgs e)
    {
        Console.WriteLine(e.Message);
        Console.WriteLine(e.Error?.InnerException?.Message);
    }
}