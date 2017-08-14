//----------------------------------------------------------------------------
// Created on Thu Jun 26 15:28:42 2014 Raphael Thoulouze
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

[CustomEditor(typeof(PKFxRenderingPlugin))]
[CanEditMultipleObjects]
public class PKFxRenderingPluginEditor : Editor
{

	SerializedProperty		m_ShowAdvanced;
	SerializedProperty		m_TimeMultiplier;
	SerializedProperty		m_EnableSoftParticles;
	SerializedProperty		m_EnableDistortion;
	SerializedProperty		m_UseDepthGrabToZTest;
	SerializedProperty		m_DepthGrabFormat;
	SerializedProperty		m_EnableBlur;
	SerializedProperty		m_BlurFactor;
	SerializedProperty		m_UseSceneMesh;
	SerializedProperty		m_SceneMeshPkmmPath;
	SerializedProperty		m_TextureLODBias;

	//----------------------------------------------------------------------------

	void OnEnable()
	{
		m_ShowAdvanced = serializedObject.FindProperty("m_ShowAdvancedSettings");
		m_EnableSoftParticles = serializedObject.FindProperty("m_EnableSoftParticles");
		m_EnableDistortion = serializedObject.FindProperty("m_EnableDistortion");
		m_UseDepthGrabToZTest = serializedObject.FindProperty("m_UseDepthGrabToZTest");
		m_DepthGrabFormat = serializedObject.FindProperty("m_DepthGrabFormat");
		m_EnableBlur = serializedObject.FindProperty("m_EnableBlur");
		m_BlurFactor = serializedObject.FindProperty("m_BlurFactor");
		m_UseSceneMesh = serializedObject.FindProperty("m_UseSceneMesh");
		m_SceneMeshPkmmPath = serializedObject.FindProperty("m_SceneMeshPkmmPath");
		m_TextureLODBias = serializedObject.FindProperty("m_TextureLODBias");
		m_TimeMultiplier = serializedObject.FindProperty("m_TimeMultiplier");
	}

	//----------------------------------------------------------------------------

	public override void OnInspectorGUI()
	{
		if (string.IsNullOrEmpty(PKFxManager.m_CurrentVersionString))
			PKFxManager.Startup();
		EditorGUILayout.LabelField("PopcornFX plugin "
									+ PKFxManager.m_PluginVersion
									+ " (Build "
									+ PKFxManager.m_CurrentVersionString + ")");

		DrawDefaultInspector();

		EditorGUILayout.PropertyField(m_ShowAdvanced);
		if (m_ShowAdvanced.boolValue)
		{
			EditorGUI.indentLevel++;

			EditorGUILayout.Slider(m_TimeMultiplier, 0.0f, 8.0f);
			EditorGUILayout.PropertyField(this.m_EnableSoftParticles);
			if (this.m_EnableSoftParticles.boolValue)
			{
				if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Direct3D11)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(this.m_DepthGrabFormat);
					EditorGUILayout.PropertyField(this.m_UseDepthGrabToZTest);
					EditorGUI.indentLevel--;
				}
			}
			EditorGUILayout.PropertyField(this.m_EnableDistortion);
			if (m_EnableDistortion.boolValue)
			{
				m_EnableSoftParticles.boolValue = true;
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.m_EnableBlur);
				if (m_EnableBlur.boolValue)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(this.m_BlurFactor);
					m_BlurFactor.floatValue = Mathf.Clamp(m_BlurFactor.floatValue,0.0f,1.0f);
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.PropertyField(this.m_UseSceneMesh);
			if (m_UseSceneMesh.boolValue)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.m_SceneMeshPkmmPath);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.PropertyField(this.m_TextureLODBias);
			EditorGUI.indentLevel--;
		}
		serializedObject.ApplyModifiedProperties();
	}

	//----------------------------------------------
	[InitializeOnLoad]
	public class PlayModeChangeWatcher
	{
		static PlayModeChangeWatcher()
		{
			EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
		}

		private static void PlaymodeStateChanged()
		{
			if (!EditorApplication.isPlaying && !EditorApplication.isPaused)
				PKFxManager.Reset();
		}
	}
}
