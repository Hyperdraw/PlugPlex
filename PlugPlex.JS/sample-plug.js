var SamplePlug = new Plug("SamplePlug");

function print(msg) {
    SamplePlug.Call("Print", {msg: msg});
}