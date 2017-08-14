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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class PKFxFX : PKFxPackDependent 
{
	private struct SAttributePinned
	{
		public int			m_Type;
		public float		m_Value0;
		public float		m_Value1;
		public float		m_Value2;
		public float		m_Value3;
	}
	private struct SSamplerPinned
	{
		public int			m_Type1; // sampler type
		public int			m_Type2; // sampler shape's type
		public float		m_SizeX; // also radius or text len
		public float		m_SizeY; // also inner radius
		public float		m_SizeZ; // also height
		public float		m_PosX;
		public float		m_PosY;
		public float		m_PosZ;
		public float		m_EulX;
		public float		m_EulY;
		public float		m_EulZ;
		public int			m_MeshChanged;
		public int			m_HashCode;
		public int			m_IndexCount;
		public int			m_VertexCount;
		public int			m_BoneCount;
		public int			m_SamplingChannels;
		public IntPtr		m_Data; // text or image or curve's or shapemesh's data
	}

	[SerializeField]
	private bool			m_IsPlaying = false;
	private bool 			m_IsStopped = false;
	private int 			m_FXGUID = -1;

	public int FXGUID
	{
		get { return this.m_FXGUID; }
	}
	public List<PKFxManager.Attribute> 		m_FxAttributesList;
	public List<PKFxManager.Sampler> 		m_FxSamplersList;
	public bool 							m_PlayOnStart = true;
	static public Dictionary<int, PKFxFX>	m_ListEffects = new Dictionary<int, PKFxFX>();

	private bool 							m_ForceUpdateAttributes = false;
	private SAttributePinned[] 				m_AttributesCache;
	private GCHandle						m_AttributesGCH;
	private IntPtr							m_AttributesHandler;
	private SSamplerPinned[] 				m_SamplersCache;
	private GCHandle						m_SamplersGCH;
	private IntPtr							m_SamplersHandler;
	private float[] 						m_SamplersCurvesDataCache;
	private GCHandle						m_SamplersCurvesDataGCH;
	private IntPtr							m_SamplersCurvesDataHandler;

	public delegate void					OnFxStoppedDelegate(PKFxFX component);
	public OnFxStoppedDelegate				m_OnFxStopped = null;
	public UnityEngine.Object				m_BoundFx;
	public string 							m_FxName;
	public string							FxPath { get { return m_FxName; } }

	//----------------------------------------------------------------------------

	void Awake()
	{
		if (this.m_FxAttributesList == null)
			this.m_FxAttributesList = new List<PKFxManager.Attribute>();
		if (this.m_FxSamplersList == null)
			this.m_FxSamplersList = new List<PKFxManager.Sampler>();
		m_IsPlaying = false;
	}

	//----------------------------------------------------------------------------

	IEnumerator	Start()
	{
		base.BaseInitialize();
		yield return WaitForPack(true);

		if (!string.IsNullOrEmpty(m_FxName))
			PKFxManager.PreLoadFxIFN(FxPath);
		
		if (this.m_PlayOnStart)
			this.StartEffect();
		yield return null;
	}

	//----------------------------------------------------------------------------

	void LateUpdate()
	{
		if (!this.m_IsPlaying)
			return;
		PKFxManager.UpdateTransformEffect(this.m_FXGUID, this.transform);
		UpdateAttributes(false);
	}

	//----------------------------------------------------------------------------

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position, "FX.png", true);
	}

	//----------------------------------------------------------------------------

	void OnDestroy()
	{
		this.KillEffect();
		this.StopEffect();
		for (int i = 0; i < m_FxSamplersList.Count; i++)
		{
			if (m_SamplersCache != null && m_SamplersCache[i].m_Data != IntPtr.Zero &&
				m_SamplersCache[i].m_Type1 != (int)PKFxManager.ESamplerType.SamplerCurve)
				Marshal.FreeHGlobal(m_SamplersCache[i].m_Data);
		}
		m_SamplersCache = null;
		if (this.m_AttributesGCH.IsAllocated)
			this.m_AttributesGCH.Free();
		if (this.m_SamplersGCH.IsAllocated)
			this.m_SamplersGCH.Free();
		if (this.m_SamplersCurvesDataGCH.IsAllocated)
			this.m_SamplersCurvesDataGCH.Free();
	}

	//----------------------------------------------------------------------------

	public void StartEffect()
	{
		if (m_IsStopped)
		{
			Debug.LogWarning("[PKFX] Attempt to start an effect while the stopped effect is still running.");
			return;
		}
		if (this.m_IsPlaying) return;
		if (!string.IsNullOrEmpty(m_FxName))
			this.m_FXGUID = PKFxManager.CreateEffect(this.FxPath, this.transform);
		this.m_IsPlaying = (this.m_FXGUID != -1);
		
		if (this.m_FXGUID != -1)
		{
			if (m_ListEffects.ContainsKey(this.m_FXGUID))
				m_ListEffects[this.m_FXGUID] = this;
			else
				m_ListEffects.Add(this.m_FXGUID, this);
		}
		
		this.LoadAttributes(PKFxManager.ListEffectAttributesFromGUID(this.m_FXGUID), false);
		this.LoadSamplers(PKFxManager.ListEffectSamplersFromFx(this.m_FxName), false);
		UpdateAttributes(true);
	}

	//----------------------------------------------------------------------------

	public void TerminateEffect()
	{
		if (!this.m_IsPlaying || this.m_FXGUID == -1) return;
		PKFxManager.TerminateFx(this.m_FXGUID);
		//this.m_FXGUID = -1;
		this.m_IsPlaying = false;
	}

	//----------------------------------------------------------------------------
	
	public void StopEffect()
	{
#if PK_COMPILED
		Debug.LogError("[PKFX] You can't call StopEffect() in the compiled version of the plugin, use TerminateEffect() instead.");
#else
		if (!this.m_IsPlaying || this.m_FXGUID == -1) return;
		if (PKFxManager.StopFx(this.m_FXGUID))
			m_IsStopped = true;
#endif
	}

	//----------------------------------------------------------------------------

	public void OnFxStopPlaying()
	{
		m_IsPlaying = false;
		m_IsStopped = false;
	}

	//----------------------------------------------------------------------------

	public void KillEffect()
	{
		if (!this.m_IsPlaying || this.m_FXGUID == -1 || !PKFxManager.KillIndividualEffectEnabled()) return;
		PKFxManager.KillFx(this.m_FXGUID);
		//this.m_FXGUID = -1;
		this.m_IsPlaying = false;
	}

	//----------------------------------------------------------------------------

	public bool IsPlayable()
	{
		return ((!m_IsPlaying) && (m_FXGUID != -1));
	}

	//----------------------------------------------------------------------------

	public bool IsPlaying()
	{
		return m_IsPlaying;
	}

	//----------------------------------------------------------------------------

	public bool Alive()
	{
		if (!this.m_IsPlaying || this.m_FXGUID == -1) return false;
		return PKFxManager.IsFxAlive(this.m_FXGUID);
	}

	//----------------------------------------------------------------------------

	#region /!\ Attributes /!\

	public void OnFxHotReloaded(int newGuid)
	{
		if (newGuid != -1)
		{
			if (newGuid != m_FXGUID)
			{
				m_ListEffects.Remove(this.m_FXGUID);
				m_FXGUID = newGuid;
				m_ListEffects.Add(this.m_FXGUID, this);
			}
			m_ForceUpdateAttributes = true;
			m_IsPlaying = true;
		}
	}

	//----------------------------------------------------------------------------

	public PKFxManager.Attribute GetAttribute(string name)
	{
		foreach (PKFxManager.Attribute attr in m_FxAttributesList)
		{
			if (attr.m_Descriptor.Name == name)
				return attr;
		}
		return null;
	}

	//----------------------------------------------------------------------------

	public PKFxManager.Attribute GetAttribute(string name, PKFxManager.BaseType type)
	{
		foreach (PKFxManager.Attribute attr in m_FxAttributesList)
		{
			if (attr.m_Descriptor.Name == name && attr.m_Descriptor.Type == type)
				return attr;
		}
		return null;
	}

	//----------------------------------------------------------------------------

	public void SetAttribute(PKFxManager.Attribute attr)
	{
		if (this.AttributeExists(attr.m_Descriptor) == false)
			Debug.LogError("[PKFX] FX.SetAttribute : " + attr.m_Descriptor.Name + " doesn't exist");
		else
		{
			for (int i = 0; i < m_FxAttributesList.Count; i++)
			{
				if (m_FxAttributesList[i].m_Descriptor.Name == attr.m_Descriptor.Name)
				{
					m_FxAttributesList[i].m_Value0 = attr.m_Value0;
					m_FxAttributesList[i].m_Value1 = attr.m_Value1;
					m_FxAttributesList[i].m_Value2 = attr.m_Value2;
					m_FxAttributesList[i].m_Value3 = attr.m_Value3;
				}
			}
		}
	}

	//----------------------------------------------------------------------------

	public PKFxManager.Sampler GetSampler(string name)
	{
		foreach (PKFxManager.Sampler sampler in m_FxSamplersList)
		{
			if (sampler.m_Descriptor.Name == name)
				return sampler;
		}
		return null;
	}

	//----------------------------------------------------------------------------

	public PKFxManager.Sampler GetSampler(string name, PKFxManager.ESamplerType type)
	{
		foreach (PKFxManager.Sampler sampler in m_FxSamplersList)
		{
			if (sampler.m_Descriptor.Name == name && sampler.m_Descriptor.Type == (int)type)
				return sampler;
		}
		return null;
	}

	//----------------------------------------------------------------------------

	public void SetSampler(PKFxManager.Sampler sampler)
	{
		if (this.SamplerExists(sampler.m_Descriptor) == false)
			Debug.LogError("[PKFX] FX.SetSampler : " + sampler.m_Descriptor.Name + " doesn't exist");
		else
		{
			for (int i = 0; i < m_FxSamplersList.Count; i++)
			{
				if (m_FxSamplersList[i].m_Descriptor.Name == sampler.m_Descriptor.Name &&
					m_FxSamplersList[i].m_Descriptor.Type == sampler.m_Descriptor.Type)
				{
					m_FxSamplersList[i].Copy(sampler);
				}
			}
		}
	}

	//----------------------------------------------------------------------------
	
	public void LoadSamplers(List<PKFxManager.SamplerDesc> FxSamplersDesc, bool flushAttributes)
	{
		if (flushAttributes)
			this.m_FxSamplersList.Clear();
		List<PKFxManager.Sampler> newList = new List<PKFxManager.Sampler>();
		
		foreach (PKFxManager.SamplerDesc desc in FxSamplersDesc)
		{
			if (this.SamplerExists(desc) == false)
				newList.Add(new PKFxManager.Sampler(desc));
			else
				newList.Add(GetSamplerFromDesc(desc));
		}
		this.m_FxSamplersList = newList;
	}

	//----------------------------------------------------------------------------

	public void ResetAttributesToDefault(List<PKFxManager.AttributeDesc> FxAttributesDesc)
	{
		this.m_FxAttributesList.Clear();
		List<PKFxManager.Attribute> newList = new List<PKFxManager.Attribute>();

		foreach (PKFxManager.AttributeDesc desc in FxAttributesDesc)
		{
			newList.Add(new PKFxManager.Attribute(desc));
		}
		this.m_FxAttributesList = newList;
	}

	//----------------------------------------------------------------------------

	public void LoadAttributes(List<PKFxManager.AttributeDesc> FxAttributesDesc, bool flushAttributes)
	{
		if (flushAttributes)
			this.m_FxAttributesList.Clear();
		List<PKFxManager.Attribute> newList = new List<PKFxManager.Attribute>();
	
		foreach (PKFxManager.AttributeDesc desc in FxAttributesDesc)
		{
			if (this.AttributeExists(desc) == false)
				newList.Add(new PKFxManager.Attribute(desc));
			else
				newList.Add(GetAttributeFromDesc(desc));
		}
		this.m_FxAttributesList = newList;
	}

	//----------------------------------------------------------------------------

	private void AllocAttributesCacheIFN()
	{
		if (this.m_AttributesCache == null || this.m_AttributesCache.Length < m_FxAttributesList.Count)
		{
			this.m_AttributesCache = new SAttributePinned[m_FxAttributesList.Count];
			if (this.m_AttributesGCH.IsAllocated)
				this.m_AttributesGCH.Free();
			this.m_AttributesGCH = GCHandle.Alloc(this.m_AttributesCache, GCHandleType.Pinned);
			this.m_AttributesHandler = this.m_AttributesGCH.AddrOfPinnedObject();
		}
		if (this.m_SamplersCache == null || this.m_SamplersCache.Length < m_FxSamplersList.Count)
		{
			this.m_SamplersCache = new SSamplerPinned[m_FxSamplersList.Count];
			if (this.m_SamplersGCH.IsAllocated)
				this.m_SamplersGCH.Free();
			this.m_SamplersGCH = GCHandle.Alloc(this.m_SamplersCache, GCHandleType.Pinned);
			this.m_SamplersHandler = this.m_SamplersGCH.AddrOfPinnedObject();
		}
	}

	//----------------------------------------------------------------------------

	private void AllocCurvesDataCacheIFN()
	{
		int requiredSize = 0;
		foreach (var sampler in m_FxSamplersList)
		{
			if (sampler.m_CurvesTimeKeys != null)
			{
				requiredSize += sampler.m_CurvesTimeKeys.Length;
				foreach (var curve in sampler.m_CurvesArray)
				{
					requiredSize += curve.keys.Length * 3;
				}
			}
		}
		if (this.m_SamplersCurvesDataCache == null || this.m_SamplersCurvesDataCache.Length < requiredSize)
		{
			this.m_SamplersCurvesDataCache = new float[requiredSize];
			if (this.m_SamplersCurvesDataGCH.IsAllocated)
				this.m_SamplersCurvesDataGCH.Free();
			this.m_SamplersCurvesDataGCH = GCHandle.Alloc(m_SamplersCurvesDataCache, GCHandleType.Pinned);
			this.m_SamplersCurvesDataHandler = this.m_SamplersCurvesDataGCH.AddrOfPinnedObject();
		}
	}

	//----------------------------------------------------------------------------

	private static class PKImageConverter
	{
		public static void ARGB2BGRA(ref byte[] data)
		{
			for (int id = 0; id < data.Length;)
			{
				byte[] col = new byte[4] { data[id + 3], data[id + 2], data[id + 1], data[id] };
				data[id++] = col[0];
				data[id++] = col[1];
				data[id++] = col[2];
				data[id++] = col[3];
			}
		}

		public static void RGBA2BGRA(ref byte[] data)
		{
			for (int id = 0; id < data.Length; id += 4)
			{
				byte tmp = data[id];
				data[id] = data[id + 2];
				data[id + 2] = tmp;
			}
		}

		public static void RGB2BGR(ref byte[] data)
		{
			for (int id = 0; id < data.Length; id += 3)
			{
				byte tmp = data[id];
				data[id] = data[id + 2];
				data[id + 2] = tmp;
			}
		}
	}

	//----------------------------------------------------------------------------

	private void UpdateMesh(IntPtr outPtr, int[] trianglesSrc, Mesh mesh, int samplingChannels, bool withSkinning)
	{
		int offset;
		int vertexCount = mesh.vertexCount;

		Marshal.Copy(trianglesSrc, 0, outPtr, trianglesSrc.Length);
		outPtr = new IntPtr(outPtr.ToInt64() + trianglesSrc.Length * sizeof(int));

		//Positions
		if ((samplingChannels & (int)PKFxManager.EMeshChannels.Channel_Position) != 0)
		{
			float[] verticesData = new float[vertexCount * 3];
			Vector3[] verticesSrc = mesh.vertices;
			offset = 0;
			if (verticesSrc.Length == vertexCount)
			{
				foreach (var v in verticesSrc)
				{
					verticesData[offset + 0] = v.x;
					verticesData[offset + 1] = v.y;
					verticesData[offset + 2] = v.z;
					offset += 3;
				}
			}
			else
			{
				Debug.LogWarning("[PKFX] " + "The FX wants to sample Positions but the Mesh " + mesh.name + " doesn't have them.");
			}
			Marshal.Copy(verticesData, 0, outPtr, verticesData.Length);
			outPtr = new IntPtr(outPtr.ToInt64() + vertexCount * sizeof(float) * 3);
		}
		//Normals
		if ((samplingChannels & (int)PKFxManager.EMeshChannels.Channel_Normal) != 0)
		{
			float[] normalsData = new float[vertexCount * 3];
			Vector3[] normalsSrc = mesh.normals;
			offset = 0;
			if (normalsSrc.Length == vertexCount)
			{
				foreach (var n in normalsSrc)
				{
					normalsData[offset + 0] = n.x;
					normalsData[offset + 1] = n.y;
					normalsData[offset + 2] = n.z;
					offset += 3;
				}
			}
			else
			{
				Debug.LogWarning("[PKFX] " + "The FX wants to sample Normals but the Mesh " + mesh.name + " doesn't have them.");
			}
			Marshal.Copy(normalsData, 0, outPtr, normalsData.Length);
			
			outPtr = new IntPtr(outPtr.ToInt64() + vertexCount * sizeof(float) * 3);
		}
		//Tangents
		if ((samplingChannels & (int)PKFxManager.EMeshChannels.Channel_Tangent) != 0)
		{
			float[] tangentsData = new float[vertexCount * 4];
			Vector4[] tangentsSrc = mesh.tangents;
			offset = 0;
			if (tangentsSrc.Length == vertexCount)
			{
				foreach (var t in tangentsSrc)
				{
					tangentsData[offset + 0] = t.x; ;
					tangentsData[offset + 1] = t.y;
					tangentsData[offset + 2] = t.z;
					tangentsData[offset + 3] = t.w;
					offset += 4;
				}
			}
			else
			{
				Debug.LogWarning("[PKFX] " + "The FX wants to sample Tangents but the Mesh " + mesh.name + " doesn't have them.");
			}
			Marshal.Copy(tangentsData, 0, outPtr, tangentsData.Length);
			outPtr = new IntPtr(outPtr.ToInt64() + vertexCount * sizeof(float) * 4);
		}
		//UVs
		if ((samplingChannels & (int)PKFxManager.EMeshChannels.Channel_UV) != 0)
		{
			float[] uvsData = new float[vertexCount * 2];
			Vector2[] uvsSrc = mesh.uv;
			offset = 0;
			if (uvsSrc.Length == vertexCount)
			{
				foreach (var uv in uvsSrc)
				{
					uvsData[offset + 0] = uv.x; ;
					uvsData[offset + 1] = uv.y;
					offset += 2;
				}
			}
			else
			{
				Debug.LogWarning("[PKFX] " + "The FX wants to sample UVs but the Mesh " + mesh.name + " doesn't have them.");
			}
			Marshal.Copy(uvsData, 0, outPtr, uvsData.Length);
			outPtr = new IntPtr(outPtr.ToInt64() + vertexCount * sizeof(float) * 2);
		}
		//Vertex colors
		if ((samplingChannels & (int)PKFxManager.EMeshChannels.Channel_VertexColor) != 0)
		{
			float[] colorsData = new float[vertexCount * 4];
			Color[] colorsSrc = mesh.colors;
			offset = 0;
			if (colorsSrc.Length == vertexCount)
			{
				foreach (var c in colorsSrc)
				{
					colorsData[offset + 0] = c.r;
					colorsData[offset + 1] = c.g;
					colorsData[offset + 2] = c.b;
					colorsData[offset + 3] = c.a;
					offset += 4;
				}
			}
			else
			{
				Debug.LogWarning("[PKFX] " + "The FX wants to sample Vertex Colors but the Mesh " + mesh.name + " doesn't have them.");
			}
			Marshal.Copy(colorsData, 0, outPtr, colorsData.Length);
			outPtr = new IntPtr(outPtr.ToInt64() + vertexCount * sizeof(float) * 4);
		}

		if (withSkinning && mesh.boneWeights.Length > 0)
		{
			BoneWeight[] boneWeightsSrc = mesh.boneWeights;
			float[] boneWeightsData = new float[boneWeightsSrc.Length * 8];
			offset = 0;
			foreach (var b in boneWeightsSrc)
			{
				boneWeightsData[offset + 0] = b.boneIndex0;
				boneWeightsData[offset + 1] = b.boneIndex1;
				boneWeightsData[offset + 2] = b.boneIndex2;
				boneWeightsData[offset + 3] = b.boneIndex3;
				offset += 4;
			}
			foreach (var b in boneWeightsSrc)
			{
				boneWeightsData[offset + 0] = b.weight0;
				boneWeightsData[offset + 1] = b.weight1;
				boneWeightsData[offset + 2] = b.weight2;
				boneWeightsData[offset + 3] = b.weight3;
				offset += 4;
			}
			Marshal.Copy(boneWeightsData, 0, outPtr, boneWeightsData.Length);
		}
	}

	private void UpdateBones(IntPtr outPtr, PKFxManager.Sampler sampler)
	{
		int offset = 0;
		int iBone = 0;
		Matrix4x4 rootMat = sampler.m_SkinnedMeshRenderer.transform.parent.worldToLocalMatrix;
		Matrix4x4 mat = Matrix4x4.identity;
		float[] skeletonData = sampler.m_SkinnedMeshData.m_SkeletonDataBuffer;

		foreach (var bone in sampler.m_SkinnedMeshRenderer.bones)
		{
			mat = rootMat * bone.localToWorldMatrix * sampler.m_SkinnedMeshData.m_Bindposes[iBone];

			skeletonData[offset + 0] = mat[0, 0];
			skeletonData[offset + 1] = mat[0, 1];
			skeletonData[offset + 2] = mat[0, 2];
			skeletonData[offset + 3] = mat[0, 3];
			skeletonData[offset + 4] = mat[1, 0];
			skeletonData[offset + 5] = mat[1, 1];
			skeletonData[offset + 6] = mat[1, 2];
			skeletonData[offset + 7] = mat[1, 3];
			skeletonData[offset + 8] = mat[2, 0];
			skeletonData[offset + 9] = mat[2, 1];
			skeletonData[offset + 10] = mat[2, 2];
			skeletonData[offset + 11] = mat[2, 3];
			skeletonData[offset + 12] = mat[3, 0];
			skeletonData[offset + 13] = mat[3, 1];
			skeletonData[offset + 14] = mat[3, 2];
			skeletonData[offset + 15] = mat[3, 3];

			iBone++;
			offset += 16;
		}
		Marshal.Copy(skeletonData, 0, outPtr, skeletonData.Length);
	}

	public void UpdateAttributes(bool forceUpdate)
	{
		int	attrCount = -1;
		int	smpCount = -1;

		if (m_FxAttributesList.Count <= 0 && m_FxSamplersList.Count <= 0)
			return;

		AllocAttributesCacheIFN();
		AllocCurvesDataCacheIFN();

		// Attributes
		for (int i = 0; i < m_FxAttributesList.Count; i++)
		{
			PKFxManager.Attribute	srcAttr 	= 	m_FxAttributesList[i];

			bool					wasChanged =	m_AttributesCache[i].m_Value0 != srcAttr.m_Value0 ||
													m_AttributesCache[i].m_Value1 != srcAttr.m_Value1 ||
													m_AttributesCache[i].m_Value2 != srcAttr.m_Value2 ||
													m_AttributesCache[i].m_Value3 != srcAttr.m_Value3;

			if (wasChanged || forceUpdate || m_ForceUpdateAttributes)
			{
				m_AttributesCache[i].m_Type = (int)(m_FxAttributesList[i].m_Descriptor.Type);
				m_AttributesCache[i].m_Value0 = srcAttr.m_Value0;
				m_AttributesCache[i].m_Value1 = srcAttr.m_Value1;
				m_AttributesCache[i].m_Value2 = srcAttr.m_Value2;
				m_AttributesCache[i].m_Value3 = srcAttr.m_Value3;
				attrCount = i;
			}
		}
		if (attrCount >= 0)
		{
			if (!PKFxManager.EffectSetAttributes(this.m_FXGUID, attrCount + 1, this.m_AttributesHandler))
			{
				Debug.LogError("[PKFX] Attribute through pinned memory failed.");
				Debug.LogError("[PKFX] Did you try to change an FX without stopping it beforehand?");
			}
		}
		// Samplers
		int	samplerCurveOffset = 0;

		for (int i = 0; i < m_FxSamplersList.Count; i++)
		{
			PKFxManager.Sampler		srcAttr 	= 	m_FxSamplersList[i];
			int						smpType		=	(int)(m_FxSamplersList[i].m_Descriptor.Type);
			Vector3					smpPos		=	new Vector3(m_SamplersCache[i].m_PosX, m_SamplersCache[i].m_PosY, m_SamplersCache[i].m_PosZ);
			Vector3					smpEul		=	new Vector3(m_SamplersCache[i].m_EulX, m_SamplersCache[i].m_EulY, m_SamplersCache[i].m_EulZ);
			Vector3					smpSize		=	new Vector3(m_SamplersCache[i].m_SizeX, m_SamplersCache[i].m_SizeY, m_SamplersCache[i].m_SizeZ);
			int						smpChannels	=	m_SamplersCache[i].m_SamplingChannels;
			bool					wasChanged;

			m_SamplersCache[i].m_Type1 = smpType;
			if (m_SamplersCache[i].m_Type1 == (int)PKFxManager.ESamplerType.SamplerShape)
			{
				bool isMesh = (	srcAttr.m_ShapeType == (int)PKFxManager.EShapeType.MeshShape ||
								srcAttr.m_ShapeType == (int)PKFxManager.EShapeType.SkinnedMeshShape);

				bool withSkinning = srcAttr.m_ShapeType == (int)PKFxManager.EShapeType.SkinnedMeshShape &&
									srcAttr.m_SkinnedMeshRenderer != null;

				if (srcAttr.m_EditorShapeType == (int)PKFxManager.EShapeType.MeshShape + 1) //MeshFilter
				{
					if (srcAttr.m_MeshFilter == null)
					{
						srcAttr.m_Mesh = null;
					}
					else if (srcAttr.m_Mesh != srcAttr.m_MeshFilter.sharedMesh)
					{
						srcAttr.m_Mesh = srcAttr.m_MeshFilter.sharedMesh;
						if (srcAttr.m_Mesh != null)
							srcAttr.m_MeshHashCode = srcAttr.m_Mesh.name.GetHashCode();
						else
							srcAttr.m_MeshHashCode = 0;
					}
				}

				wasChanged = srcAttr.m_MeshHashCode != m_SamplersCache[i].m_HashCode ||
							 smpSize != srcAttr.m_Dimensions ||
							 m_SamplersCache[i].m_Type2 != srcAttr.m_ShapeType ||
							 smpPos != srcAttr.m_ShapeCenter ||
							 smpEul != srcAttr.m_EulerOrientation ||
							 (isMesh && smpChannels != srcAttr.m_SamplingChannels);

				if (m_SamplersCache[i].m_MeshChanged != 0 && m_SamplersCache[i].m_Data != IntPtr.Zero &&
					srcAttr.m_SkinnedMeshData == null)
				{
					Marshal.FreeHGlobal(m_SamplersCache[i].m_Data);
					m_SamplersCache[i].m_Data = IntPtr.Zero;
				}
				m_SamplersCache[i].m_MeshChanged = 0;

				if (wasChanged || forceUpdate || m_ForceUpdateAttributes)
				{
					m_SamplersCache[i].m_SamplingChannels = srcAttr.m_SamplingChannels;
					if (isMesh && srcAttr.m_Mesh != null)
					{
						if (forceUpdate || m_SamplersCache[i].m_HashCode != srcAttr.m_MeshHashCode ||
							m_SamplersCache[i].m_Type2 != (int)m_FxSamplersList[i].m_ShapeType ||
							smpChannels != srcAttr.m_SamplingChannels)
						{
							int[] trianglesSrc = srcAttr.m_Mesh.triangles;

							int boneCount = withSkinning ? srcAttr.m_SkinnedMeshRenderer.bones.Length : 0;
							int triangleSize = trianglesSrc.Length * sizeof(int);
							int verticesSize = (srcAttr.m_SamplingChannels & (int)PKFxManager.EMeshChannels.Channel_Position) != 0 ? srcAttr.m_Mesh.vertexCount * sizeof(float) * 3 : 0;
							int normalsSize = (srcAttr.m_SamplingChannels & (int)PKFxManager.EMeshChannels.Channel_Normal) != 0 ? srcAttr.m_Mesh.vertexCount * sizeof(float) * 3 : 0;
							int tangentsSize = (srcAttr.m_SamplingChannels & (int)PKFxManager.EMeshChannels.Channel_Tangent) != 0 ? srcAttr.m_Mesh.vertexCount * sizeof(float) * 4 : 0;
							int uvsSize = (srcAttr.m_SamplingChannels & (int)PKFxManager.EMeshChannels.Channel_UV) != 0 ? srcAttr.m_Mesh.vertexCount * sizeof(float) * 2 : 0;
							int vertexColorsSize = (srcAttr.m_SamplingChannels & (int)PKFxManager.EMeshChannels.Channel_VertexColor) != 0 ? srcAttr.m_Mesh.vertexCount * sizeof(float) * 4 : 0;
							int boneWeightsSize = boneCount != 0 ? srcAttr.m_Mesh.boneWeights.Length * sizeof(float) * 8 : 0;
							int totalSize = triangleSize + verticesSize + normalsSize + tangentsSize + uvsSize + vertexColorsSize + boneWeightsSize;

							if (totalSize > 0)
							{
								m_SamplersCache[i].m_IndexCount = srcAttr.m_Mesh.triangles.Length;
								m_SamplersCache[i].m_VertexCount = srcAttr.m_Mesh.vertices.Length;
								m_SamplersCache[i].m_BoneCount = boneCount;

								if (m_SamplersCache[i].m_Data != IntPtr.Zero)
									Marshal.FreeHGlobal(m_SamplersCache[i].m_Data);
								m_SamplersCache[i].m_Data = Marshal.AllocHGlobal(totalSize);
								UpdateMesh(m_SamplersCache[i].m_Data, trianglesSrc, srcAttr.m_Mesh, srcAttr.m_SamplingChannels, withSkinning);
								m_SamplersCache[i].m_MeshChanged = 0x2A; // nonzero, why not 42?
								m_SamplersCache[i].m_HashCode = srcAttr.m_MeshHashCode;
								srcAttr.m_SkinnedMeshData = null;
							}
						}
					}
					else
					{
						if (m_SamplersCache[i].m_Data != IntPtr.Zero)
						{
							Marshal.FreeHGlobal(m_SamplersCache[i].m_Data);
							m_SamplersCache[i].m_Data = IntPtr.Zero;
						}
						m_SamplersCache[i].m_IndexCount = 0;
						m_SamplersCache[i].m_VertexCount = 0;
						m_SamplersCache[i].m_MeshChanged = 0x2A; // nonzero, why not 42?
						m_SamplersCache[i].m_HashCode = 0;
						m_SamplersCache[i].m_BoneCount = 0;
					}
					srcAttr.m_TextureChanged = false;
					m_SamplersCache[i].m_Type2 = (int)(m_FxSamplersList[i].m_ShapeType);
					m_SamplersCache[i].m_SizeX = srcAttr.m_Dimensions.x;
					m_SamplersCache[i].m_SizeY = srcAttr.m_Dimensions.y;
					m_SamplersCache[i].m_SizeZ = srcAttr.m_Dimensions.z;
					m_SamplersCache[i].m_PosX = srcAttr.m_ShapeCenter.x;
					m_SamplersCache[i].m_PosY = srcAttr.m_ShapeCenter.y;
					m_SamplersCache[i].m_PosZ = srcAttr.m_ShapeCenter.z;
					m_SamplersCache[i].m_EulX = srcAttr.m_EulerOrientation.x;
					m_SamplersCache[i].m_EulY = srcAttr.m_EulerOrientation.y;
					m_SamplersCache[i].m_EulZ = srcAttr.m_EulerOrientation.z;
					smpCount = i;
				}
			}
			else if (m_SamplersCache[i].m_Type1 == (int)PKFxManager.ESamplerType.SamplerImage)
			{
				wasChanged = srcAttr.m_TextureChanged ||
							 (int)srcAttr.m_TextureTexcoordMode != (int)m_SamplersCache[i].m_PosX;
				if (wasChanged || forceUpdate || m_ForceUpdateAttributes)
				{
					if (srcAttr.m_Texture == null)
					{
						m_SamplersCache[i].m_SizeX = 0;
						m_SamplersCache[i].m_SizeY = 0;
						if (m_SamplersCache[i].m_Data != IntPtr.Zero)
							Marshal.FreeHGlobal(m_SamplersCache[i].m_Data);
						m_SamplersCache[i].m_Data = IntPtr.Zero;
						int size = 0;
						m_SamplersCache[i].m_SizeZ = size;
						m_SamplersCache[i].m_PosX = 0;
					}
					else
					{
						byte[] data = srcAttr.m_Texture.GetRawTextureData();
						if (data.Length == 0)
							Debug.LogError("[PKFX] Sampler " + srcAttr.m_Descriptor.Name + " : Could not get raw texture data. Enable read/write in import settings.");
						
						if (srcAttr.m_Texture.format == TextureFormat.DXT1)
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.DXT1;
						else if (srcAttr.m_Texture.format == TextureFormat.DXT5)
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.DXT5;
						else if (srcAttr.m_Texture.format == TextureFormat.ARGB32)
						{
							PKImageConverter.ARGB2BGRA(ref data);
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.BGRA8;
						}
						else if(srcAttr.m_Texture.format == TextureFormat.RGBA32)
						{
							PKImageConverter.RGBA2BGRA(ref data);
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.BGRA8;
						}
						else if (srcAttr.m_Texture.format == TextureFormat.BGRA32)
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.BGRA8;
						else if (srcAttr.m_Texture.format == TextureFormat.RGB24)
						{
							PKImageConverter.RGB2BGR(ref data);
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.BGR8;
						}
						else if (srcAttr.m_Texture.format == TextureFormat.PVRTC_RGB4)
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.RGB4_PVRTC1;
						else if (srcAttr.m_Texture.format == TextureFormat.PVRTC_RGBA4)
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.RGBA4_PVRTC1;
						else if (srcAttr.m_Texture.format == TextureFormat.PVRTC_RGB2)
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.RGB2_PVRTC1;
						else if (srcAttr.m_Texture.format == TextureFormat.PVRTC_RGBA2)
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.RGBA2_PVRTC1;
						else if (srcAttr.m_Texture.format == TextureFormat.ETC_RGB4)
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.RGB8_ETC1;
						else if (srcAttr.m_Texture.format == TextureFormat.ETC2_RGB)
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.RGB8_ETC2;
						else if (srcAttr.m_Texture.format == TextureFormat.ETC2_RGBA8)
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.RGBA8_ETC2;
						else if (srcAttr.m_Texture.format == TextureFormat.ETC2_RGBA1)
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.RGB8A1_ETC2;
						else
						{
							m_SamplersCache[i].m_Type2 = (int)PKFxManager.EImageFormat.Invalid;
							Debug.LogError("[PKFX] Sampler " + srcAttr.m_Descriptor.Name + " texture format not supported : " +
							               srcAttr.m_Texture.format);
						}

						m_SamplersCache[i].m_SizeX = srcAttr.m_Texture.width;
						m_SamplersCache[i].m_SizeY = srcAttr.m_Texture.height;
						if (m_SamplersCache[i].m_Data != IntPtr.Zero)
							Marshal.FreeHGlobal(m_SamplersCache[i].m_Data);
						int size = data.Length;
						m_SamplersCache[i].m_Data = Marshal.AllocHGlobal(size);
						Marshal.Copy(data, 0, m_SamplersCache[i].m_Data, size);
						m_SamplersCache[i].m_SizeZ = size;
						m_SamplersCache[i].m_PosX = (int)srcAttr.m_TextureTexcoordMode;
					}
					srcAttr.m_TextureChanged = false;
					smpCount = i;
				}
			}
			else if (m_SamplersCache[i].m_Type1 == (int)PKFxManager.ESamplerType.SamplerCurve)
			{
				int timeKeysCount = srcAttr.m_CurvesTimeKeys == null ? 0 : srcAttr.m_CurvesTimeKeys.Length;

				if (timeKeysCount == 0 && timeKeysCount != m_SamplersCache[i].m_SizeX)
				{
					if (this.m_SamplersCurvesDataGCH.IsAllocated)
						this.m_SamplersCurvesDataGCH.Free();
					m_SamplersCurvesDataCache = null;
					m_SamplersCache[i].m_Data = IntPtr.Zero;
					m_SamplersCache[i].m_SizeX = 0;
					smpCount = i;
				}
				else if (srcAttr.m_CurvesArray != null)
				{
					int curvesCount = srcAttr.m_CurvesArray.Length;
					int keyDataOffset = (1 + curvesCount * 3);

					wasChanged = timeKeysCount != m_SamplersCache[i].m_SizeX;

					//check key times changed
					if (!wasChanged && !forceUpdate && !m_ForceUpdateAttributes)
					{
						for (int keyId = 0; keyId < timeKeysCount; ++keyId)
						{
							if (m_SamplersCache[i].m_Data != IntPtr.Zero)
							{
								int realId = samplerCurveOffset + (keyId * keyDataOffset);
								float key = srcAttr.m_CurvesTimeKeys[keyId];
								if (key != m_SamplersCurvesDataCache[realId])
								{
									wasChanged = true;
									break;
								}
							}
						}
					}
					////check values changed
					if (!wasChanged && !forceUpdate && !m_ForceUpdateAttributes)
					{
						for (int curveId = 0; curveId < srcAttr.m_CurvesArray.Length; ++curveId)
						{
							if (m_SamplersCache[i].m_Data != IntPtr.Zero)
							{
								var curve = srcAttr.m_CurvesArray[curveId];
								for (int keyId = 0; keyId < curve.keys.Length; ++keyId)
								{
									int realId = samplerCurveOffset + (keyId * keyDataOffset) + 1 + curveId * 3;
									var key = curve.keys[keyId];
									if (key.value != m_SamplersCurvesDataCache[realId + 0] ||
										key.inTangent != m_SamplersCurvesDataCache[realId + 1] ||
										key.outTangent != m_SamplersCurvesDataCache[realId + 2])
									{
										wasChanged = true;
										break;
									}
								}
							}
						}
					}
					if (wasChanged || forceUpdate || m_ForceUpdateAttributes)
					{
						m_SamplersCache[i].m_Data = new IntPtr(m_SamplersCurvesDataHandler.ToInt64() + samplerCurveOffset);
						// copy values
						for (int keyId = 0; keyId < srcAttr.m_CurvesTimeKeys.Length; ++keyId)
						{
							int realId = samplerCurveOffset + (keyId * keyDataOffset);
							m_SamplersCurvesDataCache[realId] = srcAttr.m_CurvesTimeKeys[keyId];
							for (int curveId = 0; curveId < srcAttr.m_CurvesArray.Length; ++curveId)
							{
								var curve = srcAttr.m_CurvesArray[curveId];
								int curveRealId = realId + 1 + curveId * 3;
								var key = curve.keys[keyId]; //Check if key exist
								m_SamplersCurvesDataCache[curveRealId + 0] = key.value;
								m_SamplersCurvesDataCache[curveRealId + 1] = key.inTangent;
								m_SamplersCurvesDataCache[curveRealId + 2] = key.outTangent;
							}
						}
						samplerCurveOffset += timeKeysCount * keyDataOffset;
						m_SamplersCache[i].m_SizeX = timeKeysCount; //key count
						m_SamplersCache[i].m_SizeY = srcAttr.m_CurvesArray.Length; //dimension
						smpCount = i;
					}
				}
			}
			else if (m_SamplersCache[i].m_Type1 == (int)PKFxManager.ESamplerType.SamplerText)
			{

				wasChanged = srcAttr.m_Text.Length != m_SamplersCache[i].m_SizeX;

				if (!wasChanged && srcAttr.m_Text.Length > 0 && m_SamplersCache[i].m_Data == IntPtr.Zero)
					wasChanged = true;

				if (!wasChanged && m_SamplersCache[i].m_Data !=  IntPtr.Zero &&
					Marshal.PtrToStringAnsi(m_SamplersCache[i].m_Data) != srcAttr.m_Text)
					wasChanged = true;

				if (wasChanged || forceUpdate || m_ForceUpdateAttributes)
				{
					if (m_SamplersCache[i].m_Data != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(m_SamplersCache[i].m_Data);
						m_SamplersCache[i].m_Data = IntPtr.Zero;
					}
					int size = srcAttr.m_Text.Length;
					if (size > 0)
					{
						m_SamplersCache[i].m_Data = Marshal.StringToHGlobalAnsi(srcAttr.m_Text);
					}
					m_SamplersCache[i].m_SizeX = size;
					smpCount = i;
				}
			}
		}
		if (smpCount >= 0)
		{
			if (!PKFxManager.EffectSetSamplers(this.m_FXGUID, smpCount + 1, this.m_SamplersHandler))
			{
				Debug.LogError("[PKFX] Sampler through pinned memory failed.");
				Debug.LogError("[PKFX] Did you try to change an FX without stopping it beforehand?");
			}
		}
		for (int i = 0; i < m_FxSamplersList.Count; i++)
		{
			PKFxManager.Sampler srcAttr = m_FxSamplersList[i];

			bool withSkinning = m_FxSamplersList[i].m_ShapeType == (int)PKFxManager.EShapeType.SkinnedMeshShape &&
								m_FxSamplersList[i].m_SkinnedMeshRenderer != null;

			if (withSkinning)
			{
				int boneCount = srcAttr.m_SkinnedMeshRenderer != null ? srcAttr.m_SkinnedMeshRenderer.bones.Length : 0;

				if (srcAttr.m_SkinnedMeshData == null)
				{
					int skeletonSize = boneCount != 0 ? srcAttr.m_SkinnedMeshRenderer.bones.Length * sizeof(float) * 16 : 0;

					if (m_SamplersCache[i].m_Data != IntPtr.Zero)
						Marshal.FreeHGlobal(m_SamplersCache[i].m_Data);
					m_SamplersCache[i].m_Data = Marshal.AllocHGlobal(skeletonSize);
					srcAttr.m_SkinnedMeshData = new PKFxManager.Sampler.SkinnedMeshData();
					srcAttr.m_SkinnedMeshData.InitData(srcAttr.m_SkinnedMeshRenderer);
				}

				UpdateBones(m_SamplersCache[i].m_Data, srcAttr);
				if (!PKFxManager.EffectUpdateSamplerSkinning(this.m_FXGUID, i, this.m_SamplersHandler, Time.deltaTime))
				{
					Debug.LogError("[PKFX] Skinning through pinned memory failed.");
				}
			}
		}
		m_ForceUpdateAttributes = false;
	}

	//----------------------------------------------------------------------------

	public PKFxManager.Sampler GetSamplerFromDesc(PKFxManager.SamplerDesc desc)
	{
		foreach (PKFxManager.Sampler attr in this.m_FxSamplersList)
			if (attr.m_Descriptor.Name == desc.Name && attr.m_Descriptor.Type == desc.Type)
				return attr;
		return null;
	}

	//----------------------------------------------------------------------------

	public void DeleteAttribute(PKFxManager.AttributeDesc desc)
	{
		foreach (PKFxManager.Attribute attr in this.m_FxAttributesList)
			if (attr.m_Descriptor.Name == desc.Name && attr.m_Descriptor.Type == desc.Type)
			{
				this.m_FxAttributesList.Remove(attr);
				return;
			}
	}

	//----------------------------------------------------------------------------

	public PKFxManager.Attribute GetAttributeFromDesc(PKFxManager.AttributeDesc desc)
	{
		foreach (PKFxManager.Attribute attr in this.m_FxAttributesList)
			if (attr.m_Descriptor.Name == desc.Name && attr.m_Descriptor.Type == desc.Type)
			{
				attr.m_Descriptor = desc;
				return attr;
			}
		return null;
	}

	//----------------------------------------------------------------------------

	public bool SamplerExists(PKFxManager.SamplerDesc desc)
	{
		foreach (PKFxManager.Sampler attr in this.m_FxSamplersList)
			if (attr.m_Descriptor.Name == desc.Name && attr.m_Descriptor.Type == desc.Type)
				return true;
		return false;
	}

	//----------------------------------------------------------------------------

	public bool AttributeExists(PKFxManager.AttributeDesc desc)
	{
		foreach (PKFxManager.Attribute attr in this.m_FxAttributesList)
			if (attr.m_Descriptor.Name == desc.Name && attr.m_Descriptor.Type == desc.Type)
				return true;
		return false;
	}

	//----------------------------------------------------------------------------

	bool AttributesDescExistIn(List<PKFxManager.AttributeDesc> FxAttributesDesc, PKFxManager.Attribute attr)
	{
		foreach (PKFxManager.AttributeDesc desc in FxAttributesDesc)
			if (attr.m_Descriptor.Name == desc.Name && attr.m_Descriptor.Type == desc.Type)
				return true;
		return false;
	}

	#endregion

}
