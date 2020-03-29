using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codescreenme
{
  public class HttpContextItemsMiddleware
  {
    private readonly RequestDelegate _next;
    public static readonly object HttpContextItemsMiddlewareKey = new Object();

    public HttpContextItemsMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
      httpContext.Items[HttpContextItemsMiddlewareKey] = "K-9";

      await _next(httpContext);
    }
  }

  public static class HttpContextItemsMiddlewareExtensions
  {
    public static IApplicationBuilder
        UseHttpContextItemsMiddleware(this IApplicationBuilder app)
    {
      return app.UseMiddleware<HttpContextItemsMiddleware>();
    }
  }
}
