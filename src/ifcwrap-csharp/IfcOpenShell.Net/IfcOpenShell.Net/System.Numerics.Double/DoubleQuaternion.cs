// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if NET5_0_OR_GREATER
using System.Runtime.Intrinsics;
#endif

namespace System.Numerics.Double
{
    /// <summary>Represents a vector that is used to encode three-dimensional physical rotations.</summary>
    /// <remarks>The <see cref="System.Numerics.Quaternion" /> structure is used to efficiently rotate an object about the (x,y,z) vector by the angle theta, where:
    /// <c>w = cos(theta/2)</c></rema
    public struct DoubleQuaternion : IEquatable<DoubleQuaternion>
    {
        private const double SlerpEpsilon = 1e-6f;

        /// <summary>The X value of the vector component of the quaternion.</summary>
        public double X;

        /// <summary>The Y value of the vector component of the quaternion.</summary>
        public double Y;

        /// <summary>The Z value of the vector component of the quaternion.</summary>
        public double Z;

        /// <summary>The rotation component of the quaternion.</summary>
        public double W;

        internal const int Count = 4;

        /// <summary>Constructs a quaternion from the specified components.</summary>
        /// <param name="x">The value to assign to the X component of the quaternion.</param>
        /// <param name="y">The value to assign to the Y component of the quaternion.</param>
        /// <param name="z">The value to assign to the Z component of the quaternion.</param>
        /// <param name="w">The value to assign to the W component of the quaternion.</param>
        public DoubleQuaternion(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>Creates a quaternion from the specified vector and rotation parts.</summary>
        /// <param name="vectorPart">The vector part of the quaternion.</param>
        /// <param name="scalarPart">The rotation part of the quaternion.</param>
        public DoubleQuaternion(Vector3 vectorPart, double scalarPart)
        {
            X = vectorPart.X;
            Y = vectorPart.Y;
            Z = vectorPart.Z;
            W = scalarPart;
        }

        /// <summary>Gets a quaternion that represents a zero.</summary>
        /// <value>A quaternion whose values are <c>(0, 0, 0, 0)</c>.</value>
        public static DoubleQuaternion Zero
        {
            get => default;
        }

        /// <summary>Gets a quaternion that represents no rotation.</summary>
        /// <value>A quaternion whose values are <c>(0, 0, 0, 1)</c>.</value>
        public static DoubleQuaternion Identity
        {
            get => new DoubleQuaternion(0, 0, 0, 1);
        }

        public double this[int index]
        {
            get => GetElement(this, index);
            set => this = WithElement(this, index, value);
        }

        /// <summary>Gets the element at the specified index.</summary>
        /// <param name="quaternion">The vector of the element to get.</param>
        /// <param name="index">The index of the element to get.</param>
        /// <returns>The value of the element at <paramref name="index" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
        internal static double GetElement(DoubleQuaternion quaternion, int index)
        {
            if ((uint)index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return GetElementUnsafe(ref quaternion, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double GetElementUnsafe(ref DoubleQuaternion quaternion, int index)
        {
            Debug.Assert(index >= 0 && index < Count);
            return Unsafe.Add(ref Unsafe.As<DoubleQuaternion, double>(ref quaternion), index);
        }

        /// <summary>Sets the element at the specified index.</summary>
        /// <param name="quaternion">The vector of the element to get.</param>
        /// <param name="index">The index of the element to set.</param>
        /// <param name="value">The value of the element to set.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
        internal static DoubleQuaternion WithElement(DoubleQuaternion quaternion, int index, double value)
        {
            if ((uint)index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            DoubleQuaternion result = quaternion;
            SetElementUnsafe(ref result, index, value);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SetElementUnsafe(ref DoubleQuaternion quaternion, int index, double value)
        {
            Debug.Assert(index >= 0 && index < Count);
            Unsafe.Add(ref Unsafe.As<DoubleQuaternion, double>(ref quaternion), index) = value;
        }

        /// <summary>Gets a value that indicates whether the current instance is the identity quaternion.</summary>
        /// <value><see langword="true" /> if the current instance is the identity quaternion; otherwise, <see langword="false" />.</value>
        /// <altmember cref="System.Numerics.Quaternion.Identity"/>
        public readonly bool IsIdentity
        {
            get => this == Identity;
        }

        /// <summary>Adds each element in one quaternion with its corresponding element in a second quaternion.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The quaternion that contains the summed values of <paramref name="value1" /> and <paramref name="value2" />.</returns>
        /// <remarks>The <see cref="System.Numerics.Quaternion.op_Addition" /> method defines the operation of the addition operator for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
        public static DoubleQuaternion operator +(DoubleQuaternion value1, DoubleQuaternion value2)
        {
            DoubleQuaternion ans;

            ans.X = value1.X + value2.X;
            ans.Y = value1.Y + value2.Y;
            ans.Z = value1.Z + value2.Z;
            ans.W = value1.W + value2.W;

            return ans;
        }

        /// <summary>Divides one quaternion by a second quaternion.</summary>
        /// <param name="value1">The dividend.</param>
        /// <param name="value2">The divisor.</param>
        /// <returns>The quaternion that results from dividing <paramref name="value1" /> by <paramref name="value2" />.</returns>
        /// <remarks>The <see cref="System.Numerics.Quaternion.op_Division" /> method defines the division operation for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
        public static DoubleQuaternion operator /(DoubleQuaternion value1, DoubleQuaternion value2)
        {
            DoubleQuaternion ans;

            double q1x = value1.X;
            double q1y = value1.Y;
            double q1z = value1.Z;
            double q1w = value1.W;

            //-------------------------------------
            // Inverse part.
            double ls = value2.X * value2.X + value2.Y * value2.Y +
                       value2.Z * value2.Z + value2.W * value2.W;
            double invNorm = 1.0f / ls;

            double q2x = -value2.X * invNorm;
            double q2y = -value2.Y * invNorm;
            double q2z = -value2.Z * invNorm;
            double q2w = value2.W * invNorm;

            //-------------------------------------
            // Multiply part.

            // cross(av, bv)
            double cx = q1y * q2z - q1z * q2y;
            double cy = q1z * q2x - q1x * q2z;
            double cz = q1x * q2y - q1y * q2x;

            double dot = q1x * q2x + q1y * q2y + q1z * q2z;

            ans.X = q1x * q2w + q2x * q1w + cx;
            ans.Y = q1y * q2w + q2y * q1w + cy;
            ans.Z = q1z * q2w + q2z * q1w + cz;
            ans.W = q1w * q2w - dot;

            return ans;
        }

        /// <summary>Returns a value that indicates whether two quaternions are equal.</summary>
        /// <param name="value1">The first quaternion to compare.</param>
        /// <param name="value2">The second quaternion to compare.</param>
        /// <returns><see langword="true" /> if the two quaternions are equal; otherwise, <see langword="false" />.</returns>
        /// <remarks>Two quaternions are equal if each of their corresponding components is equal.
        /// The <see cref="System.Numerics.Quaternion.op_Equality" /> method defines the operation of the equality operator for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
        public static bool operator ==(DoubleQuaternion value1, DoubleQuaternion value2)
        {
            return (value1.X == value2.X)
                && (value1.Y == value2.Y)
                && (value1.Z == value2.Z)
                && (value1.W == value2.W);
        }

        /// <summary>Returns a value that indicates whether two quaternions are not equal.</summary>
        /// <param name="value1">The first quaternion to compare.</param>
        /// <param name="value2">The second quaternion to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="value1" /> and <paramref name="value2" /> are not equal; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(DoubleQuaternion value1, DoubleQuaternion value2)
        {
            return !(value1 == value2);
        }

        /// <summary>Returns the quaternion that results from multiplying two quaternions together.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The product quaternion.</returns>
        /// <remarks>The <see cref="System.Numerics.Quaternion.op_Multiply" /> method defines the operation of the multiplication operator for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
        public static DoubleQuaternion operator *(DoubleQuaternion value1, DoubleQuaternion value2)
        {
            DoubleQuaternion ans;

            double q1x = value1.X;
            double q1y = value1.Y;
            double q1z = value1.Z;
            double q1w = value1.W;

            double q2x = value2.X;
            double q2y = value2.Y;
            double q2z = value2.Z;
            double q2w = value2.W;

            // cross(av, bv)
            double cx = q1y * q2z - q1z * q2y;
            double cy = q1z * q2x - q1x * q2z;
            double cz = q1x * q2y - q1y * q2x;

            double dot = q1x * q2x + q1y * q2y + q1z * q2z;

            ans.X = q1x * q2w + q2x * q1w + cx;
            ans.Y = q1y * q2w + q2y * q1w + cy;
            ans.Z = q1z * q2w + q2z * q1w + cz;
            ans.W = q1w * q2w - dot;

            return ans;
        }

        /// <summary>Returns the quaternion that results from scaling all the components of a specified quaternion by a scalar factor.</summary>
        /// <param name="value1">The source quaternion.</param>
        /// <param name="value2">The scalar value.</param>
        /// <returns>The scaled quaternion.</returns>
        /// <remarks>The <see cref="System.Numerics.Quaternion.op_Multiply" /> method defines the operation of the multiplication operator for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
        public static DoubleQuaternion operator *(DoubleQuaternion value1, double value2)
        {
            DoubleQuaternion ans;

            ans.X = value1.X * value2;
            ans.Y = value1.Y * value2;
            ans.Z = value1.Z * value2;
            ans.W = value1.W * value2;

            return ans;
        }

        /// <summary>Subtracts each element in a second quaternion from its corresponding element in a first quaternion.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The quaternion containing the values that result from subtracting each element in <paramref name="value2" /> from its corresponding element in <paramref name="value1" />.</returns>
        /// <remarks>The <see cref="System.Numerics.Quaternion.op_Subtraction" /> method defines the operation of the subtraction operator for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
        public static DoubleQuaternion operator -(DoubleQuaternion value1, DoubleQuaternion value2)
        {
            DoubleQuaternion ans;

            ans.X = value1.X - value2.X;
            ans.Y = value1.Y - value2.Y;
            ans.Z = value1.Z - value2.Z;
            ans.W = value1.W - value2.W;

            return ans;
        }

        /// <summary>Reverses the sign of each component of the quaternion.</summary>
        /// <param name="value">The quaternion to negate.</param>
        /// <returns>The negated quaternion.</returns>
        /// <remarks>The <see cref="System.Numerics.Quaternion.op_UnaryNegation" /> method defines the operation of the unary negation operator for <see cref="System.Numerics.Quaternion" /> objects.</remarks>
        public static DoubleQuaternion operator -(DoubleQuaternion value)
        {
            DoubleQuaternion ans;

            ans.X = -value.X;
            ans.Y = -value.Y;
            ans.Z = -value.Z;
            ans.W = -value.W;

            return ans;
        }

        /// <summary>Adds each element in one quaternion with its corresponding element in a second quaternion.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The quaternion that contains the summed values of <paramref name="value1" /> and <paramref name="value2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DoubleQuaternion Add(DoubleQuaternion value1, DoubleQuaternion value2)
        {
            return value1 + value2;
        }

        /// <summary>Concatenates two quaternions.</summary>
        /// <param name="value1">The first quaternion rotation in the series.</param>
        /// <param name="value2">The second quaternion rotation in the series.</param>
        /// <returns>A new quaternion representing the concatenation of the <paramref name="value1" /> rotation followed by the <paramref name="value2" /> rotation.</returns>
        public static DoubleQuaternion Concatenate(DoubleQuaternion value1, DoubleQuaternion value2)
        {
            DoubleQuaternion ans;

            // Concatenate rotation is actually q2 * q1 instead of q1 * q2.
            // So that's why value2 goes q1 and value1 goes q2.
            double q1x = value2.X;
            double q1y = value2.Y;
            double q1z = value2.Z;
            double q1w = value2.W;

            double q2x = value1.X;
            double q2y = value1.Y;
            double q2z = value1.Z;
            double q2w = value1.W;

            // cross(av, bv)
            double cx = q1y * q2z - q1z * q2y;
            double cy = q1z * q2x - q1x * q2z;
            double cz = q1x * q2y - q1y * q2x;

            double dot = q1x * q2x + q1y * q2y + q1z * q2z;

            ans.X = q1x * q2w + q2x * q1w + cx;
            ans.Y = q1y * q2w + q2y * q1w + cy;
            ans.Z = q1z * q2w + q2z * q1w + cz;
            ans.W = q1w * q2w - dot;

            return ans;
        }

        /// <summary>Returns the conjugate of a specified quaternion.</summary>
        /// <param name="value">The quaternion.</param>
        /// <returns>A new quaternion that is the conjugate of <see langword="value" />.</returns>
        public static DoubleQuaternion Conjugate(DoubleQuaternion value)
        {
            DoubleQuaternion ans;

            ans.X = -value.X;
            ans.Y = -value.Y;
            ans.Z = -value.Z;
            ans.W = value.W;

            return ans;
        }

        /// <summary>Creates a quaternion from a unit vector and an angle to rotate around the vector.</summary>
        /// <param name="axis">The unit vector to rotate around.</param>
        /// <param name="angle">The angle, in radians, to rotate around the vector.</param>
        /// <returns>The newly created quaternion.</returns>
        /// <remarks><paramref name="axis" /> vector must be normalized before calling this method or the resulting <see cref="System.Numerics.Quaternion" /> will be incorrect.</remarks>
        public static DoubleQuaternion CreateFromAxisAngle(Vector3 axis, double angle)
        {
            DoubleQuaternion ans;

            double halfAngle = angle * 0.5f;
            double s = Math.Sin(halfAngle);
            double c = Math.Cos(halfAngle);

            ans.X = axis.X * s;
            ans.Y = axis.Y * s;
            ans.Z = axis.Z * s;
            ans.W = c;

            return ans;
        }

        /// <summary>Creates a quaternion from the specified rotation matrix.</summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <returns>The newly created quaternion.</returns>
        public static DoubleQuaternion CreateFromRotationMatrix(DoubleMatrix4x4 matrix)
        {
            double trace = matrix.M11 + matrix.M22 + matrix.M33;

            DoubleQuaternion q = default;

            if (trace > 0.0f)
            {
                double s = Math.Sqrt(trace + 1.0f);
                q.W = s * 0.5f;
                s = 0.5f / s;
                q.X = (matrix.M23 - matrix.M32) * s;
                q.Y = (matrix.M31 - matrix.M13) * s;
                q.Z = (matrix.M12 - matrix.M21) * s;
            }
            else
            {
                if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
                {
                    double s = Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
                    double invS = 0.5f / s;
                    q.X = 0.5f * s;
                    q.Y = (matrix.M12 + matrix.M21) * invS;
                    q.Z = (matrix.M13 + matrix.M31) * invS;
                    q.W = (matrix.M23 - matrix.M32) * invS;
                }
                else if (matrix.M22 > matrix.M33)
                {
                    double s = Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
                    double invS = 0.5f / s;
                    q.X = (matrix.M21 + matrix.M12) * invS;
                    q.Y = 0.5f * s;
                    q.Z = (matrix.M32 + matrix.M23) * invS;
                    q.W = (matrix.M31 - matrix.M13) * invS;
                }
                else
                {
                    double s = Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
                    double invS = 0.5f / s;
                    q.X = (matrix.M31 + matrix.M13) * invS;
                    q.Y = (matrix.M32 + matrix.M23) * invS;
                    q.Z = 0.5f * s;
                    q.W = (matrix.M12 - matrix.M21) * invS;
                }
            }

            return q;
        }

        /// <summary>Creates a new quaternion from the given yaw, pitch, and roll.</summary>
        /// <param name="yaw">The yaw angle, in radians, around the Y axis.</param>
        /// <param name="pitch">The pitch angle, in radians, around the X axis.</param>
        /// <param name="roll">The roll angle, in radians, around the Z axis.</param>
        /// <returns>The resulting quaternion.</returns>
        public static DoubleQuaternion CreateFromYawPitchRoll(double yaw, double pitch, double roll)
        {
            //  Roll first, about axis the object is facing, then
            //  pitch upward, then yaw to face into the new heading
            double sr, cr, sp, cp, sy, cy;

            double halfRoll = roll * 0.5f;
            sr = Math.Sin(halfRoll);
            cr = Math.Cos(halfRoll);

            double halfPitch = pitch * 0.5f;
            sp = Math.Sin(halfPitch);
            cp = Math.Cos(halfPitch);

            double halfYaw = yaw * 0.5f;
            sy = Math.Sin(halfYaw);
            cy = Math.Cos(halfYaw);

            DoubleQuaternion result;

            result.X = cy * sp * cr + sy * cp * sr;
            result.Y = sy * cp * cr - cy * sp * sr;
            result.Z = cy * cp * sr - sy * sp * cr;
            result.W = cy * cp * cr + sy * sp * sr;

            return result;
        }

        /// <summary>Divides one quaternion by a second quaternion.</summary>
        /// <param name="value1">The dividend.</param>
        /// <param name="value2">The divisor.</param>
        /// <returns>The quaternion that results from dividing <paramref name="value1" /> by <paramref name="value2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DoubleQuaternion Divide(DoubleQuaternion value1, DoubleQuaternion value2)
        {
            return value1 / value2;
        }

        /// <summary>Calculates the dot product of two quaternions.</summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The dot product.</returns>
        public static double Dot(DoubleQuaternion quaternion1, DoubleQuaternion quaternion2)
        {
            return quaternion1.X * quaternion2.X +
                   quaternion1.Y * quaternion2.Y +
                   quaternion1.Z * quaternion2.Z +
                   quaternion1.W * quaternion2.W;
        }

        /// <summary>Returns the inverse of a quaternion.</summary>
        /// <param name="value">The quaternion.</param>
        /// <returns>The inverted quaternion.</returns>
        public static DoubleQuaternion Inverse(DoubleQuaternion value)
        {
            //  -1   (       a              -v       )
            // q   = ( -------------   ------------- )
            //       (  a^2 + |v|^2  ,  a^2 + |v|^2  )

            DoubleQuaternion ans;

            double ls = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;
            double invNorm = 1.0f / ls;

            ans.X = -value.X * invNorm;
            ans.Y = -value.Y * invNorm;
            ans.Z = -value.Z * invNorm;
            ans.W = value.W * invNorm;

            return ans;
        }

        /// <summary>Performs a linear interpolation between two quaternions based on a value that specifies the weighting of the second quaternion.</summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="amount">The relative weight of <paramref name="quaternion2" /> in the interpolation.</param>
        /// <returns>The interpolated quaternion.</returns>
        public static DoubleQuaternion Lerp(DoubleQuaternion quaternion1, DoubleQuaternion quaternion2, double amount)
        {
            double t = amount;
            double t1 = 1.0f - t;

            DoubleQuaternion r = default;

            double dot = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y +
                        quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;

            if (dot >= 0.0f)
            {
                r.X = t1 * quaternion1.X + t * quaternion2.X;
                r.Y = t1 * quaternion1.Y + t * quaternion2.Y;
                r.Z = t1 * quaternion1.Z + t * quaternion2.Z;
                r.W = t1 * quaternion1.W + t * quaternion2.W;
            }
            else
            {
                r.X = t1 * quaternion1.X - t * quaternion2.X;
                r.Y = t1 * quaternion1.Y - t * quaternion2.Y;
                r.Z = t1 * quaternion1.Z - t * quaternion2.Z;
                r.W = t1 * quaternion1.W - t * quaternion2.W;
            }

            // Normalize it.
            double ls = r.X * r.X + r.Y * r.Y + r.Z * r.Z + r.W * r.W;
            double invNorm = 1.0f / Math.Sqrt(ls);

            r.X *= invNorm;
            r.Y *= invNorm;
            r.Z *= invNorm;
            r.W *= invNorm;

            return r;
        }

        /// <summary>Returns the quaternion that results from multiplying two quaternions together.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The product quaternion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DoubleQuaternion Multiply(DoubleQuaternion value1, DoubleQuaternion value2)
        {
            return value1 * value2;
        }

        /// <summary>Returns the quaternion that results from scaling all the components of a specified quaternion by a scalar factor.</summary>
        /// <param name="value1">The source quaternion.</param>
        /// <param name="value2">The scalar value.</param>
        /// <returns>The scaled quaternion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DoubleQuaternion Multiply(DoubleQuaternion value1, double value2)
        {
            return value1 * value2;
        }

        /// <summary>Reverses the sign of each component of the quaternion.</summary>
        /// <param name="value">The quaternion to negate.</param>
        /// <returns>The negated quaternion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DoubleQuaternion Negate(DoubleQuaternion value)
        {
            return -value;
        }

        /// <summary>Divides each component of a specified <see cref="System.Numerics.Quaternion" /> by its length.</summary>
        /// <param name="value">The quaternion to normalize.</param>
        /// <returns>The normalized quaternion.</returns>
        public static DoubleQuaternion Normalize(DoubleQuaternion value)
        {
            DoubleQuaternion ans;

            double ls = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;

            double invNorm = 1.0f / Math.Sqrt(ls);

            ans.X = value.X * invNorm;
            ans.Y = value.Y * invNorm;
            ans.Z = value.Z * invNorm;
            ans.W = value.W * invNorm;

            return ans;
        }

        /// <summary>Interpolates between two quaternions, using spherical linear interpolation.</summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="amount">The relative weight of the second quaternion in the interpolation.</param>
        /// <returns>The interpolated quaternion.</returns>
        public static DoubleQuaternion Slerp(DoubleQuaternion quaternion1, DoubleQuaternion quaternion2, double amount)
        {
            double t = amount;

            double cosOmega = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y +
                             quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;

            bool flip = false;

            if (cosOmega < 0.0f)
            {
                flip = true;
                cosOmega = -cosOmega;
            }

            double s1, s2;

            if (cosOmega > (1.0f - SlerpEpsilon))
            {
                // Too close, do straight linear interpolation.
                s1 = 1.0f - t;
                s2 = (flip) ? -t : t;
            }
            else
            {
                double omega = Math.Acos(cosOmega);
                double invSinOmega = 1 / Math.Sin(omega);

                s1 = Math.Sin((1.0f - t) * omega) * invSinOmega;
                s2 = (flip)
                    ? -Math.Sin(t * omega) * invSinOmega
                    : Math.Sin(t * omega) * invSinOmega;
            }

            DoubleQuaternion ans;

            ans.X = s1 * quaternion1.X + s2 * quaternion2.X;
            ans.Y = s1 * quaternion1.Y + s2 * quaternion2.Y;
            ans.Z = s1 * quaternion1.Z + s2 * quaternion2.Z;
            ans.W = s1 * quaternion1.W + s2 * quaternion2.W;

            return ans;
        }

        /// <summary>Subtracts each element in a second quaternion from its corresponding element in a first quaternion.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The quaternion containing the values that result from subtracting each element in <paramref name="value2" /> from its corresponding element in <paramref name="value1" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DoubleQuaternion Subtract(DoubleQuaternion value1, DoubleQuaternion value2)
        {
            return value1 - value2;
        }

        /// <summary>Returns a value that indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><see langword="true" /> if the current instance and <paramref name="obj" /> are equal; otherwise, <see langword="false" />. If <paramref name="obj" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
        /// <remarks>The current instance and <paramref name="obj" /> are equal if <paramref name="obj" /> is a <see cref="System.Numerics.Quaternion" /> object and the corresponding components of each matrix are equal.</remarks>
        public override readonly bool Equals(object? obj)
        {
            return (obj is DoubleQuaternion other) && Equals(other);
        }

        /// <summary>Returns a value that indicates whether this instance and another quaternion are equal.</summary>
        /// <param name="other">The other quaternion.</param>
        /// <returns><see langword="true" /> if the two quaternions are equal; otherwise, <see langword="false" />.</returns>
        /// <remarks>Two quaternions are equal if each of their corresponding components is equal.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(DoubleQuaternion other)
        {
            // This function needs to account for floating-point equality around NaN
            // and so must behave equivalently to the underlying double/double.Equals

#if NET5_0_OR_GREATER
            if (Vector128.IsHardwareAccelerated)
            {
                return Vector128.LoadUnsafe(ref Unsafe.AsRef(in X)).Equals(Vector128.LoadUnsafe(ref other.X));
            }
#endif
            return SoftwareFallback(in this, other);

            static bool SoftwareFallback(in DoubleQuaternion self, DoubleQuaternion other)
            {
                return self.X.Equals(other.X)
                    && self.Y.Equals(other.Y)
                    && self.Z.Equals(other.Z)
                    && self.W.Equals(other.W);
            }
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>The hash code.</returns>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, W);
        }

        /// <summary>Calculates the length of the quaternion.</summary>
        /// <returns>The computed length of the quaternion.</returns>
        public readonly double Length()
        {
            double lengthSquared = LengthSquared();
            return Math.Sqrt(lengthSquared);
        }

        /// <summary>Calculates the squared length of the quaternion.</summary>
        /// <returns>The length squared of the quaternion.</returns>
        public readonly double LengthSquared()
        {
            return X * X + Y * Y + Z * Z + W * W;
        }

        /// <summary>Returns a string that represents this quaternion.</summary>
        /// <returns>The string representation of this quaternion.</returns>
        /// <remarks>The numeric values in the returned string are formatted by using the conventions of the current culture. For example, for the en-US culture, the returned string might appear as <c>{X:1.1 Y:2.2 Z:3.3 W:4.4}</c>.</remarks>
        public override readonly string ToString() =>
            $"{{X:{X} Y:{Y} Z:{Z} W:{W}}}";
    }
}
