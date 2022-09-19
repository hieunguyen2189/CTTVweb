using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
namespace CTTVWebsite.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class MyMiddleware
    {
        private readonly RequestDelegate _next;

        public MyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {


            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            body = HttpUtility.UrlDecode(body);
            var regex = @"^[a-zA-Z0-9_$\-=&\\ \/\:\*,\r\n]*$";
            Regex regx = new Regex(regex);
            if (!regx.IsMatch(body) && body != "")
            {
                return;
                context.Items.Add("myparam", "test");
                // does not match
            }
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            await _next(context);


        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MyMiddlewareExtensions
    {
        public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyMiddleware>();
        }
    }
}
