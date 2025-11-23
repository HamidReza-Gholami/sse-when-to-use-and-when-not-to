# 🚀 Server-Sent Events (SSE) Demonstration

This project implements a robust and resilient Server-Sent Events (SSE) solution in ASP.NET Core using **Channels** for high-performance, decoupled communication, and supports **automatic reconnection with message resumption** via the `Last-Event-ID` mechanism.

## 🌟 Key Features

* **Asynchronous Streaming:** Utilizes `IAsyncEnumerable<T>` and `Channel<T>` for efficient, non-blocking data streaming.
* **Automatic Resumption:** The server tracks recent events (ID and Data) and uses the client-sent `Last-Event-ID` header to replay missed messages after a connection drop.
* **Singleton Service:** The central `SseService` is registered as a Singleton, ensuring a single, shared communication channel for all connected clients.
* **Clean Disconnection:** Uses the `CancellationToken` to detect client disconnections and gracefully exit the streaming loop, preventing resource leaks.

---

## 📁 Project Structure

This project is organized into distinct layers for clean separation of concerns:

| Folder/File | Description |
| :--- | :--- |
| `SSE.Backend.Services` | Contains the core business logic, primarily the **`SseService.cs`** that manages the message Channel and event history. |
| `SSE.Client.Handlers` | Contains the **`SseHandler.cs`** responsible for handling the HTTP request, setting SSE headers, reading `Last-Event-ID`, and streaming events. |
| `SSE.Client.Features` | Contains the **`SseEndpoints.cs`** extension method for mapping the SSE stream and notification POST endpoints. |
| `Program.cs` | Registers the **`SseService` as a Singleton** and maps all endpoints. |
| `wwwroot/js/site.js` | The client-side JavaScript that uses the native **`EventSource`** API. |
| `Pages/Index.cshtml` | The simple client view that displays the received messages. |

---

## 🛠️ Getting Started

### 1. Configure Services (`Program.cs`)

Ensure your central message service is registered as a **singleton** so all clients share the same channel instance:

```csharp
// Program.cs

// 1. Register the core SSE service
builder.Services.AddSseBackend();

// ... other setup ...

var app = builder.Build();

// 2. Map the endpoints defined in SseEndpoints.cs
app.MapSseEndpoints();

app.Run();

### 2. The Core Service (`SseService.cs`)

This service manages the event history for resumption and the real-time channel.

```

```csharp
public record SseEvent(long Id, string Data);

public class SseService
{
    // ... (Implementation with Channel and _recentEvents list)
}

```

### 3. Client Connection (`site.js`)

The client automatically handles reconnection and message ID tracking. Note the use of the **relative URL** `/stream`.

```javascript
document.addEventListener("DOMContentLoaded", () => {
    // Connect to the stream endpoint
    var eventSource = new EventSource("/stream");
    
    eventSource.onmessage = (event) => {
        var log = document.getElementById("log");
        // Append the received data
        log.innerHTML += `<p>${event.data}</p>`;
    };

    eventSource.onerror = () => {
        console.log("SSE connection lost. Retrying...");
        // The EventSource object handles the reconnection automatically
    }; 
});
```