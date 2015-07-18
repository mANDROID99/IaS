using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace IaS.WorldBuilder.MeshLoader
{
	[ExecuteInEditMode]
	public class CombinedProcMesh : MonoBehaviour
	{
		[SerializeField] 
		private String groupName;
		[SerializeField]
		private List<PartVisibility> partsEnabled = new List<PartVisibility>();
		public IList<PartVisibility> PartsEnabled { get { return partsEnabled; } }

		public String GroupName { get { return groupName;	} set{ groupName = value; } }

		public void Attach(GameObject childGo, bool defaultEnabled, String partName)
		{
			childGo.transform.parent = this.transform;
			childGo.transform.localPosition = new Vector3 ();
			partsEnabled.Add (new PartVisibility(){enabled=!defaultEnabled, partName=partName});
			SetPartEnabled (partsEnabled.Count - 1, defaultEnabled);
		}

		public String GetPartName(int idx)
		{
			return partsEnabled [idx].partName;
		}

		public bool IsPartEnabled(int idx)
		{
			return partsEnabled [idx].enabled;
		}

		public void SetPartEnabled(int idx, bool value)
		{
			partsEnabled [idx].enabled = value;
			ProcMeshPart part = MeshPartsData.Instance.GetMeshPart(groupName, partsEnabled [idx].partName);
			if(part == null){
				Debug.LogWarning("could not enable/disable part because it could not be found.");
				return;
			}

			Transform childTransform = this.transform.FindChild(part.GetMeshName(groupName));
			if(!childTransform) 
				return;

			GameObject childGO = childTransform.gameObject;
			childGO.SetActive(value);
		}

		[Serializable]
		public class PartVisibility{
			[SerializeField]
			public String partName;
			[SerializeField]
			public bool enabled;
		}
	}
}

