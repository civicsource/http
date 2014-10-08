# Archon Web API Framework

> Extension & helper methods that make working with [Asp.Net Web API](http://www.asp.net/web-api) easier.

## How to Use

Install via [nuget](https://www.nuget.org/packages/Archon.WebApi/)

```
install-package Archon.WebApi
```

Make sure to add `using Archon.WebApi;` to the top of your files to get access to any of the following extension methods:

### HttpResponseMessage.EnsureSuccess

The existing `EnsureSuccessStatusCode` is pretty terrible when it comes to throwing a useful exception message. It does not provide any way to access the content of the response after the exception is thrown when usually, the response content has the most usuful information as to why the request failed.

This new extension method, `EnsureSuccess`, will return the response content along with the status code and request URL/method in the exception message making your logs much more useful when making use of HTTP APIs.

```cs
//client is an HttpClient and request is an HttpRequestMessage
HttpResponseMessage response = await client.SendAsync(request);
await response.EnsureSuccess();

//if the response failed, it will throw an exception that looks something like this:
//Received HTTP 409 (Conflict) while POSTing URL 'http://example.com/api/thing/'.
//{"Message":"These are not the droids you are looking for."}
```
