using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Compiler.Parse
{
    /// <summary>
    /// An <see cref="IParsePipeline{T}"/> that creates a <see cref="TokenPackage"/> from its published JSON output file.
    /// </summary>
    public class JsonParsePipeline : IParsePipeline<Stream>
    {
        /// <inheritdoc/>
        public async Task<TokenPackage> ParsePackageAsync(Stream packageInput)
        {
            JsonSerializer serializer = new JsonSerializer();
            var streamReader = new StreamReader(packageInput);
            return serializer.Deserialize<TokenPackage>(new JsonTextReader(streamReader));
        }
    }

    /// <summary>
    /// An <see cref="IPublishPipeline"/> that writes a JSON output file for the given <see cref="TokenPackage"/>.
    /// </summary>
    public class JsonPublishPipeline : IPublishPipeline
    {
        /// <inheritdoc/>
        public async Task PublishPackageAsync(TokenPackage package, Stream stream)
        {
            JsonSerializer serializer = new JsonSerializer();
            var streamWriter = new StreamWriter(stream);
            serializer.Serialize(new JsonTextWriter(streamWriter), package);
            await streamWriter.FlushAsync();
        }
    }
}
