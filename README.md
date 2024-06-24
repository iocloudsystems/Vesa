| Component                | Description                                                                                                        |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| Command                  | A request to initiate a particular business process. Can be invoked by an HTTP Request with the body binding to    |
|                          | Command properties or created by an automated service processor such as a Service Bus.                             |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| CommandHandler           | Handles the command by orchestrating business domain processing and event persistence.                             |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| IDomain                  | Interface for performing business validation and enforcing business rules and logic and if successful, creates a   |
|                          | domain event for storage.                                                                                          |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| Event                    | A domain event that happened as the result of command execution to a business process or an integration event to   |
|                          | share domain changes with other services via a message hub.                                                        |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| IEventHandler            | Interface for performing additional business or system actions in reaction to an event that occurred.              |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| EventPropagationHandler  | Creates and stores events that are duplicates of the original domain event but with a Subject property matching    |
|                          | the Subject property of all interested StateViews. EventListenerWorkers will apply these events to all interested  |
|                          | StateViews and update their current state.                                                                         |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| EventPublicationHandler  | Handles an event by publishing it to a message hub for consumption by other services.                              |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| StateView                | A specific view of the state of the system assembled from the data (payload) extracted from a sequence of events   |
|                          | arranged in the order of occurrence. Can be used for enforcing business rules or as a data source for a UI screen  |
|                          | or report.                                                                                                         |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| Query                    | A request for information in the form of a StateView.                                                              |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| QueryHandler             | Handles the Query by returning a StateView from the StateViewStore.                                                |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| StateViewUpdater         | Observes events that are added in the event store and updates its state based on the data and context of the added |
|                          | event.                                                                                                             |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| EventListenerWorker      | A background service that engages a collection of EventListeners to continually listen for events.                 |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| IEventListener           | Listens for events added to an event store or published to a message hub and engages an EventProcessor to process  |
|                          | the command. If listening to a message hub, it engages an EventConsumer to consume the event from the message hub. |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| EventConsumer            | Consumes integration events from a message hub and unpacks them for processing.                                    |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| EventProcessor           | Contains a collection of event observers and notifies them of incoming events.                                     |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| EventHandlerObservers    | Contains a collection of event handlers that handle an event when notified that an event was added to the          |
|                          | EventStore or published to a message hub.                                                                          |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| IEventStore              | Interface for Event storage.                                                                                       |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| IStateViewStore          | Interface for StateView storage.                                                                                   |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
| IEventPublisher          | Interface for Event publication.                                                                                   |
|--------------------------|--------------------------------------------------------------------------------------------------------------------|
