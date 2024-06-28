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


        public abstract class ArgumentResult
        {
            private ArgumentResult() {}

            public abstract ArgumentType ArgumentType { get; }

            public virtual bool TryGetAsString(out string val)
            {
                val = default;
                return false;
            }

            public virtual bool TryGetAsInt(out int val)
            {
                val = default;
                return false;
            }

            public virtual bool TryGetAsBool(out bool val)
            {
                val = default;
                return false;
            }

            public virtual bool TryGetAsDouble(out double val)
            {
                val = default;
                return false;
            }

            public virtual bool TryGetAsEntity(out EntityInstance val)
            {
                val = null;
                return false;
            }

            public virtual bool TryGetAsIntList(out IntVector val)
            {
                val = null;
                return false;
            }

            public virtual bool TryGetAsDoubleList(out DoubleVector val)
            {
                val = null;
                return false;
            }

            public virtual bool TryGetAsStringList(out StringVector val)
            {
                val = null;
                return false;
            }

            public virtual bool TryGetAsEntityList(out aggregate_of_instance val)
            {
                val = null;
                return false;
            }
            
            internal class FromArgumentByType : ArgumentResult
            {
                public FromArgumentByType(ArgumentByType type)
                {
                    wrapped = type;
                }

                public override ArgumentType ArgumentType => wrapped.first;

                public override bool TryGetAsString(out string val)
                {
                    val = default;
                    return wrapped.TryGetAsString().TryGetValue(out val);
                }

                public override bool TryGetAsInt(out int val)
                {
                    val = default;
                    return wrapped.TryGetAsInt().TryGetValue(out val);
                }

                public override bool TryGetAsBool(out bool val)
                {
                    val = default;
                    return wrapped.TryGetAsBool().TryGetValue(out val);
                }

                public override bool TryGetAsDouble(out double val)
                {
                    val = default;
                    return wrapped.TryGetAsDouble().TryGetValue(out val);
                }

                public override bool TryGetAsEntity(out EntityInstance val)
                {
                    val = null;
                    return wrapped.TryGetAsEntity().TryGetValue(out val);
                }

                public override bool TryGetAsIntList(out IntVector val)
                {
                    val = null;
                    return wrapped.TryGetAsIntList().TryGetValue(out val);
                }

                public override bool TryGetAsDoubleList(out DoubleVector val)
                {
                    val = null;
                    return wrapped.TryGetAsDoubleList().TryGetValue(out val);
                }

                public override bool TryGetAsStringList(out StringVector val)
                {
                    val = null;
                    return wrapped.TryGetAsStringList().TryGetValue(out val);
                }

                public override bool TryGetAsEntityList(out aggregate_of_instance val)
                {
                    val = null;
                    return wrapped.TryGetAsEntityList().TryGetValue(out val);
                }
                
                
                private readonly ArgumentByType wrapped;
            }

            internal class FromAggregateOfInstance : ArgumentResult
            {
                public FromAggregateOfInstance(aggregate_of_instance agg)
                {
                    aggregate = agg;
                }

                public override ArgumentType ArgumentType => ArgumentType.Argument_AGGREGATE_OF_ENTITY_INSTANCE;

                public override bool TryGetAsEntityList(out aggregate_of_instance val)
                {
                    val = aggregate;
                    return true;
                }
                
                private readonly aggregate_of_instance aggregate;
            }
            
            internal class FromEntityInstance : ArgumentResult
            {
                public FromEntityInstance(EntityInstance agg)
                {
                    instance = agg;
                }

                public override ArgumentType ArgumentType => ArgumentType.Argument_ENTITY_INSTANCE;

                public override bool TryGetAsEntity(out EntityInstance val)
                {
                    val = instance;
                    return true;
                }
                
                private readonly EntityInstance instance;
            }
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
            return instance.TryGetAttribute(attributeName)?.TryGetAsString(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsInt(this EntityInstance instance , string attributeName, out int attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsInt(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsBool(this EntityInstance instance , string attributeName, out bool attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsBool(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsDouble(this EntityInstance instance , string attributeName, out double attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsDouble(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsEntity(this EntityInstance instance , string attributeName, out EntityInstance attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsEntity(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsIntList(this EntityInstance instance , string attributeName, out IntVector attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsIntList(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsDoubleList(this EntityInstance instance , string attributeName, out DoubleVector attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsDoubleList(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsStringList(this EntityInstance instance , string attributeName, out StringVector attributeValue)
        {
            attributeValue = default;
            return instance.TryGetAttribute(attributeName)?.TryGetAsStringList(out attributeValue) ?? false;
        }
        
        public static bool TryGetAttributeAsEntityList(this EntityInstance instance , string attributeName, out aggregate_of_instance attributeValue)
        {
            attributeValue = default;
            var att = instance.TryGetAttribute(attributeName);
            return att?.TryGetAsEntityList(out attributeValue) ?? false;
        }
        
        public static ArgumentResult TryGetAttribute(this EntityInstance instance, string attributeName) {
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

            var schema_name = instance.is_a(true).Split('.')[0];
            
            // TODO need to understand what the hell the python impl is doing
            // throw new System.NotImplementedException();
            //
            throw new System.NotImplementedException();
            return null;
        }
    }
}