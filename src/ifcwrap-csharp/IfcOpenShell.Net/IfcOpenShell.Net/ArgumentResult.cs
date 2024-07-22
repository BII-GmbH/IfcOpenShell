using System;
using System.Collections.Generic;
using System.Linq;

namespace IfcOpenShell.Net
{
    /// Abstraction class to wrap the typed result of an GetAttributeXXX call on <see cref="EntityInstance"/>.
    /// Because C#, unlike Python, is a strongly typed language (we dont talk about dynamic),
    /// we need to wrap the result of the GetAttributeXXX calls to return useful type
    /// information about the return value.
    ///
    /// The alternative would be to use dynamic, but that throws away all type safety.
    public abstract class ArgumentResult
    {
        // prevent outside inheritance
        private ArgumentResult()
        {
        }

        public abstract ArgumentType ArgumentType { get; }

        // default implementations always return no result.
        #region Get
        public virtual string GetAsString()
        {
            throw new InvalidCastException($"Cannot cast argument of type {ArgumentType} to string");
        }

        public virtual int GetAsInt()
        {
            throw new InvalidCastException($"Cannot cast argument of type {ArgumentType} to int");
        }

        public virtual bool GetAsBool()
        {
            throw new InvalidCastException($"Cannot cast argument of type {ArgumentType} to bool");
        }
        
        public virtual double GetAsDouble()
        {
            throw new InvalidCastException($"Cannot cast argument of type {ArgumentType} to double");
        }
        
        public virtual EntityInstance GetAsEntity()
        {
            throw new InvalidCastException($"Cannot cast argument of type {ArgumentType} to EntityInstance");
        }

        public virtual IReadOnlyList<int> GetAsIntList()
        {
            throw new InvalidCastException($"Cannot cast argument of type {ArgumentType} to List<int>");
        }

        public virtual IReadOnlyList<double> GetAsDoubleList()
        {
            throw new InvalidCastException($"Cannot cast argument of type {ArgumentType} to List<double>");
        }
        
        public virtual IReadOnlyList<string> GetAsStringList()
        {
            throw new InvalidCastException($"Cannot cast argument of type {ArgumentType} to List<string>");
        }
        
        public virtual IReadOnlyList<EntityInstance> GetAsEntityList()
        {
            throw new InvalidCastException($"Cannot cast argument of type {ArgumentType} to List<EntityInstance>");
        }
        #endregion
        
        #region Try Get
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

        public virtual bool TryGetAsIntList(out IReadOnlyList<int> val)
        {
            val = null;
            return false;
        }

        public virtual bool TryGetAsDoubleList(out IReadOnlyList<double> val)
        {
            val = null;
            return false;
        }

        public virtual bool TryGetAsStringList(out IReadOnlyList<string> val)
        {
            val = null;
            return false;
        }

        public virtual bool TryGetAsEntityList(out IReadOnlyList<EntityInstance> val)
        {
            val = null;
            return false;
        }
        #endregion

        /// Used to wrap the result of the C++ get_attribute call result.
        /// This may wrap one of many types, which is determined by
        /// the cpp result.
        internal class FromArgumentByType : ArgumentResult
        {
            private readonly ArgumentByType wrapped;
            
            public FromArgumentByType(ArgumentByType type)
            {
                wrapped = type;
            }

            public override ArgumentType ArgumentType => wrapped.first;

            #region Get
            public override string GetAsString()
            {
                return wrapped.TryGetAsString().TryGetValue(out var v) ? v : default;
            }

            public override int GetAsInt()
            {
                return wrapped.TryGetAsInt().TryGetValue(out var v) ? v : default;
            }

            public override bool GetAsBool()
            {
                return wrapped.TryGetAsBool().TryGetValue(out var v) ? v : default;
            }
        
            public override double GetAsDouble()
            {
                return wrapped.TryGetAsDouble().TryGetValue(out var v) ? v : default;
            }
        
            public override EntityInstance GetAsEntity()
            {
                return wrapped.TryGetAsEntity().TryGetValue(out var v) ? v : null;
            }

            public override IReadOnlyList<int> GetAsIntList()
            {
                return wrapped.TryGetAsIntList().TryGetValue(out var v) ? v : null;
            }

            public override IReadOnlyList<double> GetAsDoubleList()
            {
                
                return wrapped.TryGetAsDoubleList().TryGetValue(out var v) ? v : null;
            }
        
            public override IReadOnlyList<string> GetAsStringList()
            {
                return wrapped.TryGetAsStringList().TryGetValue(out var v) ? v : null;
            }
        
            public override IReadOnlyList<EntityInstance> GetAsEntityList()
            {
                return wrapped.TryGetAsEntityList().TryGetValue(out var v) ? v : null;
            }
            #endregion
            #region Try Get
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

