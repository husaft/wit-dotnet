# wit-dotnet

`wit-dotnet` is the .NET SDK for [Wit.ai](http://wit.ai).

## Install

From source:
```bash
git clone https://github.com/husaft/wit-dotnet
dotnet build
```

## Usage

See the `examples` folder for examples.

## API

### Wit class

The Wit constructor takes the following parameters:
* `accessToken` - the access token of your Wit instance

A minimal example looks like this:

```csharp
using Wit;

var client = new WitClient(accessToken);
client.SendMessage("set an alarm tomorrow at 7am");
```

### Logging

You can also specify a custom logger object in the Wit constructor:
``` csharp
using Wit;
var client = new WitClient(accessToken, customLogger);
```

See the [Extensions Logging module](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging) docs for more information.

## License

The license for this project can be found in LICENSE file in the root directory of this source tree.
