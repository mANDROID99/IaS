using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Meshes.Tracks;
using IaS.WorldBuilder.Xml;
using IaS.WorldBuilder.Meshes;
using IaS.WorldBuilder.Splines;
using IaS.GameObjects;

public class WorldController : MonoBehaviour {

	[SerializeField]
    private TextAsset levelXmlAsset;
	[SerializeField]
    private TextAsset levelXmlSchemaAsset;
	[SerializeField]
    private GameObject blockPrefab;
    [SerializeField]
    private GameObject trackPrefab;

    private const string BLOCK_NAME_TEMPLATE = "block_{0}";

    void Start()
    {
        this.LoadWorld();
    }

	// Update is called once per frame
	void Update () {
        
	}

    public void LoadWorld()
    {
        WorldParser worldParser = new WorldParser();
        Level level = worldParser.Parse(levelXmlAsset, levelXmlSchemaAsset);
        RemoveAllChildren();

        foreach (LevelGroup group in level.groups)
        {
            BuildGroup(group);
        }
    }

    private void RemoveAllChildren()
    {
        for (int i = 0; i < this.transform.childCount; i += 1)
        {
            GameObject childGO = this.transform.GetChild(i).gameObject;
            DestroyImmediate(childGO);
        }
    }

    private void BuildGroup(LevelGroup group)
    {
        GameObject groupGameObj = new GameObject(group.id);
        groupGameObj.transform.parent = this.transform;

        IList<MeshBlock> splitMeshBlocks = SplitMeshBlocks(group.meshes, group.splits);
        GameObject blocksContainer = GameObjectUtils.EmptyGameObject("blocks", groupGameObj.transform, new Vector3());
        GameObject tracksContainer = GameObjectUtils.EmptyGameObject("tracks", groupGameObj.transform, new Vector3());

        AdjacencyCalculator adjacencyCalculator = new AdjacencyCalculator();
        List<InstanceWrapper> instances = new List<InstanceWrapper>();

        instances.AddRange(splitMeshBlocks.Select(splitBlock =>
        {
            return new BlockGameObject.BlockGameObjectBuilder()
                .With(splitBlock, splitMeshBlocks, group.splits, blocksContainer.transform, blockPrefab)
                .Build(adjacencyCalculator);
        }));

        instances.AddRange(group.tracks.SelectMany(track =>
        {
            return new TrackGameObject.TrackGameObjectBuilder()
                .With(track, group.splits, tracksContainer.transform, trackPrefab)
                .Build();
        }));

        var splitBuilder = new TwisterGameObject.TwisterGameObjectBuilder().With(group.splits, instances.ToArray(), groupGameObj.transform);
        splitBuilder.Build();
    }

    private IList<MeshBlock> SplitMeshBlocks(IList<MeshBlock> meshBlocks, IList<Split> splits)
    {
        return meshBlocks.SelectMany(block =>
        {
            SplitTree splitTree = new SplitTree(block.bounds);
            splitTree.Split(splits);

            return splitTree.GatherSplitBounds()
                .Select(bounds => block.CopyOf(bounds));
        }).ToArray();
    }
}
