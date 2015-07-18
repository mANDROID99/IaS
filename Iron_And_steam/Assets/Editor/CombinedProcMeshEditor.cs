using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
//using ProceduralMeshes;


//[CustomEditor(typeof(CombinedProcMesh))]
public class ProcMeshEditor : Editor {

	/*
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();
		CombinedProcMesh combinedGO = (CombinedProcMesh)target;

		GUILayout.Label ("num parts: " + combinedGO.PartsEnabled.Count);
		for (int i = 0; i < combinedGO.PartsEnabled.Count; i++)
		{
			ProcMeshPart meshPart = MeshPartsData.Instance.GetMeshPart(combinedGO.GroupName, combinedGO.GetPartName(i));
			String text = String.Format("show {0}", meshPart.PartName);
			bool enabled = GUILayout.Toggle (combinedGO.IsPartEnabled(i), text);

			if(combinedGO.IsPartEnabled(i) != enabled)
			{
				combinedGO.SetPartEnabled(i, enabled);
			}
		}
	}
	*/
}
