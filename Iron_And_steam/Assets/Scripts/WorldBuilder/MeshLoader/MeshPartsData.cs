using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IaS.WorldBuilder.Meshes;

namespace IaS.WorldBuilder
{
	[ExecuteInEditMode]
	public class MeshPartsData : ScriptableObject{

		public const string GROUP_BLOCK = "block";
		public const string GROUP_OUTER_EDGE = "outer_edge";
        public const string GROUP_INNER_EDGE = "inner_edge";
		public const string GROUP_CORNER = "outer_corner";
		public const string TEMPLATE_BLOCK = "template_block";

		public const string PART_BLOCK_FRONT = "front";
		public const string PART_BLOCK_BACK = "back";
		public const string PART_BLOCK_LEFT = "left";
		public const string PART_BLOCK_RIGHT = "right";
		public const string PART_BLOCK_TOP = "top";
		public const string PART_BLOCK_BOTTOM = "bottom";

		public const string PART_OUTER_EDGE_FRONT = "edge_front";
		public const string PART_OUTER_EDGE_RIGHT = "edge_right";
		public const string PART_OUTER_EDGE_LEFT = "edge_left";

		public const string PART_OUTER_CORNER = "outer_corner";

        public const string PART_INNER_EDGE_FRONT = "inner_edge_front";
        public const string PART_INNER_EDGE_RIGHT = "inner_edge_right";
        public const string PART_INNER_EDGE_LEFT = "inner_edge_left";

		[SerializeField]
		private List<String> meshGroupDicKeys = new List<string>();
		[SerializeField]
		private List<ProcMeshGroup> meshGroupDicValues = new List<ProcMeshGroup>();
		private static MeshPartsData instance = null;
		private Dictionary<string, ProcMeshGroup> meshGroups;

		void OnEnable() {
			meshGroups = new Dictionary<string, ProcMeshGroup> ();
			for(int i = 0, c = Math.Min(meshGroupDicKeys.Count, meshGroupDicValues.Count); i < c; i++)
			{
				meshGroups.Add(meshGroupDicKeys[i], meshGroupDicValues[i]);
			}
		}

		public static MeshPartsData CreateAsset(){
			MeshPartsData instance = ScriptableObject.CreateInstance<MeshPartsData> ();
			MeshPartsData.instance = instance;

			String blockEdgeMeshGenerator = typeof(BlockOuterEdgeMeshGenerator).ToString();
			String blockSideMeshGenerator = typeof(BlockSideMeshGenerator).ToString();
			String blockCornerMeshGenerator = typeof(BlockCornerMeshGenerator).ToString();
            String blockInnerMeshGenerator = typeof(BlockInnerEdgeMeshGenerator).ToString();

			instance.meshGroups = new Dictionary<string, ProcMeshGroup>()
			{
				{GROUP_BLOCK,
					new ProcMeshGroup(GROUP_BLOCK, new ProcMeshPart[]{
						new ProcMeshPart(PART_BLOCK_FRONT, TEMPLATE_BLOCK, true, blockSideMeshGenerator),
						new ProcMeshPart(PART_BLOCK_BACK, TEMPLATE_BLOCK, true, blockSideMeshGenerator),
						new ProcMeshPart(PART_BLOCK_LEFT, TEMPLATE_BLOCK, true, blockSideMeshGenerator),
						new ProcMeshPart(PART_BLOCK_RIGHT, TEMPLATE_BLOCK, true, blockSideMeshGenerator),
						new ProcMeshPart(PART_BLOCK_TOP, TEMPLATE_BLOCK, true, blockSideMeshGenerator),
						new ProcMeshPart(PART_BLOCK_BOTTOM, TEMPLATE_BLOCK, true, blockSideMeshGenerator),
					})
				},
				{GROUP_OUTER_EDGE,
					new ProcMeshGroup(GROUP_OUTER_EDGE, new ProcMeshPart[]{
						new ProcMeshPart(PART_OUTER_EDGE_FRONT, TEMPLATE_BLOCK, true, blockEdgeMeshGenerator),
						new ProcMeshPart(PART_OUTER_EDGE_LEFT, TEMPLATE_BLOCK, false, blockEdgeMeshGenerator),
						new ProcMeshPart(PART_OUTER_EDGE_RIGHT, TEMPLATE_BLOCK, false, blockEdgeMeshGenerator),
					})
				},
				{GROUP_CORNER,
					new ProcMeshGroup(GROUP_CORNER, new ProcMeshPart[]{
						new ProcMeshPart(PART_OUTER_CORNER, TEMPLATE_BLOCK, true, blockCornerMeshGenerator),
					})
				},
                {GROUP_INNER_EDGE,
                    new ProcMeshGroup(GROUP_INNER_EDGE, new ProcMeshPart[]{
                        new ProcMeshPart(PART_INNER_EDGE_FRONT, TEMPLATE_BLOCK, true, blockInnerMeshGenerator),
                        new ProcMeshPart(PART_INNER_EDGE_RIGHT, TEMPLATE_BLOCK, true, blockInnerMeshGenerator),
                        new ProcMeshPart(PART_INNER_EDGE_LEFT, TEMPLATE_BLOCK, true, blockInnerMeshGenerator)
                    })
                }
			};

			instance.meshGroupDicKeys = instance.meshGroups.Keys.ToList ();
			instance.meshGroupDicValues = instance.meshGroups.Values.ToList ();

			AssetDatabase.DeleteAsset ("Assets/Procedural/MeshPartsData.asset");
			AssetDatabase.CreateAsset (instance, "Assets/Procedural/MeshPartsData.asset");
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
			return instance;
		}

