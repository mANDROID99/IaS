using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using IaS.WorldBuilder;

namespace IASTest
{
    [TestFixture]
    [Category("Splitter Tests")]
    internal class SplitterTests
    {

        private static Split CreateSplit(Vector3 axis, float value)
        {
            return new Split("id", "group_id", null, axis, new Vector3(), value, Split.RestrictionType.Both);
        }

        static BlockBounds[] meshBlockSamples =  { new BlockBounds(0, 0, 0, 5, 5, 5), new BlockBounds(-1, 1, 1, 0, 2, 2), new BlockBounds(1, 5, 1, 3, 7, 3) };

        private static IEnumerable<TestCaseData> splitSamples
        {
            get
            {
                yield return new TestCaseData(new SplitTestCase
                {
                    bounds = { meshBlockSamples[0] },
                    splits = { new SplitData{
                        split = CreateSplit(Vector3.right, 2),
                        expectedLeftBounds = { new BlockBounds(0, 0, 0, 2, 5, 5) },
                        expectedRightBounds = { new BlockBounds(2, 0, 0, 5, 5, 5) }
                    }}
                }).SetName("Horizontal");

                yield return new TestCaseData(new SplitTestCase
                {
                    bounds = { meshBlockSamples[0] },
                    splits = { new SplitData{
                        split = CreateSplit(Vector3.up, 2),
                        expectedLeftBounds = { new BlockBounds(0, 0, 0, 5, 2, 5) },
                        expectedRightBounds = { new BlockBounds(0, 2, 0, 5, 5, 5) }
                    }}
                }).SetName("Vertical");

                yield return new TestCaseData(new SplitTestCase
                {
                    bounds = { meshBlockSamples[0] },
                    splits = { new SplitData{
                        split = CreateSplit(Vector3.forward, 2),
                        expectedLeftBounds = { new BlockBounds(0, 0, 0, 5, 5, 2) },
                        expectedRightBounds = { new BlockBounds(0, 0, 2, 5, 5, 5) }
                    }}
                }).SetName("Depthical");

                yield return new TestCaseData(new SplitTestCase
                {
                    bounds = { meshBlockSamples[0], meshBlockSamples[1] },
                    splits = { new SplitData{
                        split = CreateSplit(Vector3.right, 1),
                        expectedLeftBounds = { new BlockBounds(0, 0, 0, 1, 5, 5), new BlockBounds(-1, 1, 1, 0, 2, 2) },
                        expectedRightBounds = { new BlockBounds(1, 0, 0, 5, 5, 5) }
                    }}
                }).SetName("Multi blocks 1");

                yield return new TestCaseData(new SplitTestCase
                {
                    bounds = { meshBlockSamples[0], meshBlockSamples[2] },
                    splits = { new SplitData{
                        split = CreateSplit(Vector3.up, 6),
                        expectedLeftBounds =  { new BlockBounds(0, 0, 0, 5, 5, 5), new BlockBounds(1, 5, 1, 3, 6, 3) },
                        expectedRightBounds = { new BlockBounds(1, 6, 1, 3, 7, 3) }
                    }}
                }).SetName("Multi blocks 2");

                yield return new TestCaseData(new SplitTestCase
                {
                    bounds = { meshBlockSamples[0] },
                    splits = { 
                        new SplitData{
                            split = CreateSplit(Vector3.right, 1),
                            expectedLeftBounds =  { new BlockBounds(0, 0, 0, 1, 1, 5), new BlockBounds(0, 1, 0, 1, 5, 5), },
                            expectedRightBounds = { new BlockBounds(1, 0, 0, 5, 1, 5), new BlockBounds(1, 1, 0, 5, 5, 5) }
                        },
                        new SplitData{
                            split = CreateSplit(Vector3.up, 1),
                            expectedLeftBounds =  { new BlockBounds(0, 0, 0, 1, 1, 5), new BlockBounds(1, 0, 0, 5, 1, 5), },
                            expectedRightBounds = { new BlockBounds(0, 1, 0, 1, 5, 5), new BlockBounds(1, 1, 0, 5, 5, 5) }
                        }
                    }
                }).SetName("Multiple Splits");
            }
        }

        [Test, Description("Splitting Meshblocks Parameterized"), TestCaseSource("splitSamples")]
        public void Split_MeshBlocks_Parameterized(SplitTestCase testCase)
        {
            BlockBounds[] splitBounds = testCase.bounds.SelectMany(bounds => {
                SplitTree splitTree = new SplitTree(bounds);
                foreach(SplitData splitData in testCase.splits)
                {
                    splitTree.Split(splitData.split);
                }
                return splitTree.GatherSplitBounds();
            }).ToArray();

            foreach(SplitData splitData in testCase.splits)
            {
                Split split = splitData.split;
                float dist;
                BlockBounds[] lhs = splitBounds.Where(bounds => split.Constrains(true, bounds, out dist) == Split.ConstraintResult.Included).ToArray();
                BlockBounds[] rhs = splitBounds.Where(bounds => split.Constrains(false, bounds, out dist) == Split.ConstraintResult.Included).ToArray();

                AssertBlocksMatchBlockBounds(lhs, splitData.expectedLeftBounds);
                AssertBlocksMatchBlockBounds(rhs, splitData.expectedRightBounds);
            }
        }


        private MeshBlock CreateTestMeshBlock(BlockBounds bounds, String id = "id")
        {
            return new MeshBlock(id, null, MeshBlock.TypeCuboid, bounds, new BlockRotation(), 0);
        }

        private void AssertBlocksMatchBlockBounds(IList<BlockBounds> actualBounds, IList<BlockBounds> expectedBlockBounds)
        {
            Assert.That(expectedBlockBounds, Is.SubsetOf(actualBounds));
        }

        internal class SplitTestCase
        {
            internal List<BlockBounds> bounds = new List<BlockBounds>();
            internal List<SplitData> splits = new List<SplitData>();
        }

        internal class SplitData
        {
            internal Split split;
            internal List<BlockBounds> expectedLeftBounds = new List<BlockBounds>();
            internal List<BlockBounds> expectedRightBounds = new List<BlockBounds>();
        }

    }
}
