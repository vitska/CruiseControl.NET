﻿using Nancy;
using Nancy.Bootstrapper;
using System.Collections.Generic;
using System.Linq;
using System.IO.Compression;

namespace Cruise.DashboardBroker.Service {
    public static class CompressionExtenstion
    {
        /// <summary>
        /// Registers the compresssion check
        /// </summary>
        /// <param name="pipelines"></param>
        public static void RegisterCompressionCheck(this IPipelines pipelines)
        {
            pipelines.AfterRequest.AddItemToEndOfPipeline(CheckForCompression);
        }

        /// <summary>
        /// Checks some compatibility
        /// </summary>
        /// <param name="context"></param>
        static void CheckForCompression(NancyContext context)
        {
            if (!RequestIsGzipCompatible(context.Request))
            {
                return;
            }

            if (context.Response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }

            if (!ResponseIsCompatibleMimeType(context.Response))
            {
                return;
            }

            if (ContentLengthIsTooSmall(context.Response))
            {
                return;
            }

            CompressResponse(context.Response);
        }

        /// <summary>
        /// Compresses the response and output its
        /// </summary>
        /// <param name="response"></param>
        static void CompressResponse(Response response)
        {
            response.Headers["Content-Encoding"] = "deflate";

            var contents = response.Contents;
            response.Contents = responseStream =>
            {
                using (var compression = new DeflateStream(responseStream, CompressionMode.Compress))
                {
                    contents(compression);
                }
            };
        }

        /// <summary>
        /// Determines if the response is small enough to fit in a packet.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        static bool ContentLengthIsTooSmall(Response response)
        {
            string contentLength;
            if (response.Headers.TryGetValue("Content-Length", out contentLength))
            {
                var length = long.Parse(contentLength);
                if (length < 4096)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Valid mimes to be compressed
        /// </summary>
        public static List<string> ValidMimes = new List<string>()
                                                    {
                                                        "text/css",
                                                        "text/html",
                                                        "text/plain",
                                                        "application/xml",
                                                        "application/json",
                                                        "application/xaml+xml",
                                                        "application/x-javascript",
                                                        "application/javascript"
                                                    };

        /// <summary>
        /// Checks if the response corresponds to any mime that could be compressed
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        static bool ResponseIsCompatibleMimeType(Response response)
        {
            return ValidMimes.Any(x => response.ContentType.StartsWith(x));
        }

        /// <summary>
        /// Determines if the request supports gzip
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        static bool RequestIsGzipCompatible(Request request)
        {
            return request.Headers.AcceptEncoding.Any(x => x.Contains("deflate"));
        }
    }
}
