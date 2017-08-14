//----------------------------------------------------------------------------
// Created on Fri Dec 27 14:47:38 2013 Raphael Thoulouze
//
// This program is the property of Persistant Studios SARL.
//
// You may not redistribute it and/or modify it under any conditions
// without written permission from Persistant Studios SARL, unless
// otherwise stated in the latest Persistant Studios Code License.
//
// See the Persistant Studios Code License for further details.
//----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class PKFxMenus : MonoBehaviour {

	private static void WarnRestart()
	{
		UnityEngine.Debug.LogWarning("[PKFX] Settings : For this setting to take effect, you'll need to re-open your project.");
	}

	//============ GLOBAL SETTINGS MOVED TO PKFxSettingsWindow.cs =========

	//========== CUSTOM GAMEOBJECTS CREATION ==============================
	//
	//
	//			
	//
	//
	//=====================================================================

	[MenuItem("GameObject/Create PopcornFX/Effect", false, 10)]
	static void CreateEmptyFX(MenuCommand menuCommand) {
		// Create a custom game object
		GameObject go = new GameObject("Fx");
		PKFxFX fx = go.AddComponent<PKFxFX>();
		string[] pkfxs = Directory.GetFiles("Assets/StreamingAssets/PackFx/", "*.pkfx", SearchOption.AllDirectories);
		if (pkfxs.Length > 0)
		{
			fx.m_FxName = pkfxs[0].Substring("Assets/StreamingAssets/PackFx/".Length);
			UnityEngine.Debug.Log(fx.m_FxName);
		}
		// Ensure it gets reparented if this was a context click (otherwise does 
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

	//----------------------------------------------------------------------------

	[MenuItem("GameObject/Create PopcornFX/Camera", false, 10)]
	static void CreatePKCamera(MenuCommand menuCommand) {
		// Create a custom game object
		GameObject go = new GameObject("PKFxCamera");
		go.AddComponent<Camera>();
		go.AddComponent<PKFxRenderingPlugin>();
		// Ensure it gets reparented if this was a context click (otherwise does 
		//GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

	//========== HELP =====================================================
	//
	//
	//
	//
	//
	//=====================================================================

	[MenuItem("Help/PopcornFX Wiki")]
	static void LinkToWiki()
	{
		Application.OpenURL("http://wiki.popcornfx.com/index.php/Unity");
	}

	//----------------------------------------------------------------------------

	[MenuItem("Help/Open PopcornFX Log")]
	static void OpenLog()
	{
		if (PKFxManager.FileLoggingEnabled() && File.Exists(PKFxManager.m_LogFilePath))
			Application.OpenURL(PKFxManager.m_LogFilePath);
		else
			UnityEngine.Debug.LogError("[PKFX] Log file not found, try enabling the log in the PopcornFX Preferences and/or restarting Unity.");
	}

	//----------------------------------------------------------------------------

	[MenuItem("Assets/PopcornFX/Move Sounds From Pack to Resources")]
	public static void MoveSounds()
	{
		var allFiles = Directory.GetFiles(Application.streamingAssetsPath + "/PackFx", "*.*", SearchOption.AllDirectories);
		List<string> files = new List<string>();
		foreach (string file in allFiles)
		{
			if (file.ToLower().EndsWith(".aif")
				|| file.ToLower().EndsWith(".wav")
				|| file.ToLower().EndsWith(".mp3")
				|| file.ToLower().EndsWith(".ogg"))
				files.Add(file.Replace(@"\", "/"));
		}
		foreach (string file in files)
		{
			string pathName = file.Substring((Application.streamingAssetsPath + "/PackFx/").Length);
			string dirName = Path.GetDirectoryName(Application.dataPath + "/Resources/PKFxSounds/" + pathName);
			if (!Directory.Exists(dirName))
				Directory.CreateDirectory(dirName);
			if (File.Exists(Application.dataPath + "/Resources/PKFxSounds/" + pathName))
			{
				File.Delete(Application.dataPath + "/Resources/PKFxSounds/" + pathName);
				UnityEngine.Debug.Log("Replacing " + pathName + " in Resources/PKFxSounds/");
			}
			else
				UnityEngine.Debug.Log("Moving " + pathName + " to Resources/PKFxSounds/");
			File.Move(file, Application.dataPath + "/Resources/PKFxSounds/" + pathName);
		}
	}

	//========== CUSTOM SHADERS ===========================================
	//
	//
	//
	//
	//
	//=====================================================================

	[MenuItem("Assets/PopcornFX/Create Missing Shader Assets")]
	public static void CreateShadersAssets()
	{
		var allFiles = Directory.GetFiles(Application.streamingAssetsPath + "/PackFx", "*.*", SearchOption.AllDirectories);
		List<string> files = new List<string>();
		foreach (string file in allFiles)
		{
			if (file.ToLower().EndsWith(".hlsl") || file.ToLower().EndsWith(".glsl"))
				files.Add(file);
		}
		foreach (string file in files)
		{
			string pathName = file.Substring((Application.streamingAssetsPath + "/PackFx/").Length);
			string dirName = Path.GetDirectoryName(Application.dataPath + "/Resources/PKFxShaders/" + pathName);
			if (!Directory.Exists(dirName))
				Directory.CreateDirectory(dirName);
			string filePath = dirName + "/" + Path.GetFileNameWithoutExtension(pathName) + ".asset";
			if (!File.Exists(filePath))
			{
				string assetPath = ("Assets" + filePath.Substring(Application.dataPath.Length)).Replace('\\', '/');
				UnityEngine.Debug.Log("[PKFX] Creating " + assetPath);
				PkFxCustomShader shaderAsset = ScriptableObject.CreateInstance<PkFxCustomShader>();
				shaderAsset.m_GlobalShader = true;
				shaderAsset.m_ShaderName = pathName.Replace('\\', '/');
				AssetDatabase.CreateAsset(shaderAsset, assetPath);
			}
		}
		AssetDatabase.SaveAssets();
	}

	//========== DEPLOYMENT ===============================================
	//
	//
	//
	//
	//
	//=====================================================================

	[MenuItem("Assets/PopcornFX/Create Pack's Index For Android Deployment")]
	public static void BakePack()
	{
		if (!Directory.Exists(Application.streamingAssetsPath + "/PackFx"))
			UnityEngine.Debug.LogWarning("[PKFX] No pack found. Packs must be baked to StreamingAssets in a PackFx directory.");
		
		// Create an index of the PackFX content.
		FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath + "/Index.txt");
		FileStream fs = File.Create(Application.streamingAssetsPath + "/Index.txt");
		
		List<string> files = new List<string>();
		files.AddRange(Directory.GetFiles(Application.streamingAssetsPath + "/PackFx", "*", SearchOption.AllDirectories));
		files.Add(Application.streamingAssetsPath + "/PKconfig.cfg");
		for (int i = 0; i < files.Count; i++)
			files[i] = files[i].Replace(@"\", "/");
		files.Sort();
		var md5 = MD5.Create();
		
		// Write the file containing the index of the Pack's files and their hashes.
		foreach (string s in files)
		{
			if (s.Substring(s.LastIndexOf('/') + 1)[0] != '.' && s.Substring(s.LastIndexOf('.') + 1) != "meta")
			{
				string hash = System.BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(s))).Replace("-", "");
				string virtualPath = s.Substring(s.LastIndexOf("StreamingAssets") + "StreamingAssets".Length + 1);
				fs.Write(Encoding.ASCII.GetBytes(virtualPath + ":" + hash + "\n"), 0, virtualPath.Length + hash.Length + 2);
			}
		}
		fs.Close();
		UnityEngine.Debug.Log("[PKFX] Done indexing pack for android deployment.");
	}

}
