﻿namespace Naos.Core.Common.Serialization
{
    using System;
    using System.IO;
    using Newtonsoft.Json;

    public class JsonNetSerializer : ITextSerializer
    {
        private readonly JsonSerializer serializer;

        public JsonNetSerializer(JsonSerializerSettings settings = null)
        {
            this.serializer = JsonSerializer.Create(settings ?? DefaultJsonSerializerSettings.Create());
        }

        public void Serialize(object data, Stream output)
        {
            var writer = new JsonTextWriter(new StreamWriter(output));
            this.serializer.Serialize(writer, data, data.GetType());
            writer.Flush();
        }

        public object Deserialize(Stream input, Type type)
        {
            using (var sr = new StreamReader(input))
            using (var reader = new JsonTextReader(sr))
            {
                return this.serializer.Deserialize(reader, type);
            }
        }
    }
}
