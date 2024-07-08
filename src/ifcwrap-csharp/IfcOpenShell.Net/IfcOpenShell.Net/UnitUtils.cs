using System;

namespace IfcOpenShell
{
    public static class UnitUtils
    {
        internal static string prefixSymbol(string prefix)
        {
            // assume base unit -> no prefix needed
            if(string.IsNullOrEmpty(prefix))
                return string.Empty;
            return prefix switch
            {
                "EXA" => "E",
                "PETA" => "P",
                "TERA" => "T",
                "GIGA" => "G",
                "MEGA" => "M",
                "KILO" => "k",
                "HECTO" => "h",
                "DECA" => "da",
                "DECI" => "d",
                "CENTI" => "c",
                "MILLI" => "m",
                "MICRO" => "μ",
                "NANO" => "n",
                "PICO" => "p",
                "FEMTO" => "f",
                "ATTO" => "a",
                _ => throw new ArgumentOutOfRangeException(nameof(prefix), prefix, $"Cannot find prefix symbol '{prefix}'")
            };
        }

        internal static string unitSymbol(string unit) => unit switch
            {
                // TODO: I think this is missing units?!?!?! for example why is hertz not in here?
                // NOTE: IN PYTHON THE SI UNITS ARE DEFINED AS UPPERCASE - THAT IS ANNOYING THOUGH, so dont do it here
                // METERs are spelled correctly in c# too!
                // si units
                "cubic meter" => "m3",
                "gram" => "g",
                "second" => "s",
                "square meter" => "m2",
                "meter" => "m",
                // non si units
                "cubic inch" => "in3",
                "cubic foot" => "ft3",
                "cubic yard" => "yd3",
                "square inch" => "in2",
                "square foot" => "ft2",
                "square yard" => "yd2",
                "square mile" => "mi2",
                // conversion based units that are not in the "non-si units" list
                "thou" => "th",
                "inch" => "in",
                "foot" => "ft",
                "yard" => "yd",
                "mile" => "mi",
                "square thou" => "th2",
                "acre" => "ac",
                "cubic thou" => "th3",
                "cubic mile" => "mi3",
                "litre" => "L",
                "fluid ounce UK" => "fl oz",
                "fluid ounce US" => "fl oz",
                "pint UK" => "pt",
                "pint US" => "pt",
                "gallon UK" => "gal",
                "gallon US" => "gal",
                "degree" => "°",
                "ounce" => "oz",
                "pound" => "lb",
                "ton UK" => "ton",
                "ton US" => "ton",
                "lbf" => "lbf",
                "kip" => "kip",
                "psi" => "psi",
                "ksi" => "ksi",
                "minute" => "min",
                "hour" => "hr",
                "day" => "day",
                "btu" => "btu",
                "fahrenheit" => "°F",
                _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, $"Cannot find unit '{unit}'")
            };
        
        internal static double prefixMultiplier(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return 1.0;
            }
            return prefix switch
            {
                "EXA" => 1e18,
                "PETA" => 1e15,
                "TERA" => 1e12,
                "GIGA" => 1e9,
                "MEGA" => 1e6,
                "KILO" => 1e3,
                "HECTO" => 1e2,
                "DECA" => 1e1,
                "DECI" => 1e-1,
                "CENTI" => 1e-2,
                "MILLI" => 1e-3,
                "MICRO" => 1e-6,
                "NANO" => 1e-9,
                "PICO" => 1e-12,
                "FEMTO" => 1e-15,
                "ATTO" => 1e-18,
                _ => 1.0
            };
        }
        
        
        private static readonly string[] measureClasses =
        {
            "IfcNumericMeasure",
            "IfcLengthMeasure",
            "IfcAreaMeasure",
            "IfcVolumeMeasure",
            "IfcMassMeasure"
        };

        private static readonly string[] measureClassModifiers =
        {
            "Ifc", "Measure", "Non", "Positive", "Negative"
        };

        public static string GetMeasureUnitType(string measureClass)
        {
            if (measureClass == "IfcNumericMeasure")
                // See https://github.com/buildingSMART/IFC4.3.x-development/issues/71
                return "USERDEFINED";
            foreach (var text in measureClassModifiers)
            {
                measureClass = measureClass.Replace(text, "");
            }

            return measureClass.ToUpper() + "UNIT";
        }
        
        // def get_unit_symbol(unit: ifcopenshell.entity_instance) -> str:
        // symbol = ""
        // if unit.is_a("IfcSIUnit"):
        // symbol += prefix_symbols.get(unit.Prefix, "")
        //     symbol += unit_symbols.get(unit.Name.replace("METER", "METRE"), "?")
        // if unit.is_a("IfcContextDependentUnit") and unit.UnitType == "USERDEFINED":
        // symbol = unit.Name
        // return symbol

        public static string GetUnitSymbol(EntityInstance unit)
        {
            if(unit.is_a("IfcContextDependentUnit") && unit.GetAttributeAsString("UnitType") == "USERDEFINED")
            {
                return unit.GetAttributeAsString("Name");
            }
            
            var symbol = string.Empty;
            if (unit.is_a("IfcSIUnit"))
            {
                symbol += prefixSymbol(unit.GetAttributeAsString("Prefix"));
            }

            // NOTE: In the C# implementation the unit symbol method expects everything to be lowercase - this is different in python
            var unitName = unit.GetAttributeAsString("Name")?.ToLower().Replace("metre", "meter").Replace("_", " ");
            symbol += unitSymbol(unitName);
            return symbol;
        }
    }
}
            