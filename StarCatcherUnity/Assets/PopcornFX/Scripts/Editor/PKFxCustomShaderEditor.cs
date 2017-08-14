//----------------------------------------------------------------------------
// Created on Thu Jul 2 14:28:23 2015 Valentin Bas
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
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(PkFxCustomShader))]
public class PKFxCustomShaderEditor : Editor
{
	SerializedProperty	m_IsSoft;
	SerializedProperty	m_HasSoftAnimBlend;
	SerializedProperty	m_IsDistortion;
	SerializedProperty	m_IsMesh;
	SerializedProperty	m_HasMeshTexture;
	SerializedProperty	m_IsGL3;
	
	SerializedProperty	m_Api;
	SerializedProperty	m_VertexType;
	SerializedProperty	m_PixelType;
	SerializedProperty	m_ShaderName;
	SerializedProperty	m_ShaderGroup;
	SerializedProperty	m_ShaderConstantList;

	bool				m_ShowShaderTypes;
	bool				m_ShowShaderConstants;

	bool				m_InShaderConstantsLoading;

	public enum EShaderApiEditor
	{
		DX9,
		DX11,
		GL2,
		GL3,
		GLES
	}

	//----------------------------------------------------------------------------

	void OnEnable()
	{
		PKFxManager.Startup();
		if (!PKFxManager.TryLoadPackRelative())
			PKFxManager.LoadPack(PKFxManager.m_PackPath + "/PackFx");

		m_InShaderConstantsLoading = false;
		ReloadConstants(false);

		m_IsSoft = serializedObject.FindProperty("m_IsSoft");
		m_HasSoftAnimBlend = serializedObject.FindProperty("m_HasSoftAnimBlend");
		m_IsDistortion = serializedObject.FindProperty("m_IsDistortion");
		m_IsMesh = serializedObject.FindProperty("m_IsMesh");
		m_HasMeshTexture = serializedObject.FindProperty("m_HasMeshTexture");
		m_IsGL3 = serializedObject.FindProperty("m_IsGL3");

		m_Api = serializedObject.FindProperty("m_Api");
		m_VertexType = serializedObject.FindProperty("m_VertexType");
		m_PixelType = serializedObject.FindProperty("m_PixelType");
		m_ShaderName = serializedObject.FindProperty("m_ShaderName");
		m_ShaderGroup = serializedObject.FindProperty("m_ShaderGroup");
		m_ShaderConstantList = serializedObject.FindProperty("m_ShaderConstantList");
		m_ShowShaderTypes = false;
		m_ShowShaderConstants = true;
	}

	//----------------------------------------------------------------------------

	private EShaderApiEditor ShaderApiToShaderApiEditor(PkFxCustomShader.EShaderApi api, bool isGL3)
	{
		EShaderApiEditor apiType = (EShaderApiEditor)api;
		if (apiType == EShaderApiEditor.GL2 && isGL3)
			apiType = EShaderApiEditor.GL3;
		else if (apiType == EShaderApiEditor.GL3)
			apiType = EShaderApiEditor.GLES;
		return apiType;
	}

	//----------------------------------------------------------------------------

	private void SetShaderApiFromShaderApiEditor(EShaderApiEditor editorApi, SerializedProperty api, SerializedProperty isGL3)
	{
		api.intValue = (int)editorApi;
		isGL3.boolValue = false;
		if (editorApi == EShaderApiEditor.GL3)
		{
			api.intValue = (int)PkFxCustomShader.EShaderApi.GL;
			isGL3.boolValue = true;
		}
		else if (editorApi == EShaderApiEditor.GLES)
			api.intValue = (int)PkFxCustomShader.EShaderApi.GLES;
	}

	//----------------------------------------------------------------------------

