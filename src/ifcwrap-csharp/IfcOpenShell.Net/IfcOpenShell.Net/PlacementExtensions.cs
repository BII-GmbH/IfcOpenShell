using System;
using System.IO;

namespace IfcOpenShell
{
    public static class PlacementExtensions
    {
        // TODO: use the matrix4 type that ifcopenshell 0.8.0 introduced
        /// <summary>
        /// Parse a local placement into a 4x4 transformation matrix.
        /// This is typically used to find the location and rotation of an element. The
        ///     transformation matrix takes the form of:
        ///
        /// .. code::
        ///
        /// [ [ x_x, y_x, z_x, x   ]
        /// [ x_y, y_y, z_y, y   ]
        /// [ x_z, y_z, z_z, z   ]
        /// [ 0.0, 0.0, 0.0, 1.0 ] ]
        ///
        /// Example:
        ///
        /// .. code:: python
        ///
        ///     placement = file.by_type("IfcBeam")[0].ObjectPlacement
        /// matrix = ifcopenshell.util.placement.get_local_placement(placement)
        ///
        /// :param placement: The IfcLocalPlacement entity
        /// :type placement: ifcopenshell.entity_instance, optional
        /// :return: A 4x4 numpy matrix
        /// :rtype: MatrixType
        /// </summary>
        /// <param name="placement">The placement of the entity</param>
        /// <returns></returns>
        public static matrix4 GetLocalPlacement(EntityInstance placement)
        {
            
            if (placement == null)
            {
                return matrix4.Identity();
            }

            matrix4 parent;
            if (!placement.TryGetAttributeAsEntity("PlacementRelTo", out var relTo))
            {
                parent = matrix4.Identity();
            }
            else
            {
                parent = GetLocalPlacement(relTo);
            }
            if(placement.TryGetAttributeAsEntity("RelativePlacement", out var relativePlacement))
            {
                return matrix4.Multiply(parent,
                    GetAxis2Placement(relativePlacement));
            }
            // this case probably cannot happen, as it does not exist in the python implementation
            return parent;
        }
        
        public static matrix4 GetAxis2Placement(EntityInstance placement)
        {
            if (placement == null)
            {
                return matrix4.Identity();
            }

            var zero = Vec3.New(0,0,0);
            var unitX = Vec3.New(1,0,0);
            var unitY = Vec3.New(0,1,0);
            var unitZ = Vec3.New(0,0,1);

            var x = unitX;
            var z = unitZ;
            var o = unitY;
            
            
            var ifcClass = placement.Is();
            switch (ifcClass)
            {
                case "IfcAxis2Placement3D":
                case "IfcAxis2PlacementLinear":
                    if (placement.TryGetAttributeAsEntity("Axis", out var axis) &&
                        axis.TryGetAttributeAsDoubleList("DirectionRatios", out var ratios))
                    {
                        z = Vec3.New(ratios[0], ratios[1], ratios[2]);
                    }
                    
                    if (placement.TryGetAttributeAsEntity("RefDirection", out var refAxis) &&
                        refAxis.TryGetAttributeAsDoubleList("DirectionRatios", out var r2))
                    {
                        x = Vec3.New(r2[0], r2[1], r2[2]);
                    }

                    // Location can be one of multiple types, but only IfcCartesianPoint is supported
                    if (placement.TryGetAttributeAsEntity("Location", out var location) &&
                        location.TryGetAttributeAsDoubleList("Coordinates", out var coords))
                    {
                        o = Vec3.New(coords[0], coords[1], coords[2]);
                    }
                    else
                    {
                        Console.WriteLine($"WARNING. Placement location of type {location.Is()} is not yet supported " +
                                          $"and placement {placement} may be placed incorrectly.");
                        o = zero;
                    }
                    break;
                case "IfcAxis2Placement2D":
                    if (placement.TryGetAttributeAsEntity("RefDirection", out var refAxis2D) &&
                        refAxis2D.TryGetAttributeAsDoubleList("DirectionRatios", out var r2d))
                    {
                        x = Vec3.New(r2d[0], r2d[1], 0);
                    }
                    else
                    {
                        x = unitX;
                    }
                    if (placement.TryGetAttributeAsEntity("Location", out var location2d) &&
                        location2d.TryGetAttributeAsDoubleList("Coordinates", out var coords2d))
                    {
                        o = Vec3.New(coords2d[0], coords2d[1], 0.0);
                    }
                    else
                    {
                        Console.WriteLine($"WARNING. Placement {ifcClass} is missing attribute `Location` or its `Coordinates`.");
                        o = zero;
                    }
                    
                    break;
                
                case "IfcAxis1Placement":
                    if (placement.TryGetAttributeAsEntity("Axis", out var axis1))
                    {
                        z = axis1.TryGetAttributeAsDoubleList("DirectionRatios", out var ratios1)
                            ? Vec3.New(ratios1[0], ratios1[1], ratios1[2])
                            : unitZ;
                        
                        if (placement.TryGetAttributeAsEntity("Location", out var location1) &&
                            location1.TryGetAttributeAsDoubleList("Coordinates", out var coords1))
                        {
                            o = Vec3.New(coords1[0], coords1[1], coords1[2]);
                        }
                        else
                        {
                            Console.WriteLine($"WARNING. Placement {ifcClass} is missing attribute `Location` or its `Coordinates`.");
                            o = zero;
                        }
                    }
                    
                    break;
                default:
                    throw new InvalidDataException($"IfcPlacement does not have a subtype of {ifcClass}");
            }
            return matrix4.FromOriginAndAxes(o,z,x);
        }
    }
}