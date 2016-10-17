using System;
using Microsoft.AspNetCore.Builder;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.VirtualDirectoryHelpers
{
    public static class VirtualDirectoryApplicationBuilderExtensions
    {
        /// <summary>
        /// Run the application in a virtual directory
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="virtualDirectory">The virtual directory the app is running under</param>
        /// <param name="rewritePath">Indicates whether to rewrite the path if the request contains the virtual directory</param>
        public static void UseVirtualDirectory(this IApplicationBuilder app, string virtualDirectory, bool rewritePath = true)
        {
            app.Use(async (context, next) =>
            {
                PathString remaining;

                if (rewritePath &&
                    !string.IsNullOrEmpty(virtualDirectory) &&
                    context.Request.Path.StartsWithSegments(virtualDirectory, StringComparison.OrdinalIgnoreCase, out remaining))
                {
                    context.Request.Path = remaining;
                }
                context.Request.PathBase = virtualDirectory;

                await next();
            });
        }

        /// <summary>
        /// Run the application in a virtual directory
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="header">Name of the header that indicates the virtual directory, the default value is "X-Virtual-Directory"</param>
        /// <param name="rewritePath">Indicates whether to rewrite the path if the request contains the virtual directory</param>
        /// <remarks>
        /// This is useful for running an app in a virtual directory behind a reverse proxy such as NGINX. 
        /// The reverse proxy can pass the application a header to indicate the virtual directory. By default the header will be "X-Virtual-Directory"
        /// </remarks>
        public static void UseVirtualDirectoryFromHeader(this IApplicationBuilder app, string header = "X-Virtual-Directory", bool rewritePath = true)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Headers.ContainsKey(header))
                {
                    var virtualDirectory = context.Request.Headers[header].FirstOrDefault();

                    PathString remaining;

                    if (rewritePath &&
                        !string.IsNullOrEmpty(virtualDirectory) &&
                        context.Request.Path.StartsWithSegments(virtualDirectory, StringComparison.OrdinalIgnoreCase, out remaining))
                    {
                        context.Request.Path = remaining;
                    }
                    context.Request.PathBase = context.Request.Headers[header].FirstOrDefault();
                }
                await next();
            });
        }
    }
}
