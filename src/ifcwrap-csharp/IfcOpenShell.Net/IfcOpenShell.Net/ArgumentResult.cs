﻿using System;
using System.Reflection.Emit;

namespace IfcOpenShell
{
    public abstract class ArgumentResult
    {
        private ArgumentResult()
        {
        }

        public abstract ArgumentType ArgumentType { get; }

        #region Get
        public virtual string GetAsString()
        {
            return default;
        }

        public virtual int GetAsInt()
        {
            return default;
        }

        public virtual bool GetAsBool()
        {
            return default;
        }
        
        public virtual double GetAsDouble()
        {
            return default;
        }
        
        public virtual EntityInstance GetAsEntity()
        {
            return null;
        }

        public virtual IntVector GetAsIntList()
        {
            return null;
        }

        public virtual DoubleVector GetAsDoubleList()
        {
            return null;
        }
        
        public virtual StringVector GetAsStringList()
        {
            return null;
        }
        
        public virtual aggregate_of_instance GetAsEntityList()
        {
            return null;
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
        #endregion

        internal class FromArgumentByType : ArgumentResult
        {
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

            public override IntVector GetAsIntList()
            {
                return wrapped.TryGetAsIntList().TryGetValue(out var v) ? v : null;
            }

            public override DoubleVector GetAsDoubleList()
            {
                return wrapped.TryGetAsDoubleList().TryGetValue(out var v) ? v : null;
            }
        
            public override StringVector GetAsStringList()
            {
                return wrapped.TryGetAsStringList().TryGetValue(out var v) ? v : null;
            }
        
            public override aggregate_of_instance GetAsEntityList()
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
            #endregion

            private readonly ArgumentByType wrapped;
        }
        
        internal class FromAggregateOfInstance : ArgumentResult
        {
            public FromAggregateOfInstance(aggregate_of_instance agg)
            {
                aggregate = agg;
            }

            public override ArgumentType ArgumentType => ArgumentType.Argument_AGGREGATE_OF_ENTITY_INSTANCE;

            public override aggregate_of_instance GetAsEntityList()
            {
                return aggregate;
            }

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

            public override EntityInstance GetAsEntity()
            {
                return instance;
            }
            
            public override bool TryGetAsEntity(out EntityInstance val)
            {
                val = instance;
                return true;
            }

            private readonly EntityInstance instance;
        }
        
        internal class FromString : ArgumentResult
        {
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

            private readonly string instance;
        }
        
        internal class FromInt : ArgumentResult
        {
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

            private readonly int instance;
        }
        
        internal class FromDouble : ArgumentResult
        {
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

            private readonly double instance;
        }
    }
}