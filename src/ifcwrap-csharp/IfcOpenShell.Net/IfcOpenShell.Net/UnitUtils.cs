﻿using System;
using System.Linq;

namespace IfcOpenShell
{
    public static class UnitUtils
    {
        internal static readonly string[] siUnitNames =
        {
            "AMPERE",
            "BECQUEREL",
            "CANDELA",
            "COULOMB",
            "CUBIC_METRE",
            "DEGREE_CELSIUS",
            "FARAD",
            "GRAM",
            "GRAY",
            "HENRY",
            "HERTZ",
            "JOULE",
            "KELVIN",
            "LUMEN",
            "LUX",
            "MOLE",
            "NEWTON",
            "OHM",
            "PASCAL",
            "RADIAN",
            "SECOND",
            "SIEMENS",
            "SIEVERT",
            "SQUARE_METRE",
            "METRE",
            "STERADIAN",
            "TESLA",
            "VOLT",
            "WATT",
            "WEBER"
        };

        // See https://github.com/buildingSMART/IFC4.3.x-development/issues/72
        internal static string siUnitTypeToUnitName(string unitType) => unitType switch {
            "ABSORBEDDOSEUNIT" => "GRAY",
            "AMOUNTOFSUBSTANCEUNIT" => "MOLE",
            "AREAUNIT" => "SQUARE_METRE",
            "DOSEEQUIVALENTUNIT" => "SIEVERT",
            "ELECTRICCAPACITANCEUNIT" => "FARAD",
            "ELECTRICCHARGEUNIT" => "COULOMB",
            "ELECTRICCONDUCTANCEUNIT" => "SIEMENS",
            "ELECTRICCURRENTUNIT" => "AMPERE",
            "ELECTRICRESISTANCEUNIT" => "OHM",
            "ELECTRICVOLTAGEUNIT" => "VOLT",
            "ENERGYUNIT" => "JOULE",
            "FORCEUNIT" => "NEWTON",
            "FREQUENCYUNIT" => "HERTZ",
            "ILLUMINANCEUNIT" => "LUX",
            "INDUCTANCEUNIT" => "HENRY",
            "LENGTHUNIT" => "METRE",
            "LUMINOUSFLUXUNIT" => "LUMEN",
            "LUMINOUSINTENSITYUNIT" => "CANDELA",
            "MAGNETICFLUXDENSITYUNIT" => "TESLA",
            "MAGNETICFLUXUNIT" => "WEBER",
            "MASSUNIT" => "GRAM",
            "PLANEANGLEUNIT" => "RADIAN",
            "POWERUNIT" => "WATT",
            "PRESSUREUNIT" => "PASCAL",
            "RADIOACTIVITYUNIT" => "BECQUEREL",
            "SOLIDANGLEUNIT" => "STERADIAN",
            "THERMODYNAMICTEMPERATUREUNIT" => "KELVIN", // Or, DEGREE_CELSIUS, but this is a quirk of IFC
            "TIMEUNIT"=> "SECOND",
            "VOLUMEUNIT"=> "CUBIC_METRE",
            "USERDEFINED"=> "METRE",
            _ => throw new ArgumentOutOfRangeException(nameof(unitType), unitType, $"Unknown unit type {unitType}")
        };
        
        
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

        internal static string unitSymbol(string unit)
        {
            // assume base unit -> no prefix needed
            if(string.IsNullOrEmpty(unit))
                return string.Empty;
            return unit switch
            {
                // si units
                "SECOND" => "s",
                "METRE" => "m",
                "GRAM" => "g", // si standard is kilogram, but that would mean weird edge cases for the unit prefix, so use gram instead
                "AMPERE" => "A",
                "KELVIN" => "K",
                "MOLE" => "mol",
                "CANDELA" => "cd",
                // derived si units
                "BECQUEREL" => "Bq",
                "COULOMB" => "C",
                "CUBIC_METRE" => "m³",
                "DEGREE_CELSIUS" => "°C",
                "FARAD" => "F",
                "GRAY" => "Gy",
                "HENRY" => "H",
                "HERTZ" => "Hz",
                "JOULE" => "J",
                "LUMEN" => "lm",
                "LUX" => "lx",
                "NEWTON" => "N",
                "OHM" => "Ω",
                "PASCAL" => "Pa",
                "RADIAN" => "rad",
                "SIEMENS" => "S",
                "SIEVERT" => "Sv",
                "SQUARE_METRE" => "m²",
                "STERADIAN" => "sr",
                "TESLA" => "T",
                "VOLT" => "V",
                "WATT" => "W",
                "WEBER" => "Wb",
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
        }

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
        // if unit.Is("IfcSIUnit"):
        // symbol += prefix_symbols.get(unit.Prefix, "")
        //     symbol += unit_symbols.get(unit.Name.replace("METER", "METRE"), "?")
        // if unit.Is("IfcContextDependentUnit") and unit.UnitType == "USERDEFINED":
        // symbol = unit.Name
        // return symbol

        public static string GetUnitSymbol(EntityInstance unit)
        {
            if(unit.Is("IfcContextDependentUnit") && unit.GetAttributeAsString("UnitType") == "USERDEFINED")
            {
                return unit.GetAttributeAsString("Name");
            }
            
            var symbol = string.Empty;
            if (unit.Is("IfcSIUnit"))
            {
                symbol += prefixSymbol(unit.GetAttributeAsString("Prefix"));
            }
            var unitName = unit.GetAttributeAsString("Name")?.Replace("METER","METRE") ?? string.Empty;
            // (derived) si units are spelled uppercase, non-si units are lowercase 
            if(!siUnitNames.Contains(unitName))
            {
                unitName = unitName.Replace("_", " ").ToLower();
            }
            symbol += unitSymbol(unitName);
            return symbol;
        }
    }
}
            