            public override bool TryGetAsIntList(out IReadOnlyList<int> val)
            {
                val = null;
                if (wrapped.TryGetAsIntList().TryGetValue(out var vec))
                {
                    val = vec;
                    return true;
                }
                return false;
                
            }

            public override bool TryGetAsDoubleList(out IReadOnlyList<double> val)
            {
                val = null;
                if (wrapped.TryGetAsDoubleList().TryGetValue(out var vec))
                {
                    val = vec;
                    return true;
                }
                return false;
            }

            public override bool TryGetAsStringList(out IReadOnlyList<string> val)
            {
                val = null;
                if (wrapped.TryGetAsStringList().TryGetValue(out var vec))
                {
                    val = vec;
                    return true;
                }
                return false;
            }

            public override bool TryGetAsEntityList(out IReadOnlyList<EntityInstance> val)
            {
                val = null;
                if (wrapped.TryGetAsEntityList().TryGetValue(out var vec))
                {
                    val = vec;
                    return true;
                }
                return false;
            }
            #endregion
        }
        
        /// Used to wrap an <see cref="aggregate_of_instance"/> that we need to return as ArgumentResult to satisfy an interface.
        /// Used only from within the C# extension layer.
        internal class FromAggregateOfInstance : ArgumentResult
        {
            private readonly aggregate_of_instance aggregate;
            
            public FromAggregateOfInstance(aggregate_of_instance agg)
            {
                aggregate = agg;
            }

            public override ArgumentType ArgumentType => ArgumentType.Argument_AGGREGATE_OF_ENTITY_INSTANCE;

            public override IReadOnlyList<EntityInstance> GetAsEntityList()
            {
                return aggregate;
            }

            public override bool TryGetAsEntityList(out IReadOnlyList<EntityInstance> val)
            {
                val = aggregate;
                return true;
            }
        }
        
        /// Used to wrap an <see cref="EntityInstance"/> that we need to return as ArgumentResult to satisfy an interface.
        /// Used only from within the C# extension layer.
        internal class FromEntityInstance : ArgumentResult
        {
            private readonly EntityInstance instance;
            
            public FromEntityInstance(EntityInstance agg)
            {
                instance = agg;
            }

            public override ArgumentType ArgumentType => ArgumentType.Argument_ENTITY_INSTANCE;

            public override EntityInstance GetAsEntity()
            {
                return instance;
            }
            
            public override bool TryGetAsEntity(out EntityInstance val)
            {
                val = instance;
                return true;
            }
        }
        
        /// Used to wrap an <see cref="string"/> that we need to return as ArgumentResult to satisfy an interface.
        /// Used only from within the C# extension layer.
        internal class FromString : ArgumentResult
        {
            private readonly string instance;
            
            public FromString(string agg)
            {
                instance = agg;
            }

            public override ArgumentType ArgumentType => ArgumentType.Argument_STRING;

            public override string GetAsString()
            {
                return instance;
            }
            
            public override bool TryGetAsString(out string val)
            {
                val = instance;
                return true;
            }
        }
        
        /// Used to wrap an <see cref="int"/> that we need to return as ArgumentResult to satisfy an interface.
        /// Used only from within the C# extension layer.
        internal class FromInt : ArgumentResult
        {
            private readonly int instance;
            
            public FromInt(int agg)
            {
                instance = agg;
            }

            public override ArgumentType ArgumentType => ArgumentType.Argument_INT;

            public override int GetAsInt()
            {
                return instance;
            }
            
            public override bool TryGetAsInt(out int val)
            {
                val = instance;
                return true;
            }
        }
        
        /// Used to wrap a <see cref="double"/> that we need to return as ArgumentResult to satisfy an interface.
        /// Used only from within the C# extension layer.
        internal class FromDouble : ArgumentResult
        {
            private readonly double instance;
            
            public FromDouble(double agg)
            {
                instance = agg;
            }

            public override ArgumentType ArgumentType => ArgumentType.Argument_DOUBLE;

            public override double GetAsDouble()
            {
                return instance;
            }
            
            public override bool TryGetAsDouble(out double val)
            {
                val = instance;
                return true;
            }
        }
        
        /// Used to wrap an <see cref="IEnumerable{T}"/> with T=string that we need to return as ArgumentResult to satisfy an interface.
        /// Note that the enumerable will be evaluated when creating this object & stored as a list.
        /// Used only from within the C# extension layer.
        internal class FromStringList : ArgumentResult
        {
            private readonly List<string> instance;
         
            public FromStringList(IEnumerable<string> strings)
            {
                instance = strings.ToList();
            }

            public override ArgumentType ArgumentType => ArgumentType.Argument_AGGREGATE_OF_STRING;

            public override IReadOnlyList<string> GetAsStringList()
            {
                return instance;
            }
        }
    }
}