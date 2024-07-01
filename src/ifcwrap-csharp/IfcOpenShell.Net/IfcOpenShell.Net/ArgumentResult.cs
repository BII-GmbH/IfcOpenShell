using System;
using System.Reflection.Emit;

namespace IfcOpenShell
{
    public abstract class ArgumentResult
    {
        private ArgumentResult()
        {
        }

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
        
        internal class FromString : ArgumentResult
        {
            public FromString(string agg)
            {
                instance = agg;
            }

            public override ArgumentType ArgumentType => ArgumentType.Argument_STRING;

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

            public override bool TryGetAsDouble(out double val)
            {
                val = instance;
                return true;
            }

            private readonly double instance;
        }

        
    }
}