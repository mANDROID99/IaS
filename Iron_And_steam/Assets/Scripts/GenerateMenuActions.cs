using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using IaS.WorldBuilder;

public class GenerateMenuActions : MonoBehaviour {

	/*
    private const string PROCEDURAL_PATH = "Assets/Procedural";
	private const string PROC_MESHES_PATH = "Assets/Procedural/Meshes";
	private const string TEMPLATES_PATH = "Assets/Procedural/Templates";

	private static bool[,] BooleanCombinations(int nBooleans){
		bool[,] combinations = new bool[(int)Mathf.Pow(2, nBooleans), nBooleans];
		for (int i = 0; i < combinations.GetLength(0); i++) {
			for (int j = 0; j < nBooleans; j++){
				combinations[i, j] = (i & (1 << j)) != 0;
			}
		}
		return combinations;
	}

    [MenuItem("Generate/Generate Prefabs %#u")]
    static void Generate()
    {
		MeshPartsData meshPartsData = MeshPartsData.CreateAsset();
		foreach (ProcMeshGroup GroupDto in meshPartsData.AllMeshGroups ()) 
		{
			GameObject combinedInstance = CreateCombinedProcMeshGameObj(GroupDto.GroupName);
			foreach (ProcMeshPart part in GroupDto.Parts)
			{
				Mesh mesh = PrepareMesh(GroupDto.GroupName, part.GetMeshName(GroupDto.GroupName));
				part.mesh = mesh;
				ProcMeshGenerator meshGenerator = ProcMeshGeneratorFactory.Get(Type.GetType(part.MeshGeneratorType));
				meshGenerator.BuildMesh(mesh, part);

				combinedInstance.GetComponent<CombinedProcMesh>()
					.Attach(CreateAttachedGameObj(mesh, part.TemplateName, part.GetMeshName(GroupDto.GroupName))
					        , part.DefaultEnabled, part.PartName);
			}
			ReplacePrefab (GroupDto.GroupName, combinedInstance);
			UnityEngine.Object.DestroyImmediate (combinedInstance);
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		CleanupEditor.Cleanup ();
		Debug.Log("Done generating prefabs");
    }

	private static GameObject CreateAttachedGameObj(Mesh mesh, String templateName, String instanceName)
	{
		String templatePath = String.Format ("{0}/{1}.prefab", TEMPLATES_PATH, templateName);
		UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath (templatePath, typeof(GameObject));
		if (prefab == null) {
			throw new Exception(String.Format("Could not find template located at path: {0}", templatePath));
		}

		GameObject templateInstance = (GameObject) PrefabUtility.InstantiatePrefab (prefab);
		templateInstance.GetComponent<MeshFilter> ().sharedMesh = mesh;
		templateInstance.name = instanceName;
		return templateInstance;
	}

	private static GameObject CreateCombinedProcMeshGameObj(String groupName)
	{
		GameObject combinedInstance = new GameObject (groupName, typeof(CombinedProcMesh));
		combinedInstance.isStatic = true;
		combinedInstance.GetComponent<CombinedProcMesh> ().GroupName = groupName;
		return combinedInstance;
	}

	private static Mesh PrepareMesh(String groupName, String meshName)
	{
		String meshFolder = String.Format ("{0}/{1}", PROC_MESHES_PATH, groupName);
		if (!AssetDatabase.IsValidFolder (meshFolder)) {
			AssetDatabase.CreateFolder (PROC_MESHES_PATH, groupName);
		}
		string meshPath = string.Format ("{0}/{1}.prefab", meshFolder, meshName);
		Mesh mesh = (Mesh) AssetDatabase.LoadAssetAtPath (meshPath, typeof(Mesh));
		if (!mesh) {
			mesh = new Mesh ();
			AssetDatabase.CreateAsset (mesh, meshPath);
		} else {
			mesh.Clear();
		}
		return mesh;
	}

	private static void ReplacePrefab(String prefabName, GameObject replacement)
	{
		string prefabPath = string.Format ("{0}/{1}.prefab", PROCEDURAL_PATH, prefabName);
		UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath (prefabPath, typeof(GameObject));
		if (!prefab) {
			prefab = PrefabUtility.CreatePrefab (prefabPath, replacement);
		} else {
			prefab = PrefabUtility.ReplacePrefab(replacement, prefab, ReplacePrefabOptions.ReplaceNameBased);	
		}
	}*/
}
