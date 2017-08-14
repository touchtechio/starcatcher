//----------------------------------------------------------------------------
// Created on Tue Jun 16 2015 18:53:57 Valentin Bas
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

[System.Serializable]
public class PkFxCustomShader : ScriptableObject
{
	public enum EVertexShaderTypeDX9
	{
		VertexP = 0,
		VertexPC,
		VertexPCT,
		VertexPCTA,
		VertexRibbon,
		VertexPC_Mesh,
		VertexPCD_Mesh,
		VertexMAX
	}

	public enum EPixelShaderTypeDX9
	{
		PixelP = EVertexShaderTypeDX9.VertexMAX,
		PixelPC,
		PixelPCT,
		PixelPCTA,
		PixelPCT_Soft,
		PixelPCTA_Soft,
		PixelD,
		PixelDA,
		PixelRibbon,
		PixelPC_Mesh,
		PixelPCD_Mesh,
	}

	public enum EVertexShaderTypeDX11
	{
		VertexPCT = 0,
		VertexPC,
		VertexPCTA,
		VertexPCTA_Depth,
		VertexPCTN,
		VertexPCTNA,
		VertexPCT_Depth,
		VertexRibbon,
		VertexPC_Mesh,
		VertexPCD_Mesh,
		VertexMAX
	}

	public enum EPixelShaderTypeDX11
	{
		PixelPC = EVertexShaderTypeDX11.VertexMAX,
		PixelPCT,
		PixelPCTA,
		PixelPCTA_Distortion,
		PixelPCTA_Soft,
		PixelPCTL,
		PixelPCTLA,
		PixelPCT_Distortion,
		PixelPCT_Soft,
		PixelRibbon,
		PixelPC_Mesh,
		PixelPCD_Mesh,
	}

	public enum EVertexShaderTypeGL
	{
		VertexP = 0,
		VertexPC,
		VertexPCT,
		VertexPCTA,
		VertexPCTA_Soft,
		VertexPCT_Soft,
		VertexRibbon_GL3,
		VertexRibbon_GL2,
		VertexPC_Mesh_GL3,
		VertexPC_Mesh_GL2,
		VertexMAX
	}

	public enum EPixelShaderTypeGL
	{
		PixelP = EVertexShaderTypeGL.VertexMAX,
		PixelPC,
		PixelPCT,
		PixelPCTA,
		PixelPCTA_Dist,
		PixelPCTA_Soft,
		PixelPCT_Dist,
		PixelPCT_Soft,
		PixelRibbon_GL3,
		PixelRibbon_GL2
	}

	public enum EVertexShaderTypeGLES
	{
		VertexP = 0,
		VertexPC,
		VertexPCT,
		VertexPCTA,
		VertexPCTA_Soft,
		VertexPCT_Soft,
		VertexRibbon,
		VertexPC_Mesh,
		VertexMAX
	}

	public enum EPixelShaderTypeGLES
	{
		PixelP = EVertexShaderTypeGLES.VertexMAX,
		PixelPC,
		PixelPCT,
		PixelPCTA,
		PixelPCTA_Dist,
		PixelPCTA_Soft,
		PixelPCT_Dist,
		PixelPCT_Soft,
		PixelRibbon,
	}

	public enum EShaderApi
	{
		DX9,
		DX11,
		GL,
		GLES
	}

	private struct SShaderConstantPinned
	{
		public int				m_Type;
		public float			m_Value0;
		public float			m_Value1;
		public float			m_Value2;
		public float			m_Value3;
	}

	private SShaderConstantPinned[]		m_ShaderConstantsCache;
	private GCHandle					m_ShaderConstantsGCH;
	private IntPtr						m_ShaderConstantsHandler;

	public bool					m_IsSoft;
	public bool					m_HasSoftAnimBlend;
	public bool					m_IsDistortion;
	public bool					m_IsMesh;
	public bool					m_HasMeshTexture;
	public bool					m_IsGL3;

	public EShaderApi			m_Api;
	public int					m_VertexType;
	public int					m_PixelType;
	public string				m_ShaderName;
	public string				m_ShaderGroup;
	public bool					m_GlobalShader;

	public uint					m_LoadedShaderId;

	public List<PKFxManager.ShaderConstant> m_ShaderConstantList;

	//----------------------------------------------------------------------------

	public void SetConstant(PKFxManager.ShaderConstant constant)
	{
		if (this.ShaderConstantExist(constant.m_Descriptor) == false)
			Debug.LogError("[PKFX] CustomShader.SetConstant : " + constant.m_Descriptor.Name + " doesn't exist");
		else
		{
			for (int i = 0; i < m_ShaderConstantList.Count; i++)
			{
				if (m_ShaderConstantList[i].m_Descriptor.Name == constant.m_Descriptor.Name)
				{
					m_ShaderConstantList[i].m_Value0 = constant.m_Value0;
					m_ShaderConstantList[i].m_Value1 = constant.m_Value1;
					m_ShaderConstantList[i].m_Value2 = constant.m_Value2;
					m_ShaderConstantList[i].m_Value3 = constant.m_Value3;
				}
			}
		}
	}

