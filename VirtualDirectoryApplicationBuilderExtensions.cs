using Microsoft.AspNetCore.Builder;
using System.Linq;

namespace AspNetCore.VirtualDirectoryHelpers
{
    public static class VirtualDirectoryApplicationBuilderExtensions
    {
        /// <summary>
        /// Run the application in a virtual directory
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="virtualDirectory">The virtual directory the app is running under</param>
        public static void UseVirtualDirectory(this IApplicationBuilder app, string virtualDirectory)
        {
            app.Use(async (context, next) =>
            {
                context.Request.PathBase = virtualDirectory;
                await next();
            });
        }

        /// <summary>
        /// Run the application in a virtual directory
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="header">Name of the header that indicates the virtual directory, the default value is "X-Virtual-Directory"</param>
        /// <remarks>
        /// This is useful for running an app in a virtual directory behind a reverse proxy such as NGINX. 
        /// The reverse proxy can pass the application a header to indicate the virtual directory. By default the header will be "X-Virtual-Directory"
        /// </remarks>
        public static void UseVirtualDirectoryFromHeader(this IApplicationBuilder app, string header = "X-Virtual-Directory")
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Headers.ContainsKey(header))
                {
                    context.Request.PathBase = context.Request.Headers[header].FirstOrDefault();
                }
                await next();
            });
        }
    }
}
