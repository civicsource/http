# Archon Web API Framework

> Extension & helper methods that make working with [Asp.Net Web API](http://www.asp.net/web-api) easier.

## How to Use

Install via [nuget](https://www.nuget.org/packages/Archon.WebApi/)

```
install-package Archon.WebApi
```

Make sure to add `using Archon.WebApi;` to the top of your files to get access to any of the following extension methods.

* [The Link Concept](#the-link-concept)
* [A Better Ensure Success](#a-better-ensure-success)
* [Authorization Class](#authorization)
* [Authorize Correctly](#authorize-correctly)
* [Rewrite Authorization Tokens in URL to HTTP Header](#rewrite-authorization-tokens-in-url-to-http-header)
* [Rewrite Accept Parameter in URL to HTTP Accept Header](#rewrite-accept-parameter-in-url-to-http-accept-header)
* [Test Against External HTTP APIs](#test-against-external-http-apis)
* [Convert a csv list querystring argument to an array of values](#csv-array-converter-attributes)

### The Link Concept

The `Link` interface should be used to create .net HTTP API clients. It is inspired by the book _[Designing Evolvable Web APIs with ASP.NET](http://chimera.labs.oreilly.com/books/1234000001708)_. It implements a link relation. Specifically:

> In the web world, media types are used to convey what a resource represents, and a link relation suggests why you should care about that resource.

If you were building an API client for the [Github API](https://developer.github.com/v3/), rather than providing something like this:

```c#
var github = new GithubClient("my-api-key");
var repos = github.GetRepositories("username");
```

`Link` allows you to provide an interface more akin to this:

```c#
var client = new HttpClient();
var repos = await client.SendAsync(new GetGithubRepositories("username"));
```

This uses the native `HttpClient` to do what it is good at, sending HTTP requests while at the same time providing a nice porcelain wrapper around the HTTP particulars. However, if you want to be closer to the metal, `Link` doesn't try to leakily abstract away the fact that you are making an HTTP request. You can also do something like this:

```c#
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

```c#
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

### A Better Ensure Success

The existing `EnsureSuccessStatusCode` is pretty terrible when it comes to throwing a useful exception message. It does not provide any way to access the content of the response after the exception is thrown when usually, the response content has the most useful information as to why the request failed.

This new extension method, `EnsureSuccess`, will return the response content along with the status code and request URL/method in the exception message making your logs much more useful when making use of HTTP APIs.

```c#
//client is an HttpClient and request is an HttpRequestMessage
HttpResponseMessage response = await client.SendAsync(request);
await response.EnsureSuccess();

//if the response failed, it will throw an exception that looks something like this:
//Received HTTP 409 (Conflict) while POSTing URL 'http://example.com/api/thing/'.
//{"Message":"These are not the droids you are looking for."}
```

### Authorization

The `Authorization` class abstracts away parsing an authorization HTTP header. It supports `Bearer` and `Basic` authorization schemes.

```c#
var auth = Authorization.Basic("username", "password");
request.Headers.Authorization = auth.AsHeader();
//base64 encodes the username:password and creates a new AuthenticationHeaderValue
```

```c#
var auth = Authorization.Bearer("my-opaque-auth-token");
request.Headers.Authorization = auth.AsHeader();
//creates a new AuthenticationHeaderValue with the token value
```

### Authorize Correctly

The `401 Unauthorized` response should be reserved for requests that are not authenticated. The `403 Forbidden` response should be used for requests that are correctly authenticated, but do not have access to a particular resource. [See here for a discussion](http://stackoverflow.com/questions/3297048/403-forbidden-vs-401-unauthorized-http-responses) on this.

Unfortunately, the built-in `System.Web.Http.AuthorizeAttribute` always returns a `401` no matter the nuances of the situation. The `AuthorizeCorrectlyMiddleware` makes ASP.Net MVC behave like it should. In `Startup.Configure`:

```c#
//...register all of your other stuff...
app.UseCorrectAuthorization();
app.UseMvc();
```

### Rewrite Authorization Tokens in URL to HTTP Header

If only there was a way to set arbitrary HTTP headers for an `a` tag. You want to create a link to download a CSV file but the API endpoint the link is pointing to requires authentication. Using the `AuthHeaderManipulator`, you can just include the authorization header as a querystring for the link and it will be rewritten to a proper HTTP Authorization header.

```html
<a href="/api/stuff.csv?auth=my-auth-token"></a>
```

```c#
//config is the global HttpConfiguration
config.MessageHandlers.Add(new AuthHeaderManipulator());
```

### Rewrite Accept Parameter in URL to HTTP Accept Header

Configuring the `AcceptHeaderHandler` will rewrite a query string `accept` parameter to a proper `HTTP Accept` header.

```html
<a href="/api/resource/which/normally/returns/json/but/honors/accept/headers?accept=text/csv"></a>
```

```c#
//config is the global HttpConfiguration
config.MessageHandlers.Add(new AcceptHeaderHandler());
```

### Test Against External HTTP APIs

When you are testing some code that calls out to an external HTTP service using the `HttpClient`, you will want to isolate and mock out the external HTTP request. You can use the `FakeHttpHandler` to do that for you.

First, set the fake handler up:

```c#
//create your fake handler
var fake = new FakeHttpHandler();

//configure the HttpClient
var client = new HttpClient(fake);

//register the fake HttpClient in your dependency container of choice
```

Your code under test:

```c#
var response = await client.PostAsJsonAsync("/api/stuff/", new
{
	name = "Homer Simpson",
	location = "Springfield"
});
```

Your test code:

```c#
fake.Action = (req, c) =>
{
	dynamic payload = req.Content.ReadAsAsync<ExpandoObject>().Result;
	//do some asserts on the payload

	//mock out the response you want to send
	return Task.FromResult(req.CreateResponse(HttpStatusCode.BadRequest, new
	{
		message = "These aren't the droids you are looking for."
	}));
};
```

If you don't specify any `Action`, the `FakeHttpHandler` will return an `HTTP 200 (OK)`.

### Log API Exceptions via [log4net](http://logging.apache.org/log4net/)

If you use `log4net` for all of your logging needs, you will want to log unhandled exceptions in your WebAPI project. Register the `Log4netExceptionLogger` (available in the [`Archon.WebApi.Logging`](https://www.nuget.org/packages/Archon.WebApi.Logging/) package) to do just that.

```c#
//config is the global HttpConfiguration
config.Services.Add(typeof(IExceptionLogger), new Log4netExceptionLogger());
```
### CSV Array Converter Attributes

The `CsvArrayConverterAttribute` is an action filter attribute that will take a csv string passed in the querystring and turn it into an array of a given type

```
https://example.com/mystuff/1,2,3
```

```c#
[HttpGet]
[Route("mystuff/{ids}")]
[CsvArrayConverter("ids",typeof(int)]
public HttpResponseMessage DoSomethingWithIds(int[] ids)
{
	foreach(var id in ids)
	{
		//do something	
	}
}
```
