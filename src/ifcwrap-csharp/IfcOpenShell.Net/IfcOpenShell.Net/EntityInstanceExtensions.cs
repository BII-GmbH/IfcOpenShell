#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace IfcOpenShell {   
    public static class EntityInstanceExtensions {

        public static class Settings
        {
            public const bool UnpackNonAggregateInverses = false;
        }

        /// <summary>
        /// Retrieves the construction type element of an element occurrence
        /// </summary>
        /// <param name="element">The element occurrence</param>
        /// <returns>The related type element, can be null</returns>
        public static EntityInstance? GetConstructionType(this EntityInstance element)
        {
            if (element.Is("IfcTypeObject"))
                return element;
            else if(element.TryGetAttributeAsEntityList("IsTypedBy", out var typedBy) && typedBy.Count > 0)
            {
                // according to doc there can be either 0 or one elements
                // https://ifc43-docs.standards.buildingsmart.org/IFC/RELEASE/IFC4x3/HTML/lexical/IfcObject.htm
                if(typedBy.First().TryGetAttributeAsEntity("RelatingType", out var type))
                    return type;
            }
            else if (element.TryGetAttributeAsEntityList("IsDefinedBy", out var definers)) // ifc 2x3
            {
                foreach (var def in definers)
                {
                    if (def.Is("IfcRelDefinesByType") && def.TryGetAttributeAsEntity("RelatingType", out var type))
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Return the unique id of the element in the ifc file.
        /// However, this id only exists for ifc objects that inherit from IfcRoot, so may be null.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string? Guid(this EntityInstance element)
        {
            if (element.Is("IfcRoot"))
            {
                return element.GetAttributeAsString("GlobalId");
            }
            return null;
        } 
        
        
        /// <summary>
        /// Return a dictionary of the entity_instance's properties (Python and IFC) and their values.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="includeIdentifier">Whether or not to include the STEP numerical identifier</param>
        /// <param name="recursive">Whether or not to convert referenced IFC elements into dictionaries too. All attributes also apply recursively</param>
        /// <param name="scalarOnly">Filters out all values that are IFC instances</param>
        /// <param name="ignore">A list of attribute names to ignore</param>
        /// <returns>A dictionary of properties and their corresponding values</returns>
        public static IReadOnlyDictionary<string, ArgumentResult> GetInfo(this EntityInstance instance, bool includeIdentifier=true, bool recursive = false, bool scalarOnly=false, IEnumerable<string>? ignore=null)
        {
            var ignoreHashSet = ignore?.ToHashSet() ?? new HashSet<string>();
            
            var result = new Dictionary<string, ArgumentResult>();
            try
            {
                if(includeIdentifier) result.Add("id", new ArgumentResult.FromInt(instance.id()));
                result.Add("type", new ArgumentResult.FromString(instance.Is()));
            }
            catch (Exception e)
            {
                // TODO: logging instead
                throw;
            }

            var attributeNames = instance.get_attribute_names();
            
            foreach (var attributeName in attributeNames)
            {
                try
                {
                    if (ignoreHashSet.Contains(attributeName))
                        continue;
                    if(!instance.TryGetAttributeAsEntity(attributeName, out var attribute)) 
                        continue;
                    var shouldInclude = true;
                    if (recursive || scalarOnly)
                        throw new NotImplementedException();
                    if(shouldInclude)
                        result.Add(attributeName, new ArgumentResult.FromEntityInstance(attribute));
                }
                catch (Exception e)
                {
                    // TODO: logging instead
                    throw;
                }
            }
            return result;
        }
        
        // TODO: find a way to have this auto-generated
    	public static bool TryGetValue(this StringArgument arg, out string? value)
        {
            if (arg.HasValue())
            {
                value = arg.GetValue();
                return true;
            }
            value = default;
            return false;
        }
    
        public static bool TryGetValue(this IntArgument arg, out int value)
        {
            if (arg.HasValue())
            {
                value = arg.GetValue();
                return true;
            }
            value = default;
            return false;
        }
        
        public static bool TryGetValue(this BoolArgument arg, out bool value)
        {
            if (arg.HasValue())
            {
                value = arg.GetValue();
                return true;
            }
            value = default;
            return false;
        }
        
        public static bool TryGetValue(this DoubleArgument arg, out double value)
        {
            if (arg.HasValue())
            {
                value = arg.GetValue();
                return true;
            }
            value = default;
            return false;
        }
        
        public static bool TryGetValue(this EntityArgument arg, out EntityInstance value)
        {
            if (arg.HasValue())
            {
                value = arg.GetValue();
                return true;
            }
            value = default;
            return false;
        }
        
        public static bool TryGetValue(this IntListArgument arg, out IntVector? value)
        {
            if (arg.HasValue())
            {
                value = arg.GetValue();
                return true;
            }
            value = default;
            return false;
        }
        
        public static bool TryGetValue(this DoubleListArgument arg, out DoubleVector? value)
        {
            if (arg.HasValue())
            {
                value = arg.GetValue();
                return true;
            }
            value = default;
            return false;
        }
        
        public static bool TryGetValue(this StringListArgument arg, out StringVector? value)
        {
            if (arg.HasValue())
            {
                value = arg.GetValue();
                return true;
            }
            value = default;
            return false;
        }
        
        public static bool TryGetValue(this EntityListArgument arg, out aggregate_of_instance? value)
        {
            if (arg.HasValue())
            {
                value = arg.GetValue();
                return true;
            }
            value = default;
            return false;
        }
        
        // internal static EntityInstance wrapValue(IfcBaseInterface value, IfcFile file)
        // {
        //     return value is EntityInstance ei ? new EntityInstance()
        // }
        // {
        //     return 
        // }

        // internal static IfcBaseInterface walk<T, Ret>(Func<T, bool> condition, Func<T, Ret> g, T value) 
        //     where T: IfcBaseInterface where Ret: IfcBaseInterface
        // {
        //     if (condition(value))
        //     {
        //         return g(value);
        //     }
        //     else
        //     {
        //         return value;
        //     }
        // }
        
        // internal static IEnumerable<Ret> walk<T, Ret>(Func<T, bool> condition, Func<T, Ret> g, IEnumerable<T> values)
        // {
        //     return values.Select(entry => walk(condition, g, entry));
        // }
        
        #region Get Attribute
        public static string? GetAttributeAsString(this EntityInstance instance , string attributeName)
        {
            return instance.GetAttribute(attributeName)?.GetAsString();
        }
        
        public static int GetAttributeAsInt(this EntityInstance instance , string attributeName)
        {
            return instance.GetAttribute(attributeName)?.GetAsInt() ?? default;
        }
        
        public static bool GetAttributeAsBool(this EntityInstance instance , string attributeName)
        {
            return instance.GetAttribute(attributeName)?.GetAsBool() ?? default;
        }
        
        public static double GetAttributeAsDouble(this EntityInstance instance , string attributeName)
        {
            return instance.GetAttribute(attributeName)?.GetAsDouble() ?? default;
        }
        
        public static EntityInstance? GetAttributeAsEntity(this EntityInstance instance , string attributeName)
        {
            return instance.GetAttribute(attributeName)?.GetAsEntity();
        }
        
        public static IntVector? GetAttributeAsIntList(this EntityInstance instance , string attributeName)
        {
            return instance.GetAttribute(attributeName)?.GetAsIntList();
        }
        
        public static DoubleVector? GetAttributeAsDoubleList(this EntityInstance instance , string attributeName)
        {
            return instance.GetAttribute(attributeName)?.GetAsDoubleList();
        }
        
        public static StringVector? GetAttributeAsStringList(this EntityInstance instance , string attributeName)
        {
            return instance.GetAttribute(attributeName)?.GetAsStringList();
        }
        
        public static aggregate_of_instance? GetAttributeAsEntityList(this EntityInstance instance , string attributeName)
        {
            return instance.GetAttribute(attributeName)?.GetAsEntityList();
        }
        #endregion
        
        #region Try Get Attribute
        public static bool TryGetAttributeAsString(this EntityInstance instance , string attributeName, out string attributeValue)
        {
            attributeValue = default;
            return instance.GetAttribute(attributeName)?.TryGetAsString(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsInt(this EntityInstance instance , string attributeName, out int attributeValue)
        {
            attributeValue = default;
            return instance.GetAttribute(attributeName)?.TryGetAsInt(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsBool(this EntityInstance instance , string attributeName, out bool attributeValue)
        {
            attributeValue = default;
            return instance.GetAttribute(attributeName)?.TryGetAsBool(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsDouble(this EntityInstance instance , string attributeName, out double attributeValue)
        {
            attributeValue = default;
            return instance.GetAttribute(attributeName)?.TryGetAsDouble(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsEntity(this EntityInstance instance , string attributeName, out EntityInstance attributeValue)
        {
            attributeValue = default;
            return instance.GetAttribute(attributeName)?.TryGetAsEntity(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsIntList(this EntityInstance instance , string attributeName, out IntVector attributeValue)
        {
            attributeValue = default;
            return instance.GetAttribute(attributeName)?.TryGetAsIntList(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsDoubleList(this EntityInstance instance , string attributeName, out DoubleVector attributeValue)
        {
            attributeValue = default;
            return instance.GetAttribute(attributeName)?.TryGetAsDoubleList(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsStringList(this EntityInstance instance , string attributeName, out StringVector attributeValue)
        {
            attributeValue = default;
            return instance.GetAttribute(attributeName)?.TryGetAsStringList(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsEntityList(this EntityInstance instance , string attributeName, out aggregate_of_instance attributeValue)
        {
            attributeValue = default;
            var att = instance.GetAttribute(attributeName);
            return att?.TryGetAsEntityList(out attributeValue) ?? false;
        }
        #endregion
        
        public static EntityArgument? TryGetAttributeAtIndex(this EntityInstance? instance, uint key)
        {
            if (instance == null) return null;
            if(key >= instance.Length())
            {
                throw new IndexOutOfRangeException($"Attribute index {key} out of range for instance of type {instance.Is()}");
            }
            return instance.get_argument(key).TryGetAsEntity();
        }

        public static ArgumentResult GetArgument(this EntityInstance instance, uint argIndex)
        {
            return new ArgumentResult.FromArgumentByType(instance.get_argument(argIndex));
        }
        
        public static ArgumentResult? GetAttribute(this EntityInstance instance, string attributeName) {
            // attribute categories:
            const int INVALID = 0;
            const int FORWARD = 1;
            const int INVERSE = 2;
            
            // 0 == INVALID, 1 == FORWARD, 2 == INVERSE
            var attributeCategory = instance.get_attribute_category(attributeName);
            if (attributeCategory == FORWARD)
            {
                var idx = instance.get_argument_index(attributeName);
                // TODO need to implement weird pointer lookup thing

                return new ArgumentResult.FromArgumentByType(instance.get_argument(idx));
            }
            else if(attributeCategory == INVERSE)
            {
                var inverse = instance.get_inverse(attributeName);
                if (Settings.UnpackNonAggregateInverses)
                {
                    // TODO need to understand what the hell the python impl is doing
                    throw new System.NotImplementedException();
                }
                return new ArgumentResult.FromAggregateOfInstance(inverse);
            }

            var schema_name = instance.Is(true).Split('.')[0];
            
            // TODO need to understand what the hell the python impl is doing
            // throw new System.NotImplementedException();
            //
            //throw new System.NotImplementedException();
            return null;
        }
        
        
    }
}