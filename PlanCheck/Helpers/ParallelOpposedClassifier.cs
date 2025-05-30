using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanCheck.Helpers
{
    public static class ParallelOpposedClassifier
    {
        /// <summary>
        /// Returns true if every pair of angles in the list is within tolerance of being either equal
        /// or 180° opposed. Angles are expected in [0, 360).
        /// </summary>
        public static bool AreAnglesParallelOrOpposed(IEnumerable<double> angles, double toleranceDegrees = 5.0)
        {
            if (angles == null) throw new ArgumentNullException(nameof(angles));

            // Generate all unique unordered pairs of angles
            return angles
                .Select((angle, idx) => (angle: Normalize360(angle), idx))
                .SelectMany(
                    x => angles
                            .Skip(x.idx + 1)
                            .Select(y => (a: x.angle, b: Normalize360(y)))
                )
                .All(pair =>
                {
                    var diff = AngleDifference(pair.a, pair.b);
                    return diff <= toleranceDegrees
                           || Math.Abs(diff - 180.0) <= toleranceDegrees;
                });
        }

        private static double Normalize360(double angle)
        {
            var a = angle % 360.0;
            return a < 0 ? a + 360.0 : a;
        }

        private static double AngleDifference(double a, double b)
        {
            var diff = Math.Abs(a - b) % 360.0;
            return diff > 180.0 ? 360.0 - diff : diff;
        }
    }
}
