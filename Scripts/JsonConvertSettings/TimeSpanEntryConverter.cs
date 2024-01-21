using System;
using Godot.Collections;
using Newtonsoft.Json;

public class TimeSpanEntryConverter : JsonConverter
{
   public override bool CanConvert(Type objectType)
   {
      return objectType == typeof(TimeSpanEntry);
   }



   public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
   {
      Dictionary dict = serializer.Deserialize<Dictionary>(reader);
      return new TimeSpanEntry(dict);
   }



   public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
   {
      TimeSpanEntry entry = value as TimeSpanEntry;
      Dictionary dict = entry.ToDictionary();
      serializer.Serialize(writer, dict);
   }
}