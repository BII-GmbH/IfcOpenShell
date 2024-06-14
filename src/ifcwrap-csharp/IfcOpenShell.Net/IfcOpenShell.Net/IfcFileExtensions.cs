using System;

namespace IfcOpenShell
{
    public static class IfcFileExtensions
    {

        private static double prefixMultiplier(string prefix)
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

        /// <summary>
        /// Returns a unit scale factor to convert to and from IFC project units and SI units.
        /// </summary>
        /// <returns>The scale factor</returns>
        public static double CalculateUnitScale(this IfcFile file, string unitType = "LENGTHUNIT")
        {
            var unitAssignment = file.instances_by_type("IfcUnitAssignment");
            if (unitAssignment == null || unitAssignment.Count == 0)
            {
                return 1.0;
            }
            var units = unitAssignment[0];
            var unitScale = 1.0;

            if (units.TryGetAttributeAsEntityList("Units", out var unitList))
            {
                foreach (var unit in unitList)
                {
                    if (!unit.TryGetAttributeAsString("UnitType", out var localUnitType) ||
                        !localUnitType.Equals(unitType))
                    {
                        continue;
                    }
                    var localUnit = unit;
                    while (localUnit != null && localUnit.is_a("IfcConversionBasedUnit"))
                    {
                        if (localUnit.TryGetAttributeAsEntity("ConversionFactor", out var convFactor))
                        {
                            unitScale *= convFactor.TryGetAttributeAsDouble("ValueComponent", out var convValue) ? convValue : 1.0;
                            localUnit = convFactor.TryGetAttributeAsEntity("UnitComponent", out var unitComp) ? unitComp : null;
                        }
                    }

                    if (localUnit != null && localUnit.is_a("IfcSIUnit"))
                    {
                        if (localUnit.TryGetAttributeAsString("Prefix", out var prefix))
                        {
                            unitScale *= prefixMultiplier(prefix);
                        }
                    }
                    
                }
            }
            return unitScale;
        }
        
    }
}