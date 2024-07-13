var connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalRServer") // Use the URL of your SignalR hub
    .build();

connection.start().then(function () {
    console.log("SignalR connection established.");
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ChangeStatusCourtDetail", function (name, availableCourt) {
    console.log(name)
    console.log(availableCourt)
    var message = "";
    availableCourt.forEach(function (courtDetail) {
        message += `${name} booked ${courtDetail.slot} ${courtDetail.court.name} \n`;
    });
    showToast(message);
});

connection.on("ChangeCart", function () {
    showToast("Ok");
})

function showToast(message) {
    console.log("Showing toast: " + message);
    var toast = document.createElement("div");
    toast.innerText = message;

    toast.style.position = "fixed";
    toast.style.top = "10px";
    toast.style.right = "10px";
    toast.style.backgroundColor = "#cce5ff";
    toast.style.color = "#004085";
    toast.style.border = "1px solid #b8daff";
    toast.style.padding = "10px";
    toast.style.borderRadius = "5px";
    toast.style.zIndex = "1000";

    document.body.appendChild(toast);
    console.log("Toast appended to body");

    setTimeout(function () {
        document.body.removeChild(toast);
        console.log("Toast removed from body");
    }, 3000);
}
