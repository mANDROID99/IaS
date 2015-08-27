using NUnit.Framework;
using IaS.World;
using IaS.Domain;
using UnityEngine;
using System.Collections.Generic;
using IaS.World.Rotations;

namespace IaS.test.unit
{

    [TestFixture]
    public class RotateableRegionResolverTest
    {

        [Test]
        public void TwoAdjacentBlocks_OneSplitThroughTheCenterAvailable()
        {
            var resolver = new RotateableRegionResolver(new[] {
                new RotateableRegionImpl(new IntBlockBounds(0, 0, 0, 2, 2, 2)),
                new RotateableRegionImpl(new IntBlockBounds(2, 0, 0, 4, 4, 4))
            }, Pivots(true, true, true));

            IList<AxisConstraint> splits = resolver.GetAvailableSplits();
            Assert.That(splits, Has.Count.EqualTo(1));
            Assert.That(splits[0], Is.EqualTo(AxisConstraint.X(2)));
        }

        [Test]
        public void UnalignedBlocks_NoSplitsAvailable()
        {
            var resolver = new RotateableRegionResolver(new[] {
                new RotateableRegionImpl(new IntBlockBounds(0, 2, 0, 3, 4, 2)),
                new RotateableRegionImpl(new IntBlockBounds(2, 0, 0, 4, 2, 4)),
            }, Pivots(true, false, true));

            IList<AxisConstraint> splits = resolver.GetAvailableSplits();
            Assert.That(splits, Has.Count.EqualTo(0));
        }

        [Test]
        public void RotateBlock_RegionUpdatedCorrectly()
        {
            var resolver = new RotateableRegionResolver(new[] {
                new RotateableRegionImpl(new IntBlockBounds(0, 0, 0, 2, 2, 2)),
                new RotateableRegionImpl(new IntBlockBounds(2, 0, 0, 4, 4, 4))
            }, Pivots(true, false, false));

            IList<RotateableRegion> rotatedRegions = resolver.Rotate(resolver.GetAvailableSplits()[0], true, 1);
            Assert.That(rotatedRegions, Has.Count.EqualTo(1));
            Assert.That(rotatedRegions[0].RotatedBounds().Min, Is.EqualTo(new Vector3(0, 2, 0)));
            Assert.That(rotatedRegions[0].RotatedBounds().Max, Is.EqualTo(new Vector3(2, 4, 2)));
            Assert.That(rotatedRegions[0].Rotation(), Is.EqualTo(Quaternion.Euler(90, 0, 0)));
        }

        private IList<AxisPivot> Pivots(bool x, bool y, bool z, Vector3? centerX = null, Vector3? centerY = null, Vector3? centerZ = null)
        {
            List<AxisPivot> pivots = new List<AxisPivot>();

            if (x) pivots.Add(AxisPivot.PivotX(centerX ?? new Vector3(2, 2, 2)));
            if (y) pivots.Add(AxisPivot.PivotY(centerY ?? new Vector3(2, 2, 2)));
            if (z) pivots.Add(AxisPivot.PivotZ(centerZ ?? new Vector3(2, 2, 2)));

            return pivots;
        }

    }
}
