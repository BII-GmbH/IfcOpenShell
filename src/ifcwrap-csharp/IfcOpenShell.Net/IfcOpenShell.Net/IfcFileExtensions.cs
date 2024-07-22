namespace IfcOpenShell.Net
{
    public static class IfcFileExtensions
    {

        // implementation derived calculate_unit_scale from the python version at ifcopenshell/util/unit.py
        /// <summary>
        /// Returns a unit scale factor to convert the IFC project units to and from SI units.
        /// </summary>
        /// <returns>The scale factor</returns>
        public static double CalculateUnitScale(this IfcFile file, string unitType = "LENGTHUNIT")
        {
            var unitAssignment = file.ByType("IfcUnitAssignment");
            // if the IFC file does not contain any unit assignments, assume SI units
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
                    // skip non-matching units
                    if (!unit.TryGetAttributeAsString("UnitType", out var localUnitType) ||
                        !localUnitType.Equals(unitType))
                    {
                        continue;
                    }
                    // In Ifc, units can be composed of other units, so we need to
                    // traverse the unit tree to find the total conversion factor from/to the SI base unit
                    // (for example imperial units are defined as a conversion from the corresponding SI base unit).
                    // https://ifc43-docs.standards.buildingsmart.org/IFC/RELEASE/IFC4x3/HTML/lexical/IfcConversionBasedUnit.htm
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
                            // early return since we already found a matching base unit -
                            // there should never be more than one of those anyway.
                            return unitScale;
                        }
                        
                    }
                    
                }
            } 
            // If this return is used, the unit type was not found in the file, or an unexpected
            // combination of ifc unit types, so this should always return 1.0.
            return unitScale;
        }
    }
}