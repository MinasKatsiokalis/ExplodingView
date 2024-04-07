using System;
using System.Collections.Generic;
using UnityEngine;

namespace MK.ExplodingView.Utils
{
    public class UtilityFunctions
    {
        /// <summary>
        /// Calculate the average point of all the <paramref name="meshes"/>' centers.
        /// </summary>
        /// <param name="meshes"></param>
        /// <returns>The average posiiton of all the meshes' centers.</returns>
        public static Vector3 GetAverageCenter(List<Vector3> meshes)
        {
            Vector3 center = Vector3.zero;

            foreach (Vector3 meshCenter in meshes)
                center += meshCenter;

            if (meshes.Count > 0)
                center /= meshes.Count;

            return center;
        }

        /// <summary>
        /// Calculates the point on the line that is closest to the <paramref name="projectedPoint"/>.
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
                    throw new ArgumentException("Invalid axis");
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
        Z
    }
}
