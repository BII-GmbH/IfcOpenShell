using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Globalization;
using IfcOpenShell;

namespace BII.Common.Import.IfcOpenShell
{
    public sealed record UnitInformation(double ConversionFactor, string UnitSymbol);

    
    public static class IfcOpenShellUtils
    {
        [Pure]
        public static ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TKey, TValue>(
            this IEnumerable<(TKey, TValue)> entries
        ) => ImmutableDictionary.CreateRange(entries.Select(e => new KeyValuePair<TKey, TValue>(e.Item1, e.Item2)));

        
        internal static string? propertyValueToString(ImmutableDictionary<string, UnitInformation> defaultUnits, ArgumentResult arg) {
            try {
                var valueString = arg.ArgumentType switch {
                    ArgumentType.Argument_INT => arg.GetAsInt().ToString(),
                    ArgumentType.Argument_BOOL => arg.GetAsBool().ToString(),
                    ArgumentType.Argument_DOUBLE => arg.GetAsDouble().ToString(CultureInfo.InvariantCulture),
                    ArgumentType.Argument_STRING => arg.GetAsString(),
                    ArgumentType.Argument_ENTITY_INSTANCE => getStringFromProperty(defaultUnits, arg.GetAsEntity()),
                    // ArgumentType.Argument_AGGREGATE_OF_INT => $"[{arg.GetAsIntList().Select(i => i.ToString()).Join(",")}]".Some(),
                    // ArgumentType.Argument_AGGREGATE_OF_DOUBLE => $"[{arg.GetAsDoubleList().Select(i => i.ToString(CultureInfo.InvariantCulture)).Join(",")}]".Some(),
                    // ArgumentType.Argument_AGGREGATE_OF_STRING => $"[{arg.GetAsStringList().Select(i => i.ToString()).Join(",")}]".Some(),
                    _ => null
                };
                return valueString;
            }
            catch (Exception e) { /*Log.Warning(e, "Could not convert argument type {PropType} to string", arg.ArgumentType);*/ }

            return null;
        }
        
        internal static string? getStringFromProperty(ImmutableDictionary<string, UnitInformation> defaultUnits, EntityInstance property) {
            
            // this is fine to do, the IfcOpenShell wrapper also only supports this type in PSetExtensions.getProperties
            if (property.Is("IfcPropertySingleValue")) {
                // NominalValue has argument index 2 - for some reason getting this by index works better than getting by name
                const uint IfcPropertySingleValue_NominalValue = 2;
                var singleVal = property.GetArgument(IfcPropertySingleValue_NominalValue).GetAsEntity();
                
                var type = singleVal.Is();
                string propValue = null;
                switch (type) {
                    case "IfcCountMeasure":
                    case "IfcPositiveCountMeasure":
                    case "IfcVolumeMeasure":
                    case "IfcPositiveVolumeMeasure":
                    case "IfcLengthMeasure":
                    case "IfcPositiveLengthMeasure":
                    case "IfcAreaMeasure":
                    case "IfcPositiveAreaMeasure":
                    case "IfcMassMeasure":
                    case "IfcPositiveMassMeasure":
                    case "IfcTimeMeasure":
                    case "IfcPositiveTimeMeasure":
                    case "IfcThermalTransmittanceMeasure":
                    case "IfcPlaneAngleMeasure":
                    case "IfcPowerMeasure":
                    case "IfcPositivePowerMeasure":
                        string unitString;
                        if (property.TryGetAttributeAsEntity("Unit", out var unit)) {
                            // local part overrides the default unit defined in IfcProject used for this unit type
                            unitString = UnitUtils.GetUnitSymbol(unit);
                        }
                        else {
                            // uses default unit for the unit type as defined in IfcProject
                            var unitType = UnitUtils.GetMeasureUnitType(type);
                            var defaultUnit = defaultUnits[unitType];

                            unitString = defaultUnit.UnitSymbol;
                        }
                        
                        var nomValue = singleVal.GetAttributeAsDouble("wrappedValue");
                        
                        propValue = $"{nomValue.ToString(CultureInfo.InvariantCulture)} {unitString}";
                        break;
                    case "IfcText":
                    case "IfcLabel":
                    case "IfcIdentifier":
                        propValue = singleVal.GetAttributeAsString("wrappedValue");
                        break;
                    case "IfcBoolean":
                        propValue = singleVal.GetAttributeAsBool("wrappedValue").ToString();
                        break;
                    case "IfcInteger":
                        propValue = singleVal.GetAttributeAsInt("wrappedValue").ToString();
                        break;
                    case "IfcReal":
                        propValue = singleVal.GetAttributeAsDouble("wrappedValue")
                            .ToString(CultureInfo.InvariantCulture);
                        break;
                    default:
                        //Debug.LogError($"Unexpected property type: {type}");
                        break;
                    
                    //throw new NotImplementedException();
                }
                return propValue;
            }
            return null;
        }
        
    }
}