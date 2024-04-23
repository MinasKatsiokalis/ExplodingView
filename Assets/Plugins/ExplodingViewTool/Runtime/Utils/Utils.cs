using System;
using System.Linq;
using UnityEngine;

namespace MK.ExplodingView.Utils
{   
    /// <summary>
    /// Utility functions for calculations and conversions.
    /// </summary>
    public class UtilityFunctions
    {
        #region Calculators
        /// <summary>
        /// Calculate the average point of all the <paramref name="meshFilters"/>' centers.
        /// </summary>
        /// <param name="meshFilters"></param>
        /// <returns>The average posiiton of all the meshes' centers.</returns>
        public static Vector3 GetAverageCenter(Vector3[] centers)
        {
            Vector3 finalCenter = Vector3.zero;

            foreach (Vector3 center in centers)
                finalCenter += center;

            if (centers.Length > 0)
                finalCenter /= centers.Length;

            return finalCenter;
        }

        /// <summary>
        /// Get the hierarchy depth of the <paramref name="transform"/>.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns>An integer according to depth level, 0 is the first depth leyer.</returns>
        public static int CalculateHierarchyDepth(Transform transform)
        {
            int depth = 0;
            while (transform.parent != null)
            {
                depth++;
                transform = transform.parent;
            }
            return depth;
        }
        #endregion

        #region Converters
        /// <summary>
        /// Converts the <paramref name="axis"/> to a specific transform direction.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="axis"></param>
        /// <returns>Vector3 Direction</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Vector3 AxisToDirection(Transform transform, Axis axis)
        {
            switch (axis)
            {
                case Axis.Y:
                    return transform.up;
                case Axis.X:
                    return transform.right;
                case Axis.Z:
                    return transform.forward;
                default:
                    throw new ArgumentException("Invalid axis. Please provide a single value axis at a time.");
            }
        }

        /// <summary>
        /// Converts the <paramref name="axis"/> to a specific transform direction.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="axis"></param>
        /// <returns>Vector3 Direction</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Vector3 AxisToDirection(Transform transform, ModifierAxis axis)
        {
            switch (axis)
            {
                case ModifierAxis.PosY:
                    return transform.up;
                case ModifierAxis.NegY:
                    return -transform.up;
                case ModifierAxis.PosX:
                    return transform.right;
                case ModifierAxis.NegX:
                    return -transform.right;
                case ModifierAxis.PosZ:
                    return transform.forward;
                case ModifierAxis.NegZ:
                    return -transform.forward;
                default:
                    throw new ArgumentException("Invalid axis.");
            }
        }

        /// <summary>
        /// Returns the primary axis of the <paramref name="direction"/> on local space of <paramref name="transform"/>.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="direction"></param>
        /// <returns>Vector3 Normalised Direction</returns>
        public static Vector3 DirectionToPrimaryAxis(Transform transform, Vector3 direction)
        {
            // Create an array of the six direction vectors
            Vector3[] directions = new Vector3[]
            {
                transform.right,
                -transform.right,
                transform.up,
                -transform.up,
                transform.forward,
                -transform.forward
            };

            float minAngle = float.MaxValue;
            Vector3 closestDirection = directions[0];

            foreach (Vector3 dir in directions)
            {
                float angle = Vector3.Angle(direction, dir);

                if (angle < minAngle)
                {
                    minAngle = angle;
                    closestDirection = dir;
                }
            }
            return closestDirection;
        }
        #endregion

        #region Projections
        /// <summary>
        /// Calculates a point on the line as a projection of the <paramref name="projectedPoint"/>.
        /// <paramref name="pointOnLine"/> is a point on the line and <paramref name="lineDirection"/> is the direction of the line.
        /// </summary>
        /// <param name="projectedPoint"></param>
        /// <param name="pointOnLine"></param>
        /// <param name="lineDirection"></param>
        /// <returns>The position of the new point on the line.</returns>
        public static Vector3 GetPointOnLine(Vector3 projectedPoint, Vector3 pointOnLine, Vector3 lineDirection)
        {
            // Calculate the vector from the point on the line to the projected point position
            Vector3 toProjectedPoint = projectedPoint - pointOnLine;
            // Project this vector onto the line direction
            Vector3 projection = Vector3.Project(toProjectedPoint, lineDirection);
            // Calculate the new point on the line
            return pointOnLine + projection;
        }