	public override void OnInspectorGUI()
	{
		if (m_InShaderConstantsLoading)
		{
			ReloadConstants(false);
		}

		m_ShaderName.stringValue = EditorGUILayout.TextField(m_ShaderName.stringValue);
		EditorGUI.indentLevel++;
		EditorGUILayout.PropertyField(m_ShaderGroup);

		EShaderApiEditor apiType = ShaderApiToShaderApiEditor((PkFxCustomShader.EShaderApi)m_Api.intValue, m_IsGL3.boolValue);
		apiType = (EShaderApiEditor)EditorGUILayout.EnumPopup("Api", apiType);
		SetShaderApiFromShaderApiEditor(apiType, m_Api, m_IsGL3);

		EditorGUILayout.PropertyField(m_IsMesh);
		if (m_IsMesh.boolValue == false)
		{
			if ((PkFxCustomShader.EShaderApi)m_Api.intValue != PkFxCustomShader.EShaderApi.GLES)
			{
				EditorGUILayout.PropertyField(m_HasSoftAnimBlend);
				EditorGUILayout.PropertyField(m_IsDistortion);
			}
			EditorGUILayout.PropertyField(m_IsSoft);
		}
		else
			EditorGUILayout.PropertyField(m_HasMeshTexture);

		if (m_IsDistortion.boolValue || m_IsMesh.boolValue)
		{
			m_IsSoft.boolValue = false;
			m_HasSoftAnimBlend.boolValue = false;
		}

		if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GLES)
			m_HasSoftAnimBlend.boolValue = false;

		SetVertexAndPixelType();
		DrawVertexAndPixelType();

		m_ShowShaderConstants = EditorGUILayout.ToggleLeft("Show Shader Constants", m_ShowShaderConstants);
		if (m_ShowShaderConstants)
		{
			EditorGUI.indentLevel++;

			if (GUILayout.Button("Reload"))
				ReloadConstants(false);

			for (int i = 0; i < this.m_ShaderConstantList.arraySize; i++)
			{
				SerializedProperty attr = this.m_ShaderConstantList.GetArrayElementAtIndex(i);
				attr = PKFxEditor.AttributeField(attr);
			}
			EditorGUI.indentLevel--;
		}

