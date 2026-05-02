[![Tests](https://github.com/ZemMezzer/EventBus/workflows/Tests/badge.svg)](https://github.com/ZemMezzer/EventBus/actions) [![Releases](https://img.shields.io/github/release/ZemMezzer/EventBus.svg)](https://github.com/ZemMezzer/EventBus/releases)

# EventBus

Lightweight, type-safe event bus for C# built on top of [R3](https://github.com/Cysharp/R3) reactive extensions. Supports global broadcasting and isolated named channels.

## Installation

Add a reference to `TiredSiren.EventBus` in your project.

**Dependencies:**
- [R3](https://github.com/Cysharp/R3)

## Quick Start

```csharp
using R3;
using TiredSiren.EventBus;

// 1. Define your event — must implement IEvent
public class PlayerDiedEvent : IEvent
{
    public string PlayerName { get; set; }
}

// 2. Create an event bus instance
var eventBus = new EventBus();

// 3. Subscribe globally
eventBus.Observe<PlayerDiedEvent>().Subscribe(ev =>
{
    Console.WriteLine($"{ev.PlayerName} died");
});

// 4. Broadcast globally
eventBus.Broadcast(new PlayerDiedEvent { PlayerName = "Hero" });
// => "Hero died"
```

## API

### `IEvent`

All events must implement the `IEvent` marker interface.

```csharp
public interface IEvent {}
```

```csharp
public class PlayerDiedEvent : IEvent
{
    public string PlayerName { get; set; }
}
```

The interface carries no members — it exists purely as a compile-time constraint so `Observe<T>`, `Broadcast<T>` and `Publish<T>` only accept valid event types.

---

### `IEventChannel`

A marker interface used to define named channel types. Channels are identified by their type, not by instances.

```csharp
public interface IEventChannel {}
```

```csharp
public class UIChannel : IEventChannel {}
public class AudioChannel : IEventChannel {}
```

---

### `IEventBus`

The public contract of the event bus.

```csharp
public interface IEventBus
{
    EventChannel GetChannel<TChannel>() where TChannel : IEventChannel;
    Observable<TEvent> Observe<TEvent>() where TEvent : IEvent;
    void Broadcast<TEvent>(TEvent ev) where TEvent : IEvent;
}
```

Program against `IEventBus` rather than the concrete `EventBus` class for easier testing and dependency injection.

---

### `EventBus`

The default implementation of `IEventBus`. Create one per scope (app, scene, module, etc.).

---

### `Observe<TEvent>() where TEvent : IEvent`

Subscribes to events on the **global** (default) channel.

```csharp
Observable<TEvent> Observe<TEvent>() where TEvent : IEvent;
```

Use standard R3 operators to filter, transform, or throttle events before subscribing.

```csharp
eventBus.Observe<PlayerDiedEvent>()
    .Where(ev => ev.PlayerName != null)
    .Subscribe(ev => Console.WriteLine(ev.PlayerName));
```

---

### `Broadcast<TEvent>(TEvent ev) where TEvent : IEvent`

Publishes an event to the **global channel** and to **all named channels** simultaneously.

```csharp
void Broadcast<TEvent>(TEvent ev) where TEvent : IEvent;
```

```csharp
eventBus.Broadcast(new PlayerDiedEvent { PlayerName = "Hero" });
// Received by: global subscribers + all existing channel subscribers of that event type
```

---

### `GetChannel<TChannel>() where TChannel : IEventChannel`

Returns an `EventChannel` identified by `TChannel`. Channels are created lazily on first access and are fully isolated from each other and from the global channel.

```csharp
EventChannel GetChannel<TChannel>() where TChannel : IEventChannel;
```

```csharp
var uiChannel = eventBus.GetChannel<UIChannel>();
uiChannel.Observe<PlayerDiedEvent>().Subscribe(ev => UpdateUI(ev));
uiChannel.Publish(new PlayerDiedEvent { PlayerName = "Hero" });
```

---

### `EventChannel`

Returned by `GetChannel<T>()`. Has its own subscriber registry, isolated from other channels.

#### `Observe<TEvent>()`

```csharp
Observable<TEvent> Observe<TEvent>() where TEvent : IEvent;
```

Subscribes to events of type `TEvent` on this channel. When the last subscriber for a given event type disposes, the internal entry is removed automatically.

#### `Publish<TEvent>(TEvent ev)`

```csharp
void Publish<TEvent>(TEvent ev) where TEvent : IEvent;
```

Delivers the event to all active subscribers on this channel. Iterates over a snapshot of the subscriber list, so mid-dispatch disposals are safe.

---

### Disposing Subscriptions

`Subscribe()` returns an `IDisposable`. Call `.Dispose()` to unsubscribe. When the last subscriber for a given event type is disposed, the internal list is cleaned up automatically.

```csharp
var subscription = eventBus.Observe<PlayerDiedEvent>().Subscribe(ev => { ... });

// Later:
subscription.Dispose();
```

## Patterns

### Global vs Channel

Use `Observe` + `Broadcast` for app-wide events. Use `GetChannel<T>` + `Publish` for scoped, isolated communication.

```csharp
// Global — reaches all subscribers everywhere
eventBus.Broadcast(new AppPausedEvent());

// Scoped — only reaches subscribers of this specific channel
eventBus.GetChannel<UIChannel>().Publish(new ButtonClickedEvent());
```

> **Note:** `Broadcast` fans out to all existing named channels as well, so channel subscribers will also receive broadcasted events.

### Multiple Subscribers

Any number of handlers can subscribe to the same event type independently.

```csharp
eventBus.Observe<MyEvent>().Subscribe(ev => HandlerA(ev));
eventBus.Observe<MyEvent>().Subscribe(ev => HandlerB(ev));
eventBus.Observe<MyEvent>().Subscribe(ev => HandlerC(ev));

eventBus.Broadcast(new MyEvent { Data = "ping" });
// HandlerA, HandlerB, HandlerC all called
```

### Selective Unsubscription

Dispose individual subscriptions without affecting others.

```csharp
var subA = eventBus.Observe<MyEvent>().Subscribe(ev => HandlerA(ev));
var subB = eventBus.Observe<MyEvent>().Subscribe(ev => HandlerB(ev));

subA.Dispose();

eventBus.Broadcast(new MyEvent { Data = "ping" });
// Only HandlerB is called
```

### Lifetime Management with DisposableBag

Use R3's `DisposableBag` to tie subscriptions to object lifetimes.

```csharp
public class UIManager : IDisposable
{
    private readonly DisposableBag _bag = new();

    public UIManager(IEventBus eventBus)
    {
        eventBus.GetChannel<UIChannel>()
            .Observe<PlayerDiedEvent>()
            .Subscribe(OnPlayerDied)
            .AddTo(_bag);
    }

    private void OnPlayerDied(PlayerDiedEvent ev) { ... }

    public void Dispose() => _bag.Dispose();
}
```

## License

See [LICENSE](./LICENSE) for details.
