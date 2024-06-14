using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IfcOpenShell {   
    public static class EntityInstanceExtensions {

        public static class Settings
        {
            public const bool UnpackNonAggregateInverses = false;
        }
        

        // TODO: find a way to have this auto-generated
    	public static bool TryGetValue(this StringArgument arg, out string value)
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
        
        public static bool TryGetValue(this IntListArgument arg, out IntVector value)
        {
            if (arg.HasValue())
            {
                value = arg.GetValue();
                return true;
            }
            value = default;
            return false;
        }
        
        public static bool TryGetValue(this DoubleListArgument arg, out DoubleVector value)
        {
            if (arg.HasValue())
            {
                value = arg.GetValue();
                return true;
            }
            value = default;
            return false;
        }
        
        public static bool TryGetValue(this StringListArgument arg, out StringVector value)
        {
            if (arg.HasValue())
            {
                value = arg.GetValue();
                return true;
            }
            value = default;
            return false;
        }
        
        public static bool TryGetValue(this EntityListArgument arg, out aggregate_of_instance value)
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
        
        public static bool TryGetAttributeAsString(this EntityInstance instance , string attributeName, out string attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsString().TryGetValue(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsInt(this EntityInstance instance , string attributeName, out int attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsInt().TryGetValue(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsBool(this EntityInstance instance , string attributeName, out bool attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsBool().TryGetValue(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsDouble(this EntityInstance instance , string attributeName, out double attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsDouble().TryGetValue(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsEntity(this EntityInstance instance , string attributeName, out EntityInstance attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsEntity().TryGetValue(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsIntList(this EntityInstance instance , string attributeName, out IntVector attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsIntList().TryGetValue(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsDoubleList(this EntityInstance instance , string attributeName, out DoubleVector attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsDoubleList().TryGetValue(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsStringList(this EntityInstance instance , string attributeName, out StringVector attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsStringList().TryGetValue(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsEntityList(this EntityInstance instance , string attributeName, out aggregate_of_instance attributeValue)
        {
            attributeValue = default;
            var att = instance.TryGetAttribute(attributeName);
            var asEntityList = att?.TryGetAsEntityList();
            return asEntityList?.TryGetValue(out attributeValue) ?? false;
        }
        
        public static ArgumentByType TryGetAttribute(this EntityInstance instance, string attributeName) {
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

                return instance.get_argument(idx);
            }
            else if(attributeCategory == INVERSE)
            {
                var inverse = instance.get_inverse(attributeName);
                if (Settings.UnpackNonAggregateInverses)
                {
                    // TODO need to understand what the hell the python impl is doing
                    //throw new System.NotImplementedException();
                }
                    
                
            }

            var schema_name = instance.is_a(true).Split('.')[0];
            
            // TODO need to understand what the hell the python impl is doing
            // throw new System.NotImplementedException();
            //
            // throw new System.NotImplementedException();
            return null;
        }
    }
}