# Archon HTTP Utilities

> Extensions, middleware, and helper methods that make working with HttpClient and Asp.Net MVC easier as an API.

## How to Use

Install via nuget; [Archon.Http](https://www.nuget.org/packages/Archon.Http/) for HTTP clients,

```powershell
Install-Package Archon.Http
```

or [Archon.AspNetCore](https://www.nuget.org/packages/Archon.AspNetCore/) for ASP.NET Core MVC.

```powershell
Install-Package Archon.AspNetCore
```

If you aren't using ReSharper, make sure to add `using Archon.Http;` to the top of your files to get IntelliSense to detect the extension methods.
___

- [Archon HTTP Utilities](#archon-http-utilities)
	- [How to Use](#how-to-use)
- [Client utilities (`Archon.Http`)](#client-utilities-archonhttp)
	- [The Link Concept](#the-link-concept)
	- [A Better Ensure Success](#a-better-ensure-success)
	- [Query String Builder](#query-string-builder)
	- [Read JSON Response](#read-json-response)
	- [Send Request with JSON Content](#send-request-with-json-content)
	- [Authorization](#authorization)
	- [GZip Request Compression (client)](#gzip-request-compression-client)
- [ASP.NET Core server utilities (`Archon.AspNetCore`)](#aspnet-core-server-utilities-archonaspnetcore)
	- [Rewrite `accept` Parameter in URL to HTTP Accept Header](#rewrite-accept-parameter-in-url-to-http-accept-header)
	- [Rewrite `auth` Parameter in URL to HTTP Authorization Header](#rewrite-auth-parameter-in-url-to-http-authorization-header)
	- [Bind CSV Values to Routes](#bind-csv-values-to-routes)
	- [Use JSON Exception Handling](#use-json-exception-handling)
	- [GZip Request Decompression (server)](#gzip-request-decompression-server)

# Client utilities (`Archon.Http`)

## The Link Concept

The `Link` interface should be used to create .net HTTP API clients. It is inspired by the book _[Designing Evolvable Web APIs with ASP.NET](http://chimera.labs.oreilly.com/books/1234000001708)_. It implements a link relation. Specifically:

> In the web world, media types are used to convey what a resource represents, and a link relation suggests why you should care about that resource.

If you were building an API client for the [Github API](https://developer.github.com/v3/), rather than providing something like this:

```csharp
var github = new GithubClient("my-api-key");
var repos = github.GetRepositories("username");
```

`Link` allows you to provide an interface more akin to this:

```csharp
var client = new HttpClient();
var repos = await client.SendAsync(new GetGithubRepositories("username"));
```

This uses the native `HttpClient` to do what it is good at, sending HTTP requests while at the same time providing a nice porcelain wrapper around the HTTP particulars. However, if you want to be closer to the metal, `Link` doesn't try to leakily abstract away the fact that you are making an HTTP request. You can also do something like this:

```csharp
var client = new HttpClient();
var link = new GetGithubRepositories("username");

HttpResponseMessage response = await api.SendAsync(link.CreateRequest());

if (response.StatusCode == HttpStatusCode.Redirect)
{
  //do something cool
}

var repos = await link.ParseResponseAsync(response);
```

Internally, the `HttpClient.SendAsync(Link)` method is calling `CreateRequest` and `ParseResponseAsync` for your convenience.

A sample implementation of `GetGithubRepositories` could look something like this:

```csharp
public class GetGithubRepositories : Link<IEnumerable<Repo>>
{
	public string Username { get; private set; }

	public GetGithubRepositories(string username)
	{
		this.Username = username;
	}

	public HttpRequestMessage CreateRequest()
	{
		//the client being used to send this request should have a BaseAddress configured
		//this allows us to use relative URLs when building our HttpRequestMessage
		return new HttpRequestMessage(HttpMethod.Get, String.Format("repositories/{0}", Username));
	}

	public async Task<IEnumerable<Repo>> ParseResponseAsync(HttpResponseMessage response)
	{
		if (response == null)
			throw new ArgumentNullException("response");

		if (response.StatusCode == HttpStatusCode.NotFound)
			return null; //return a null object if we get a not-found response

		//see below for this method
		await response.EnsureSuccess(); //throw an error on any other non-200 response

		//parse the response body into our object we want to return
		return await response.Content.ReadAsAsync<IEnumerable<Repo>>();
	}
}
```

Read [Chapter 9: Building the Client](http://chimera.labs.oreilly.com/books/1234000001708/ch09.html) for free online for more information on the advantages of building client libraries like this.

## A Better Ensure Success

The existing `EnsureSuccessStatusCode` is pretty terrible when it comes to throwing a useful exception message. It does not provide any way to access the content of the response after the exception is thrown when usually, the response content has the most useful information as to why the request failed.

This new extension method, `EnsureSuccess`, will return the response content along with the status code and request URL/method in the exception message making your logs much more useful when making use of HTTP APIs.

```csharp
//client is an HttpClient and request is an HttpRequestMessage
HttpResponseMessage response = await client.SendAsync(request);
await response.EnsureSuccess();

//if the response failed, it will throw an exception that looks something like this:
//Received HTTP 409 (Conflict) while POSTing URL 'http://example.com/api/thing/'.
//{"Message":"These are not the droids you are looking for."}
```

## Query String Builder

This class allows you to easily create query strings from multiple parameters:

```csharp
var qs = new QueryStringBuilder();
qs.Append("hello", "world");
qs.Append("goodbye", "loneliness");

Console.WriteLine(qs.ToString()); // ?hello=world&goodbye=loneliness
```

## Read JSON Response

This is an extension method off of `HttpContent` that makes it easy to read a JSON response:

```csharp
HttpResponseMessage response = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://dogs.example.com/api"));
var dogs = await response.Content.ReadJsonAsync<IEnumerable<Dog>>();
```

## Send Request with JSON Content

This is another extension method that makes it easy to send requests with JSON content:

```csharp
var req = new HttpRequestMessage(HttpMethod.Post, "https://dogs.example.com/api")
	.WithJsonContent(new { name = "Fido", Age = 2 });

var response = await client.SendAsync(req);
```

## Authorization

The `Authorization` class abstracts away parsing an authorization HTTP header. It supports `Bearer` and `Basic` authorization schemes.

```csharp
var auth = Authorization.Basic("username", "password");
request.Headers.Authorization = auth.AsHeader();
//base64 encodes the username:password and creates a new AuthenticationHeaderValue
```

```csharp
var auth = Authorization.Bearer("my-opaque-auth-token");
request.Headers.Authorization = auth.AsHeader();
//creates a new AuthenticationHeaderValue with the token value
```
## GZip Request Compression (client)

If you need to make API calls to pass big-ish chunks of data around all at once, compressing the request payload with GZip can improve performance considerably.

On the client side, you'll need to add an instance of `GZipCompressingHandler` as a delegating handler to the main `HttpClientHandler`. This will compress the request content as well as adding a `Content-Encoding: gzip` header to signify to the server that the request body is compressed.

```csharp
MailhouseApi api = MailhouseApi.Build("http://localhost:50128/",
	() => new GZipCompressingHandler(
			new BasicAuthenticationHandler(new HttpClientHandler(), Username, Password), // Modifies headers only
			HttpMethod.Post, // POST and PUT typically contain payloads worth compressing,
			HttpMethod.Put   // while other verbs do not.
		)
	);
```

> **Note:**  
> If you are sending gzipped requests to an Azure App Service, then you won't be able to configure its IIS gateway to dynamically decompress the request, and you'll need to add a `GZipResourceFilter` [as described below](#gzip-request-decompression-server).

> **Caution:**  
> The `GZipCompressingHandler` must be the last handler to modify the request *content*, because downstream handlers will only see compressed content. Downstream handlers can still modify headers, however.


# ASP.NET Core server utilities (`Archon.AspNetCore`)

## Rewrite `accept` Parameter in URL to HTTP `Accept` Header

Adding the `AcceptHeaderMiddleware` will rewrite a query string `accept` parameter to a proper HTTP `Accept` header.

```html
<a href="/api/resource/which/normally/returns/json/but/honors/accept/headers?accept=text/csv">Download as CSV</a>
```

To use, add `app.UseAcceptHeaderRewriter()` to your `Startup.Configure`:

```csharp
// ...register anything else that might affect the request...
app.UseAcceptHeaderRewriter();
app.UseMvc();
```

## Rewrite `auth` Parameter in URL to HTTP `Authorization` Header

Adding the `AuthHeaderMiddleware` will rewrite a query string `auth` parameter to a proper HTTP `Authorization` header.

```html
<a href="/api/resource/which/requires/sso/auth/on/a/different/domain?auth=SomeBase64String">Fetch authenticated resource</a>
```

To use, add `app.UseAuthHeaderRewriter()` to your `Startup.Configure`:

```csharp
app.UseAuthHeaderRewriter();
// ...register anything else that might affect the request...
app.UseMvc();
```

## Bind CSV Values to Routes

The `CsvModelBinder` is a model binder that will take a csv string passed as a query string, route parameter, or request body and turn it into an array of a given type.

```
https://example.com/mystuff/1,2,3
```

```csharp
[HttpGet]
[Route("mystuff/{ids}")]
public HttpResponseMessage DoSomethingWithIds(int[] ids /* or IEnumerable<int> ids */)
{
	foreach (var id in ids)
	{
		//do something	
	}
}
```

Register it in `Startup.ConfigureServices`:

```csharp
services.AddMvc().AddMvcOptions(opts =>
{
	opts.ModelBinders.Add(new CsvModelBinder());
});
```

## Use JSON Exception Handling

If you are writing an API and want unhandled exceptions handled and returned to the client in a nice JSON way, you'll want to use this middleware. In your `Startup.Configure`, call this before everything else, so that it gets first-dibs at answering when an exception goes unhandled:

```csharp
app.UseJsonExceptionHandling();
// ...literally everything else...
```

This will handle exceptions in a similar way to how ASP.NET Web API used to handle them.

## GZip Request Decompression (server)

ASP.NET Core, on its own, won't handle compressed request content. IIS can do this, but Azure App Services don't provide configurability for the IIS gateway, so instead, the request decompression must be handled by the ASP.NET application. This is simple; just add the `GZipResourceFilter` in the Startup object's `services.AddMvc()` call:

```csharp
public void ConfigureServices(IServiceCollection services)
{
	// ...
	services.AddMvc(opts => opts.Filters.Add(typeof(GZipResourceFilter)))
		.AddJsonOptions(opts => JsonSettings.Configure(opts.SerializerSettings))
		.AddControllersAsServices(); // etc
	// ...
}
```

You can confirm this works using [Fiddler](https://www.telerik.com/download/fiddler/fiddler4) to inspect the request body.

> **Caution:**  
> The `Content-Encoding:` header supports multiple values to signify that multiple encodings were applied in a particular order, but `GZipResourceFilter` does not handle this. If `GZipResourceFilter` receives such a request, it will barf.
