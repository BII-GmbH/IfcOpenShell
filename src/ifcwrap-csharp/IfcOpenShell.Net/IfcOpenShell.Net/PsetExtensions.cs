using System;
using System.Collections.Generic;

namespace IfcOpenShell
{
    public static class PsetExtensions
    {
        public static IReadOnlyDictionary<string, EntityInstance> GetPset(this EntityInstance element,
            string psetName, 
            string? propertyName = null,
            bool onlyPsets = false, 
            bool onlyQtos = false,
            bool shouldInherit = false,
            bool verbose = false
        )
        {
            EntityInstance pset = null;
            EntityInstance type_pset = null;
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
                    //var elemType = element.get

                    throw new NotImplementedException();
                }

                foreach (var relationship in definedBy)
                {
                    if (relationship.is_a("IfcRelDefinesByProperties") &&
                        relationship.TryGetAttributeAsEntity("RelatingPropertyDefinition", out var definition) &&
                        definition.TryGetAttributeAsString("Name", out var pname) && pname.Equals(psetName))
                    {
                        pset = definition;
                        break;
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
                return new Dictionary<string, EntityInstance>();
            }


            if (propertyName == null)
            {
                if (type_pset != null)
                {
                    throw new NotImplementedException();
                }

                return getPropertyDefinition(pset, verbose:verbose);
            }
            var value = getPropertyDefinition(pset, propertyName: propertyName, verbose: verbose);
            
            if(value is null && type_pset != null)
            {
                //return type_pset;
            }
            return value;
        }

        private static IReadOnlyDictionary<string, EntityInstance> getPropertyDefinition(EntityInstance definition,
            string? propertyName = null, bool verbose = false)
        {
            if (definition is null)
                return null;

            var ifcClass = definition.is_a();

            if (propertyName != null)
            {
                throw new NotImplementedException();
            }

            var properties = new Dictionary<string, EntityInstance>();

            switch (ifcClass)
            {
                case "IfcElementQuantity":
                    if (definition.TryGetAttributeAsEntityList("Quantities", out var quantities))
                    {
                        var quantitiesProps = getQuantities(quantities, verbose);
                        foreach (var (name, value) in quantitiesProps)
                        {
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
                            properties.Add(definition.get_argument_name(propIndex), prop);
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

        private static IReadOnlyDictionary<string, EntityInstance> getProperties(IEnumerable<EntityInstance> properties, bool verbose = false)
        {
            var result = new Dictionary<string, EntityInstance>();

            foreach (var prop in properties)
            {
                var ifcClass = prop.is_a();
                var name = prop.TryGetAttributeAsString("Name", out var n) ? n : "Unknown";
                
                // TODO: actually read property values
                
                result.Add(name, null);
            }
            return result;
        }
        
        private static IReadOnlyDictionary<string, EntityInstance> getQuantities(IEnumerable<EntityInstance> quantities, bool verbose = false)
        {
            
            foreach (var quantity in quantities)
            {
                var quantityName = quantity;
            }
            return null;
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