	//----------------------------------------------------------------------------

	public void LoadShaderConstants(List<PKFxManager.ShaderConstantDesc> ShaderConstantsDesc, bool flushAttributes)
	{
		if (flushAttributes)
			this.m_ShaderConstantList.Clear();
		List<PKFxManager.ShaderConstant> newList = new List<PKFxManager.ShaderConstant>();

		foreach (PKFxManager.ShaderConstantDesc desc in ShaderConstantsDesc)
		{
			if (this.ShaderConstantExist(desc) == false)
				newList.Add(new PKFxManager.ShaderConstant(desc));
			else
				newList.Add(GetShaderConstantFromDesc(desc));
		}
		this.m_ShaderConstantList = newList;
	}

	//----------------------------------------------------------------------------

	private void AllocAttributesCacheIFN()
	{
		if (this.m_ShaderConstantsCache == null || this.m_ShaderConstantsCache.Length < m_ShaderConstantList.Count)
		{
			this.m_ShaderConstantsCache = new SShaderConstantPinned[m_ShaderConstantList.Count];
			if (this.m_ShaderConstantsGCH.IsAllocated)
				this.m_ShaderConstantsGCH.Free();
			this.m_ShaderConstantsGCH = GCHandle.Alloc(this.m_ShaderConstantsCache, GCHandleType.Pinned);
			this.m_ShaderConstantsHandler = this.m_ShaderConstantsGCH.AddrOfPinnedObject();
		}
	}

	//----------------------------------------------------------------------------

	public void UpdateShaderConstants(bool forceUpdate)
	{
		int constCount = -1;

		if (m_ShaderConstantList.Count == 0)
			return;

		AllocAttributesCacheIFN();
		for (int i = 0; i < m_ShaderConstantList.Count; i++)
		{
			PKFxManager.ShaderConstant srcAttr = m_ShaderConstantList[i];
			bool wasChanged =	m_ShaderConstantsCache[i].m_Value0 != srcAttr.m_Value0 ||
								m_ShaderConstantsCache[i].m_Value1 != srcAttr.m_Value1 ||
								m_ShaderConstantsCache[i].m_Value2 != srcAttr.m_Value2 ||
								m_ShaderConstantsCache[i].m_Value3 != srcAttr.m_Value3;

			if (wasChanged || forceUpdate)
			{
				m_ShaderConstantsCache[i].m_Type = (int)(m_ShaderConstantList[i].m_Descriptor.Type);
				m_ShaderConstantsCache[i].m_Value0 = srcAttr.m_Value0;
				m_ShaderConstantsCache[i].m_Value1 = srcAttr.m_Value1;
				m_ShaderConstantsCache[i].m_Value2 = srcAttr.m_Value2;
				m_ShaderConstantsCache[i].m_Value3 = srcAttr.m_Value3;
				constCount = i;
			}
		}
		if (constCount >= 0)
		{
			if (!PKFxManager.ShaderSetConstant(m_LoadedShaderId, constCount + 1, m_ShaderConstantsHandler))
			{
				Debug.LogError("[PKFX] Shader constant through pinned memory failed.");
				Debug.LogError("[PKFX] Did you try to change an FX without stopping it beforehand?");
			}
		}
	}

	//----------------------------------------------------------------------------

	public PKFxManager.ShaderConstant GetShaderConstantFromDesc(PKFxManager.ShaderConstantDesc desc)
	{
		if (m_ShaderConstantList == null)
			return null;
		foreach (PKFxManager.ShaderConstant attr in this.m_ShaderConstantList)
			if (attr.m_Descriptor.Name == desc.Name && attr.m_Descriptor.Type == desc.Type)
				return attr;
		return null;
	}

	//----------------------------------------------------------------------------

	public bool ShaderConstantExist(PKFxManager.ShaderConstantDesc desc)
	{
		if (m_ShaderConstantList == null)
			return false;
		foreach (PKFxManager.ShaderConstant attr in this.m_ShaderConstantList)
			if (attr.m_Descriptor.Name == desc.Name && attr.m_Descriptor.Type == desc.Type)
			{
				return true;
			}
		return false;
	}

	//----------------------------------------------------------------------------

	public PKFxManager.ShaderDesc GetDesc()
	{
		PKFxManager.ShaderDesc	desc;

		desc.ShaderGroup = m_ShaderGroup;
		desc.ShaderPath = m_ShaderName;
		desc.Api = (int)m_Api;
		desc.VertexType = m_VertexType;
		desc.PixelType = m_PixelType;
		return desc;
	}

	public bool ApiInUse()
	{
		if (m_Api == EShaderApi.GL)
			return SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL");
		else if (m_Api == EShaderApi.DX9)
			return SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 9");
		else if (m_Api == EShaderApi.DX11)
			return SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 11");
		else if (m_Api == EShaderApi.GLES)
			return SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL ES");
		else
		{
			Debug.LogError("Invalid API");
			return false;
		}
	}
}