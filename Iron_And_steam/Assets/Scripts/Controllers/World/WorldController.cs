using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Assets.Scripts.Controllers;
using IaS.GameState;
using IaS.GameState.Creators;
using IaS.GameState.Events;
using IaS.GameState.WorldTree;
using IaS.WorldBuilder.Xml;
using UnityEngine;

public class WorldController : MonoBehaviour, EventConsumer<GameEvent>
{

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
    [SerializeField]
    public GameObject GoalPrefab;
    [SerializeField]
    public GameObject PointerPrefab;

    private LevelTree _level;
    private bool _paused = false;
    private float _timePauseBegin;
    private float _timePausedOffset = 0;

    void Start()
    {
        LoadWorld();
    }

    public void OnEvent(GameEvent evt)
    {
        switch (evt.type)
        {
            case GameEvent.Type.PAUSED:
                _timePauseBegin = Time.time;
                _paused = true;
                break;
            case GameEvent.Type.RESUMED:
                _timePausedOffset += Time.time - _timePauseBegin;
                _paused = false;
                break;
        }

    }

    // Update is called once per frame
    void Update ()
    {
        if (_paused) return;

        float time = Time.time - _timePausedOffset;
        GlobalGameState globalGameState = new GlobalGameState(time);

	    foreach (Controller controller in _level.Data.Controllers)
	    {
	        controller.Update(this, globalGameState);
	    }
	}

    public void LoadWorld()
    {
        RemoveAllChildren();

        LevelXML levelXml = LoadWorldFromXmlFile();

        var worldContextCreator = new LevelCreator();
        var prefabs = new Prefabs(TrackPrefab, BlockPrefab, TrainPrefab, ArrowPrefab, GoalPrefab, PointerPrefab);

        _level = worldContextCreator.CreateLevel(levelXml, this.transform, prefabs);
        _level.EventRegistry.RegisterConsumer(this);
    }

    private LevelXML LoadWorldFromXmlFile()
    {
        XmlReader sourceReader = XmlReader.Create(new StringReader(LevelXmlAsset.text));
        XDocument xDoc = new XDocument(XDocument.Load(sourceReader));
        XElement xLevel = xDoc.Element(LevelXML.ElementLevel);
        return LevelXML.FromElement(xLevel);
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
