document.addEventListener("DOMContentLoaded", () => {
    var eventSource = new EventSource("/stream");
    debugger;
    eventSource.onmessage = (event) => {
        var log = document.getElementById("log");
        log.innerHTML += `<p>${event.data}</p>`;
    };

    eventSource.onerror = () => {
        console.log("SSE connection lost. Retrying...");
    }; 
});