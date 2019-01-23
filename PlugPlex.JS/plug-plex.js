function Plug(plugname, onopen) {
    var This = this;
    this.name = plugname;
    this.ws = new WebSocket("ws://localhost:23524/" + this.name);
    this.ws.onopen = function () {
        if (onopen) {
            onopen();
        }
    }
    this.ws.onerror = function () {
        console.error("Failed to connect to plug. Maybe there is no plug by that name.");
    }
    this.callback = function () { };
    this.ws.onmessage = function (e) {
        This.callback(JSON.parse(e.data));
    }

    this.Call = function (name, args) {
        var nameLength = name.length.toString();
        nameLength = (nameLength.length == 1 ? "0" : "") + nameLength;

        return new Promise((resolve) => {
            This.callback = resolve;
            this.ws.send(nameLength + name + JSON.stringify(args));
        });
    }
}