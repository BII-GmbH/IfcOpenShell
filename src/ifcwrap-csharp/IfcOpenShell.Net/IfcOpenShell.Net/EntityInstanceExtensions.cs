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
        
        public static bool TryGetAttributeAsString(this EntityInstance instance, string attributeName, out string attributeValue) {
            attributeValue = null;
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

                var arg = instance.get_argument(idx);
                return arg.try_get_as_string(out attributeValue);
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
            return false;
        }
        
        // public static bool TryGetAttribute(this EntityInstance instance, string attributeName, out Attribute att) {
        //     att = null;
        //     // attribute categories:
        //     const int INVALID = 0;
        //     const int FORWARD = 1;
        //     const int INVERSE = 2;
        //     
        //     // 0 == INVALID, 1 == FORWARD, 2 == INVERSE
        //     var attributeCategory = instance.get_attribute_category(attributeName);
        //     if (attributeCategory == FORWARD)
        //     {
        //         var idx = instance.get_argument_index(attributeName);
        //         // TODO need to understand what the hell the python impl is doing
        //         throw new NotImplementedException();
        //     }
        //     else if(attributeCategory == INVERSE)
        //     {
        //         instance.get_inverse(attributeName);
        //     }
        //     
        //     throw new System.NotImplementedException();
        // }
    }
}