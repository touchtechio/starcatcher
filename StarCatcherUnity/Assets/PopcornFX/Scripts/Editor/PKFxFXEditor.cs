//----------------------------------------------------------------------------
// Created on Tue Sep 2 18:09:33 2014 Raphael Thoulouze
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
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(PKFxFX))]
[CanEditMultipleObjects]
public class PKFxFXEditor : Editor
{
    SerializedProperty 	m_FxName;
	SerializedProperty 	m_FxAttributes;
	SerializedProperty 	m_FxSamplers;
	SerializedProperty 	m_PlayOnStart;
	SerializedProperty 	m_IsPlaying;

	SerializedProperty	m_Smp;
	SerializedProperty	m_ShapeType;
	SerializedProperty	m_EditorShapeType;
	SerializedProperty	m_ShapeCenter;
	SerializedProperty	m_Dimensions;
	SerializedProperty	m_EulerOrientation;

	string[] 			m_FxList;
	SerializedProperty	m_BoundFx;

	//----------------------------------------------------------------------------

	void OnEnable()
	{
		PKFxManager.Startup();
		if (!PKFxManager.TryLoadPackRelative())
			PKFxManager.LoadPack(PKFxManager.m_PackPath + "/PackFx");
		this.m_FxName = serializedObject.FindProperty("m_FxName");
		this.m_FxAttributes = serializedObject.FindProperty("m_FxAttributesList");
		this.m_FxSamplers = serializedObject.FindProperty("m_FxSamplersList");
		this.m_IsPlaying = serializedObject.FindProperty("m_IsPlaying");
		this.m_PlayOnStart = serializedObject.FindProperty("m_PlayOnStart");
		this.m_BoundFx = serializedObject.FindProperty("m_BoundFx");

		serializedObject.ApplyModifiedProperties();
		Reload(false);
	}
	
	//----------------------------------------------------------------------------

	public void OnSceneGUI()
	{
		for (int i = 0; i < this.m_FxSamplers.arraySize; i++)
		{
			m_Smp = this.m_FxSamplers.GetArrayElementAtIndex(i);
			if (m_Smp.FindPropertyRelative("m_Descriptor.Type").intValue == (int)PKFxManager.ESamplerType.SamplerShape)
			{
				m_ShapeType = m_Smp.FindPropertyRelative("m_ShapeType");
				m_ShapeCenter = m_Smp.FindPropertyRelative("m_ShapeCenter");
				m_Dimensions = m_Smp.FindPropertyRelative("m_Dimensions");
				m_EulerOrientation = m_Smp.FindPropertyRelative("m_EulerOrientation");
				
				if (m_ShapeType.intValue == (int)PKFxManager.EShapeType.BoxShape)
					DrawCube(i);
				else if (m_ShapeType.intValue == (int)PKFxManager.EShapeType.SphereShape)
					DrawSphere(i);
				else if (m_ShapeType.intValue == (int)PKFxManager.EShapeType.CylinderShape)
					DrawCylinder(i);
				else if (m_ShapeType.intValue == (int)PKFxManager.EShapeType.CapsuleShape)
					DrawCapsule(i);
			}
		}
	}

	//----------------------------------------------------------------------------

    public override void OnInspectorGUI()
    {
		serializedObject.Update();
		if (!string.IsNullOrEmpty(m_FxName.stringValue) && File.Exists("Assets/StreamingAssets/PackFx/" + m_FxName.stringValue))
		{
			m_BoundFx.objectReferenceValue = AssetDatabase.LoadAssetAtPath("Assets/StreamingAssets/PackFx/" + m_FxName.stringValue, typeof(Object));
		}
		m_BoundFx.objectReferenceValue = EditorGUILayout.ObjectField("FX", m_BoundFx.objectReferenceValue, typeof(Object), false);
		string tmpPath = "";
		if (m_BoundFx.objectReferenceValue != null)
			tmpPath = AssetDatabase.GetAssetPath(m_BoundFx.objectReferenceValue);
		if (tmpPath.StartsWith("Assets/StreamingAssets/PackFx/") && tmpPath.EndsWith(".pkfx"))
		{
			if (tmpPath.Substring("Assets/StreamingAssets/PackFx/".Length) != m_FxName.stringValue)
			{
				m_FxName.stringValue = tmpPath.Substring("Assets/StreamingAssets/PackFx/".Length);
				serializedObject.ApplyModifiedProperties();
				if (!string.IsNullOrEmpty(this.m_FxName.stringValue))
					PKFxManager.PreLoadFxIFN(this.m_FxName.stringValue);
				Reload(true);
				if (Application.isPlaying)
					(serializedObject.targetObject as PKFxFX).StartEffect();
			}
		}
		else
		{
			if (!string.IsNullOrEmpty(tmpPath))
				Debug.LogError("[PKFX] Invalid FX path.\n" +
				               "Effects must be baked in Assets/StreamingAssets/PackFx/ (case sensitive) and have a .pkfx extension");
			else
				m_FxName.stringValue = "";
			m_BoundFx.objectReferenceValue = null;
			serializedObject.ApplyModifiedProperties();
			Reload(false);
		}
		EditorGUI.BeginDisabledGroup(true);
		EditorGUILayout.PropertyField(this.m_IsPlaying);
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.PropertyField(this.m_PlayOnStart);

		for (int i = 0; i < this.m_FxSamplers.arraySize; i++)
		{
			SerializedProperty smp = this.m_FxSamplers.GetArrayElementAtIndex(i);
			SamplerField(smp);
		}

		if (m_FxAttributes.hasMultipleDifferentValues == false)
		{
			for (int i = 0; i < this.m_FxAttributes.arraySize; i++)
			{
				SerializedProperty attr = this.m_FxAttributes.GetArrayElementAtIndex(i);
				attr = PKFxEditor.AttributeField(attr);
			}
		}
		serializedObject.ApplyModifiedProperties();
		displayPlaybackBtns();
    }

