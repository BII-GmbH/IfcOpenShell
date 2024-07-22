#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace IfcOpenShell.Net
{
    /// The methods in this file are incomplete duplications of the methods in ifcopenshell-python/util/element.py
    public static class PsetExtensions
    {
        public static IReadOnlyDictionary<string, ArgumentResult>? GetPset(this EntityInstance element,
            string psetName, 
            string? propertyName = null,
            bool onlyPsets = false, 
            bool onlyQuantities = false,
            bool shouldInherit = true,
            bool verbose = false
        )
        {
            EntityInstance? pset = null;
            IReadOnlyDictionary<string, ArgumentResult>? typePset = null;
            if (element.Is("IfcTypeObject") && 
                element.TryGetAttributeAsEntityList("HasPropertySets", out var typeObjectPsets) &&
                typeObjectPsets != null)
            {
                foreach (var definition in typeObjectPsets)
                {
                    if (definition.TryGetAttributeAsString("Name", out var pname) && pname.Equals(psetName))
                    {
                        pset = definition;
                        break;
                    }
                }
            } 
            else if ((element.Is("IfcMaterialDefinition") || element.Is("IfcProfileDef")) && 
                element.TryGetAttributeAsEntityList("HasProperties", out var materialPsets) &&
                materialPsets != null)
            {
                foreach (var definition in materialPsets)
                {
                    var pname = definition.GetAttributeAsString("Name");
                    if (pname.Equals(psetName))
                    {
                        pset = definition;
                        break;
                    }
                }
            } 
            else if (element.TryGetAttributeAsEntityList("IsDefinedBy", out var definedBy))
            {
                if (shouldInherit)
                {
                    var elemType = element.GetConstructionType();
                    if (elemType != null)
                        typePset = elemType.GetPset(psetName, propertyName, shouldInherit: false, verbose: verbose);
                }

                foreach (var relationship in definedBy)
                {
                    
                    if (relationship.Is("IfcRelDefinesByProperties") &&
                        relationship.TryGetAttributeAsEntity("RelatingPropertyDefinition", out var definition))
                    {
                        if (definition.TryGetAttributeAsString("Name", out var pname) && pname.Equals(psetName))
                        {
                            pset = definition;
                            break;
                        }
                    }
                }
            }

            if (pset != null)
            {
                if(onlyPsets && !pset.Is("IfcPropertySet"))
                {
                    pset = null;
                } else if(onlyQuantities && !pset.Is("IfcElementQuantity"))
                {
                    pset = null;
                }
            }

            if (typePset != null && propertyName == null)
            {
                // branch has not been encountered in our testing.
                // If this is needed in the future look at ifcopenshell-python/util/element.py:get_pset
                throw new NotImplementedException();
            }

            if (pset == null && typePset == null)
            {
                // python returns null here - i don't like null, so return empty list instead
                return new Dictionary<string, ArgumentResult>();
            }
            if (propertyName == null)
            {
                if (typePset != null)
                {
                    // Currently unreachable because the branch in line 83 is the only way this can be reached
                    // branch has not been encountered in our testing.
                    // If this is needed in the future look at ifcopenshell-python/util/element.py:get_pset
                    throw new NotImplementedException();
                }

                return pset.getPropertyDefinition(verbose:verbose);
            }
            var value = pset.getPropertyDefinition(propertyName: propertyName, verbose: verbose);
            
            if(value != null && typePset != null)
            {
                return typePset;
            }
            return value;
        }

        public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, ArgumentResult>>
            GetPsets(this EntityInstance instance,
                bool onlyPsets = false,
                bool onlyQuantities = false,
                bool shouldInherit = true,
                bool verbose = false
            )
        {
            var results =
                new Dictionary<string, IReadOnlyDictionary<string, ArgumentResult>>();
            
            if (instance.Is("IfcTypeObject") && 
                instance.TryGetAttributeAsEntityList("HasPropertySets", out var hasPropertySets))
            {
                foreach (var definition in hasPropertySets)
                {
                    var name = definition.TryGetAttributeAsString("Name", out var n) ? n : "Unknown";
                    if ((onlyPsets && !definition.Is("IfcPropertySet")) ||
                        (onlyQuantities && !definition.Is("IfcElementQuantity")))
                    {
                        continue;
                    }

                    var pset = instance.getPropertyDefinition() ?? new Dictionary<string, ArgumentResult>();
                    results[name] = pset;
                }
            } else if (instance.Is("IfcMaterialDefinition") || instance.Is("IfcProfileDef"))
            {
                if (onlyQuantities)
                    return results;
                // ifc2x3 may be missing this
                if(instance.TryGetAttributeAsEntityList("HasProperties", out var props))
                {
                    foreach (var definition in props)
                    {
                        var name = definition.TryGetAttributeAsString("Name", out var n) ? n : "Unknown";
                        var pset = instance.getPropertyDefinition() ?? new Dictionary<string, ArgumentResult>();
                        results[name] = pset;
                    }
                }
            } else if (instance.TryGetAttributeAsEntityList("IsDefinedBy", out var definers))
            {
                if (shouldInherit)
                {
                    var type = instance.GetConstructionType();
                    if (type != null)
                    {
                        var typeResults = GetPsets(type, onlyPsets, onlyQuantities, shouldInherit: false, verbose: verbose);
                        foreach (var kvp in typeResults)
                        {
                            results.Add(kvp.Key, kvp.Value);
                        }
                    }
                }

                foreach (var relationship in definers)
                {
                    if (relationship.Is("IfcRelDefinesByProperties") &&
                        relationship.TryGetAttributeAsEntity("RelatingPropertyDefinition", out var def))
                    {
                        if ((onlyPsets && !def.Is("IfcPropertySet")) || 
                            (onlyQuantities && !def.Is("IfcElementQuantity")))
                        {
                            continue;
                        }
                        var name = def.TryGetAttributeAsString("Name", out var n) ? n : "Unknown";

                        var pset = def.getPropertyDefinition() ?? new Dictionary<string, ArgumentResult>();
                        results[name] = pset;
                    }
                }
            }
            return results;
        }
        
        private static IReadOnlyDictionary<string, ArgumentResult>? getPropertyDefinition(this EntityInstance? definition,
            string? propertyName = null, bool verbose = false)
        {
            if (definition is null)
                return null;

            var ifcClass = definition.Is();

            if (propertyName != null)
            {
                throw new NotImplementedException();
            }

            var properties = new Dictionary<string, ArgumentResult>();

            switch (ifcClass)
            {
                case "IfcElementQuantity":
                    if (definition.TryGetAttributeAsEntityList("Quantities", out var quantities))
                    {
                        var quantitiesProps = getQuantities(quantities, verbose);
                        foreach (var (name, value) in quantitiesProps)
                        {
                            // TODO: what if already exists?
                            properties.Add(name, value);
                        }
                    }
                    break;
                case "IfcPropertySet":
                    getPropertiesAndAddToResult(definition, "HasProperties");
                    break;
                case "IfcMaterialProperties":
                case "IfcProfileProperties":
                    getPropertiesAndAddToResult(definition, "Properties");
                    break;
                
                default:
                    for (uint propIndex = 0; propIndex < definition.Length(); propIndex++)
                    {
                        if(definition.TryGetAttributeAtIndex(propIndex)?.TryGetValue(out var prop) ?? false)
                        {
                            properties.Add(definition.get_argument_name(propIndex), new ArgumentResult.FromEntityInstance(prop));
                        }
                    }
                    break;
                    
            }
            properties["id"] = new ArgumentResult.FromInt(definition.id());
            return properties;
            
            
            void getPropertiesAndAddToResult(EntityInstance? fromInstance, string propertyToQuery)
            {
                if (fromInstance != null && fromInstance.TryGetAttributeAsEntityList(propertyToQuery, out var propertiesList))
                {
                    var props = getProperties(propertiesList, verbose);
                    foreach (var (name, value) in props)
                    {
                        properties.Add(name, value);
                    }
                }
            }
            
        }

        private static IReadOnlyDictionary<string, ArgumentResult> getProperties(IEnumerable<EntityInstance> properties, bool verbose = false)
        {
            var result = new Dictionary<string, ArgumentResult>();

            foreach (var prop in properties)
            {
                var ifcClass = prop.Is();
                var name = prop.TryGetAttributeAsString("Name", out var n) ? n : "Unknown";

                // TODO: verbose setting
                switch (ifcClass)
                {
                    case "IfcPropertySingleValue":
                        // this diverges from the python version that directly returns the .NominalValue,
                        // but discarding the unit here makes no sense to me
                        result.Add(name, new ArgumentResult.FromEntityInstance(prop));
                        break;
                    case "IfcPropertyEnumeratedValue":
                    {
                        // argument index 2 is the enumeration values
                        var enumValues = prop.get_argument(2).TryGetAsEntityList();
                        if (enumValues.TryGetValue(out var enumConstants))
                        {
                            var stringValues = enumConstants.Select(e => e.GetAttributeAsString("wrappedValue"));
                            result.Add(name, new ArgumentResult.FromStringList(stringValues));
                        }

                        break;
                    }
                    case "IfcPropertyListValue":
                    {
                        // argument index 2 is the enumeration values
                        var enumValues = prop.get_argument(2).TryGetAsEntityList();
                        if (enumValues.TryGetValue(out var enumConstants))
                        {
                            var stringValues = enumConstants.Select(e => e.GetAttributeAsString("wrappedValue"));
                            result.Add(name, new ArgumentResult.FromStringList(stringValues));
                        }

                        break;
                    }
                    case "IfcPropertyBoundedValue":
                        // implementing this type is really easy, except for return type magic because we suddenly need to wrap a dictionary<string, EntityInstance>
                        throw new NotImplementedException();
                        break;
                    case "IfcPropertyTableValue":
                        // implementing this type is really easy, except for return type magic because we suddenly need to wrap a dictionary<string, EntityInstance>
                        throw new NotImplementedException();
                        break;
                    case "IfcComplexProperty":
                        // implementing this type is not that complicated, except for return type magic because we suddenly need to wrap a dictionary<string, EntityInstance>
                        throw new NotImplementedException();
                        break;
                    default: break;
                }
            }
            return result;
        }
        
        private static IReadOnlyDictionary<string, ArgumentResult> getQuantity(IEnumerable<EntityInstance> quantities, string searchName, bool verbose = false)
        {
            var result = new Dictionary<string, ArgumentResult>();

            foreach (var quantity in quantities)
            {
                var quantityName = quantity.TryGetAttributeAsString("Name", out var n) ? n : "Unknown";
                if(quantityName != searchName)
                    continue;
                
                if (quantity.Is("IfcPhysicalSimpleQuantity") &&
                    quantity.TryGetAttributeAsEntity("Unit", out var unit))
                {
                    result[quantityName] = new ArgumentResult.FromEntityInstance(unit);
                } else if (quantity.Is("IfcPhysicalComplexQuantity"))
                {
                    foreach (var (name, value) in quantity.GetInfo())
                    {
                        if(value != null && name != "Name" && name != "HasQuantities")
                        {
                            result.Add(name, value);
                        }
                    }

                    if (quantity.TryGetAttributeAsEntityList("HasQuantities", out var subQuantities))
                    {
                        throw new NotImplementedException();
                    }
                }
                // python impl also returns inside the loop
                return result;
                
            }

            // fallback: return empty dictionary
            return result;
        }
        
        
        private static IReadOnlyDictionary<string, ArgumentResult> getQuantities(IEnumerable<EntityInstance> quantities, bool verbose = false)
        {
            var result = new Dictionary<string, ArgumentResult>();

            foreach (var quantity in quantities)
            {
                var quantityName = quantity.TryGetAttributeAsString("Name", out var n) ? n : "Unknown";
                
                if (quantity.Is("IfcPhysicalSimpleQuantity"))
                {
                    // 3 IfcPhysicalSimpleQuantity.Unit 
                    result[quantityName] = new ArgumentResult.FromArgumentByType(quantity.get_argument(3));
                } else if (quantity.Is("IfcPhysicalComplexQuantity"))
                {
                    foreach (var (name, value) in quantity.GetInfo())
                    {
                        if(value != null && name != "Name" && name != "HasQuantities")
                        {
                            result.Add(name, value);
                        }
                    }

                    if (quantity.TryGetAttributeAsEntityList("HasQuantities", out var subQuantities))
                    {
                        throw new NotImplementedException("IfcPhysicalComplexQuantity.HasQuantities not implemented");
                    }
                }
            }

            // fallback: return empty dictionary
            return result;
        }
    }
}