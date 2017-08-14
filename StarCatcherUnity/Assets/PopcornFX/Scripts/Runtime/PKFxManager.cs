//----------------------------------------------------------------------------
// Created on Thu Oct 31 11:53:00 2013 Maxime Dumas
//
// This program is the property of Persistant Studios SARL.
//
// You may not redistribute it and/or modify it under any conditions
// without written permission from Persistant Studios SARL, unless
// otherwise stated in the latest Persistant Studios Code License.
//
// See the Persistant Studios Code License for further details.
//----------------------------------------------------------------------------

#if UNITY_5_5 || UNITY_5_6 || UNITY_2017
#define UNITY_REVERSE_DEPTH
#endif

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using AOT;

public static partial class PKFxManager : object
{
	public enum E_AvailableCamEvents
	{
		BeforeImageEffectsOpaque = UnityEngine.Rendering.CameraEvent.BeforeImageEffectsOpaque,
		BeforeImageEffects = UnityEngine.Rendering.CameraEvent.BeforeImageEffects,
	}

#region Global conf
	[XmlRoot ("PKFxGlobalConf")]
	public class PKFxConf
	{
		public bool					enableFileLog = true;
		public bool					enableEffectsKill = false;
		public bool					enablePackFxInPersistentDataPath = false;
		public bool					useOrthographicProjection = false;

		public E_AvailableCamEvents	globalEventSetting = E_AvailableCamEvents.BeforeImageEffectsOpaque;

		public void Save()
		{
			string confPath = m_PackPath + "/PKconfig.cfg";

			var serializer = new XmlSerializer(typeof(PKFxConf));
			if (!Directory.Exists(Path.GetDirectoryName(confPath)))
			    Directory.CreateDirectory(Path.GetDirectoryName(confPath));
			var stream = new FileStream(confPath, FileMode.Create);
			using(var sw = new StreamWriter(stream, Encoding.ASCII))
			{
				serializer.Serialize(sw, this);
			}
			stream.Close();
		}
	}

#endregion // Global conf

	//----------------------------------------------------------------------------

	#region Unity device enums

	// copy pasted from http://docs.unity3d.com/Documentation/Manual/NativePluginInterface.html
	public enum GfxDeviceRenderer
	{
		kGfxRendererOpenGL = 0,					// OpenGL
		kGfxRendererD3D9 = 1,					// Direct3D 9
		kGfxRendererD3D11 = 2,					// Direct3D 11
		kGfxRendererGCM = 3,					// Sony PlayStation 3 GCM
		kGfxRendererNull = 4,					// "null" device (used in batch mode)
		kGfxRendererHollywood = 5,				// Nintendo Wii
		kGfxRendererXenon = 6,					// Xbox 360
		kGfxRendererOpenGLES = 7,				// OpenGL ES 1.1
		kGfxRendererOpenGLES20Mobile = 8,		// OpenGL ES 2.0 mobile variant
		kGfxRendererMolehill = 9,				// Flash 11 Stage3D
		kGfxRendererOpenGLES20Desktop = 10		// OpenGL ES 2.0 desktop variant (i.e. NaCl)
	}
	public enum GfxDeviceEventType
	{
		kGfxDeviceEventInitialize = 0,
		kGfxDeviceEventShutdown = 1,
		kGfxDeviceEventBeforeReset = 2,
		kGfxDeviceEventAfterReset = 3
	}

	#endregion // Unity device enums

	//----------------------------------------------------------------------------

#region dll types

	// Binary mask to avoid collision with other native plugins
	public const UInt32 POPCORN_MAGIC_NUMBER = 0x5AFE0000; // STAY SAFE, M8!
	public const UInt32 PK_DESC_NAME_MAX_LEN = 0x40;
	public const UInt32 PK_DESC_DESC_MAX_LEN = 0x80;

	public enum BaseType
	{
		Int = 22,
		Int2,
		Int3,
		Int4,
		Float = 28,
		Float2,
		Float3,
		Float4
	}

	public enum DepthGrabFormat
	{
		Depth16Bits = 16,
		Depth24Bits = 24
	}

