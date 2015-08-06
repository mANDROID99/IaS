using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(WorldController))]
public class WorldControllerEditor : Editor {
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();
		
		WorldController worldController = (WorldController)target;
		if (GUILayout.Button ("load Level")) {
			worldController.LoadWorld();
		}
	}
}