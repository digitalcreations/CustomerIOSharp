#region License
//   Copyright 2010 John Sheehan
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
#endregion
#region Acknowledgements
// Original JsonSerializer contributed by Daniel Crenna (@dimebrain)
#endregion

namespace CustomerIOSharp
{
    using System.IO;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Text;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    using RestSharp.Portable.Serializers;

    /// <summary>
    /// Default JSON serializer for request bodies
    /// Doesn't currently use the SerializeAs attribute, defers to Newtonsoft's attributes
    /// </summary>
    public class SerializerWrapper : ISerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        /// <summary>
        /// Default serializer with overload for allowing custom Json.NET settings
        /// </summary>
        public SerializerWrapper(Newtonsoft.Json.JsonSerializer serializer = null)
        {
            this.ContentType = new MediaTypeHeaderValue("application/json");
            this._serializer = serializer 
                ?? new Newtonsoft.Json.JsonSerializer
                       {
                           MissingMemberHandling = MissingMemberHandling.Ignore,
                           NullValueHandling = NullValueHandling.Ignore,
                           DefaultValueHandling = DefaultValueHandling.Include,
                           ContractResolver = new CamelCasePropertyNamesContractResolver()
                       };

            // make sure we use the correct datetime conversion so customer.io understands us
            foreach (var converter in this._serializer.Converters.OfType<DateTimeConverterBase>().ToList())
            {
                this._serializer.Converters.Remove(converter);
            }
            this._serializer.Converters.Add(new UnixTimestampConverter());
        }

        /// <summary>
        /// Serialize the object as JSON
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>JSON as String</returns>
        public byte[] Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';

                    this._serializer.Serialize(jsonTextWriter, obj);

                    var result = stringWriter.ToString();
                    return Encoding.UTF8.GetBytes(result);
                }
            }
        }

        /// <summary>
        /// Content type for serialized content
        /// </summary>
        public MediaTypeHeaderValue ContentType { get; set; }
    }
}
