using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AlienInvasionLogistics.Source.Utilities;

public class ReflectIntoFormat
{
    public static Dictionary<string, dynamic> ReflectIntoDictionary<T>(T model) where T : class
    {
        return typeof(T).GetProperties().ToDictionary(
            property => property.Name, property => property.GetValue(model));
    }

    // EF Core models have bidirectional navigation properties that form reference cycles
    // (e.g. Player → Nation → Player). ReferenceLoopHandling.Ignore stops Newtonsoft from
    // throwing on those cycles; the second occurrence of a reference is simply omitted.
    // NullValueHandling.Ignore skips unloaded navigation properties (null references from
    // un-included includes), keeping the output lean.
    private static readonly JsonSerializerSettings EfSerializerSettings = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore,
    };

    private static readonly JsonSerializerSettings EfSerializerSettingsIndented = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented
    };

    /// <summary>
    /// Serializes an EF Core model to a JSON string.
    /// Navigation property cycles are silently broken at the second reference.
    /// Unloaded (null) navigation properties are omitted.
    /// </summary>
    public static string ModelToJson<T>(T model, bool indented = false) where T : class
    {
        return JsonConvert.SerializeObject(model, indented ? EfSerializerSettingsIndented : EfSerializerSettings);
    }

    /// <summary>
    /// Deserializes a JSON string back into an EF Core model instance.
    /// The returned object is a plain POCO — it is not tracked by any DbContext.
    /// Attach it via context.Attach() or context.Entry(model).State = EntityState.Modified
    /// before saving if you need EF Core to track changes.
    /// </summary>
    public static T JsonToModel<T>(string json) where T : class
    {
        return JsonConvert.DeserializeObject<T>(json, EfSerializerSettings);
    }
}