using Assets.Scripts.Controllers;
using IaS.GameState;
using IaS.GameState.Creators;
using IaS.GameState.WorldTree;
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
    [SerializeField]
    public GameObject ArrowPrefab;

    private LevelTree _level;

    void Start()
    {
        LoadWorld();
    }

	// Update is called once per frame
	void Update () {
	    foreach (Controller controller in _level.Data.Controllers)
	    {
	        controller.Update(this);
	    }
	}

    public void LoadWorld()
    {
        RemoveAllChildren();

        var worldParser = new WorldParser();
        LevelXML levelXml;
        levelXml = worldParser.Parse(LevelXmlAsset, LevelXmlSchemaAsset);

        var worldContextCreator = new LevelCreator();
        var prefabs = new Prefabs(TrackPrefab, BlockPrefab, TrainPrefab, ArrowPrefab);

        _level = worldContextCreator.CreateLevel(levelXml, this.transform, prefabs);
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
