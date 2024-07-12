using System;

namespace IfcOpenShell
{
    public static class IfcFileExtensions
    {

        /// <summary>
        /// Returns a unit scale factor to convert to and from IFC project units and SI units.
        /// </summary>
        /// <returns>The scale factor</returns>
        public static double CalculateUnitScale(this IfcFile file, string unitType = "LENGTHUNIT")
        {
            var unitAssignment = file.ByType("IfcUnitAssignment");
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
                    while (localUnit != null && localUnit.Is("IfcConversionBasedUnit"))
                    {
                        if (localUnit.TryGetAttributeAsEntity("ConversionFactor", out var convFactor))
                        {
                            unitScale *= convFactor.TryGetAttributeAsDouble("ValueComponent", out var convValue) ? convValue : 1.0;
                            localUnit = convFactor.TryGetAttributeAsEntity("UnitComponent", out var unitComp) ? unitComp : null;
                        }
                    }

                    if (localUnit != null && localUnit.Is("IfcSIUnit"))
                    {
                        if (localUnit.TryGetAttributeAsString("Prefix", out var prefix))
                        {
                            unitScale *= UnitUtils.prefixMultiplier(prefix);
                        }
                    }
                    
                }
            }
            return unitScale;
        }
        
    }
}