        /// <summary>
        /// This is used to determine which axis to project the part on.
        /// The furthest projection point is chosen, which means this part will move opposite to the respective axis.
        /// E.g. if the part is projected on the X axis, it will move away the X axis of the "center" transform.
        /// </summary>
        /// <param name="centerTransform"></param>
        /// <param name="position"></param>
        /// <param name="directionAxis"></param>
        /// <returns>The point according to which the part will move.</returns>
        public static Vector3 GetProjectionPoint(Transform centerTransform, Vector3 position, Axis directionAxis)
        {
            Vector3 originalPosition = position;
            Vector3 centerPosition = centerTransform.position;

            Func<Transform, Axis, Vector3> getTransformDirection = (center, axis) => AxisToDirection(center, axis);
            Func<Axis, Vector3> getPointOnLine = axis => GetPointOnLine(originalPosition, centerPosition, getTransformDirection(centerTransform, axis));
            Func<Axis, float> getDistance = axis => Vector3.Distance(originalPosition, getPointOnLine(axis));

            if (directionAxis == Axis.X || directionAxis == Axis.Y || directionAxis == Axis.Z)
                return getPointOnLine(directionAxis);
            else
            {
                Axis[] axes;
                switch (directionAxis)
                {
                    case Axis.YX:
                        axes = new[] { Axis.Y, Axis.X };
                        break;
                    case Axis.XZ:
                        axes = new[] { Axis.X, Axis.Z };
                        break;
                    case Axis.YZ:
                        axes = new[] { Axis.Y, Axis.Z };
                        break;
                    default:
                        axes = new[] { Axis.Y, Axis.X, Axis.Z };
                        break;
                }
                // Find the axis that gives the maximum distance
                Axis axis = axes.Aggregate((a, b) => getDistance(a) >= getDistance(b) ? a : b);

                return getPointOnLine(axis);
            }
        }

        /// <summary>
        /// Takes a <paramref name="point"/> in 3D space and a plane defined by a <paramref name="planePoint"/> and a <paramref name="planeNormal"/>, and it calculates the projection of the point onto the plane. 
        /// The projection is the closest point on the plane to the original point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="planeNormal"></param>
        /// <param name="planePoint"></param>
        /// <returns></returns>
        public static Vector3 GetPointOnPlane(Vector3 point, Vector3 planePoint, Vector3 planeNormal)
        {
            Vector3 toPoint = point - planePoint;
            float distance = Vector3.Dot(toPoint, planeNormal);
            return point - distance * planeNormal;
        }

        /// <summary>
        /// This is used to determine which plane to project the part on.
        /// The furthest projection point is chosen, which means this part will move opposite to the respective plane.
        /// E.g. if the part is projected on the X axis-plane, it will move away the X axis-plane of the "center" transform.
        /// </summary>
        /// <param name="centerTransform"></param>
        /// <param name="position"></param>
        /// <param name="directionAxis"></param>
        /// <returns></returns>
        public static Vector3 GetProjectionPointOnPlane(Transform centerTransform, Vector3 position, Axis directionAxis)
        {
            Vector3 originalPosition = position;
            Vector3 centerPosition = centerTransform.position;

            Func<Transform, Axis, Vector3> getTransformDirection = (center, axis) => AxisToDirection(center, axis);
            Func<Axis, Vector3> getPointOnPlane = axis => GetPointOnPlane(originalPosition, centerPosition, getTransformDirection(centerTransform, axis));
            Func<Axis, float> getDistance = axis => Vector3.Distance(originalPosition, getPointOnPlane(axis));

            if (directionAxis == Axis.X || directionAxis == Axis.Y || directionAxis == Axis.Z)
                return getPointOnPlane(directionAxis);
            else
            {
                Axis[] axes;
                switch (directionAxis)
                {
                    case Axis.YX:
                        axes = new[] { Axis.Y, Axis.X };
                        break;
                    case Axis.XZ:
                        axes = new[] { Axis.X, Axis.Z };
                        break;
                    case Axis.YZ:
                        axes = new[] { Axis.Y, Axis.Z };
                        break;
                    default:
                        axes = new[] { Axis.Y, Axis.X, Axis.Z };
                        break;
                }
                // Find the axis that gives the maximum distance
                Axis axis = axes.Aggregate((a, b) => getDistance(a) >= getDistance(b) ? a : b);

                return getPointOnPlane(axis);
            }
        }
        #endregion
    }

    /// <summary>
    /// This struct is used to store the original and exploded positions of the part.
    /// </summary>
    [Serializable]
    public struct ExplodablePositions
    {
        #region Properties
        public Transform OriginalPositionPoint { set; get; }
        public Transform ExplodedPositionPoint { set; get; }
        #endregion

        #region Constructor
        public ExplodablePositions(Transform originalPositionPoint, Transform explodedPositionPoint)
        {
            OriginalPositionPoint = originalPositionPoint;
            ExplodedPositionPoint = explodedPositionPoint;
        }
        #endregion
    }
}
