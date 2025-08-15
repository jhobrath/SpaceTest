using GalagaFigther.Models;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther
{
    public static class CollisionUtils
    {
        public static bool IsColliding(OrientedBoundingBox obb1, OrientedBoundingBox obb2)
        {
            // The axes to test are the two unique axes from each OBB.
            Vector2[] axes = new Vector2[4];
            axes[0] = Vector2.Normalize(obb1.Corners[1] - obb1.Corners[0]); // obb1's first axis
            axes[1] = Vector2.Normalize(obb1.Corners[2] - obb1.Corners[1]); // obb1's second axis
            axes[2] = Vector2.Normalize(obb2.Corners[1] - obb2.Corners[0]); // obb2's first axis
            axes[3] = Vector2.Normalize(obb2.Corners[2] - obb2.Corners[1]); // obb2's second axis

            // Iterate through all four axes
            for (int i = 0; i < 4; i++)
            {
                Vector2 axis = axes[i];

                // Project both OBBs onto the current axis
                (float min1, float max1) = ProjectOntoAxis(obb1.Corners, axis);
                (float min2, float max2) = ProjectOntoAxis(obb2.Corners, axis);

                // If there's no overlap on this axis, then there is no collision
                if (max1 < min2 || max2 < min1)
                {
                    return false;
                }
            }

            // If an overlap was found on all axes, then the OBBs are colliding
            return true;
        }

        /// <summary>
        /// Checks for a collision between an Oriented Bounding Box (OBB) and an axis-aligned Rectangle.
        /// This is a simplified SAT check where the Rectangle's axes are the global X and Y axes.
        /// </summary>
        /// <param name="obb">The OBB.</param>
        /// <param name="rec">The Raylib Rectangle.</param>
        /// <returns>True if a collision is detected, false otherwise.</returns>
        public static bool IsColliding(OrientedBoundingBox obb, Rectangle rec)
        {
            // The axes to test are the OBB's axes and the global X and Y axes.
            Vector2[] axes = new Vector2[4];
            axes[0] = Vector2.Normalize(obb.Corners[1] - obb.Corners[0]); // obb's first axis
            axes[1] = Vector2.Normalize(obb.Corners[2] - obb.Corners[1]); // obb's second axis
            axes[2] = Vector2.UnitX; // Global X-axis
            axes[3] = Vector2.UnitY; // Global Y-axis

            // Project the OBB's Corners and the Rectangle's Corners onto each axis.
            Vector2[] recCorners = new Vector2[4];
            recCorners[0] = new Vector2(rec.X, rec.Y);
            recCorners[1] = new Vector2(rec.X + rec.Width, rec.Y);
            recCorners[2] = new Vector2(rec.X + rec.Width, rec.Y + rec.Height);
            recCorners[3] = new Vector2(rec.X, rec.Y + rec.Height);

            for (int i = 0; i < 4; i++)
            {
                Vector2 axis = axes[i];
                (float min1, float max1) = ProjectOntoAxis(obb.Corners, axis);
                (float min2, float max2) = ProjectOntoAxis(recCorners, axis);

                if (max1 < min2 || max2 < min1)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Projects a set of vertices onto a given axis and returns the min and max projected values.
        /// </summary>
        /// <param name="vertices">An array of Vector2 vertices.</param>
        /// <param name="axis">The axis to project onto (must be normalized).</param>
        /// <returns>A tuple containing the minimum and maximum projected values.</returns>
        private static (float min, float max) ProjectOntoAxis(Vector2[] vertices, Vector2 axis)
        {
            float min = Vector2.Dot(vertices[0], axis);
            float max = min;

            for (int i = 1; i < vertices.Length; i++)
            {
                float projection = Vector2.Dot(vertices[i], axis);
                if (projection < min) min = projection;
                if (projection > max) max = projection;
            }

            return (min, max);
        }
    }
}
