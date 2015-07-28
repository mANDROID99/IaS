using System;
using System.Collections.Generic;
using Assets.Scripts.Controllers;
using IaS.GameState;
using IaS.GameState.Creators;
using IaS.WorldBuilder.Xml;
using UnityEngine;

public class WorldController : MonoBehaviour {

	[SerializeField]
    public TextAsset LevelXmlAsset;
	[SerializeField]
    public TextAsset LevelXmlSchemaAsset;
	[SerializeField]
    public GameObject BlockPrefab;
    [SerializeField]
    public GameObject TrackPrefab;
    [SerializeField]
    public GameObject TrainPrefab;

    private WorldContext _world;
    private Controller[] _controllers = new Controller[0];

    void Start()
    {
        LoadWorld();
    }

	// Update is called once per frame
	void Update () {
	    foreach (Controller controllers in _controllers)
	    {
	        controllers.Update(this);
	    }
	}

    public void LoadWorld()
    {
        RemoveAllChildren();

        var worldParser = new WorldParser();
        LevelDTO levelDto;
        levelDto = worldParser.Parse(LevelXmlAsset, LevelXmlSchemaAsset);

        var controllers = new List<Controller>();
        var worldContextCreator = new WorldContextCreator();
        var prefabs = new Prefabs(TrackPrefab, BlockPrefab, TrainPrefab, this.transform);
        var eventRegistry = new EventRegistry();

        _world = worldContextCreator.CreateWorld(levelDto);
        worldContextCreator.CreateWorldControllers(_world, eventRegistry, controllers, prefabs);
        _controllers = controllers.ToArray();
    }

    private void RemoveAllChildren()
    {
        for (var i = 0; i < transform.childCount; i += 1)
        {
            var childGo = transform.GetChild(i).gameObject;
            DestroyImmediate(childGo);
        }
    }

    
}
