using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using IaS.GameObjects;
using IaS.GameState;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Xml;
using IaS.Controllers.World;
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

    private GroupContext[] _groups;
    private Controller[] _controllers;

    void Start()
    {
        this.LoadWorld();
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
        var worldParser = new WorldParser();
        var level = worldParser.Parse(LevelXmlAsset, LevelXmlSchemaAsset);
        RemoveAllChildren();

        var groupContextCreator = new GroupContextCreator();
        var groupGameObjectBuilder = new GroupGameObjectBuilder(BlockPrefab, TrackPrefab);
        var controllers = new List<Controller>();

        _groups = level.Groups.Select(group =>
        {
            var eventRegistry = new EventRegistry();
            GroupContext groupCtx = groupContextCreator.CreateGroupContext(group, eventRegistry);
            GameObject groupGameObject;
            InstanceWrapper[] instances = groupGameObjectBuilder.BuildGroupGameObject(groupCtx, this.transform, out groupGameObject);

            controllers.Add(new BlockRotater(groupCtx, instances));
            controllers.Add(new TrainController(groupGameObject.transform, TrainPrefab, groupCtx, 0));

            return groupCtx;
        }).ToArray();

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
