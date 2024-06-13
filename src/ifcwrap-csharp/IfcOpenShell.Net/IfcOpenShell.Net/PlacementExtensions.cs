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

            var ifcClass = placement.is_a();
            
            switch (ifcClass)
            {
                case "IfcAxis2Placement3D":
                case "IfcAxis2PlacementLinear":
                    if (placement.TryGetAttributeAsEntity("Axis", out var axis) &&
                        axis.TryGetAttributeAsDoubleList("DirectionRatios", out var ratios))
                    {
                        
                    }
                    break;
                
                case "IfcAxis2Placement2D":
                    break;
                
                case "IfcAxis1Placement":
                    break;
                default:
                    throw new InvalidDataException($"IfcPlacement does not have a subtype of {ifcClass}");
            }
        }
    }
}