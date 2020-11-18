using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TBA.Common
{
    /// <summary>
    /// JSON parsing rules for reading the settings file and converting to the POCO
    /// </summary>
    /// <typeparam name="T">The return object type</typeparam>
    /// <remarks>The ReadJson implementation was found here: https://gist.github.com/lucd/cdd57a2602bd975ec0a6#gistcomment-2339326</remarks>
    public sealed class RuntimeSettingsJsonConverter<T> : JsonConverter where T : new()
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var data = JObject.Load(reader);
            var result = new T();

            foreach (var prop in result.GetType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance))
            {
                string propName = string.Empty;
                //filter out non-Json attributes
                var attr = prop.GetCustomAttributes(false).Where(a => a.GetType() == typeof(JsonPropertyAttribute)).FirstOrDefault();
                if (attr != null)
                {
                    propName = ((JsonPropertyAttribute)attr).PropertyName;
                }
                //If no JsonPropertyAttribute existed, or no PropertyName was set,
                //still attempt to deserialize the class member
                if (string.IsNullOrWhiteSpace(propName))
                {
                    propName = prop.Name;
                }
                //split by the delimiter, and traverse recursively according to the path
                var nests = propName.Split('/');
                object propValue = null;
                JToken token = null;
                for (var i = 0; i < nests.Length; i++)
                {
                    if (token == null)
                    {
                        token = data[nests[i]];
                    }
                    else
                    {
                        token = token[nests[i]];
                    }
                    if (token == null)
                    {
                        //silent fail: exit the loop if the specified path was not found
                        break;
                    }
                    else
                    {
                        //store the current value
                        if (token is JValue)
                        {
                            propValue = ((JValue)token).Value;
                        }
                    }
                }

                if (propValue != null)
                {
                    //workaround for numeric values being automatically created as Int64 (long) objects.
                    if (propValue is long && prop.PropertyType == typeof(int))
                    {
                        prop.SetValue(result, Convert.ToInt32(propValue));
                    }
                    else
                    {
                        prop.SetValue(result, propValue);
                    }
                }
            }
            return result;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}