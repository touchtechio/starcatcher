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
using System.IO;
using System.Collections;


[CustomEditor(typeof(PKFxSceneMeshBuilder))]
public class PKFxSceneMeshBuilderEditor : Editor {

	SerializedProperty m_OutputPkmmPath;
	SerializedProperty m_GameObjectsToSearch;
	SerializedProperty m_MeshGameObjects;

	//----------------------------------------------------------------------------

	void OnEnable()
	{
		this.m_OutputPkmmPath = serializedObject.FindProperty("m_OutputPkmmPath");
		this.m_GameObjectsToSearch = serializedObject.FindProperty("m_GameObjectsToSearch");
		this.m_MeshGameObjects = serializedObject.FindProperty("m_MeshGameObjects");
	}

	//----------------------------------------------------------------------------

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		DrawDefaultInspector();
		if (GUILayout.Button("Build meshes"))
		{
			FindMeshes();
			BuildMeshes();
		}
		serializedObject.ApplyModifiedProperties();
	}

	//----------------------------------------------------------------------------

	private void FindMeshes()
	{
		this.m_MeshGameObjects.ClearArray();
		for (int i = 0; i < m_GameObjectsToSearch.arraySize; i++)
		{
			FillMeshesWithChildren(m_GameObjectsToSearch.GetArrayElementAtIndex(i).objectReferenceValue as GameObject);
		}
	}

	//----------------------------------------------------------------------------

	private void FillMeshesWithChildren(GameObject o)
	{
		foreach (Transform t in o.transform)
		{
			FillMeshesWithChildren(t.gameObject);
		}

		if (o.GetComponent<MeshFilter>() != null)
		{
			// add gameObject to meshGameObject
			this.m_MeshGameObjects.InsertArrayElementAtIndex(this.m_MeshGameObjects.arraySize);
			this.m_MeshGameObjects.GetArrayElementAtIndex(this.m_MeshGameObjects.arraySize - 1).objectReferenceValue = o as Object;
		}
	}

	//----------------------------------------------------------------------------

	private void BuildMeshes()
	{
		string			outputPkmm = m_OutputPkmmPath.stringValue;
		if (outputPkmm.Length <= 0)
		{
			Debug.LogError("[PKFX] SceneMeshBuilder: invalid mesh path");
			return;
		}
		PKFxManager.Startup();
		if (!PKFxManager.TryLoadPackRelative())
			PKFxManager.LoadPack(PKFxManager.m_PackPath + "/PackFx");
		PKFxManager.SceneMeshClear();
		for (int meshi = 0; meshi < m_MeshGameObjects.arraySize; meshi++)
		{
			GameObject			obj = m_MeshGameObjects.GetArrayElementAtIndex(meshi).objectReferenceValue as GameObject;
			Mesh				mesh = obj.GetComponent<MeshFilter>().sharedMesh;

			if (!PKFxManager.SceneMeshAddMesh(mesh, obj.transform.localToWorldMatrix))
			{
				Debug.LogError("[PKFX] SceneMeshBuilder: failed to load mesh " + obj.name + "");
			}
		}
		string dir = Path.GetDirectoryName(outputPkmm);
		dir = Path.Combine("PackFx", dir);
		dir = Path.Combine(PKFxManager.m_PackPath, dir);
		Directory.CreateDirectory(dir);
		int		primCount = PKFxManager.SceneMeshBuild(outputPkmm);
		if (primCount < 0)
		{
			Debug.LogError("[PKFX] SceneMeshBuilder: failed to save scene mesh " + this.name + "");
		}
		else
		{
			Debug.Log("[PKFX] SceneMeshBuilder: mesh ok " + this.name + " (prim count: " + primCount.ToString() + ")");
		}
	}
}
