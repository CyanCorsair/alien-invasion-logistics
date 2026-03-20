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
}