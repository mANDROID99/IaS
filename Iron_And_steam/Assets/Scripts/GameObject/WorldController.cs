using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Tracks;
using IaS.WorldBuilder.Xml;
using IaS.WorldBuilder.Meshes;
using IaS.WorldBuilder.Splines;
using IaS.GameObjects;
using IaS.GameState;

public class WorldController : MonoBehaviour {

	[SerializeField]
    public TextAsset levelXmlAsset;
	[SerializeField]
    public TextAsset levelXmlSchemaAsset;
	[SerializeField]
    public GameObject blockPrefab;
    [SerializeField]
    public GameObject trackPrefab;
    [SerializeField]
    public GameObject trainPrefab;

    private const string BLOCK_NAME_TEMPLATE = "block_{0}";
    private BlockRotater blockRotater = null;
    private WorldContext world = new WorldContext();

    void Start()
    {
        this.LoadWorld();
    }

	// Update is called once per frame
	void Update () {
        if (blockRotater != null) blockRotater.Update(this);
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

        BuildDynamicObjects();
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

        // blocks
        instances.AddRange(splitMeshBlocks.Select(splitBlock =>
        {
            return new BlockController.BlockGameObjectBuilder()
                .With(splitBlock, splitMeshBlocks, group.splits, blocksContainer.transform, blockPrefab)
                .Build(adjacencyCalculator);
        }));

        // tracks
        instances.AddRange(group.tracks.SelectMany(track =>
        {
            return new TrackController.TrackGameObjectBuilder()
                .With(track, group.splits, tracksContainer.transform, trackPrefab)
                .Build(world);
        }));

        this.blockRotater = new BlockRotater(world, group.splits, instances.ToArray());
    }

    private void BuildDynamicObjects()
    {
        TrainController.AddToWorld(this.transform, trainPrefab, world, 0);
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
