---
title: Writing Plugs
---

# Writing Plugs

Creating you own PlugPlex plugs is actually very easy. All you need is [Visual Studio](https://visualstudio.com) and knowledge of C#.
Start by creating a class library project, and add a reference to PlugPlex.exe and websocket-sharp.dll. Those files can be found in the
location where you installed PlugPlex (if you used the installer, it will be installed in C:\Program Files (x86)\PlugPlex). Create a
new (public) class and make it extend Plug:

```C#
using PlugPlex;

public class MyPlug : Plug{
...
}
```

Now implement the `name` property within the class:

```C#
public override string name {
	get { return "MyPlug"; }
}
```

## Parameters in Plugs

Parameters are passed to functions in plugs with *schemas*. A schema is a class with
public fields for each parameter. For example, for a basic `Print(`*string*`)` function, you would
make a class called PrintSchema and add a public string field. Then you would define the `Print`
function as taking a PrintSchema parameter. For example:

```C#
public void Print(PrintSchema args){
	Console.WriteLine(args.message);
}

...

public class PrintSchema{
	public string message;
}
```

If the function takes multiple parameters, put them all in the scheme, like this:

```C#
public int Add(AddSchema args){
	return args.A + args.B;
}

...

public class AddSchema{
	public int A;
	public int B;
}
```

## Using the Plug

To use your plug, build the project and copy the DLL into the PlugPlex `Plugs` folder.
Go to the folder where you installed PlugPlex (if you used the installer, it will be installed in C:\Program Files (x86)\PlugPlex),
and find the `Plugs` folder. put your plug DLL there. Now start PlugPlex, and you can use you plug in javascript.

## Calling Plugs From Javascript

To call a plug from javascript, you must first establish a connection to the plug. First, include plug-plex.js:

```html
<script src="https://cdn.jsdelivr.net/gh/Hyperdraw/PlugPlex@1.0.0/PlugPlex.JS/plug-plex.js"></script>
```

Now, create a new `Plug` object, passing the name of the plug. This should be the same as the `name` property of the plug class.
The second parameter of the constructor is a callback function, called when the connection is ready.
Do not try to use the plug until the connection is complete and the callback has been called.

```javascript
var myPlug = new Plug("MyPlug", plugLoadedCallback);

function plugLoadedCallback(){
	//Call plug functions...
}
```

To call a plug function, use the plug object's `Call` function. The first parameter is the name of the
function to call. The second parameter is an object to pass as the arguments to the function.
The object should match the functions schema. For example, the AddSchema had two fields - A and B,
so the object would look like this: `{A: 7, B: 4}` to add 7 and 4. Keep in mind that
**calls are asynchronous**, and return values are returned as [Promise objects](http://promisejs.org).
If you want to wait for an asynchronous call, use `await`. Sadly, `await` can only be used in `async` functions, so
make `plugLoadedCallback` `async`. Try this:

```javascript
var myPlug = new Plug("MyPlug", plugLoadedCallback);

async function plugLoadedCallback(){
	// Add two numbers
	var sum = await myPlug.Call("Add", {A: 45, B: 19});
	
	// Print the sum to the browser console
	console.log(sum);

	// Print the sum to the PlugPlex console
	await myPlug.Call("Print", {message: sum.toString()});
}
```


**Tip:**
*You can pass on optional third parameter the the Plug contructor, which is a callback called if the connection failed, such as if the user does not have that plug, or doesn't have PlugPlex running*

Now you can create wrapper functions:

```javascript
async function Add(a, b){
	return (await myPlug.Call("Add", {A: a, B: b}));
}

async function Print(message){
	await myPlug.Call("Print", {message: message});
}
```

And use them like this:

```javascript
async function plugLoadedCallback(){
	await Print(await Add(75, 62));
}
```

So get out there and start making plugs!