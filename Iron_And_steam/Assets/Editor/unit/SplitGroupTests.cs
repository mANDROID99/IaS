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
        
        static BlockBounds[] meshBlockSamples =  { new BlockBounds(0, 0, 0, 5, 5, 5), new BlockBounds(-1, 1, 1, 0, 2, 2), new BlockBounds(1, 5, 1, 3, 7, 3) };

        private static IEnumerable<TestCaseData> splitSamples
        {
            get
            {
                yield return new TestCaseData(new SplitTestCase
                {
                    bounds = { meshBlockSamples[0] },
                    splits = { new SplitData{
                        split = new Split("", Vector3.right, new Vector3(), 2, Split.RestrictionType.Both),
                        expectedLeftBounds = { new BlockBounds(0, 0, 0, 2, 5, 5) },
                        expectedRightBounds = { new BlockBounds(2, 0, 0, 5, 5, 5) }
                    }}
                }).SetName("Horizontal");

                yield return new TestCaseData(new SplitTestCase
                {
                    bounds = { meshBlockSamples[0] },
                    splits = { new SplitData{
                        split = new Split("", Vector3.up, new Vector3(), 2, Split.RestrictionType.Both),
                        expectedLeftBounds = { new BlockBounds(0, 0, 0, 5, 2, 5) },
                        expectedRightBounds = { new BlockBounds(0, 2, 0, 5, 5, 5) }
                    }}
                }).SetName("Vertical");

                yield return new TestCaseData(new SplitTestCase
                {
                    bounds = { meshBlockSamples[0] },
                    splits = { new SplitData{
                        split = new Split("", Vector3.forward, new Vector3(), 2, Split.RestrictionType.Both),
                        expectedLeftBounds = { new BlockBounds(0, 0, 0, 5, 5, 2) },
                        expectedRightBounds = { new BlockBounds(0, 0, 2, 5, 5, 5) }
                    }}
                }).SetName("Depthical");

                yield return new TestCaseData(new SplitTestCase
                {
                    bounds = { meshBlockSamples[0], meshBlockSamples[1] },
                    splits = { new SplitData{
                        split = new Split("", Vector3.right, new Vector3(), 1, Split.RestrictionType.Both),
                        expectedLeftBounds = { new BlockBounds(0, 0, 0, 1, 5, 5), new BlockBounds(-1, 1, 1, 0, 2, 2) },
                        expectedRightBounds = { new BlockBounds(1, 0, 0, 5, 5, 5) }
                    }}
                }).SetName("Multi blocks 1");

                yield return new TestCaseData(new SplitTestCase
                {
                    bounds = { meshBlockSamples[0], meshBlockSamples[2] },
                    splits = { new SplitData{
                        split = new Split("", Vector3.up, new Vector3(), 6, Split.RestrictionType.Both),
                        expectedLeftBounds =  { new BlockBounds(0, 0, 0, 5, 5, 5), new BlockBounds(1, 5, 1, 3, 6, 3) },
                        expectedRightBounds = { new BlockBounds(1, 6, 1, 3, 7, 3) }
                    }}
                }).SetName("Multi blocks 2");

                yield return new TestCaseData(new SplitTestCase
                {
                    bounds = { meshBlockSamples[0] },
                    splits = { 
                        new SplitData{
                            split = new Split("", Vector3.right, new Vector3(), 1, Split.RestrictionType.Both),
                            expectedLeftBounds =  { new BlockBounds(0, 0, 0, 1, 1, 5), new BlockBounds(0, 1, 0, 1, 5, 5), },
                            expectedRightBounds = { new BlockBounds(1, 0, 0, 5, 1, 5), new BlockBounds(1, 1, 0, 5, 5, 5) }
                        },
                        new SplitData{
                            split = new Split("", Vector3.up, new Vector3(), 1, Split.RestrictionType.Both),
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
                Split.ConstraintResult constraintResult;
                BlockBounds[] lhs = splitBounds.Where(bounds => split.Constrains(true, bounds, out constraintResult) >= 0).ToArray();
                BlockBounds[] rhs = splitBounds.Where(bounds => split.Constrains(false, bounds, out constraintResult) >= 0).ToArray();

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
