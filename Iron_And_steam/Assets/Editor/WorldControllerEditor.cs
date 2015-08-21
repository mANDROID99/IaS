using System;
using UnityEngine;
using UnityEditor;
using IaS.World;

[CustomEditor(typeof(WorldCreator))]
public class WorldControllerEditor : Editor {
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();

        WorldCreator worldController = (WorldCreator)target;
		if (GUILayout.Button ("load Level")) {
			worldController.LoadWorld();
		}
	}
}