	//----------------------------------------------------------------------------
	
	private void displayPlaybackBtns()
	{
		serializedObject.Update();
		Object[] effects = serializedObject.targetObjects;
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Reload Attributes"))
		{
			PKFxManager.UnloadEffect(m_FxName.stringValue);
			Reload(false);
		}
		if (GUILayout.Button("Reset Attributes To Default"))
		{
			foreach (PKFxFX fx in effects)
			{
				List<PKFxManager.AttributeDesc> FxAttributesDesc = PKFxManager.ListEffectAttributesFromFx(fx.FxPath);
				fx.ResetAttributesToDefault(FxAttributesDesc);
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Stop"))
		{
			foreach (PKFxFX fx in effects)
			{
				fx.StopEffect();
			}
		}
		if (GUILayout.Button("Terminate"))
		{
			foreach (PKFxFX fx in effects)
			{
				fx.TerminateEffect();
			}
		}
		if (PKFxManager.KillIndividualEffectEnabled() && GUILayout.Button("Kill"))
		{
			foreach (PKFxFX fx in effects)
			{
				fx.KillEffect();
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		EditorGUI.BeginDisabledGroup(this.m_IsPlaying.boolValue);
		if (GUILayout.Button("Start"))
		{
			foreach (PKFxFX fx in effects)
			{
				fx.StartEffect();
			}
		}
		EditorGUI.EndDisabledGroup();
		if (GUILayout.Button("Restart"))
		{
			foreach (PKFxFX fx in effects)
			{
				fx.TerminateEffect();
				fx.StartEffect();
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	//----------------------------------------------------------------------------

	void Reload(bool flushAttributes)
	{
		serializedObject.Update();
		Object[] effects = serializedObject.targetObjects;
		foreach (PKFxFX fx in effects)
		{
			List<PKFxManager.AttributeDesc> FxAttributesDesc = PKFxManager.ListEffectAttributesFromFx(fx.FxPath);
			fx.LoadAttributes(FxAttributesDesc, flushAttributes);
			List<PKFxManager.SamplerDesc> FxSamplersDesc = PKFxManager.ListEffectSamplersFromFx(fx.FxPath);
			fx.LoadSamplers(FxSamplersDesc, flushAttributes);
		}
		serializedObject.ApplyModifiedProperties();
	}

	//----------------------------------------------------------------------------

	private SerializedProperty SamplerField(SerializedProperty sampler)
	{
		SerializedProperty m_Name = sampler.FindPropertyRelative("m_Descriptor.Name");
		SerializedProperty m_Type = sampler.FindPropertyRelative("m_Descriptor.Type");
		if (m_Type.intValue == (int)PKFxManager.ESamplerType.SamplerShape)
		{
			m_ShapeType = sampler.FindPropertyRelative("m_ShapeType");
			m_EditorShapeType = sampler.FindPropertyRelative("m_EditorShapeType");
			EditorGUILayout.LabelField(m_Name.stringValue);
			EditorGUI.indentLevel++;
			m_EditorShapeType.intValue = EditorGUILayout.Popup(m_EditorShapeType.intValue + 1, ShapeTypes);
			m_EditorShapeType.intValue--; // -1 to remove the index of None in ShapeTypes
			m_ShapeType.intValue = m_EditorShapeType.intValue;
			if (m_ShapeType.intValue >= 5)
				m_ShapeType.intValue--; //Remove the index of MESHFILTER in ShapeTypes
			if (m_EditorShapeType.intValue == (int)PKFxManager.EShapeType.BoxShape)
				BoxField(sampler);
			else if (m_EditorShapeType.intValue == (int)PKFxManager.EShapeType.SphereShape)
				SphereField(sampler);
			else if (m_EditorShapeType.intValue == (int)PKFxManager.EShapeType.CylinderShape)
				CylinderField(sampler);
			else if (m_EditorShapeType.intValue == (int)PKFxManager.EShapeType.CapsuleShape)
				CapsuleField(sampler);
			else if (m_EditorShapeType.intValue == (int)PKFxManager.EShapeType.MeshShape)
				MeshField(sampler);
			else if (m_EditorShapeType.intValue == (int)PKFxManager.EShapeType.MeshShape + 1) //MeshFilter
				MeshFilterField(sampler);
			else if (m_EditorShapeType.intValue == (int)PKFxManager.EShapeType.SkinnedMeshShape + 1) //Offset cause of the MeshFilter
				SkinnedMeshField(sampler);
			
			EditorGUI.indentLevel--;
		}
		else if (m_Type.intValue == (int)PKFxManager.ESamplerType.SamplerCurve)
		{
			SerializedProperty m_CurvesArray = sampler.FindPropertyRelative("m_CurvesArray");
			SerializedProperty m_CurvesTimeKeys = sampler.FindPropertyRelative("m_CurvesTimeKeys");
			EditorGUILayout.LabelField(m_Name.stringValue);
			EditorGUI.indentLevel++;
			int popupIndex = m_CurvesArray.arraySize == 0 ? 0 : m_CurvesArray.arraySize;
			int arraySize = EditorGUILayout.Popup(popupIndex, CurveDimensions);
			MultipleCurvesEditor(m_CurvesArray, arraySize);
			if (m_CurvesArray.arraySize != 0)
			{
				int iKey = 0;
				m_CurvesTimeKeys.arraySize = m_CurvesArray.GetArrayElementAtIndex(0).animationCurveValue.keys.Length;
				foreach (var key in m_CurvesArray.GetArrayElementAtIndex(0).animationCurveValue.keys)
				{
					m_CurvesTimeKeys.GetArrayElementAtIndex(iKey++).floatValue = key.time;
				}
			}
			else
				m_CurvesTimeKeys.arraySize = 0;
			EditorGUI.indentLevel--;
		}
		else if (m_Type.intValue == (int)PKFxManager.ESamplerType.SamplerImage)
		{
			EditorGUI.DrawRect(new Rect(0,0,10,10), new Color(0,1,0,1));
			SerializedProperty m_Tex = sampler.FindPropertyRelative("m_Texture");
			SerializedProperty m_TexChanged = sampler.FindPropertyRelative("m_TextureChanged");
			SerializedProperty m_TextureTexcoordMode = sampler.FindPropertyRelative("m_TextureTexcoordMode");
			m_TexChanged.boolValue = false;
			Texture2D newTex = (Texture2D)EditorGUILayout.ObjectField(m_Name.stringValue, m_Tex.objectReferenceValue, typeof(Texture2D), false);
			if (newTex != m_Tex.objectReferenceValue)
			{
				m_TexChanged.boolValue = true;
				m_Tex.objectReferenceValue = newTex;
			}
			EditorGUI.indentLevel++;
			EditorGUILayout.LabelField("Texcoord Mode");
			PKFxManager.ETexcoordMode newType = (PKFxManager.ETexcoordMode)EditorGUILayout.EnumPopup((PKFxManager.ETexcoordMode)m_TextureTexcoordMode.intValue);
			m_TextureTexcoordMode.intValue = (int)newType;
			EditorGUI.indentLevel--;
		}
		else if (m_Type.intValue == (int)PKFxManager.ESamplerType.SamplerText)
		{
			SerializedProperty m_Text = sampler.FindPropertyRelative("m_Text");

			EditorGUI.DrawRect(new Rect(0,0,10,10), new Color(0,0,1,1));
			EditorGUILayout.LabelField(m_Name.stringValue);
			EditorGUI.indentLevel++;
			m_Text.stringValue = EditorGUILayout.TextField(m_Text.stringValue);
			EditorGUI.indentLevel--;
		}
		else if (m_Type.intValue == (int)PKFxManager.ESamplerType.SamplerUnsupported)
		{
			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.DrawRect(new Rect(0, 0, 10, 10), new Color(0, 0, 1, 1));
			EditorGUILayout.LabelField(m_Name.stringValue);
			EditorGUI.EndDisabledGroup();
		}
		return sampler;
	}

	public void MultipleCurvesEditor(SerializedProperty curvesArray, int curveCount)
	{
		//delete curve IFN
		while (curvesArray.arraySize > curveCount)
			curvesArray.DeleteArrayElementAtIndex(curvesArray.arraySize - 1);
		//create curve IFN
		while (curvesArray.arraySize < curveCount)
		{
			curvesArray.InsertArrayElementAtIndex(curvesArray.arraySize);
			AnimationCurve curve;
			if (curvesArray.arraySize == 1)
			{
				curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 0.0f);
				curve.postWrapMode = WrapMode.Clamp;
				curve.preWrapMode = WrapMode.Clamp;
			}
			else
				curve = curvesArray.GetArrayElementAtIndex(0).animationCurveValue;
			curvesArray.GetArrayElementAtIndex(curvesArray.arraySize - 1).animationCurveValue = curve;
		}
		for (int i = 0; i < curvesArray.arraySize; ++i)
		{
			Keyframe[] keysCache = curvesArray.GetArrayElementAtIndex(i).animationCurveValue.keys;
			EditorGUILayout.PropertyField(curvesArray.GetArrayElementAtIndex(i), new GUIContent(CurveDimensionsNames[i]));
			MultipleCurvesCheckModify(curvesArray, i, keysCache);
		}
	}

	public void MultipleCurvesCheckModify(SerializedProperty curvesArray, int curveIndex, Keyframe[] keysCache)
	{
		AnimationCurve curve = curvesArray.GetArrayElementAtIndex(curveIndex).animationCurveValue;
		if (curve.keys.Length > 0)
		{
			if (curve.keys.Length < 2)
			{
				curve.keys = keysCache;
				curvesArray.GetArrayElementAtIndex(curveIndex).animationCurveValue = curve;
			}
			//add key
			else if (curve.keys.Length > keysCache.Length)
			{
				List<float> addedKeys = new List<float>();
				MultipleCurvesFindKeysCountChanges(curve.keys, keysCache, addedKeys);
				foreach (var key in addedKeys)
					MultipleCurvesAddKey(curvesArray, curveIndex, key);
			}
			//delete key
			else if (curve.keys.Length < keysCache.Length)
			{
				List<float> deletedKeys = new List<float>();
				MultipleCurvesFindKeysCountChanges(keysCache, curve.keys, deletedKeys);
				foreach (var key in deletedKeys)
					MultipleCurvesDeleteKey(curvesArray, curveIndex, key);
			}
			//change key
			else if (curve.keys.Length == keysCache.Length)
			{
				curvesArray.GetArrayElementAtIndex(curveIndex).animationCurveValue = curve;
				List<float> oldKeys;
				List<float> newKeys;
				MultipleCurvesFindKeysChanges(curve.keys, keysCache, out oldKeys, out newKeys);
				if (newKeys.Count != 0)
				{
					for (int iKey = 0; iKey < newKeys.Count; ++iKey)
						MultipleCurvesChangeKey(curvesArray, curveIndex, oldKeys[iKey], newKeys[iKey]);
				}
			}
		}
	}

	public void MultipleCurvesFindKeysCountChanges(Keyframe[] refKeys, Keyframe[] compKeys, List<float> diffKeys)
	{
		foreach (var key in refKeys)
		{
			bool found = false;
			foreach (var othkey in compKeys)
			{
				if (key.time == othkey.time)
				{
					found = true;
					break;
				}
			}
			if (!found)
				diffKeys.Add(key.time);
		}
	}

	public void MultipleCurvesFindKeysChanges(Keyframe[] actualKeys, Keyframe[] cacheKeys, out List<float> oldKeys, out List<float> newKeys)
	{
		List<float> addedKeys = new List<float>();
		List<float> deletedKeys = new List<float>();
		MultipleCurvesFindKeysCountChanges(actualKeys, cacheKeys, addedKeys);
		MultipleCurvesFindKeysCountChanges(cacheKeys, actualKeys, deletedKeys);
		if (addedKeys.Count != 0 && addedKeys.Count == deletedKeys.Count)
		{
			addedKeys.Sort();
			deletedKeys.Sort();
		}
		oldKeys = deletedKeys;
		newKeys = addedKeys;
	}

	public void MultipleCurvesAddKey(SerializedProperty curvesArray, int sourceCurveIndex, float time)
	{
		for (int i = 0; i < curvesArray.arraySize; ++i)
		{
			if (i == sourceCurveIndex)
				continue;
			AnimationCurve curve = curvesArray.GetArrayElementAtIndex(i).animationCurveValue;
			curve.AddKey(time, curve.Evaluate(time));
			curvesArray.GetArrayElementAtIndex(i).animationCurveValue = curve;
		}
	}

	public void MultipleCurvesDeleteKey(SerializedProperty curvesArray, int sourceCurveIndex, float time)
	{
		for (int i = 0; i < curvesArray.arraySize; ++i)
		{
			if (i == sourceCurveIndex)
				continue;
			AnimationCurve curve = curvesArray.GetArrayElementAtIndex(i).animationCurveValue;
			for (int iKey = 0; iKey < curve.keys.Length; ++iKey)
			{
				if (curve.keys[iKey].time == time)
				{
					curve.RemoveKey(iKey);
					curvesArray.GetArrayElementAtIndex(i).animationCurveValue = curve;
					break;
				}
			}
		}
	}

	public void MultipleCurvesChangeKey(SerializedProperty curvesArray, int sourceCurveIndex, float oldTime, float newTime)
	{
		for (int i = 0; i < curvesArray.arraySize; ++i)
		{
			if (i == sourceCurveIndex)
				continue;
			AnimationCurve curve = curvesArray.GetArrayElementAtIndex(i).animationCurveValue;
			int iKey;
			for (iKey = 0; iKey < curve.keys.Length; ++iKey)
			{
				if (curve.keys[iKey].time == oldTime)
				{
					Keyframe keyframe = curve.keys[iKey];
					keyframe.time = newTime;
					curve.RemoveKey(iKey);
					curve.AddKey(keyframe);
					curvesArray.GetArrayElementAtIndex(i).animationCurveValue = curve;
					break;
				}
			}
		}
	}

	public void MultipleCurvesUpdateKeys(SerializedProperty curvesArray, int sourceCurveIndex)
	{
		AnimationCurve sourceCurve = curvesArray.GetArrayElementAtIndex(sourceCurveIndex).animationCurveValue;
		for (int i = 0; i < curvesArray.arraySize; ++i)
		{
			if (i == sourceCurveIndex)
				continue;
			AnimationCurve curve = curvesArray.GetArrayElementAtIndex(i).animationCurveValue;
			curve.keys = sourceCurve.keys;
			curvesArray.GetArrayElementAtIndex(i).animationCurveValue = curve;
		}
	}


	//----------------------------------------------------------------------------

	private static readonly string[] ShapeTypes = { "None", "BOX", "SPHERE", "CYLINDER", "CAPSULE", "MESH", "MESHFILTER", "SKINNEDMESH" };
	private static readonly string[] CurveDimensions = { "None", "1", "2", "3", "4" };
	private static readonly string[] CurveDimensionsNames = { "X", "Y", "Z", "W" };

	private static void 				BoxField(SerializedProperty sampler)
	{
		SerializedProperty m_ShapeCenter = sampler.FindPropertyRelative("m_ShapeCenter");
		SerializedProperty m_Dimensions = sampler.FindPropertyRelative("m_Dimensions");
		SerializedProperty m_EulerOrientation = sampler.FindPropertyRelative("m_EulerOrientation");
		EditorGUILayout.PropertyField(m_Dimensions);
		EditorGUILayout.PropertyField(m_ShapeCenter);
		EditorGUILayout.PropertyField(m_EulerOrientation);
	}

	private static void 				SphereField(SerializedProperty sampler)
	{
		SerializedProperty m_ShapeCenter = sampler.FindPropertyRelative("m_ShapeCenter");
		SerializedProperty m_Dimensions = sampler.FindPropertyRelative("m_Dimensions");
		SerializedProperty m_EulerOrientation = sampler.FindPropertyRelative("m_EulerOrientation");
		Vector3 tmp = m_Dimensions.vector3Value;
		tmp.y = EditorGUILayout.FloatField("Inner Radius", Mathf.Min(tmp.x, tmp.y));
		tmp.x = EditorGUILayout.FloatField("Radius", Mathf.Max(tmp.x, tmp.y));
		m_Dimensions.vector3Value = tmp;
		EditorGUILayout.PropertyField(m_ShapeCenter);
		EditorGUILayout.PropertyField(m_EulerOrientation);
	}

	private static void 				CylinderField(SerializedProperty sampler)
	{
		SerializedProperty m_ShapeCenter = sampler.FindPropertyRelative("m_ShapeCenter");
		SerializedProperty m_Dimensions = sampler.FindPropertyRelative("m_Dimensions");
		SerializedProperty m_EulerOrientation = sampler.FindPropertyRelative("m_EulerOrientation");
		Vector3 tmp = m_Dimensions.vector3Value;
		tmp.y = EditorGUILayout.FloatField("Inner Radius", Mathf.Min(tmp.x, tmp.y));
		tmp.x = EditorGUILayout.FloatField("Radius", Mathf.Max(tmp.x, tmp.y));
		tmp.z = EditorGUILayout.FloatField("Height", tmp.z);
		m_Dimensions.vector3Value = tmp;
		EditorGUILayout.PropertyField(m_ShapeCenter);
		EditorGUILayout.PropertyField(m_EulerOrientation);
	}

	private static void 				CapsuleField(SerializedProperty sampler)
	{
		SerializedProperty m_ShapeCenter = sampler.FindPropertyRelative("m_ShapeCenter");
		SerializedProperty m_Dimensions = sampler.FindPropertyRelative("m_Dimensions");
		SerializedProperty m_EulerOrientation = sampler.FindPropertyRelative("m_EulerOrientation");
		Vector3 tmp = m_Dimensions.vector3Value;
		tmp.y = EditorGUILayout.FloatField("Inner Radius", Mathf.Min(tmp.x, tmp.y));
		tmp.x = EditorGUILayout.FloatField("Radius", Mathf.Max(tmp.x, tmp.y));
		tmp.z = EditorGUILayout.FloatField("Height", tmp.z);
		m_Dimensions.vector3Value = tmp;
		EditorGUILayout.PropertyField(m_ShapeCenter);
		EditorGUILayout.PropertyField(m_EulerOrientation);
	}

	private static void SamplingChannelsFields(SerializedProperty sampler, bool enableVelocity)
	{
		SerializedProperty m_SamplingChannels = sampler.FindPropertyRelative("m_SamplingChannels");

		EditorGUILayout.Toggle("Sample Positions", true);
		m_SamplingChannels.intValue |= (int)PKFxManager.EMeshChannels.Channel_Position;

		if (EditorGUILayout.Toggle("Sample Normals", (m_SamplingChannels.intValue & (int)PKFxManager.EMeshChannels.Channel_Normal) != 0))
			m_SamplingChannels.intValue |= (int)PKFxManager.EMeshChannels.Channel_Normal;
		else
			m_SamplingChannels.intValue &= ~(int)PKFxManager.EMeshChannels.Channel_Normal;
		if (EditorGUILayout.Toggle("Sample Tangents", (m_SamplingChannels.intValue & (int)PKFxManager.EMeshChannels.Channel_Tangent) != 0))
			m_SamplingChannels.intValue |= (int)PKFxManager.EMeshChannels.Channel_Tangent;
		else
			m_SamplingChannels.intValue &= ~(int)PKFxManager.EMeshChannels.Channel_Tangent;
		if (!enableVelocity)
			m_SamplingChannels.intValue &= ~(int)PKFxManager.EMeshChannels.Channel_Velocity;
		else
		{
			if (EditorGUILayout.Toggle("Sample Velocity", (m_SamplingChannels.intValue & (int)PKFxManager.EMeshChannels.Channel_Velocity) != 0))
				m_SamplingChannels.intValue |= (int)PKFxManager.EMeshChannels.Channel_Velocity;
			else
				m_SamplingChannels.intValue &= ~(int)PKFxManager.EMeshChannels.Channel_Velocity;
		}
		if (EditorGUILayout.Toggle("Sample UVs", (m_SamplingChannels.intValue & (int)PKFxManager.EMeshChannels.Channel_UV) != 0))
			m_SamplingChannels.intValue |= (int)PKFxManager.EMeshChannels.Channel_UV;
		else
			m_SamplingChannels.intValue &= ~(int)PKFxManager.EMeshChannels.Channel_UV;
		if (EditorGUILayout.Toggle("Sample Vertex Color", (m_SamplingChannels.intValue & (int)PKFxManager.EMeshChannels.Channel_VertexColor) != 0))
			m_SamplingChannels.intValue |= (int)PKFxManager.EMeshChannels.Channel_VertexColor;
		else
			m_SamplingChannels.intValue &= ~(int)PKFxManager.EMeshChannels.Channel_VertexColor;
	}

	private static void MeshField(SerializedProperty sampler)
	{
		SerializedProperty m_ShapeCenter = sampler.FindPropertyRelative("m_ShapeCenter");
		SerializedProperty m_Dimensions = sampler.FindPropertyRelative("m_Dimensions");
		SerializedProperty m_EulerOrientation = sampler.FindPropertyRelative("m_EulerOrientation");
		SerializedProperty m_SkinnedMeshRenderer = sampler.FindPropertyRelative("m_SkinnedMeshRenderer");
		SerializedProperty m_MeshFilter = sampler.FindPropertyRelative("m_MeshFilter");
		SerializedProperty m_Mesh = sampler.FindPropertyRelative("m_Mesh");
		SerializedProperty m_MeshHashCode = sampler.FindPropertyRelative("m_MeshHashCode");

		EditorGUILayout.PropertyField(m_Mesh);
		if (m_Mesh.objectReferenceValue != null)
			m_MeshHashCode.intValue = (m_Mesh.objectReferenceValue as Mesh).name.GetHashCode();
		else
			m_MeshHashCode.intValue = 0;
		m_SkinnedMeshRenderer.objectReferenceValue = null;
		m_MeshFilter.objectReferenceValue = null;
		EditorGUILayout.PropertyField(m_Dimensions);
		EditorGUILayout.PropertyField(m_ShapeCenter);
		EditorGUILayout.PropertyField(m_EulerOrientation);

		SamplingChannelsFields(sampler, false);
	}

	private static void MeshFilterField(SerializedProperty sampler)
	{
		SerializedProperty m_ShapeCenter = sampler.FindPropertyRelative("m_ShapeCenter");
		SerializedProperty m_Dimensions = sampler.FindPropertyRelative("m_Dimensions");
		SerializedProperty m_EulerOrientation = sampler.FindPropertyRelative("m_EulerOrientation");
		SerializedProperty m_SkinnedMeshRenderer = sampler.FindPropertyRelative("m_SkinnedMeshRenderer");
		SerializedProperty m_MeshFilter = sampler.FindPropertyRelative("m_MeshFilter");
		SerializedProperty m_Mesh = sampler.FindPropertyRelative("m_Mesh");
		SerializedProperty m_MeshHashCode = sampler.FindPropertyRelative("m_MeshHashCode");

		EditorGUILayout.PropertyField(m_MeshFilter);
		if (m_MeshFilter.objectReferenceValue != null)
			m_Mesh.objectReferenceValue = (m_MeshFilter.objectReferenceValue as MeshFilter).sharedMesh;
		if (m_Mesh.objectReferenceValue != null)
			m_MeshHashCode.intValue = (m_Mesh.objectReferenceValue as Mesh).name.GetHashCode();
		else
			m_MeshHashCode.intValue = 0;
		m_SkinnedMeshRenderer.objectReferenceValue = null;
		EditorGUILayout.PropertyField(m_Dimensions);
		EditorGUILayout.PropertyField(m_ShapeCenter);
		EditorGUILayout.PropertyField(m_EulerOrientation);

		SamplingChannelsFields(sampler, false);
	}

	private static void SkinnedMeshField(SerializedProperty sampler)
	{
		SerializedProperty m_ShapeCenter = sampler.FindPropertyRelative("m_ShapeCenter");
		SerializedProperty m_Dimensions = sampler.FindPropertyRelative("m_Dimensions");
		SerializedProperty m_EulerOrientation = sampler.FindPropertyRelative("m_EulerOrientation");
		SerializedProperty m_SkinnedMeshRenderer = sampler.FindPropertyRelative("m_SkinnedMeshRenderer");
		SerializedProperty m_MeshFilter = sampler.FindPropertyRelative("m_MeshFilter");
		SerializedProperty m_Mesh = sampler.FindPropertyRelative("m_Mesh");
		SerializedProperty m_MeshHashCode = sampler.FindPropertyRelative("m_MeshHashCode");

		EditorGUILayout.PropertyField(m_SkinnedMeshRenderer);
		m_MeshFilter.objectReferenceValue = null;
		if (m_SkinnedMeshRenderer.objectReferenceValue != null)
		{
			Mesh mesh = (m_SkinnedMeshRenderer.objectReferenceValue as SkinnedMeshRenderer).sharedMesh;
			if (mesh != null)
			{
				if (System.String.IsNullOrEmpty(mesh.name))
				{
					Debug.LogError("The mesh referenced by the SkinnedMeshRenderer must have a name");
					return;
				}
				m_MeshHashCode.intValue = mesh.name.GetHashCode();
				m_Mesh.objectReferenceValue = mesh;
			}
			else
			{
				m_MeshHashCode.intValue = 0;
				m_Mesh.objectReferenceValue = null;
			}
		}
		else
		{
			m_Mesh.objectReferenceValue = null;
			m_MeshHashCode.intValue = 0;
		}
		EditorGUILayout.PropertyField(m_Dimensions);
		EditorGUILayout.PropertyField(m_ShapeCenter);
		EditorGUILayout.PropertyField(m_EulerOrientation);

		SamplingChannelsFields(sampler, true);
	}

	//----------------------------------------------------------------------------

	public void DrawSphere(int i)
	{
		PKFxFX fx = (PKFxFX)target;
		float radius = m_Dimensions.vector3Value.x;
		float innerRadius = m_Dimensions.vector3Value.y;
		Vector3 center = m_ShapeCenter.vector3Value;
		Vector3 objectPos = ((GameObject)fx.gameObject).transform.position;
		Quaternion rotation = Quaternion.Euler(((GameObject)fx.gameObject).transform.eulerAngles
		                                       + m_EulerOrientation.vector3Value);

		Handles.color = Color.blue;
		innerRadius = Handles.RadiusHandle(rotation, objectPos + center, Mathf.Min(radius, innerRadius));
		Handles.color = Color.cyan;
		radius = Handles.RadiusHandle(rotation, objectPos + center, Mathf.Max(radius, innerRadius));
		m_Dimensions.vector3Value = new Vector3(radius,
		                                        innerRadius,
		                                        m_Dimensions.vector3Value.z);
		m_Dimensions.serializedObject.ApplyModifiedProperties();
	}

	//----------------------------------------------------------------------------

	private void _PrimitiveCapsule(ref float radius, Vector2 minMax, ref float height, Vector3 center, Quaternion rotation)
	{
		Vector3 topCenter = center + new Vector3(0f, height/2f, 0f);
		Vector3 lowCenter = center - new Vector3(0f, height/2f, 0f);

		Vector3 dir = topCenter - center;
		dir = rotation * dir;
		topCenter = center + dir;
		
		dir = lowCenter - center;
		dir = rotation * dir;
		lowCenter = center + dir;

		if (minMax.x != -1)
		{
			radius = Handles.RadiusHandle(rotation, topCenter, Mathf.Max(radius, minMax.x));
			radius = Handles.RadiusHandle(rotation, lowCenter, Mathf.Max(radius, minMax.x));
		}
		else if (minMax.y != -1)
		{
			radius = Handles.RadiusHandle(rotation, topCenter, Mathf.Min(radius, minMax.y));
			radius = Handles.RadiusHandle(rotation, lowCenter, Mathf.Min(radius, minMax.y));
		}
		Handles.DrawLine(topCenter + rotation * new Vector3(radius,0f,0f), lowCenter + rotation * new Vector3(radius,0f,0f));
		Handles.DrawLine(topCenter - rotation * new Vector3(radius,0f,0f), lowCenter - rotation * new Vector3(radius,0f,0f));
		Handles.DrawLine(topCenter + rotation * new Vector3(0f,0f,radius), lowCenter + rotation * new Vector3(0f,0f,radius));
		Handles.DrawLine(topCenter - rotation * new Vector3(0f,0f,radius), lowCenter - rotation * new Vector3(0f,0f,radius));
	}

	public void DrawCapsule(int i)
	{
		PKFxFX fx = (PKFxFX)target;

		float radius = m_Dimensions.vector3Value.x;
		float innerRadius = m_Dimensions.vector3Value.y;
		float height = m_Dimensions.vector3Value.z;
		Vector3 center = ((GameObject)fx.gameObject).transform.position + m_ShapeCenter.vector3Value;
		Quaternion rotation = Quaternion.Euler(((GameObject)fx.gameObject).transform.eulerAngles
		                                       + m_EulerOrientation.vector3Value);

		Handles.color = Color.blue;
		_PrimitiveCapsule(ref innerRadius, new Vector2(-1, radius), ref height, center, rotation);

		Handles.color = Color.cyan;
		_PrimitiveCapsule(ref radius, new Vector2(innerRadius, -1), ref height, center, rotation);

		m_Dimensions.vector3Value = new Vector3(radius,
		                                        innerRadius,
		                                        height);
		m_Dimensions.serializedObject.ApplyModifiedProperties();
	}

	//----------------------------------------------------------------------------

	public void _PrimitiveCylinder(ref float radius, ref float height, Vector3 center, Quaternion rotation)
	{
		Vector3 topCenter = center + new Vector3(0f, height/2f, 0f);
		Vector3 lowCenter = center - new Vector3(0f, height/2f, 0f);

		Vector3 dir = topCenter - center;
		dir = rotation * dir;
		topCenter = center + dir;

		dir = lowCenter - center;
		dir = rotation * dir;
		lowCenter = center + dir;

		Handles.CircleCap(0, topCenter, rotation * Quaternion.FromToRotation(Vector3.forward, Vector3.up), radius);
		Handles.CircleCap(0, lowCenter, rotation * Quaternion.FromToRotation(Vector3.forward, Vector3.up), radius);
		Handles.DrawLine(topCenter + rotation * new Vector3(radius,0f,0f), lowCenter + rotation * new Vector3(radius,0f,0f));
		Handles.DrawLine(topCenter - rotation * new Vector3(radius,0f,0f), lowCenter - rotation * new Vector3(radius,0f,0f));
		Handles.DrawLine(topCenter + rotation * new Vector3(0f,0f,radius), lowCenter + rotation * new Vector3(0f,0f,radius));
		Handles.DrawLine(topCenter - rotation * new Vector3(0f,0f,radius), lowCenter - rotation * new Vector3(0f,0f,radius));
	}

	public void DrawCylinder(int i)
	{
		PKFxFX fx = (PKFxFX)target;
		float radius = m_Dimensions.vector3Value.x;
		float innerRadius = m_Dimensions.vector3Value.y;
		float height = m_Dimensions.vector3Value.z;
		Vector3 center = ((GameObject)fx.gameObject).transform.position + m_ShapeCenter.vector3Value;
		Quaternion rotation = Quaternion.Euler(((GameObject)fx.gameObject).transform.eulerAngles
		                                       + m_EulerOrientation.vector3Value);

		Handles.color = Color.blue;
		_PrimitiveCylinder(ref innerRadius, ref height, center, rotation);

		Handles.color = Color.cyan;
		_PrimitiveCylinder(ref radius, ref height, center, rotation);

		m_Dimensions.vector3Value = new Vector3(radius,
		                                        innerRadius,
		                                        height);
		m_Dimensions.serializedObject.ApplyModifiedProperties();
	}

	//----------------------------------------------------------------------------

	public void DrawCube(int i)
	{
		PKFxFX fx = (PKFxFX)target;
		Vector3		center = ((GameObject)fx.gameObject).transform.position + m_ShapeCenter.vector3Value;
		Quaternion rotation = Quaternion.Euler(((GameObject)fx.gameObject).transform.eulerAngles
		                                       + m_EulerOrientation.vector3Value);
		Vector3		size = m_Dimensions.vector3Value;
		Vector3		A = center + rotation * new Vector3(-size.x/2, size.y/2, size.z/2);
		Vector3		B = center + rotation * new Vector3(size.x/2, size.y/2, size.z/2);
		Vector3		C = center + rotation * new Vector3(size.x/2, -size.y/2, size.z/2);
		Vector3		D = center + rotation * new Vector3(-size.x/2, -size.y/2, size.z/2);
		Vector3		E = center + rotation * new Vector3(-size.x/2, size.y/2, -size.z/2);
		Vector3		F = center + rotation * new Vector3(size.x/2, size.y/2,  -size.z/2);
		Vector3		G = center + rotation * new Vector3(size.x/2, -size.y/2, -size.z/2);
		Vector3		H = center + rotation * new Vector3(-size.x/2, -size.y/2, -size.z/2);
		Vector3[]	face = new Vector3[5];

		Handles.color = Color.cyan;
		face[0] = A;
		face[1] = B;
		face[2] = C;
		face[3] = D;
		face[4] = A;
		Handles.DrawPolyLine(face);
		face[0] = A;
		face[1] = E;
		face[2] = H;
		face[3] = D;
		face[4] = A;
		Handles.DrawPolyLine(face);
		face[0] = B;
		face[1] = F;
		face[2] = G;
		face[3] = C;
		face[4] = B;
		Handles.DrawPolyLine(face);
		face[0] = E;
		face[1] = F;
		face[2] = G;
		face[3] = H;
		face[4] = E;
		Handles.DrawPolyLine(face);
	}	
}