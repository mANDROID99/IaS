using UnityEngine;
using System.Collections;
using IaS.WorldBuilder;
using IaS.WorldBuilder.Meshes;
using IaS.WorldBuilder.MeshLoader;

public class CleanupEditor{

	// this function is needed, because Unity sometimes keeps static variables in memory
	// even after a script has finished running.
	public static void Cleanup()
	{
		ProcMeshGeneratorFactory.ClearInstances ();
		MeshPartsData.ClearInstance ();
	}


}
