using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;
using System.Threading;	// So we can make a very brief delay after asset import (some reason it makes window go blank).

public class AssetWindow : EditorWindow {

	// After we call 'ScanForAssets();' we'll store all the package locations into this.
	public static List<string> assetPackages = new List<string>();

	// Ignore this, this is for the scroll bar in Editor Window.

	private static Vector2 scrollPosition;

	[MenuItem("Window/Quick Asset")]
	public static void Init(){
		AssetWindow window = (AssetWindow)EditorWindow.GetWindow (typeof(AssetWindow));
		window.Show ();
		window.minSize = new Vector2 (400, 600);
		window.maxSize = new Vector2 (400, 600);
		ScanForAssets ();
		//EditorApplication.update += Update;
	}

	// Use this to scan the AppData folder for any assets we have.
	public static void ScanForAssets(){
		string appPath = Path.GetFullPath (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData));

		// The reason here I have and 'If Else' statement is I'm just trying to be safe here, not sure if ApplicationData will always
		// go to Roaming folder first, so to be safe I have a backup that includes \Roaming in the link.
		if (appPath.Contains (@"\Roaming")) {
			assetPackages.AddRange(Directory.GetFiles (appPath + @"\Unity\Asset Store-5.x", "*.unitypackage", SearchOption.AllDirectories));
			assetPackages = assetPackages.OrderBy (Path.GetFileName).ToList ();
		} else {
			assetPackages.AddRange(Directory.GetFiles (appPath + @"\Roaming\Unity\Asset Store-5.x", "*.unitypackage", SearchOption.AllDirectories));
			assetPackages = assetPackages.OrderBy (Path.GetFileName).ToList ();
		}
	}

	// When the window is closed - we clear out all the data (just encase).
	void OnDestroy(){
		assetPackages.Clear ();
	}

	public static void ImportingNow(string packageName){
		Debug.Log ("IMPORTING. Standby");
		Finished ();

	}

	public static void Finished(){
		ScanForAssets ();
		AssetDatabase.importPackageStarted -= ImportingNow;
	}


	void OnGUI(){
		
		GUILayout.Label ("Quick Asset", EditorStyles.boldLabel);
		GUILayout.Label ("Found: " + assetPackages.Count + " Assets");
		GUILayout.Label ("Click on assets below to import into project...");

		EditorGUILayout.BeginVertical ();
		scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition, GUILayout.Width (400), GUILayout.Height (400));
		int btnCount = -1;

			// First we want to gather all our packages, then just get the names of them and place them as buttons.
		if (btnCount != assetPackages.Count) {
			for (int i = 0; i < assetPackages.Count; i++) {
				btnCount++;
				string packageName = Path.GetFileNameWithoutExtension (assetPackages [btnCount]);
				if (GUILayout.Button (packageName)) {
					int id = btnCount;
					string packageIdToNamePath = assetPackages [id];
					ImportAssets.ImportAsset (packageIdToNamePath);
				}
			}
		}
			EditorGUILayout.EndScrollView ();
			EditorGUILayout.EndVertical ();
		if (GUILayout.Button ("Re-Scan")) {
			ScanForAssets ();
		}
	}
}
