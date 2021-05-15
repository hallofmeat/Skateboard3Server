using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using NLog;

namespace Skateboard3Server.Web.Formatter
{
    //Very loosly based on XmlSerializerOutputFormatter but we need it not to have encoding type in the Content-Type header or in the xml document definition
    //https://github.com/dotnet/aspnetcore/blob/v3.1.15/src/Mvc/Mvc.Formatters.Xml/src/XmlSerializerOutputFormatter.cs
    public class PoxOutputFormatter : OutputFormatter
    {
        private readonly ConcurrentDictionary<Type, object> _serializerCache = new ConcurrentDictionary<Type, object>();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        private readonly XmlWriterSettings _writerSettings = new XmlWriterSettings
        {
            Encoding = Encoding.ASCII,
            OmitXmlDeclaration = true, //We manually write this in the body write
            CloseOutput = false,
            CheckCharacters = false,
            Indent = false,
        };

        public PoxOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/xml").CopyAsReadOnly());
        }

        protected override bool CanWriteType(Type type)
        {
            return type != null;
        }

        private void Serialize(XmlSerializer xmlSerializer, XmlWriter xmlWriter, object value)
        {
            //Remove all namespaces
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            xmlSerializer.Serialize(xmlWriter, value, namespaces);
        }

        private XmlSerializer GetCachedSerializer(Type type)
        {
            if (!_serializerCache.TryGetValue(type, out var serializer))
            {
                try
                {
                    // If the serializer does not support this type it will throw an exception.
                    serializer = new XmlSerializer(type);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Failed to create output XmlSerializer for {type.FullName}");
                }

                if (serializer != null)
                {
                    _serializerCache.TryAdd(type, serializer);
                }
            }

            return (XmlSerializer)serializer;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            //TODO maybe need wrapping logic?
            var value = context.Object;
            var xmlSerializer = GetCachedSerializer(context.ObjectType);

            var httpContext = context.HttpContext;
            var response = httpContext.Response;

            //Used so we get correct content-length
            var fileBufferingWriteStream = new FileBufferingWriteStream();
            var responseStream = fileBufferingWriteStream;
            try
            {
                await using (var textWriter = context.WriterFactory(responseStream, Encoding.ASCII))
                {
                    //Hack because XmlWriter really wants to add encoding="us-ascii" which segfaults skate 3
                    await textWriter.WriteAsync("<?xml version=\"1.0\"?>");
                    using var xmlWriter = XmlWriter.Create(textWriter, _writerSettings);
                    Serialize(xmlSerializer, xmlWriter, value);
                }

                response.ContentLength = fileBufferingWriteStream.Length;
                await fileBufferingWriteStream.DrainBufferAsync(response.Body);
            }
            finally
            {
                await fileBufferingWriteStream.DisposeAsync();
            }

        }
    }
}
