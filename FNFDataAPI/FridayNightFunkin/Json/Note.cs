using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FridayNightFunkin.Json
{
    public class Note
    {
        [JsonProperty("mustHitSection")]
        public bool MustHitSection { get; set; }

        [JsonProperty("typeOfSection")]
        public long TypeOfSection { get; set; }

        [JsonProperty("lengthInSteps")]
        public long LengthInSteps { get; set; }

        [JsonProperty("sectionNotes")]
        [JsonConverter(typeof(OnlyDecimalConverter))]
        public List<List<decimal>> sectionNotes { get; set; } 

        [JsonProperty("bpm", NullValueHandling = NullValueHandling.Ignore)]
        public long? Bpm { get; set; }

        [JsonProperty("changeBPM", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ChangeBpm { get; set; }
    }
    
    public class OnlyDecimalConverter : JsonConverter
    {
        public static bool DisableHurtNotes { get; set; }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<List<decimal>>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            List<List<decimal>> onlyDecimals = new List<List<decimal>>();
            List<decimal> listOfDecimals = new List<decimal>();

            foreach (JToken token in array.Children())
            {
                if (DisableHurtNotes)
                    foreach (var item in token.ToArray())
                    {
                        if (item.Type == JTokenType.String)
                        {
                            token[1] = int.MaxValue; // set non existing NoteType if hurt note has been found
                        }
                    }

                foreach (var item in token)
                {
                    if (item.Type == JTokenType.Float || item.Type == JTokenType.Integer)
                        listOfDecimals.Add(item.ToObject<decimal>()); // filter all non compatible types
                }
                
                onlyDecimals.Add(listOfDecimals.ToList());
                listOfDecimals.Clear();
            }
            
            return onlyDecimals;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var notes = (List<List<decimal>>)value;

            writer.WriteStartArray();

            foreach (var note in notes)
            {
                writer.WriteStartArray();

                foreach (var n in note)
                {
                    writer.WriteValue(n);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }
    }
}