	public enum CamFlags
	{
		UseDepthGrabberTexture	= (1 << 0),
		ScreenResolutionChanged	= (1 << 1),
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CamDesc
	{
		public Matrix4x4 ViewMatrix;
		public Matrix4x4 ProjectionMatrix;
		public float DT;
		public int RenderPass;
		public float NearClip;
		public float FarClip;
		public IntPtr DepthRT;
		public int DepthBpp;
		public float LODBias;
		public int Flags;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct FxDesc
	{
		public string		FxPath;
		public Matrix4x4	Transforms;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ShaderDesc
	{
		public string		ShaderPath;
		public string		ShaderGroup;
		public int			Api;
		public int			VertexType;
		public int			PixelType;
	}

	public enum EAttrDescFlag : byte
	{
		HasMin	= 0x01,
		HasMax	= 0x02,
		HasDesc	= 0x04
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct S_AttributeDesc
	{
		public int			Type;
		public char			MinMaxFlag;
		public IntPtr		Name;
		public IntPtr		Description;
		public float		DefaultValue0;
		public float		DefaultValue1;
		public float		DefaultValue2;
		public float		DefaultValue3;
		public float		MinValue0;
		public float		MinValue1;
		public float		MinValue2;
		public float		MinValue3;
		public float		MaxValue0;
		public float		MaxValue1;
		public float		MaxValue2;
		public float		MaxValue3;
	}

	public enum ESamplerType : int
	{
		SamplerShape = 0,
		SamplerCurve,
		SamplerImage,
		SamplerText,
		SamplerUnsupported
	}

	public enum ETexcoordMode : int
	{
		Clamp = 0,
		Wrap
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct S_SamplerDesc
	{
		public int			Type;
		public IntPtr		Name;
		public IntPtr		Description;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct S_ShaderConstantDesc
	{
		public int			Type;
		public IntPtr		Name;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct S_Stats
	{
		public float		UpdateTime;
		public float		RenderTime;
		public int			TotalMemoryFootprint;
		public int			TotalParticleMemory;
		public int			UnusedParticleMemory;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct S_SoundDescriptor
	{
		public int			ChannelGroup;
		public IntPtr		Path;
		public IntPtr		EventStart;
		public IntPtr		EventStop;
		public Vector3		WorldPosition;
		public float		Volume;
		public float		StartTimeOffsetInSeconds;
		public float		PlayTimeInSeconds;
		public int			UserData;
	}

    #endregion // dll types

    //----------------------------------------------------------------------------

    #region dll imports

#if !PK_COMPILED

#if UNITY_IPHONE && !UNITY_EDITOR
	private const string kPopcornPluginName = "__Internal";
#elif UNITY_XBOXONE && !UNITY_EDITOR
	private const string kPopcornPluginName = "PK-UnityPlugin_XBoxOne";
#else
    private const string kPopcornPluginName = "PK-UnityPlugin";
#endif

	[DllImport(kPopcornPluginName)]
	public static extern IntPtr GetRuntimeVersion();
	[DllImport(kPopcornPluginName)]
	public static extern void GetStats(ref S_Stats stats);
	[DllImport(kPopcornPluginName)]
	public static extern int LoadFx(FxDesc path);
	[DllImport(kPopcornPluginName)]
	public static extern bool StopFx(int guid);
	[DllImport(kPopcornPluginName)]
	public static extern bool TerminateFx(int guid);
	[DllImport(kPopcornPluginName)]
	public static extern bool KillFx(int guid);
	[DllImport(kPopcornPluginName)]
	public static extern bool IsFxAlive(int guid);
	[DllImport(kPopcornPluginName)]
	public static extern void PreLoadFxIFN(string path);
	[DllImport(kPopcornPluginName)]
	public static extern uint LoadShader(ShaderDesc desc);
	[DllImport(kPopcornPluginName)]
	public static extern void UnloadShader(string path);
	[DllImport(kPopcornPluginName)]
	public static extern IntPtr GetDefaultShaderString(int api, int type);
	[DllImport(kPopcornPluginName)]
	public static extern int ShaderConstantsCount(string path, int api);
	[DllImport(kPopcornPluginName)]
	public static extern bool ShaderFillConstantDesc(string path, int constantId, ref S_ShaderConstantDesc desc);
	[DllImport(kPopcornPluginName)]
	public static extern bool ShaderSetConstant(uint shaderId, int constantCount, [In]IntPtr attributes);
	[DllImport(kPopcornPluginName)]
	public static extern bool LoadPack(string path);
	[DllImport(kPopcornPluginName)]
	public static extern bool EffectSetTransforms(int guid, Matrix4x4 tranforms);
	[DllImport(kPopcornPluginName)]
	public static extern int EffectSamplersCountFromFx(string fxName);
	[DllImport(kPopcornPluginName)]
	public static extern bool EffectFillSamplerDescFromFx(string fxName, int samplerID, ref S_SamplerDesc desc);
	[DllImport(kPopcornPluginName)]
	public static extern bool EffectSetSamplers(int guid, int samplerCount, [In]IntPtr samplers);
	[DllImport(kPopcornPluginName)]
	public static extern bool EffectUpdateSamplerSkinning(int guid, int samplerId, [In]IntPtr samplers, float dt);
	[DllImport(kPopcornPluginName)]
	public static extern int EffectAttributesCount(int guid);
	[DllImport(kPopcornPluginName)]
	public static extern bool EffectFillAttributeDesc(int fxGUID, int attrID, ref S_AttributeDesc desc);
	[DllImport(kPopcornPluginName)]
	public static extern int EffectAttributesCountFromFx(string fxName);
	[DllImport(kPopcornPluginName)]
	public static extern bool EffectFillAttributeDescFromFx(string fxName, int attrID, ref S_AttributeDesc desc);
	[DllImport(kPopcornPluginName)]
	public static extern bool EffectSetAttributes(int guid, int attributeCount, [In]IntPtr attributes);
	[DllImport(kPopcornPluginName)]
	public static extern void SetDelegateOnFxStopped(IntPtr delegatePtr);
	[DllImport(kPopcornPluginName)]
	public static extern void SetDelegateOnFxHotReloaded(IntPtr delegatePtr);
	[DllImport(kPopcornPluginName)]
	public static extern void SetDelegateOnAudioSpectrumData(IntPtr delegatePtr);
	[DllImport(kPopcornPluginName)]
	public static extern void SetDelegateOnAudioWaveformData(IntPtr delegatePtr);
	[DllImport(kPopcornPluginName)]
	public static extern void SetDelegateOnStartSound(IntPtr delegatePtr);
	[DllImport(kPopcornPluginName)]
	public static extern void SetReversedZBuffer(bool zBufferReversed);
	[DllImport(kPopcornPluginName)]
	public static extern void SetupColorSpace(bool isSRGB);
	[DllImport(kPopcornPluginName)]
	public static extern bool UnloadEffect(string path);
	[DllImport(kPopcornPluginName)]
	public static extern void LogicalUpdate(float dt);
	[DllImport(kPopcornPluginName)]
	public static extern void UpdateParticles(CamDesc desc);
	[DllImport(kPopcornPluginName)]
	public static extern void UpdateCamDesc(int camID, CamDesc desc, bool update);
	[DllImport(kPopcornPluginName)]
	public static extern void Reset();
	[DllImport(kPopcornPluginName)]
	public static extern void DeepReset();
	[DllImport(kPopcornPluginName)]
	public static extern bool LoadPkmmAsSceneMesh(string pkmmVirtualPath);
	[DllImport(kPopcornPluginName)]
	public static extern void SceneMeshClear();
	[DllImport(kPopcornPluginName)]
	public static extern bool SceneMeshAddRawMesh(int indicesCount, int[] indices, int verticesCount, Vector3[] vertices, Vector3[] normals, Matrix4x4 MeshMatrix);
	[DllImport(kPopcornPluginName)]
	public static extern int SceneMeshBuild(string outputPkmmVirtualPath);
	[DllImport(kPopcornPluginName)]
	public static extern void UnitySetGraphicsDevice(IntPtr device, int deviceType, int eventType);
	[DllImport(kPopcornPluginName)]
	public static extern void UnityRenderEvent(int camID);
	[DllImport(kPopcornPluginName)]
	public static extern void EnableSpawnerIDs(bool enable);
	[DllImport(kPopcornPluginName)]
	public static extern void WriteProfileReport(string path);
	[DllImport(kPopcornPluginName)]
	public static extern void ProfilerSetEnable(bool enable);
	[DllImport(kPopcornPluginName)]
	public static extern IntPtr GetRenderEventFunc();
	[DllImport(kPopcornPluginName)]
	public static extern IntPtr GetGLConstantsCountEvent();
	[DllImport(kPopcornPluginName)]
	public static extern void GLConstantsCountEvent(int eventId);
	[DllImport(kPopcornPluginName)]
	public static extern void SetUseOrthographicProjection(bool enable);
	[DllImport(kPopcornPluginName)]
	public static extern void TransformAllParticles(Matrix4x4 transform);

#if PKFX_CHECK_SIGNATURES
		[DllImport(kPopcornPluginName)]
		public static extern byte ShareSecrets(IntPtr CheckSignCb);
#endif
#endif //!PK_COMPILED
#endregion

    //----------------------------------------------------------------------------

    private static int ftoi(float fff)
	{
		return BitConverter.ToInt32(BitConverter.GetBytes(fff), 0);
	}
	
	private static float itof(int i)
	{
		return BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
	}

	//----------------------------------------------------------------------------

	#region Data structures
	[Serializable]
	public class SamplerDesc
	{
		public int			Type;
		public string		Name;
		public string		Description;

		public SamplerDesc(SamplerDesc desc)
		{
			this.Type = desc.Type;
			this.Name = desc.Name;
		}

		public SamplerDesc(S_SamplerDesc desc)
		{
			this.Type = desc.Type;
			this.Name = Marshal.PtrToStringAnsi(desc.Name);
			//			if ((byte)desc.MinMaxFlag & EAttrDescFlag.HasDesc)
			this.Description = Marshal.PtrToStringAnsi(desc.Description);
		}

		public SamplerDesc(string name, int type)
		{
			this.Type = type;
			this.Name = name;
		}
	}

	public enum EShapeType : int
	{
		BoxShape = 0,
		SphereShape,
		CylinderShape,
		CapsuleShape,
		MeshShape,
		SkinnedMeshShape
	}

	public enum EMeshChannels : int
	{
		Channel_Position = 0x1,
		Channel_Normal = 0x2,
		Channel_Tangent = 0x4,
		Channel_Velocity = 0x8,
		Channel_UV = 0x10,
		Channel_VertexColor = 0x20
	}

	public enum EImageFormat : int
	{
		Invalid = 0,
		BGR8 = 3,
		BGRA8,
		DXT1 = 8,
		DXT3 = 10,
		DXT5 = 12,
		RGB8_ETC1 = 16,
		RGB8_ETC2,
		RGBA8_ETC2,
		RGB8A1_ETC2,
		RGB4_PVRTC1,
		RGB2_PVRTC1,
		RGBA4_PVRTC1,
		RGBA2_PVRTC1
	}

	#region Shape Sampler Descriptors

	public class SamplerDescShapeBox
	{
		public Vector3	Center;
		public Vector3	Dimensions;
		public Vector3	EulerOrientation;

		public SamplerDescShapeBox()
		{
			Center = new Vector3(0, 0, 0);
			Dimensions = new Vector3(1, 1, 1);
			EulerOrientation = new Vector3(0, 0, 0);
		}	

		public SamplerDescShapeBox(Vector3 center, Vector3 dimension, Vector3 euler)
		{
			Center = center;
			Dimensions = dimension;
			EulerOrientation = euler;
		}	
	}

	public class SamplerDescShapeSphere
	{
		public Vector3	Center;
		public float	InnerRadius;
		public float	Radius;
		public Vector3	EulerOrientation;

		public SamplerDescShapeSphere()
		{
			Center = new Vector3(0, 0, 0);
			InnerRadius = 1.0f;
			Radius = 1.0f;
			EulerOrientation = new Vector3(0, 0, 0);
		}

		public SamplerDescShapeSphere(Vector3 center, float radius, float innerRadius, Vector3 euler)
		{
			Center = center;
			InnerRadius = innerRadius;
			Radius = radius;
			EulerOrientation = euler;
		}
	}

	public class SamplerDescShapeCylinder
	{
		public Vector3	Center;
		public float	InnerRadius;
		public float	Radius;
		public float	Height;
		public Vector3	EulerOrientation;

		public SamplerDescShapeCylinder()
		{
			Center = new Vector3(0, 0, 0);
			InnerRadius = 1.0f;
			Radius = 1.0f;
			Height = 1.0f;
			EulerOrientation = new Vector3(0, 0, 0);
		}

		public SamplerDescShapeCylinder(Vector3 center, float radius, float innerRadius, float height, Vector3 euler)
		{
			Center = center;
			InnerRadius = innerRadius;
			Radius = radius;
			Height = height;
			EulerOrientation = euler;
		}
	}

	public class SamplerDescShapeCapsule
	{
		public Vector3	Center;
		public float	InnerRadius;
		public float	Radius;
		public float	Height;
		public Vector3	EulerOrientation;

		public SamplerDescShapeCapsule()
		{
			Center = new Vector3(0, 0, 0);
			InnerRadius = 1.0f;
			Radius = 1.0f;
			Height = 1.0f;
			EulerOrientation = new Vector3(0, 0, 0);
		}

		public SamplerDescShapeCapsule(Vector3 center, float radius, float innerRadius, float height, Vector3 euler)
		{
			Center = center;
			InnerRadius = innerRadius;
			Radius = radius;
			Height = height;
			EulerOrientation = euler;
		}
	}

	public class SamplerDescShapeMesh
	{
		public Vector3	Center;
		public Vector3	Dimensions;
		public Vector3	EulerOrientation;
		public Mesh		Mesh;
		public int		SamplingChannels;

		public SamplerDescShapeMesh()
		{
			Center = new Vector3(0, 0, 0);
			Dimensions = new Vector3(1, 1, 1);
			EulerOrientation = new Vector3(0, 0, 0);
			SamplingChannels |= (int)EMeshChannels.Channel_Position;
		}

		public SamplerDescShapeMesh(Vector3 center, Vector3 dimension, Vector3 euler, Mesh mesh, int samplingChannels)
		{
			Center = center;
			Dimensions = dimension;
			EulerOrientation = euler;
			Mesh = mesh;
			SamplingChannels = samplingChannels;
		}
	}

	public class SamplerDescShapeMeshFilter
	{
		public Vector3 Center;
		public Vector3 Dimensions;
		public Vector3 EulerOrientation;
		public MeshFilter MeshFilter;
		public int SamplingChannels;

		public SamplerDescShapeMeshFilter()
		{
			Center = new Vector3(0, 0, 0);
			Dimensions = new Vector3(1, 1, 1);
			EulerOrientation = new Vector3(0, 0, 0);
			SamplingChannels |= (int)EMeshChannels.Channel_Position;
		}

		public SamplerDescShapeMeshFilter(Vector3 center, Vector3 dimension, Vector3 euler, MeshFilter mesh, int samplingChannels)
		{
			Center = center;
			Dimensions = dimension;
			EulerOrientation = euler;
			MeshFilter = mesh;
			SamplingChannels = samplingChannels;
		}
	}

	public class SamplerDescShapeSkinnedMesh
	{
		public Vector3				Center;
		public Vector3				Dimensions;
		public Vector3				EulerOrientation;
		public SkinnedMeshRenderer	SkinnedMesh;
		public int					SamplingChannels;

		public SamplerDescShapeSkinnedMesh()
		{
			Center = new Vector3(0, 0, 0);
			Dimensions = new Vector3(1, 1, 1);
			EulerOrientation = new Vector3(0, 0, 0);
			SamplingChannels |= (int)EMeshChannels.Channel_Position;
		}

		public SamplerDescShapeSkinnedMesh(Vector3 center, Vector3 dimension, Vector3 euler, SkinnedMeshRenderer skinnedMesh, int samplingChannels)
		{
			Center = center;
			Dimensions = dimension;
			EulerOrientation = euler;
			SkinnedMesh = skinnedMesh;
			SamplingChannels = samplingChannels;
		}
	}

	#endregion

	[Serializable]
	public class Sampler
	{
		public SamplerDesc			m_Descriptor;
		public int					m_ShapeType;
		public int					m_EditorShapeType;
		public MeshFilter			m_MeshFilter;
		public Mesh					m_Mesh;
		public SkinnedMeshRenderer	m_SkinnedMeshRenderer;
		public int					m_MeshHashCode;
		public int					m_SamplingChannels;
		public Vector3				m_ShapeCenter = Vector3.zero;
		public Vector3				m_Dimensions = Vector3.one;
		public Vector3				m_EulerOrientation = Vector3.zero;
		public Texture2D			m_Texture;
		public bool					m_TextureChanged;
		public ETexcoordMode		m_TextureTexcoordMode;
		public AnimationCurve[]		m_CurvesArray;
		public float[]				m_CurvesTimeKeys;
		public string				m_Text = "";

		public class SkinnedMeshData
		{
			public float[] m_SkeletonDataBuffer = null;
			public Matrix4x4[] m_Bindposes = null;

			public void InitData(SkinnedMeshRenderer skinnedMeshRenderer)
			{
				m_SkeletonDataBuffer = new float[skinnedMeshRenderer.bones.Length * 16];
				m_Bindposes = skinnedMeshRenderer.sharedMesh.bindposes;
			}
		}

		public SkinnedMeshData m_SkinnedMeshData = null;

		public void Copy(Sampler other)
		{
			m_ShapeType = other.m_ShapeType;
			m_EditorShapeType = other.m_EditorShapeType;
			m_Mesh = other.m_Mesh;
			m_SkinnedMeshRenderer = other.m_SkinnedMeshRenderer;
			m_MeshHashCode = other.m_MeshHashCode;
			m_SamplingChannels = other.m_SamplingChannels;
			m_ShapeCenter = other.m_ShapeCenter;
			m_Dimensions = other.m_Dimensions;
			m_EulerOrientation = other.m_EulerOrientation;
			m_Texture = other.m_Texture;
			m_TextureChanged = other.m_TextureChanged;
			m_TextureTexcoordMode = other.m_TextureTexcoordMode;
			m_CurvesArray = other.m_CurvesArray;
			m_CurvesTimeKeys = other.m_CurvesTimeKeys;
			m_Text = other.m_Text;
			m_SkinnedMeshData = other.m_SkinnedMeshData;
		}

		public Sampler(SamplerDesc dsc)
		{
			m_Descriptor = new SamplerDesc(dsc);
			m_ShapeType = -1;
			m_EditorShapeType = -1;
		}

		public Sampler(string name, SamplerDescShapeBox dsc)
		{
			m_Descriptor = new SamplerDesc(name, (int)ESamplerType.SamplerShape);
			m_ShapeCenter = dsc.Center;
			m_Dimensions = dsc.Dimensions;
			m_EulerOrientation = dsc.EulerOrientation;
			m_ShapeType = (int)EShapeType.BoxShape;
			m_EditorShapeType = (int)EShapeType.BoxShape;
		}

		public Sampler(string name, SamplerDescShapeSphere dsc)
		{
			m_Descriptor = new SamplerDesc(name, (int)ESamplerType.SamplerShape);
			m_ShapeCenter = dsc.Center;
			m_Dimensions = new Vector3(dsc.Radius, dsc.InnerRadius);
			m_Dimensions.y = Mathf.Min(m_Dimensions.x, m_Dimensions.y);
			m_Dimensions.x = Mathf.Max(m_Dimensions.x, m_Dimensions.y);
			m_EulerOrientation = dsc.EulerOrientation;
			m_ShapeType = (int)EShapeType.SphereShape;
			m_EditorShapeType = (int)EShapeType.SphereShape;
		}

		public Sampler(string name, SamplerDescShapeCylinder dsc)
		{
			m_Descriptor = new SamplerDesc(name, (int)ESamplerType.SamplerShape);
			m_ShapeCenter = dsc.Center;
			m_Dimensions = new Vector3(dsc.Radius, dsc.InnerRadius, dsc.Height);
			m_Dimensions.y = Mathf.Min(m_Dimensions.x, m_Dimensions.y);
			m_Dimensions.x = Mathf.Max(m_Dimensions.x, m_Dimensions.y);
			m_EulerOrientation = dsc.EulerOrientation;
			m_ShapeType = (int)EShapeType.CylinderShape;
			m_EditorShapeType = (int)EShapeType.CylinderShape;
		}

		public Sampler(string name, SamplerDescShapeCapsule dsc)
		{
			m_Descriptor = new SamplerDesc(name, (int)ESamplerType.SamplerShape);
			m_ShapeCenter = dsc.Center;
			m_Dimensions = new Vector3(dsc.Radius, dsc.InnerRadius, dsc.Height);
			m_Dimensions.y = Mathf.Min(m_Dimensions.x, m_Dimensions.y);
			m_Dimensions.x = Mathf.Max(m_Dimensions.x, m_Dimensions.y);
			m_EulerOrientation = dsc.EulerOrientation;
			m_ShapeType = (int)EShapeType.CapsuleShape;
			m_EditorShapeType = (int)EShapeType.CapsuleShape;
		}

		public Sampler(string name, SamplerDescShapeMesh dsc)
		{
			m_Descriptor = new SamplerDesc(name, (int)ESamplerType.SamplerShape);
			m_ShapeCenter = dsc.Center;
			m_Dimensions = dsc.Dimensions;
			m_EulerOrientation = dsc.EulerOrientation;
			m_Mesh = dsc.Mesh;
			m_MeshFilter = null;
			m_SkinnedMeshRenderer = null;
			if (m_Mesh != null)
				m_MeshHashCode = m_Mesh.name.GetHashCode();
			else
				m_MeshHashCode = 0;
			m_SkinnedMeshRenderer = null;
			m_SamplingChannels = dsc.SamplingChannels;
			m_ShapeType = (int)EShapeType.MeshShape;
			m_EditorShapeType = (int)EShapeType.MeshShape;
		}

		public Sampler(string name, SamplerDescShapeMeshFilter dsc)
		{
			m_Descriptor = new SamplerDesc(name, (int)ESamplerType.SamplerShape);
			m_ShapeCenter = dsc.Center;
			m_Dimensions = dsc.Dimensions;
			m_EulerOrientation = dsc.EulerOrientation;
			m_MeshFilter = dsc.MeshFilter;
			m_Mesh = m_MeshFilter.sharedMesh;
			m_SkinnedMeshRenderer = null;
			if (m_Mesh != null)
				m_MeshHashCode = m_Mesh.name.GetHashCode();
			else
				m_MeshHashCode = 0;
			m_SamplingChannels = dsc.SamplingChannels;
			m_ShapeType = (int)EShapeType.MeshShape;
			m_EditorShapeType = (int)EShapeType.MeshShape + 1; //MeshFilter
		}

		public Sampler(string name, SamplerDescShapeSkinnedMesh dsc)
		{
			m_Descriptor = new SamplerDesc(name, (int)ESamplerType.SamplerShape);
			m_ShapeCenter = dsc.Center;
			m_Dimensions = dsc.Dimensions;
			m_EulerOrientation = dsc.EulerOrientation;
			m_SkinnedMeshRenderer = dsc.SkinnedMesh;
			m_Mesh = dsc.SkinnedMesh.sharedMesh;
			m_MeshFilter = null;
			if (m_Mesh != null)
				m_MeshHashCode = m_Mesh.name.GetHashCode();
			else
				m_MeshHashCode = 0;
			m_SamplingChannels = dsc.SamplingChannels;
			m_ShapeType = (int)EShapeType.SkinnedMeshShape;
			m_EditorShapeType = (int)EShapeType.SkinnedMeshShape + 1; //Offset cause of the MeshFilter
		}

		public Sampler(string name, AnimationCurve[] curvesArray)
		{
			m_Descriptor = new SamplerDesc(name, (int)ESamplerType.SamplerCurve);
			m_CurvesArray = curvesArray;
			if (m_CurvesArray.Length != 0)
			{
				int iKey = 0;
				m_CurvesTimeKeys = new float[m_CurvesArray[0].keys.Length];
				foreach (var key in m_CurvesArray[0].keys)
				{
					m_CurvesTimeKeys[iKey++] = key.time;
				}
			}
			m_ShapeType = -1;
			m_EditorShapeType = -1;
		}

		public Sampler(string name, Texture2D texture, ETexcoordMode texcoordMode)
		{
			m_Descriptor = new SamplerDesc(name, (int)ESamplerType.SamplerImage);
			m_Texture = texture;
			m_TextureChanged = true;
			m_TextureTexcoordMode = texcoordMode;
			m_ShapeType = -1;
			m_EditorShapeType = -1;
		}

		public Sampler(string name, string text)
		{
			m_Descriptor = new SamplerDesc(name, (int)ESamplerType.SamplerText);
			m_Text = text;
			m_ShapeType = -1;
			m_EditorShapeType = -1;
		}
	}

	[Serializable]
	public class AttributeDesc
	{
		public AttributeDesc(S_AttributeDesc desc)
		{
			this.Type = (BaseType)desc.Type;
			this.MinMaxFlag = (int)desc.MinMaxFlag;
			this.Name = Marshal.PtrToStringAnsi(desc.Name);
			//			if ((byte)desc.MinMaxFlag & EAttrDescFlag.HasDesc)
			this.Description = Marshal.PtrToStringAnsi(desc.Description);
			this.DefaultValue0 = desc.DefaultValue0;
			this.DefaultValue1 = desc.DefaultValue1;
			this.DefaultValue2 = desc.DefaultValue2;
			this.DefaultValue3 = desc.DefaultValue3;
			this.MinValue0 = desc.MinValue0;
			this.MinValue1 = desc.MinValue1;
			this.MinValue2 = desc.MinValue2;
			this.MinValue3 = desc.MinValue3;
			this.MaxValue0 = desc.MaxValue0;
			this.MaxValue1 = desc.MaxValue1;
			this.MaxValue2 = desc.MaxValue2;
			this.MaxValue3 = desc.MaxValue3;
		}

		public AttributeDesc(BaseType type, IntPtr name)
		{
			this.Type = type;
			this.Name = Marshal.PtrToStringAnsi(name);
		}

		public AttributeDesc(BaseType type, string name)
		{
			this.Type = type;
			this.Name = name;
		}

		public BaseType 		Type;
		public int				MinMaxFlag;
		public string			Name;
		public string			Description;
		public float  			DefaultValue0;
		public float  			DefaultValue1;
		public float  			DefaultValue2;
		public float  			DefaultValue3;
		public float  			MinValue0;
		public float  			MinValue1;
		public float  			MinValue2;
		public float  			MinValue3;
		public float  			MaxValue0;
		public float  			MaxValue1;
		public float  			MaxValue2;
		public float  			MaxValue3;
	}

	//----------------------------------------------------------------------------

	[Serializable]
	public class Attribute
	{
		public AttributeDesc	m_Descriptor;
		public float			m_Value0;
		public float			m_Value1;
		public float			m_Value2;
		public float			m_Value3;

		public float ValueFloat
		{
			get { return m_Value0; }
			set {
				Debug.Assert(m_Descriptor.Type == BaseType.Float, "PKFX Attribute : Wrong type");
				m_Value0 = value;
			}
		}

		public Vector2 ValueFloat2
		{
			get { return new Vector2(m_Value0, m_Value1); }
			set {
				Debug.Assert(m_Descriptor.Type == BaseType.Float2, "PKFX Attribute : Wrong type");
				m_Value0 = value.x; m_Value1 = value.y;
			}
		}

		public Vector3 ValueFloat3
		{
			get { return new Vector3(m_Value0, m_Value1, m_Value2); }
			set {
				Debug.Assert(m_Descriptor.Type == BaseType.Float3, "PKFX Attribute : Wrong type");
				m_Value0 = value.x; m_Value1 = value.y; m_Value2 = value.z;
			}
		}

		public Vector4 ValueFloat4
		{
			get { return new Vector4(m_Value0, m_Value1, m_Value2, m_Value3); }
			set {
				Debug.Assert(m_Descriptor.Type == BaseType.Float4, "PKFX Attribute : Wrong type");
				m_Value0 = value.x; m_Value1 = value.y; m_Value2 = value.z; m_Value3 = value.w;
			}
		}

		public int ValueInt
		{
			get { return ftoi(m_Value0); }
			set {
				Debug.Assert(m_Descriptor.Type == BaseType.Int, "PKFX Attribute : Wrong type");
				m_Value0 = itof(value);
			}
		}

		public int[] ValueInt2
		{
			get {
				int[] ret = new int[2];
				ret[0] = ftoi(m_Value0);
				ret[1] = ftoi(m_Value1);
				return ret;
			}
			set {
				Debug.Assert(m_Descriptor.Type == BaseType.Int2, "PKFX Attribute : Wrong type");
				Debug.Assert(value.Length >= 2, "PKFX Attribute : Wrong array dimension");
				m_Value0 = itof(value[0]);
				m_Value1 = itof(value[1]);
			}
		}

		public int[] ValueInt3
		{
			get
			{
				int[] ret = new int[3];
				ret[0] = ftoi(m_Value0);
				ret[1] = ftoi(m_Value1);
				ret[2] = ftoi(m_Value2);
				return ret;
			}
			set
			{
				Debug.Assert(m_Descriptor.Type == BaseType.Int3, "PKFX Attribute : Wrong type");
				Debug.Assert(value.Length >= 3, "PKFX Attribute : Wrong array dimension");
				m_Value0 = itof(value[0]);
				m_Value1 = itof(value[1]);
				m_Value2 = itof(value[2]);
			}
		}

		public int[] ValueInt4
		{
			get
			{
				int[] ret = new int[4];
				ret[0] = ftoi(m_Value0);
				ret[1] = ftoi(m_Value1);
				ret[2] = ftoi(m_Value2);
				ret[3] = ftoi(m_Value3);
				return ret;
			}
			set
			{
				Debug.Assert(m_Descriptor.Type == BaseType.Int4, "PKFX Attribute : Wrong type");
				Debug.Assert(value.Length >= 4, "PKFX Attribute : Wrong array dimension");
				m_Value0 = itof(value[0]);
				m_Value1 = itof(value[1]);
				m_Value2 = itof(value[2]);
				m_Value3 = itof(value[3]);
			}
		}

		public Attribute(S_AttributeDesc desc)
		{
			this.m_Descriptor = new AttributeDesc(desc);
			this.m_Value0 = desc.DefaultValue0;
			this.m_Value1 = desc.DefaultValue1;
			this.m_Value2 = desc.DefaultValue2;
			this.m_Value3 = desc.DefaultValue3;
		}

		public Attribute(AttributeDesc desc)
		{
			this.m_Descriptor = desc;
			this.m_Value0 = desc.DefaultValue0;
			this.m_Value1 = desc.DefaultValue1;
			this.m_Value2 = desc.DefaultValue2;
			this.m_Value3 = desc.DefaultValue3;
		}

		public Attribute(string name, float val)
		{
			this.m_Descriptor = new AttributeDesc(BaseType.Float, name);
			this.ValueFloat = val;
		}

		public Attribute(string name, Vector2 val)
		{
			this.m_Descriptor = new AttributeDesc(BaseType.Float2, name);
			this.ValueFloat2 = val;
		}

		public Attribute(string name, Vector3 val)
		{
			this.m_Descriptor = new AttributeDesc(BaseType.Float3, name);
			this.ValueFloat3 = val;
		}

		public Attribute(string name, Vector4 val)
		{
			this.m_Descriptor = new AttributeDesc(BaseType.Float4, name);
			this.ValueFloat4 = val;
		}

		public Attribute(string name, int val)
		{
			this.m_Descriptor = new AttributeDesc(BaseType.Int, name);
			ValueInt = val;
		}

		public Attribute(string name, int[] val)
		{
			if (val.Length >= 1)
			{
				this.m_Descriptor = new AttributeDesc(BaseType.Int + val.Length - 1, name);
				m_Value0 = itof(val[0]);
			}
			if (val.Length >= 2)
				m_Value1 = itof(val[1]);
			if (val.Length >= 3)
				m_Value2 = itof(val[2]);
			if (val.Length >= 4)
				m_Value3 = itof(val[3]);
		}
	}

	//----------------------------------------------------------------------------

	[Serializable]
	public class ShaderConstantDesc
	{
		public ShaderConstantDesc(S_ShaderConstantDesc desc)
		{
			this.Type = (BaseType)desc.Type;
			this.Name = Marshal.PtrToStringAnsi(desc.Name);
			this.MinMaxFlag = 0;
		}

		public ShaderConstantDesc(BaseType type, IntPtr name)
		{
			this.Type = type;
			this.Name = Marshal.PtrToStringAnsi(name);
			this.MinMaxFlag = 0;
		}

		public ShaderConstantDesc(BaseType type, string name)
		{
			this.Type = type;
			this.Name = name;
			this.MinMaxFlag = 0;
		}

		public BaseType		Type;
		public string		Name;
		public int			MinMaxFlag;
		public string		Description;
	}

	//----------------------------------------------------------------------------

	[Serializable]
	public class ShaderConstant
	{
		public ShaderConstantDesc	m_Descriptor;
		public float				m_Value0;
		public float				m_Value1;
		public float				m_Value2;
		public float				m_Value3;

		public float ValueFloat
		{
			get { return m_Value0; }
			set { m_Value0 = value; }
		}

		public Vector2 ValueFloat2
		{
			get { return new Vector2(m_Value0, m_Value1); }
			set { m_Value0 = value.x; m_Value1 = value.y; }
		}

		public Vector3 ValueFloat3
		{
			get { return new Vector3(m_Value0, m_Value1, m_Value2); }
			set { m_Value0 = value.x; m_Value1 = value.y; m_Value2 = value.z; }
		}

		public Vector4 ValueFloat4
		{
			get { return new Vector4(m_Value0, m_Value1, m_Value2, m_Value3); }
			set { m_Value0 = value.x; m_Value1 = value.y; m_Value2 = value.z; m_Value3 = value.w; }
		}

		//----------------------------------------------------------------------------

		public ShaderConstant(S_ShaderConstantDesc desc)
		{
			this.m_Descriptor = new ShaderConstantDesc(desc);
			this.m_Value0 = 0.0f;
			this.m_Value1 = 0.0f;
			this.m_Value2 = 0.0f;
			this.m_Value3 = 0.0f;
		}

		public ShaderConstant(ShaderConstantDesc desc)
		{
			this.m_Descriptor = desc;
			this.m_Value0 = 0.0f;
			this.m_Value1 = 0.0f;
			this.m_Value2 = 0.0f;
			this.m_Value3 = 0.0f;
		}

		public ShaderConstant(string name, float val)
		{
			this.m_Descriptor = new ShaderConstantDesc(BaseType.Float, name);
			this.ValueFloat = val;
		}

		public ShaderConstant(string name, Vector2 val)
		{
			this.m_Descriptor = new ShaderConstantDesc(BaseType.Float2, name);
			this.ValueFloat2 = val;
		}

		public ShaderConstant(string name, Vector3 val)
		{
			this.m_Descriptor = new ShaderConstantDesc(BaseType.Float3, name);
			this.ValueFloat3 = val;
		}

		public ShaderConstant(string name, Vector4 val)
		{
			this.m_Descriptor = new ShaderConstantDesc(BaseType.Float4, name);
			this.ValueFloat4 = val;
		}
	}

	[Serializable]
	public class SoundDescriptor
	{
		public SoundDescriptor(S_SoundDescriptor desc)
		{
            this.ChannelGroup = desc.ChannelGroup;
            this.Path = Marshal.PtrToStringAnsi(desc.Path);
            this.EventStart = Marshal.PtrToStringAnsi(desc.EventStart);
            this.EventStop = Marshal.PtrToStringAnsi(desc.EventStop);
            this.WorldPosition = desc.WorldPosition;
            this.Volume = desc.Volume;
            this.StartTimeOffsetInSeconds = desc.StartTimeOffsetInSeconds;
            this.PlayTimeInSeconds = desc.PlayTimeInSeconds;
            this.UserData = desc.UserData;
		}
    
        public int              ChannelGroup;
        public string           Path;
        public string           EventStart;
        public string           EventStop;
        public Vector3          WorldPosition;
        public float            Volume;
        public float            StartTimeOffsetInSeconds;
        public float            PlayTimeInSeconds;
        public int              UserData;
    }

#endregion

	//----------------------------------------------------------------------------

	private const string m_UnityVersion = "Unity 5.2 and up";
	public const string m_PluginVersion = "2.9p5 for " + m_UnityVersion;
	public static string				m_PackPath = Application.streamingAssetsPath;
	public static string				m_CurrentVersionString = "";
	public static bool					m_PackCopied = false;
	public static bool					m_PackLoaded = false;
	public static PKFxConf				m_GlobalConf;
	public static string				m_LogFilePath = Path.GetFullPath(Path.Combine(Application.dataPath, "../popcorn.htm"));
	public static bool					m_IsStarted = false;

	private static float[]				m_Samples;
	private static GCHandle				m_SamplesHandle;
	private static bool					m_HasSpawnerIDs = false;
	private static bool					m_HasFileLogging = false;
	private static bool					m_IsUsingOrthographicProjection = false;

	//----------------------------------------------------------------------------

	public static bool	SceneMeshAddMesh(Mesh mesh, Matrix4x4 localToWorldMatrix)
	{
		int		subMeshCount = mesh.subMeshCount;
		if (subMeshCount <= 0)
		{
			Debug.LogError("[PKFX] Mesh doesn't have sub meshes");
			return false;
		}
		int		verticesCount = mesh.vertexCount;
		if (mesh.subMeshCount > 1)
			Debug.LogWarning("[PKFX] Mesh has more than 1 submesh: non opti");
		for (int subMeshId = 0; subMeshId < mesh.subMeshCount; subMeshId++)
		{
			int	indicesCount = mesh.GetIndices(subMeshId).Length;
			Debug.Log("[PKFX] Mesh (" + (subMeshId + 1).ToString() + "/" + subMeshCount.ToString() + ") idx:" + indicesCount.ToString() + " v:" + verticesCount.ToString() + " v:" + mesh.vertices.Length.ToString() + " n:" + mesh.normals.Length.ToString() + " uv:" + mesh.uv.Length.ToString());
			if (mesh.vertices.Length != verticesCount ||
			    mesh.normals.Length != verticesCount)
			{
				Debug.LogError("[PKFX] Invalid mesh");
			}

			// Load the mesh (after the brush)
			if (!SceneMeshAddRawMesh(
				indicesCount, mesh.GetIndices(subMeshId),
				verticesCount, mesh.vertices, mesh.normals,
				localToWorldMatrix
				))
			{
				Debug.LogError("[PKFX] Fail to load raw mesh");
			}
		}
		return true;
	}

	//----------------------------------------------------------------------------

	public static void Render(short cameraID)
	{
		if (cameraID >= 0)
		{
			//Do NOTHING, all is handled by the command buffer.
			//GL.IssuePluginEvent(GetRenderEventFunc(),(int)eventID);
		}
		else
			Debug.LogError("[PKFX] PKFxManager: invalid cameraID for rendering " + cameraID);
	}

	//----------------------------------------------------------------------------

	static IEnumerable<object> AndroidRetrieveConfFile()
	{
		WWW www = new WWW(Path.Combine(Application.streamingAssetsPath, "PKconfig.cfg"));
		while (!www.isDone)
			yield return www;
		File.WriteAllBytes(m_PackPath + "/PKconfig.cfg", www.bytes);
		www.Dispose();
	}

	//----------------------------------------------------------------------------

	static PKFxManager()
	{
#if PK_COMPILED
		BindSymbols();
#endif
		var serializer = new XmlSerializer(typeof(PKFxConf));
		if (Application.platform == RuntimePlatform.Android)
		{
			m_PackPath = Application.persistentDataPath;
			foreach (object o in AndroidRetrieveConfFile()){}
		}

		string confPath = m_PackPath + "/PKconfig.cfg";
		
		if (File.Exists(confPath))
		{
			var stream = new FileStream(confPath, FileMode.Open, FileAccess.Read);
			var sr = new StreamReader(stream, Encoding.ASCII);
			m_GlobalConf = serializer.Deserialize(sr) as PKFxConf;
			stream.Close();
		}
		else
		{
			Debug.LogWarning("[PKFX] Can't find conf file : " + confPath);
			m_GlobalConf = new PKFxConf();
			m_GlobalConf.Save();
		}
		EnableFileLoggingIFN(m_GlobalConf.enableFileLog);
		SetupPackInPersistantDataPathIFN(m_GlobalConf.enablePackFxInPersistentDataPath);
#if PKFX_CHECK_SIGNATURES
		PKFxCrypto.SetupCrypto();
#endif
	}

	//----------------------------------------------------------------------------

	public static void Startup()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			UnitySetGraphicsDevice(IntPtr.Zero, (int)GfxDeviceRenderer.kGfxRendererOpenGLES20Mobile, (int)GfxDeviceEventType.kGfxDeviceEventInitialize);
		// Enable Gamma-correction only if in linear color space
#if UNITY_REVERSE_DEPTH
		if (SystemInfo.usesReversedZBuffer)
			SetReversedZBuffer(true);
#endif
		SetupColorSpace(QualitySettings.activeColorSpace == ColorSpace.Linear);
		EnableKillIndividualEffect(m_GlobalConf.enableEffectsKill);
		SetUseOrthographicProjection(m_GlobalConf.useOrthographicProjection);
		m_IsUsingOrthographicProjection = m_GlobalConf.useOrthographicProjection;

		SetDelegateOnAudioSpectrumData(Marshal.GetFunctionPointerForDelegate(new Func<IntPtr, IntPtr, IntPtr>(OnAudioSpectrumData)));
		SetDelegateOnAudioWaveformData(Marshal.GetFunctionPointerForDelegate(new Func<IntPtr, IntPtr, IntPtr>(OnAudioWaveformData)));
#if !PK_COMPILED // mutual dependency issue with class PKFxFX
		SetDelegateOnFxStopped(Marshal.GetFunctionPointerForDelegate(new Action<int>(OnFxStopped)));
		SetDelegateOnFxHotReloaded(Marshal.GetFunctionPointerForDelegate(new Action<int, int>(OnFxHotReloaded)));
#endif
		SetDelegateOnStartSound(Marshal.GetFunctionPointerForDelegate(new Action<IntPtr>(PKFxSoundManager.OnStartSound)));
		m_Samples = new float[1024];
		m_SamplesHandle = GCHandle.Alloc(m_Samples, GCHandleType.Pinned);
		m_CurrentVersionString = Marshal.PtrToStringAnsi(GetRuntimeVersion());
		m_IsStarted = true;
	}

	//----------------------------------------------------------------------------

	public static bool TryLoadPackRelative()
	{
		string fromPath = Application.dataPath;
		string toPath = m_PackPath;

		if (string.IsNullOrEmpty(fromPath))
			return false;
		if (string.IsNullOrEmpty(toPath))
			return false;

		int index = fromPath.LastIndexOf("/");
		if (index > 0)
			fromPath = fromPath.Substring(0, index);

		Uri fromUri = new Uri(fromPath + "/");
		Uri toUri = new Uri(toPath + "/");

		if (fromUri.Scheme != toUri.Scheme)
			return false; // path can't be made relative.

		Uri relativeUri = fromUri.MakeRelativeUri(toUri);
		string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

		return PKFxManager.LoadPack(relativePath + "PackFx");
	}

	//----------------------------------------------------------------------------

#if !PK_COMPILED // mutual dependency issue with class PKFxFX
	private delegate void FxCallback(int guid);
	private delegate void FxHotReloadCallback(int guid, int newGuid);

	[MonoPInvokeCallback(typeof(FxCallback))]
	public static void OnFxStopped(int guid)
	{
		PKFxFX component;

		if (PKFxFX.m_ListEffects.TryGetValue(guid, out component))
		{
			if (component.m_OnFxStopped != null)
				component.m_OnFxStopped(component);
			component.OnFxStopPlaying();
		}
	}

	//----------------------------------------------------------------------------

	[MonoPInvokeCallback(typeof(FxHotReloadCallback))]
	public static void OnFxHotReloaded(int guid, int newGuid)
	{
		PKFxFX component;

		if (PKFxFX.m_ListEffects.TryGetValue(guid, out component) == true)
		{
			component.OnFxHotReloaded(newGuid);
		}
	}
#endif

	//----------------------------------------------------------------------------

	public delegate IntPtr AudioCallback(IntPtr channelName, IntPtr nbSamples);

	[MonoPInvokeCallback(typeof(AudioCallback))]
	public static IntPtr OnAudioSpectrumData(IntPtr channelName, IntPtr nbSamples)
	{
		AudioListener.GetSpectrumData(m_Samples, 0, FFTWindow.Rectangular);
		// Last value filled by Unity seems fucked up...
		m_Samples[1023] = m_Samples[1022];
		return m_SamplesHandle.AddrOfPinnedObject();
	}

	[MonoPInvokeCallback(typeof(AudioCallback))]
	public static IntPtr OnAudioWaveformData(IntPtr channelName, IntPtr nbSamples)
	{
		AudioListener.GetOutputData(m_Samples, 0);
		return m_SamplesHandle.AddrOfPinnedObject();
	}

	//----------------------------------------------------------------------------

	public static List<AttributeDesc> ListEffectAttributesFromGUID(int FxGUID)
	{
		List<AttributeDesc> attrDescList = new List<AttributeDesc>();
		int		count = EffectAttributesCount(FxGUID);

		for (int i = 0; i < count; i++)
		{
			S_AttributeDesc desc = new S_AttributeDesc();
			desc.Name = Marshal.AllocHGlobal((int)PK_DESC_NAME_MAX_LEN);
			desc.Description = Marshal.AllocHGlobal((int)PK_DESC_DESC_MAX_LEN);
			if (EffectFillAttributeDesc(FxGUID, i, ref desc))
			{
				attrDescList.Add(new AttributeDesc(desc));
			}
			Marshal.FreeHGlobal(desc.Name);
			Marshal.FreeHGlobal(desc.Description);
		}
		return attrDescList;
	}

	//----------------------------------------------------------------------------

	public static List<AttributeDesc> ListEffectAttributesFromFx(string name)
	{
		List<AttributeDesc> attrDescList = new List<AttributeDesc>();
		int count = EffectAttributesCountFromFx(name);

		for (int i = 0; i < count; i++)
		{
			S_AttributeDesc desc = new S_AttributeDesc();
			desc.Name = Marshal.AllocHGlobal((int)PK_DESC_NAME_MAX_LEN);
			desc.Description = Marshal.AllocHGlobal((int)PK_DESC_DESC_MAX_LEN);
			if (EffectFillAttributeDescFromFx(name, i, ref desc))
			{
				attrDescList.Add(new AttributeDesc(desc));
			}
			Marshal.FreeHGlobal(desc.Name);
			Marshal.FreeHGlobal(desc.Description);
		}
		return attrDescList;
	}

	//----------------------------------------------------------------------------
	
	public static List<SamplerDesc> ListEffectSamplersFromFx(string name)
	{
		List<SamplerDesc> smpDescList = new List<SamplerDesc>();
		int count = EffectSamplersCountFromFx(name);
		
		for (int i = 0; i < count; i++)
		{
			S_SamplerDesc desc = new S_SamplerDesc();
			desc.Name = Marshal.AllocHGlobal((int)PK_DESC_NAME_MAX_LEN);
			desc.Description = Marshal.AllocHGlobal((int)PK_DESC_DESC_MAX_LEN);
			if (EffectFillSamplerDescFromFx(name, i, ref desc))
			{
				smpDescList.Add(new SamplerDesc(desc));
			}
			Marshal.FreeHGlobal(desc.Name);
			Marshal.FreeHGlobal(desc.Description);
		}
		return smpDescList;
	}

	//----------------------------------------------------------------------------

	public static List<ShaderConstantDesc> ListShaderConstantsFromName(string name, int count)
	{
		List<ShaderConstantDesc> shaderConstantDescList = new List<ShaderConstantDesc>();

		for (int i = 0; i < count; i++)
		{
			S_ShaderConstantDesc desc = new S_ShaderConstantDesc();
			if (ShaderFillConstantDesc(name, i, ref desc))
			{
				shaderConstantDescList.Add(new ShaderConstantDesc(desc));
			}
		}
		return shaderConstantDescList;
	}

	//----------------------------------------------------------------------------

	public static int CreateEffect(string path, Transform t)
	{
		return CreateEffect(path, t.localToWorldMatrix);
	}

	//----------------------------------------------------------------------------

	public static int CreateEffect(string path, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		Matrix4x4 m = Matrix4x4.identity;
		m.SetTRS(position, rotation, scale);
		return CreateEffect(path, m);
	}

	//----------------------------------------------------------------------------
	
	public static int CreateEffect(string path, Matrix4x4 m)
	{
		FxDesc desc;
		desc.Transforms = m;
		desc.FxPath = path;
#if PKFX_CHECK_SIGNATURES
		string	filePath = (m_PackPath + "/PackFx/" + path);
		if (PKFxCrypto.VerifyFx(filePath))
			return LoadFx(desc);
		else
		{
			Debug.LogError("[PKFX] Signature verification failed for fx " + Path.GetFileName(filePath));
			return -1;
		}
#else
		return LoadFx(desc);
#endif
	}

	//----------------------------------------------------------------------------

	public static bool UpdateTransformEffect(int FxGUID, Transform t)
	{
		Matrix4x4 m = t.localToWorldMatrix;
		return EffectSetTransforms(FxGUID, m);
	}

	//----------------------------------------------------------------------------

#region Conf file stuff

	private static void EnableFileLoggingIFN(bool enable)
	{
		if (Application.platform == RuntimePlatform.Android ||
		    Application.platform == RuntimePlatform.IPhonePlayer)
		{
			m_HasFileLogging = false;
			return ;
		}
		try
		{
			m_HasFileLogging = enable;
			if (enable && !File.Exists(m_LogFilePath))
			{
				FileStream fs = File.Create(m_LogFilePath);
				fs.Close();
			}
			if (!enable && File.Exists(m_LogFilePath))
				File.Delete(m_LogFilePath);
		}
		catch
		{
			Debug.LogError("[PKFX] Setting up file logging failed.");
		}
	}

	public static bool FileLoggingEnabled()
	{
		return m_HasFileLogging;
	}

	//----------------------------------------------------------------------------

	private static void SetupPackInPersistantDataPathIFN(bool enable)
	{
		if (Application.platform == RuntimePlatform.Android)
			return;
		if (enable)
		{
			List<string> files = new List<string>();
			files.AddRange(Directory.GetFiles(Application.streamingAssetsPath + "/PackFx", "*", SearchOption.AllDirectories));
			for (int i = 0; i < files.Count; i++)
				files[i] = files[i].Replace(@"\", "/");
			files.Sort();

			// Copy the files to the persistent data path
			foreach (string s in files)
			{
				if (Path.GetExtension(s) != ".meta")
				{
					string relPath = s.Substring(Application.streamingAssetsPath.Length);
					FileInfo orig = new FileInfo(s);
					FileInfo dest = new FileInfo(Application.persistentDataPath + relPath);

					if (!dest.Exists)
					{
						Debug.Log("Copy " + Application.persistentDataPath + relPath);
						if (!Directory.Exists(dest.Directory.FullName))
							Directory.CreateDirectory(dest.Directory.FullName);
						File.Copy(s, dest.FullName);
					}
					else if (orig.LastWriteTime > dest.LastWriteTime)
					{
						Debug.Log("Overwriting " + Application.persistentDataPath + relPath);
						File.Copy(s, dest.FullName, true);
					}
				}
			}
			m_PackPath = Application.persistentDataPath;
		}
		else
		{
			m_PackPath = Application.streamingAssetsPath;
		}
	}

	public static bool PackInPersistantDataPathEnabled()
	{
		return m_PackPath == Application.persistentDataPath;
	}

	//----------------------------------------------------------------------------

	public static bool IsUsingOrthographicProjection()
	{
		return m_IsUsingOrthographicProjection;
	}

	//----------------------------------------------------------------------------

	private static void EnableKillIndividualEffect(bool enable)
	{
		m_HasSpawnerIDs = enable;
		EnableSpawnerIDs(enable);
	}

	//----------------------------------------------------------------------------
	
	public static bool KillIndividualEffectEnabled()
	{
		return m_HasSpawnerIDs;
	}

#endregion

	//----------------------------------------------------------------------------

	public static string GetDefaultShader(int api, int type)
	{
		return Marshal.PtrToStringAnsi(GetDefaultShaderString(api, type));
	}

	//----------------------------------------------------------------------------
}
