## 🗃️ Troubleshooting

If you encounter issues, check the following points:

### 1. No Messages Received

| **Potential Cause** | **Solution** |
|--|--|
| **Incorrect `EventSource` URL** | Ensure the JavaScript uses a relative path: `new EventSource("/stream")`. |
| **Missing Service Registration** | Confirm `builder.Services.AddSingleton<SseService>();` is in `Program.cs`.|
| **Endpoint Not Mapped** | Confirm `app.MapSseEndpoints();` is called in `Program.cs`.|
| **Response Buffering**| If deploying behind a reverse proxy (Nginx, IIS, Azure), you **must disable buffering** for the `/stream` endpoint in the proxy's configuration. The `X-Accel-Buffering: no` header in the handler is often not enough.|


### 2. Resumption (`Last-Event-ID`) is Null
| **Potential Cause** | **Solution** |
|--|--|
| **Initial Connection** | The header is **always null** on the first connection. This is expected. |
| **No Event IDs Sent** | Ensure your `SseHandler` is sending the `id: {sseEvent.Id}` field for every event. |
| **Testing Procedure** | To test resumption, you must **receive at least one event**, then interrupt the connection (e.g., stop the server), and wait for the browser to automatically reconnect. |