		EditorGUI.indentLevel--;
		serializedObject.ApplyModifiedProperties();
	}

	//----------------------------------------------------------------------------

	private void SetVertexAndPixelType()
	{
		if (m_IsMesh.boolValue)
		{
			if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GLES)
			{
				m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeGLES.VertexPC_Mesh;
				if (m_HasMeshTexture.boolValue)
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeGLES.PixelPCT;
				else
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeGLES.PixelPC;
			}
			else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GL)
			{
				if (m_IsGL3.boolValue)
					m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeGL.VertexPC_Mesh_GL3;
				else
					m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeGL.VertexPC_Mesh_GL2;
				if (m_HasMeshTexture.boolValue)
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeGL.PixelPCT;
				else
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeGL.PixelPC;
			}
			else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.DX9)
			{
				if (m_HasMeshTexture.boolValue)
				{
					m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeDX9.VertexPCD_Mesh;
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeDX9.PixelPCD_Mesh;
				}
				else
				{
					m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeDX9.VertexPC_Mesh;
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeDX9.PixelPC_Mesh;
				}
			}
			else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.DX11)
			{
				if (m_HasMeshTexture.boolValue)
				{
					m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeDX11.VertexPCD_Mesh;
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeDX11.PixelPCD_Mesh;
				}
				else
				{
					m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeDX11.VertexPC_Mesh;
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeDX11.PixelPC_Mesh;
				}
			}
			return;
		}

		if (m_IsDistortion.boolValue)
		{
			if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.DX9)
			{
				m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeDX9.VertexPCT;
				m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeDX9.PixelD;
			}
			else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.DX11)
			{
				m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeDX11.VertexPCT_Depth;
				m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeDX11.PixelPCT_Distortion;
			}
			else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GL)
			{
				m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeGL.VertexPCT_Soft;
				m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeGL.PixelPCT_Dist;
			}
			else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GLES)
			{
				m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeGLES.VertexPCT_Soft;
				m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeGLES.PixelPCT_Dist;
			}
		}
		else if (m_IsSoft.boolValue)
		{
			if (m_HasSoftAnimBlend.boolValue)
			{
				//Soft + Anim Blend
				if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.DX9)
				{
					m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeDX9.VertexPCTA;
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeDX9.PixelPCTA_Soft;
				}
				else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.DX11)
				{
					m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeDX11.VertexPCTA_Depth;
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeDX11.PixelPCTA_Soft;
				}
				else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GL)
				{
					m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeGL.VertexPCTA_Soft;
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeGL.PixelPCTA_Soft;
				}
			}
			else
			{
				//Soft
				if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.DX9)
				{
					m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeDX9.VertexPCT;
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeDX9.PixelPCT_Soft;
				}
				else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.DX11)
				{
					m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeDX11.VertexPCT_Depth;
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeDX11.PixelPCT_Soft;
				}
				else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GL)
				{
					m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeGL.VertexPCT_Soft;
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeGL.PixelPCT_Soft;
				}
				else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GLES)
				{
					m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeGLES.VertexPCT_Soft;
					m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeGLES.PixelPCT_Soft;
				}
			}
		}
		else if (m_HasSoftAnimBlend.boolValue)
		{
			//Anim Blend
			if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.DX9)
			{
				m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeDX9.VertexPCTA;
				m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeDX9.PixelPCTA;
			}
			else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.DX11)
			{
				m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeDX11.VertexPCTA;
				m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeDX11.PixelPCTA;
			}
			else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GL)
			{
				m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeGL.VertexPCTA;
				m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeGL.PixelPCTA;
			}
		}
		else
		{
			//Default
			if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.DX9)
			{
				m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeDX9.VertexPCT;
				m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeDX9.PixelPCT;
			}
			else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.DX11)
			{
				m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeDX11.VertexPCT;
				m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeDX11.PixelPCT;
			}
			else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GL)
			{
				m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeGL.VertexPCT;
				m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeGL.PixelPCT;
			}
			else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GLES)
			{
				m_VertexType.intValue = (int)PkFxCustomShader.EVertexShaderTypeGLES.VertexPCT;
				m_PixelType.intValue = (int)PkFxCustomShader.EPixelShaderTypeGLES.PixelPCT;
			}
		}
	}

	//----------------------------------------------------------------------------

	private void DrawVertexAndPixelType()
	{
		m_ShowShaderTypes = EditorGUILayout.ToggleLeft("Show Shader Types", m_ShowShaderTypes);
		if (!m_ShowShaderTypes)
			return;
		EditorGUI.indentLevel++;
		if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.DX9)
		{
			PkFxCustomShader.EVertexShaderTypeDX9 vertexType = (PkFxCustomShader.EVertexShaderTypeDX9)m_VertexType.intValue;
			vertexType = (PkFxCustomShader.EVertexShaderTypeDX9)EditorGUILayout.EnumPopup("Vertex type", vertexType);
			//m_VertexType.intValue = (int)vertexType;
			PkFxCustomShader.EPixelShaderTypeDX9 pixelType = (PkFxCustomShader.EPixelShaderTypeDX9)m_PixelType.intValue;
			pixelType = (PkFxCustomShader.EPixelShaderTypeDX9)EditorGUILayout.EnumPopup("Pixel type", pixelType);
			//m_PixelType.intValue = (int)pixelType;
		}
		else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.DX11)
		{
			PkFxCustomShader.EVertexShaderTypeDX11 vertexType = (PkFxCustomShader.EVertexShaderTypeDX11)m_VertexType.intValue;
			vertexType = (PkFxCustomShader.EVertexShaderTypeDX11)EditorGUILayout.EnumPopup("Vertex type", vertexType);
			//m_VertexType.intValue = (int)vertexType;
			PkFxCustomShader.EPixelShaderTypeDX11 pixelType = (PkFxCustomShader.EPixelShaderTypeDX11)m_PixelType.intValue;
			pixelType = (PkFxCustomShader.EPixelShaderTypeDX11)EditorGUILayout.EnumPopup("Pixel type", pixelType);
			//m_PixelType.intValue = (int)pixelType;
		}
		else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GL)
		{
			PkFxCustomShader.EVertexShaderTypeGL vertexType = (PkFxCustomShader.EVertexShaderTypeGL)m_VertexType.intValue;
			vertexType = (PkFxCustomShader.EVertexShaderTypeGL)EditorGUILayout.EnumPopup("Vertex type", vertexType);
			//m_VertexType.intValue = (int)vertexType;
			PkFxCustomShader.EPixelShaderTypeGL pixelType = (PkFxCustomShader.EPixelShaderTypeGL)m_PixelType.intValue;
			pixelType = (PkFxCustomShader.EPixelShaderTypeGL)EditorGUILayout.EnumPopup("Pixel type", pixelType);
			//m_PixelType.intValue = (int)pixelType;
		}
		else if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GLES)
		{
			PkFxCustomShader.EVertexShaderTypeGLES vertexType = (PkFxCustomShader.EVertexShaderTypeGLES)m_VertexType.intValue;
			vertexType = (PkFxCustomShader.EVertexShaderTypeGLES)EditorGUILayout.EnumPopup("Vertex type", vertexType);
			//m_VertexType.intValue = (int)vertexType;
			PkFxCustomShader.EPixelShaderTypeGLES pixelType = (PkFxCustomShader.EPixelShaderTypeGLES)m_PixelType.intValue;
			pixelType = (PkFxCustomShader.EPixelShaderTypeGLES)EditorGUILayout.EnumPopup("Pixel type", pixelType);
			//m_PixelType.intValue = (int)pixelType;
		}
		WriteDefaultShader();
		EditorGUI.indentLevel--;
	}

	//----------------------------------------------------------------------------

	private void WriteDefaultShader()
	{
		if (GUILayout.Button("Write default shaders"))
		{
			string shaderPath = PKFxManager.m_PackPath + "/PackFx/" + m_ShaderName.stringValue;
			try
			{
				if (File.Exists(shaderPath))
				{
					var sr = File.CreateText(shaderPath);
					if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GL)
						sr.WriteLine("#ifdef PK_VERTEX_SHADER\n");
					else //DX9 DX11
						sr.WriteLine("#if defined(PK_VERTEX_SHADER)\n");
					sr.WriteLine(PKFxManager.GetDefaultShader(m_Api.intValue, m_VertexType.intValue));
					sr.WriteLine("#endif\n\n//------------------------------------------------\n");
					if ((PkFxCustomShader.EShaderApi)m_Api.intValue == PkFxCustomShader.EShaderApi.GL)
						sr.WriteLine("#ifdef PK_PIXEL_SHADER\n");
					else //DX9 DX11
						sr.WriteLine("#if defined(PK_PIXEL_SHADER)\n");
					sr.WriteLine(PKFxManager.GetDefaultShader(m_Api.intValue, m_PixelType.intValue));
					sr.WriteLine("#endif\n");
					sr.Close();
				}
				else
					Debug.LogError("Shader file doesn't exists");
			}
			catch (System.Exception e)
			{
				Debug.LogError("Can't write to shader file : " + e.ToString());
			}
		}
	}

	//----------------------------------------------------------------------------

	private void ReloadConstants(bool flush)
	{
		serializedObject.Update();
		PkFxCustomShader customShader = (serializedObject.targetObject as PkFxCustomShader);

		int count = -1;

		if (customShader.m_Api == PkFxCustomShader.EShaderApi.GL && !m_InShaderConstantsLoading)
		{
			PKFxManager.ShaderConstantsCount(customShader.m_ShaderName, (int)customShader.m_Api);
			GL.IssuePluginEvent(PKFxManager.GetGLConstantsCountEvent(), (int)(PKFxManager.POPCORN_MAGIC_NUMBER | 0x00004000));
			m_InShaderConstantsLoading = true;
		}
		else
		{
			count = PKFxManager.ShaderConstantsCount(customShader.m_ShaderName, (int)customShader.m_Api);
		}

		if (count != -1)
		{
			m_InShaderConstantsLoading = false;
			List<PKFxManager.ShaderConstantDesc> FxAttributesDesc = PKFxManager.ListShaderConstantsFromName(customShader.m_ShaderName, count);
			customShader.LoadShaderConstants(FxAttributesDesc, flush);
			EditorUtility.SetDirty(target as PkFxCustomShader);
			AssetDatabase.SaveAssets();
			serializedObject.ApplyModifiedProperties();
			serializedObject.Update();
		}
	}
}
