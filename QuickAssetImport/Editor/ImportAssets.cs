using UnityEditor;
using UnityEngine;
using System.Collections;

public class ImportAssets : Editor {

	private static bool completedImport;

	// Simple Enough - just run the process - done.
	public static void ImportAsset(string package){
		AssetDatabase.importPackageStarted += AssetWindow.ImportingNow;
		AssetDatabase.ImportPackage(package,false);

	}
}
