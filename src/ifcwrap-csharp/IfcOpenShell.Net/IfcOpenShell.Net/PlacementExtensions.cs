using System;
using System.IO;
using System.Numerics;
using System.Numerics.Double;

namespace IfcOpenShell
{
    public static class PlacementExtensions
    {
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
        public static DoubleMatrix4x4 GetLocalPlacement(EntityInstance placement)
        {
            if (placement == null)
            {
                return DoubleMatrix4x4.Identity;
            }

            DoubleMatrix4x4 parent;
            if (!placement.TryGetAttributeAsEntity("PlacementRelTo", out var relTo))
            {
                parent = DoubleMatrix4x4.Identity;
            }
            else
            {
                parent = GetLocalPlacement(relTo);
            }
            if(placement.TryGetAttributeAsEntity("RelativePlacement", out var relativePlacement))
            {
                return DoubleMatrix4x4.Multiply(parent,
                    GetAxis2Placement(relativePlacement));
            }
            // this case probably cannot happen, as it does not exist in the python implementation
            return parent;
        }
        
        // TODO: Double precision matrix / vector implementations :crying:
        public static DoubleMatrix4x4 GetAxis2Placement(EntityInstance placement)
        {
            if (placement == null)
            {
                return DoubleMatrix4x4.Identity;
            }

            DoubleVector3 x = DoubleVector3.UnitX;
            DoubleVector3 z = DoubleVector3.UnitZ;
            DoubleVector3 o = DoubleVector3.UnitY;
            
            
            var ifcClass = placement.is_a();
            
            switch (ifcClass)
            {
                case "IfcAxis2Placement3D":
                case "IfcAxis2PlacementLinear":
                    if (placement.TryGetAttributeAsEntity("Axis", out var axis) &&
                        axis.TryGetAttributeAsDoubleList("DirectionRatios", out var ratios))
                    {
                        z = new DoubleVector3(ratios[0], ratios[1], ratios[2]);
                    }
                    
                    if (placement.TryGetAttributeAsEntity("RefDirection", out var refAxis) &&
                        refAxis.TryGetAttributeAsDoubleList("DirectionRatios", out var r2))
                    {
                        x = new DoubleVector3(r2[0], r2[1], r2[2]);
                    }

                    // Location can be one of multiple types, but only IfcCartesianPoint is supported
                    if (placement.TryGetAttributeAsEntity("Location", out var location) &&
                        location.TryGetAttributeAsDoubleList("Coordinates", out var coords))
                    {
                        o = new DoubleVector3(coords[0], coords[1], coords[2]);
                    }
                    else
                    {
                        Console.WriteLine($"WARNING. Placement location of type {location.is_a()} is not yet supported " +
                                          $"and placement {placement} may be placed incorrectly.");
                        o = DoubleVector3.Zero;
                    }
                    break;
                case "IfcAxis2Placement2D":
                    if (placement.TryGetAttributeAsEntity("RefDirection", out var refAxis2D) &&
                        refAxis2D.TryGetAttributeAsDoubleList("DirectionRatios", out var r2d))
                    {
                        x = new DoubleVector3(r2d[0], r2d[1], 0);
                    }
                    else
                    {
                        x = DoubleVector3.UnitX;
                    }
                    if (placement.TryGetAttributeAsEntity("Location", out var location2d) &&
                        location2d.TryGetAttributeAsDoubleList("Coordinates", out var coords2d))
                    {
                        o = new DoubleVector3(coords2d[0], coords2d[1], 0.0);
                    }
                    else
                    {
                        Console.WriteLine($"WARNING. Placement {ifcClass} is missing attribute `Location` or its `Coordinates`.");
                        o = DoubleVector3.Zero;
                    }
                    
                    break;
                
                case "IfcAxis1Placement":
                    if (placement.TryGetAttributeAsEntity("Axis", out var axis1))
                    {
                        z = axis1.TryGetAttributeAsDoubleList("DirectionRatios", out var ratios1)
                            ? new DoubleVector3(ratios1[0], ratios1[1], ratios1[2])
                            : DoubleVector3.UnitZ;
                        
                        if (placement.TryGetAttributeAsEntity("Location", out var location1) &&
                            location1.TryGetAttributeAsDoubleList("Coordinates", out var coords1))
                        {
                            o = new DoubleVector3(coords1[0], coords1[1], coords1[2]);
                        }
                        else
                        {
                            Console.WriteLine($"WARNING. Placement {ifcClass} is missing attribute `Location` or its `Coordinates`.");
                            o = DoubleVector3.Zero;
                        }
                    }
                    
                    break;
                default:
                    throw new InvalidDataException($"IfcPlacement does not have a subtype of {ifcClass}");
            }
            return AxesToMatrix(o,z,x);
                
                
            DoubleMatrix4x4 AxesToMatrix(DoubleVector3 o, DoubleVector3 z, DoubleVector3 x)
            {
                var x_ = new DoubleVector4(DoubleVector3.Normalize(x), 0.0);
                var z_ = new DoubleVector4(DoubleVector3.Normalize(z), 0.0);
                var y_ = new DoubleVector4(DoubleVector3.Normalize(DoubleVector3.Cross(z, x)), 0.0);
                return DoubleMatrix4x4.FromRowVectors(x_, y_, z_, DoubleVector4.UnitW);

            }
        }
    }
}