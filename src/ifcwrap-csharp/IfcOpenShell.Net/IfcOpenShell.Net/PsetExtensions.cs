using System;
using System.Collections.Generic;
using System.Linq;

namespace IfcOpenShell
{
    public static class PsetExtensions
    {
        public static IReadOnlyDictionary<string, EntityInstanceExtensions.ArgumentResult> GetPset(this EntityInstance element,
            string psetName, 
            string? propertyName = null,
            bool onlyPsets = false, 
            bool onlyQtos = false,
            bool shouldInherit = true,
            bool verbose = false
        )
        {
            EntityInstance pset = null;
            IReadOnlyDictionary<string, EntityInstanceExtensions.ArgumentResult> type_pset = null;
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
                    var elemType = element.get_type();
                    if (elemType != null)
                        type_pset = elemType.GetPset(psetName, propertyName, shouldInherit: false, verbose: verbose);
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

            if (type_pset != null && propertyName == null)
            {
                throw new NotImplementedException();
            }

            if (pset == null && type_pset == null)
            {
                // TODO: null?
                return new Dictionary<string, EntityInstanceExtensions.ArgumentResult>();
            }


            if (propertyName == null)
            {
                if (type_pset != null)
                {
                    throw new NotImplementedException();
                }

                return pset.getPropertyDefinition(verbose:verbose);
            }
            var value = pset.getPropertyDefinition(propertyName: propertyName, verbose: verbose);
            
            if(value is null && type_pset != null)
            {
                return type_pset;
            }
            return value;
        }

        public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, EntityInstanceExtensions.ArgumentResult>>
            GetPsets(this EntityInstance instance,
                bool onlyPsets = false,
                bool onlyQtos = false,
                bool shouldInherit = true,
                bool verbose = false
                )
        {
            var results =
                new Dictionary<string, IReadOnlyDictionary<string, EntityInstanceExtensions.ArgumentResult>>();
            
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

                    var pset = instance.getPropertyDefinition() ?? new Dictionary<string, EntityInstanceExtensions.ArgumentResult>();
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
                        var pset = instance.getPropertyDefinition() ?? new Dictionary<string, EntityInstanceExtensions.ArgumentResult>();
                        results[name] = pset;
                    }
                }
            } else if (instance.TryGetAttributeAsEntityList("IsDefinedBy", out var definers))
            {
                if (shouldInherit)
                {
                    var type = instance.get_type();
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

                        var pset = instance.getPropertyDefinition() ?? new Dictionary<string, EntityInstanceExtensions.ArgumentResult>();
                        results[name] = pset;
                    }
                }
            }
            return results;
        }
        
        /// Retrieves the construction type element of an element occurrence
        ///
        /// param element: The element occurrence
        /// type: ifcopenshell.entity_instance
        /// return: The related type element
        /// rtype: Union[ifcopenshell.entity_instance, None]

        /// Example:

        ///.. code:: python

        ///   element = ifcopenshell.by_type("IfcWall")[0]
        ///   element_type = ifcopenshell.util.element.get_type(element)
        private static EntityInstance get_type(this EntityInstance entityInstance)
        {
            if (entityInstance.is_a("IfcTypeObject")) return entityInstance;
            if (entityInstance.TryGetAttributeAsEntity("IsTypedBy", out var typedBy)) return typedBy;
            if (entityInstance.TryGetAttributeAsEntityList("IsDefinedBy", out var definedBy)) // ifc2x3
            {
                foreach (var definer in definedBy)
                {
                    if (definer.is_a("IfcRelDefinesByType"))
                    {
                        return definer.TryGetAttributeAsEntity("RelatingType", out var relType) ? relType : null;
                    }
                }                
            }
            return null;
        }
                
        private static IReadOnlyDictionary<string, EntityInstanceExtensions.ArgumentResult> getPropertyDefinition(this EntityInstance definition,
            string? propertyName = null, bool verbose = false)
        {
            if (definition is null)
                return null;

            var ifcClass = definition.is_a();

            if (propertyName != null)
            {
                throw new NotImplementedException();
            }

            var properties = new Dictionary<string, EntityInstanceExtensions.ArgumentResult>();

            switch (ifcClass)
            {
                case "IfcElementQuantity":
                    if (definition.TryGetAttributeAsEntityList("Quantities", out var quantities))
                    {
                        var quantitiesProps = getQuantities(quantities, verbose);
                        foreach (var (name, value) in quantitiesProps)
                        {
                            properties.Add(name, new EntityInstanceExtensions.ArgumentResult.FromEntityInstance(value));
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
                            properties.Add(definition.get_argument_name(propIndex), new EntityInstanceExtensions.ArgumentResult.FromEntityInstance(prop));
                        }
                    }
                    break;
                    
            }
            return properties;
            
            
            void getPropertiesAndAddToResult(EntityInstance fromInstance, string propertyToQuery)
            {
                if (fromInstance.TryGetAttributeAsEntityList(propertyToQuery, out var propertiesList))
                {
                    var props = getProperties(propertiesList, verbose);
                    foreach (var (name, value) in props)
                    {
                        properties.Add(name, value);
                    }
                }
            }
            
        }

        private static IReadOnlyDictionary<string, EntityInstanceExtensions.ArgumentResult> getProperties(IEnumerable<EntityInstance> properties, bool verbose = false)
        {
            var result = new Dictionary<string, EntityInstanceExtensions.ArgumentResult>();

            foreach (var prop in properties)
            {
                var ifcClass = prop.is_a();
                var name = prop.TryGetAttributeAsString("Name", out var n) ? n : "Unknown";

                // TODO: verbose setting
                switch (ifcClass)
                {
                    case "IfcPropertySingleValue":
                        const uint IfcPropertySingleValue_NominalValue = 2;
                        var singleVal = prop.get_argument(IfcPropertySingleValue_NominalValue);
                        result.Add(name, new EntityInstanceExtensions.ArgumentResult.FromArgumentByType(singleVal));
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
        
        private static IReadOnlyDictionary<string, EntityInstance> getQuantities(IEnumerable<EntityInstance> quantities, bool verbose = false)
        {

            throw new NotImplementedException();
        }

        public static EntityArgument TryGetAttributeAtIndex(this EntityInstance instance, uint key)
        {
            if(key >= instance.Length())
            {
                throw new IndexOutOfRangeException($"Attribute index {key} out of range for instance of type {instance.is_a()}");
            }
            return instance.get_argument(key).TryGetAsEntity();
        }
        
    }
}