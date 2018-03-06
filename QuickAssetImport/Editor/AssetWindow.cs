using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

public class AssetWindow : EditorWindow {

	// After we call 'ScanForAssets();' we'll store all the package locations into this.
	public static List<string> assetPackages = new List<string>();

	// Ignore this, this is for the scroll bar in Editor Window.
	private static Vector2 scrollPosition;

	[MenuItem("Window/Quick Asset")]
	public static void Init(){
		AssetWindow window = (AssetWindow)EditorWindow.GetWindow (typeof(AssetWindow));
		window.Show ();
		window.minSize = new Vector2 (400, 650);
		window.maxSize = new Vector2 (400, 650);
		ScanForAssets ();
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

	// Terminates the import and ends the delegate for importing.
	public static void Finished(){
		ScanForAssets ();
		AssetDatabase.importPackageStarted -= ImportingNow;
	}


	// DIDN'T USE THIS, BUT YOU CAN USE THIS ON THE RESCAN BUTTON IF YOU'D LIKE.
	// NOT TESTED AT ALL, I WAS GONNA ADD IT, BUT CHANGED MY MIND.
	// SO INSTEAD I LEFT IT HERE FOR YOU, IF YOU DECIDED TO WANT TO USE/TRY IT.
	// CAN'T PROMISE IT WILL WORK OR NOT.
	private static void ReScan(){
		// On rescanning we will check if package names already exist, if so, ignore them. And then we'll add the new packages if any.
		List<string> newPackages = new List<string> ();		// Starts out Empty.
		List<string> initialPackages = new List<string> ();	// Store the packages we currently have.
		initialPackages.AddRange (assetPackages);			// Add the packages from our initial loading into this new script.
		assetPackages.Clear ();								// Now clear the original list.


		// Now we scan for the assets in our local unity path.
		// Copy pasted the same code in ScanForAssets();.
		string appPath = Path.GetFullPath (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData));

		// The reason here I have and 'If Else' statement is I'm just trying to be safe here, not sure if ApplicationData will always
		// go to Roaming folder first, so to be safe I have a backup that includes \Roaming in the link.
		if (appPath.Contains (@"\Roaming")) {
			
			newPackages.AddRange(Directory.GetFiles (appPath + @"\Unity\Asset Store-5.x", "*.unitypackage", SearchOption.AllDirectories));
			newPackages = assetPackages.OrderBy (Path.GetFileName).ToList ();

		} else {
			newPackages.AddRange(Directory.GetFiles (appPath + @"\Roaming\Unity\Asset Store-5.x", "*.unitypackage", SearchOption.AllDirectories));
			newPackages = assetPackages.OrderBy (Path.GetFileName).ToList ();
		}
			
		// Now I re-add the initial packages that we loaded (remember we stored them in temp list above).
		assetPackages.AddRange (initialPackages);
		// Now we iterate through the newPackages (the If/Else) above. And now we check if the names match already existing names.
		// If they don't exist, then we add them.
		for (int i = 0; i < newPackages.Count; i++) {
			if (!assetPackages.Contains (newPackages [i])) {
				assetPackages.Add (newPackages [i]);
			}
		}

	}

	void OnGUI(){

		// I am ONLY using a Try/Catch because if I don't the 'EditorGUILayout.EndVertial();" gives off an error I don't understand.
		try{
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
				if(assetPackages.Count > 0){
					Debug.Log("No reason to rescan, assets are already loaded.");
					return;
				}else{
					ScanForAssets();
					// If you decide to run the untested re-scan code, uncomment the comment below then comment the line above this.
					//ReScan();
				}
		}
		}catch{
			// Just ignore the catch block... Remove the Try/Catch only if you want to figure out the error.
		}
	}

}

