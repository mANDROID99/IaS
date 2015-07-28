using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using NUnit.Framework;
using IaS.WorldBuilder;

namespace UnitTests
{
    [TestFixture]
    class IntBoundsTests
    {
        static BlockBounds[] boundsSamples = {new BlockBounds(new Vector3(0, 0, 0), new Vector3(3, 3, 3)),
                                             new BlockBounds(new Vector3(0, 0, 0), new Vector3(2, 4, 2))};


        private static object[] rotateAndSplitCases = 
        {
            new object[] { boundsSamples[0], Vector3.up, 90, new Vector3(1.5f, 1.5f, 1.5f), new BlockBounds(new Vector3(0, 0, 0), new Vector3(3, 3, 3))},
            new object[] { boundsSamples[1], Vector3.right, 180, new Vector3(1, 4, 1), new BlockBounds(new Vector3(0, 4, 0), new Vector3(2, 4, 2))},
        };

        [Test, TestCaseSource("rotateAndSplitCases")]
        public void Test_Rotate_IntBounds(BlockBounds bounds, Vector3 axis, float angle, Vector3 offset, BlockBounds expectedBounds)
        {
            Quaternion rotation = Quaternion.Euler(axis * angle);
            BlockBounds rotatedBounds = bounds.Copy();
            rotatedBounds.SetToRotationFrom(rotation, offset, bounds);
            Assert.AreEqual(expectedBounds, rotatedBounds);
        }


    }
}
