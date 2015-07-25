﻿using System;
using UnityEditor;
using UnityEngine;

namespace IaS.WorldBuilder.Meshes
{
	public class ProcMeshLoader{

		private const string TEMPLATES_PATH = "Assets/Procedural/Templates";

		public static GameObject InstantiateProcMeshTemplate(string templateName)
		{
			string templatePath = string.Format ("{0}/{1}.prefab", TEMPLATES_PATH, templateName);
			GameObject prefab = (GameObject) AssetDatabase.LoadAssetAtPath(templatePath, typeof(GameObject));
			if (prefab == null) {
				throw new Exception(string.Format("Could not find template located at path: {0}", templatePath));
			}

            return GameObject.Instantiate(prefab);
		}

        public static GameObject Instantiate(ProcMeshPart part)
		{
			GameObject templateInstance = InstantiateProcMeshTemplate (part.TemplateName);
			templateInstance.GetComponent<MeshFilter> ().sharedMesh = part.mesh;
			templateInstance.name = part.PartName;
			return templateInstance;
		}


	}
}