		public static MeshPartsData Instance {
			get {
				return instance ?? 
					(instance = (MeshPartsData)AssetDatabase.LoadAssetAtPath("Assets/Procedural/MeshPartsData.asset", typeof(ScriptableObject)));
			}
		}

		public static void ClearInstance()
		{
			instance = null;
		}

		public ProcMeshPart GetMeshPart(String groupName, String partName)
		{
			if (!meshGroups.ContainsKey (groupName)) {
				return null;
			}

			foreach (ProcMeshPart part in meshGroups[groupName].Parts) {
				if (part.PartName.Equals(partName))
					return part;
			}
			return null;
		}

		public ProcMeshGroup GetMeshGroup(String groupName)
		{
			if (!meshGroups.ContainsKey (groupName)) {
				return null;
			}

			return meshGroups [groupName];
		}

		public IList<ProcMeshGroup> AllMeshGroups()
		{
			List<ProcMeshGroup> allMeshGroups = new List<ProcMeshGroup> ();
			foreach (KeyValuePair<string, ProcMeshGroup> entry in meshGroups) {
				allMeshGroups.Add(entry.Value);
			}
			return allMeshGroups;
		}
	}

	[Serializable]
	public class ProcMeshGroup{
		[SerializeField]
		private String groupName;
		[SerializeField]
		private ProcMeshPart[] parts;

		public String GroupName { get { return groupName; } }
		public IList<ProcMeshPart>  Parts { get { return parts; } }

		public ProcMeshGroup()
		{
		}

		public ProcMeshGroup(String groupName, ProcMeshPart[] parts)
		{
			this.groupName = groupName;
			this.parts = parts;
		}
	}

	[Serializable]
	public class ProcMeshPart{
		[SerializeField]
		private string partName;
		[SerializeField]
		private bool defaultEnabled;
		[SerializeField]
		private string templateName;
		[SerializeField]
		private string meshGeneratorType;
		[SerializeField]
		public Mesh mesh;

		public String PartName { get { return partName; } }
		public bool DefaultEnabled { get { return defaultEnabled; } }
		public String TemplateName { get { return templateName;} }
		public String MeshGeneratorType { get { return meshGeneratorType;} }

		public ProcMeshPart(String partName, String templateName, bool defaultEnabled, String meshGeneratorType)
		{
			this.partName = partName;
			this.defaultEnabled = defaultEnabled;
			this.templateName = templateName;
			this.meshGeneratorType = meshGeneratorType;
			this.mesh = null;
		}

		public String GetMeshName(String groupName)
		{
			return String.Format ("{0}_{1}", groupName, partName);
		}
	}
}
