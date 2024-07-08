#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace IfcOpenShell
{
    public static class PsetExtensions
    {
        public static IReadOnlyDictionary<string, ArgumentResult>? GetPset(this EntityInstance element,
            string psetName, 
            string? propertyName = null,
            bool onlyPsets = false, 
            bool onlyQtos = false,
            bool shouldInherit = true,
            bool verbose = false
        )
        {
            EntityInstance? pset = null;
            IReadOnlyDictionary<string, ArgumentResult>? typePset = null;
            if (element.is_a("IfcTypeObject") && 
                element.TryGetAttributeAsEntityList("HasPropertySets", out var typeObjectPsets))
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
            else if ((element.is_a("IfcMaterialDefinition") || element.is_a("IfcProfileDef")) && 
                       element.TryGetAttributeAsEntityList("HasProperties", out var materialPsets))
            {
                foreach (var definition in materialPsets)
                {
                    if (definition.TryGetAttributeAsString("Name", out var pname) && pname.Equals(psetName))
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
                    //throw new NotImplementedException();
                }

                foreach (var relationship in definedBy)
                {
                    
                    if (relationship.is_a("IfcRelDefinesByProperties") &&
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
                if(onlyPsets && !pset.is_a("IfcPropertySet"))
                {
                    pset = null;
                } else if(onlyQtos && !pset.is_a("IfcElementQuantity"))
                {
                    pset = null;
                }
            }

            if (typePset != null && propertyName == null)
            {
                throw new NotImplementedException();
            }

            if (pset == null && typePset == null)
            {
                // TODO: null?
                return new Dictionary<string, ArgumentResult>();
            }


            if (propertyName == null)
            {
                if (typePset != null)
                {
                    throw new NotImplementedException();
                }

                return pset.getPropertyDefinition(verbose:verbose);
            }
            var value = pset.getPropertyDefinition(propertyName: propertyName, verbose: verbose);
            
            if(value is null && typePset != null)
            {
                return typePset;
            }
            return value;
        }

        public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, ArgumentResult>>
            GetPsets(this EntityInstance instance,
                bool onlyPsets = false,
                bool onlyQtos = false,
                bool shouldInherit = true,
                bool verbose = false
                )
        {
            var results =
                new Dictionary<string, IReadOnlyDictionary<string, ArgumentResult>>();
            
            if (instance.is_a("IfcTypeObject") && 
                instance.TryGetAttributeAsEntityList("HasPropertySets", out var hasPropertySets))
            {
                foreach (var definition in hasPropertySets)
                {
                    var name = definition.TryGetAttributeAsString("Name", out var n) ? n : "Unknown";
                    if ((onlyPsets && !definition.is_a("IfcPropertySet")) ||
                        (onlyQtos && !definition.is_a("IfcElementQuantity")))
                    {
                        continue;
                    }

                    var pset = instance.getPropertyDefinition() ?? new Dictionary<string, ArgumentResult>();
                    results[name] = pset;
                }
            } else if (instance.is_a("IfcMaterialDefinition") || instance.is_a("IfcProfileDef"))
            {
                if (onlyQtos)
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
                        var typeResults = GetPsets(type, onlyPsets, onlyQtos, shouldInherit: false, verbose: verbose);
                        foreach (var kvp in typeResults)
                        {
                            results.Add(kvp.Key, kvp.Value);
                        }
                    }
                }

                foreach (var relationship in definers)
                {
                    if (relationship.is_a("IfcRelDefinesByProperties") &&
                        relationship.TryGetAttributeAsEntity("RelatingPropertyDefinition", out var def))
                    {
                        if ((onlyPsets && !def.is_a("IfcPropertySet")) || 
                            (onlyQtos && !def.is_a("IfcElementQuantity")))
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

            var ifcClass = definition.is_a();

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
                var ifcClass = prop.is_a();
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
                        throw new NotImplementedException();
                        break;
                    case "IfcPropertyListValue":
                        throw new NotImplementedException();
                        break;
                    case "IfcPropertyBoundedValue":
                        throw new NotImplementedException();
                        break;
                    case "IfcPropertyTableValue":
                        throw new NotImplementedException();
                        break;
                    case "IfcComplexProperty":
                        throw new NotImplementedException();
                        break;
                    default: break;
                }
                
                // TODO: actually read property values
                
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
                
                if (quantity.is_a("IfcPhysicalSimpleQuantity") &&
                    quantity.TryGetAttributeAsEntity("Unit", out var unit))
                {
                    result[quantityName] = new ArgumentResult.FromEntityInstance(unit);
                } else if (quantity.is_a("IfcPhysicalComplexQuantity"))
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
                        // var subQuantitiesProps = getQuantities(subQuantities, verbose);
                        // foreach (var (subName, value) in subQuantitiesProps)
                        // {
                        //     result.Add(subName, value);
                        // }
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
                
                if (quantity.is_a("IfcPhysicalSimpleQuantity"))
                {
                    // 3 IfcPhysicalSimpleQuantity.Unit 
                    result[quantityName] = new ArgumentResult.FromArgumentByType(quantity.get_argument(3));
                } else if (quantity.is_a("IfcPhysicalComplexQuantity"))
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
                        // var subQuantitiesProps = getQuantities(subQuantities, verbose);
                        // foreach (var (subName, value) in subQuantitiesProps)
                        // {
                        //     result.Add(subName, value);
                        // }
                    }
                }
            }

            // fallback: return empty dictionary
            return result;
        }
    }
}