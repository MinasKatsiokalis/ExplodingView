using System;
using System.Linq;
using UnityEngine;

namespace MK.ExplodingView.Utils
{
    public class UtilityFunctions
    {
        /// <summary>
        /// Calculate the average point of all the <paramref name="meshFilters"/>' centers.
        /// </summary>
        /// <param name="meshFilters"></param>
        /// <returns>The average posiiton of all the meshes' centers.</returns>
        public static Vector3 GetAverageCenter(Vector3[] meshCenters)
        {
            Vector3 center = Vector3.zero;

            foreach (Vector3 meshCenter in meshCenters)
                center += meshCenter;

            if (meshCenters.Length > 0)
                center /= meshCenters.Length;

            return center;
        }

        /// <summary>
        /// Calculates a point on the line that as a projection of the <paramref name="projectedPoint"/>.
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
        /// This is used to determine which axis to project the part on.
        /// The furthest projection point is chosen, which means this part will move opposite to the respective axis.
        /// E.g. if the part is projected on the X axis, it will move away the X axis of the "center" transform.
        /// </summary>
        /// <param name="explodable"></param>
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
        /// Returns the primary axis of the <paramref name="direction"/>.
        /// Primary is the axis with the highest magnitude.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>Axis (X,Y,Z)</returns>
        public static Axis GetPrimaryDirectionAxis(Vector3 direction)
        {   
            Debug.Log(direction);
            float absX = Mathf.Abs(direction.x);
            float absY = Mathf.Abs(direction.y);
            float absZ = Mathf.Abs(direction.z);

            if (absX > absY && absX > absZ)
            {
                Debug.Log("X");
                return Axis.X;
            }
            else if (absY > absX && absY > absZ)
            {
                Debug.Log("Y");
                return Axis.Y;
            }
            else
            {
                Debug.Log("Z");
                return Axis.Z;
            }

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
    }

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

    [Serializable]
    public enum Axis
    {
        Y,
        X,
        Z,
        YX,
        YZ,
        XZ,
        XYZ
    }

    [Serializable]
    public enum ModifierAxis
    {
        PosX,
        NegX,
        PosY,
        NegY,
        PosZ,
        NegZ
    }

    [Serializable]
    public enum DistanceFactor
    {
        None,
        DistanceFromCenter,
        DistanceFromAxis
    }
}
