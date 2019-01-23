function Plug(plugname, onopen, onerror) {
    var This = this;
    this.name = plugname;
    this.active = false;
    this.ws = new WebSocket("ws://localhost:23524/" + this.name);
    this.ws.onopen = function () {
        if (onopen) {
            onopen();
        }
        This.active = true;
    }
    this.ws.onerror = function () {
        if (onerror) {
            onerror();
        }
    }
    this.callback = function () { };
    this.ws.onmessage = function (e) {
        This.callback(JSON.parse(e.data));
    }

    this.Call = function (name, args) {
        if (this.active) {
            var nameLength = name.length.toString();
            nameLength = (nameLength.length == 1 ? "0" : "") + nameLength;

            return new Promise((resolve) => {
                This.callback = resolve;
                this.ws.send(nameLength + name + JSON.stringify(args));
            });
        } else {
            console.error("Plug was called was not yet connected.");
        }
    }
}