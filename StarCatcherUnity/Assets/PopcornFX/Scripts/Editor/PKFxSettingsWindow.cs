//----------------------------------------------------------------------------
// Created on Tue Oct 27 12:27:00 2015 Raphael Thoulouze
//
// This program is the property of Persistant Studios SARL.
//
// You may not redistribute it and/or modify it under any conditions
// without written permission from Persistant Studios SARL, unless
// otherwise stated in the latest Persistant Studios Code License.
//
// See the Persistant Studios Code License for further details.
//----------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;

public class PKFxSettingsWindow : EditorWindow
{
	private static bool								m_EnableKilling;
	private static bool								m_EnableLogging;
	private static bool								m_EnablePersistentDataPath;
	private static bool								m_UseOrthographicProjection;
	private static PKFxManager.E_AvailableCamEvents	m_CamEventHook;

	//----------------------------------------------------------------------------

	[MenuItem("Edit/PopcornFX Preferences...")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(PKFxSettingsWindow), true, "PopcornFX Preferences", true);
		m_EnableKilling = PKFxManager.m_GlobalConf.enableEffectsKill;
		m_EnableLogging = PKFxManager.m_GlobalConf.enableFileLog;
		m_EnablePersistentDataPath = PKFxManager.m_GlobalConf.enablePackFxInPersistentDataPath;
		m_UseOrthographicProjection = PKFxManager.m_GlobalConf.useOrthographicProjection;
		m_CamEventHook = PKFxManager.m_GlobalConf.globalEventSetting;
	}

	//----------------------------------------------------------------------------

	void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Enable Effects Killing");
		m_EnableKilling = EditorGUILayout.Toggle(m_EnableKilling);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Enable File Log");
		m_EnableLogging = EditorGUILayout.Toggle(m_EnableLogging);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("PackFx in Persistent Data Path");
		m_EnablePersistentDataPath = EditorGUILayout.Toggle(m_EnablePersistentDataPath);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField("Use Orthographic Projection (2D project)");
		m_UseOrthographicProjection = EditorGUILayout.Toggle(m_UseOrthographicProjection);
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField("Insert Native Redering");
		m_CamEventHook = (PKFxManager.E_AvailableCamEvents)EditorGUILayout.EnumPopup(m_CamEventHook);
		EditorGUILayout.EndHorizontal();

		if ((PKFxManager.m_IsStarted && m_EnableKilling != PKFxManager.KillIndividualEffectEnabled())
			|| m_EnableLogging != PKFxManager.FileLoggingEnabled()
			|| m_EnablePersistentDataPath != PKFxManager.PackInPersistantDataPathEnabled()
			|| (PKFxManager.m_IsStarted && m_UseOrthographicProjection != PKFxManager.IsUsingOrthographicProjection()))
		{
			EditorGUILayout.HelpBox("At least one of the changes requires a restart of Unity to be effective.", MessageType.Warning, true);
		}


		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Cancel"))
		{
			EditorWindow.GetWindow(typeof(PKFxSettingsWindow), true, "PopcornFX Preferences", true).Close();
		}
		if (GUILayout.Button("Save"))
		{
			PKFxManager.m_GlobalConf.enableEffectsKill = m_EnableKilling;
			PKFxManager.m_GlobalConf.enableFileLog = m_EnableLogging;
			PKFxManager.m_GlobalConf.enablePackFxInPersistentDataPath = m_EnablePersistentDataPath;
			PKFxManager.m_GlobalConf.useOrthographicProjection = m_UseOrthographicProjection;
			PKFxManager.m_GlobalConf.globalEventSetting = m_CamEventHook;
			if (m_EnablePersistentDataPath)
				PKFxManager.m_PackPath = Application.persistentDataPath;
			else
				PKFxManager.m_PackPath = Application.streamingAssetsPath;
			PKFxManager.m_GlobalConf.Save();
			EditorWindow.GetWindow(typeof(PKFxSettingsWindow), true, "PopcornFX Preferences", true).Close();
		}
		EditorGUILayout.EndHorizontal();
	}
}
