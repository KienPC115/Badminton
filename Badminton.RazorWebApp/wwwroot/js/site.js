var connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalRServer") // Use the URL of your SignalR hub
    .build();

connection.start();

connection.on("ChangeStatusCourtDetail", function () {
    location.reload();
});

connection.on("ChangeCart", function () {
    
})