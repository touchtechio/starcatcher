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

#if !UNITY_5_2 && !UNITY_5_3 && !UNITY_5_4_0
#define UNITY_STEREOSCOPIC_EYE
#endif

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;
using System;

public class PKFxCamera : PKFxPackDependent
{
	public static short 			g_CameraUID = 0;
	public static short				GetUniqueID() { return g_CameraUID++; }
	[Tooltip("Set to true to prevent inverted axis.")]
#if UNITY_2017
    public bool						m_FlipRendering = true;
#else
    public bool						m_FlipRendering = false;
#endif
    [Tooltip("Time multiplier for particle simulation. Range [0; 8].")][HideInInspector]
	public float					m_TimeMultiplier = 1.0f;
	[Tooltip("Specifies the particles textures level-of-detail bias.")][HideInInspector]
	public float					m_TextureLODBias = -0.5f;
	protected short					m_CameraID = 0;
	protected short					m_VRReservedID = 0;
	protected short					m_CurrentCameraID = 0;
	protected Camera				m_Camera;
	protected PKFxManager.CamDesc	m_CameraDescription;
	protected uint					m_CurrentFrameID = 0;
	protected uint					m_LastUpdateFrameID = 0;
	private static int						m_LastFrameCount = -1;

	public RenderTexture m_DepthRT = null;
	private CommandBuffer m_CmdBufDepthGrabber = null;
	private static RenderTextureFormat g_DepthFormat = RenderTextureFormat.Depth;
	private static string g_DepthShaderName = "Hidden/PKFx Depth Copy";
	private static bool	g_isDepthResolved = false;
	private Material m_DepthGrabMat = null;
	private int m_PrevScreenWidth = 0;
	private int m_PrevScreenHeight = 0;
	protected bool m_IsDepthCopyEnabled = false;

	private CommandBuffer m_CmdBufDisto = null;
	protected Material m_DistortionMat = null;
	protected Material m_DistBlurMat = null;
	protected RenderTexture m_DistortionRT = null;

	[Tooltip("Enables soft particles material")]
	[HideInInspector]
	public bool m_EnableSoftParticles = false;
	[Tooltip("Enables the distortion particles material, adding a postFX pass.")]
	[HideInInspector]
	public bool m_EnableDistortion = false;
	[Tooltip("Set to true to use the depth grabbed as a depth buffer.")]
	[HideInInspector]
	public bool m_UseDepthGrabToZTest = false;
	[Tooltip("Choose the depth greb texture format.")]
	[HideInInspector]
	public PKFxManager.DepthGrabFormat m_DepthGrabFormat = PKFxManager.DepthGrabFormat.Depth16Bits;
	[Tooltip("Enables the distortion blur pass, adding another postFX pass.")]
	[HideInInspector]
	public bool m_EnableBlur = false;
	[Tooltip("Blur factor. Ajusts the blur's spread.")]
	[HideInInspector]
	public float m_BlurFactor = 0.2f;

	private CommandBuffer	m_CmdBuf;

	//----------------------------------------------------------------------------

	public int RenderPass
	{
		get { return this.m_CameraDescription.RenderPass; }
		set { this.m_CameraDescription.RenderPass = value; }
	}

	//----------------------------------------------------------------------------

	public IntPtr DepthRT
	{
		get { return this.m_CameraDescription.DepthRT; }
		set { this.m_CameraDescription.DepthRT = value; }
	}

	//----------------------------------------------------------------------------

	void Awake()
	{
		this.m_CameraID = GetUniqueID();
		m_CurrentCameraID = m_CameraID;
		if (Application.platform != RuntimePlatform.IPhonePlayer && UnityEngine.VR.VRSettings.enabled && UnityEngine.VR.VRDevice.isPresent)
			m_VRReservedID = GetUniqueID();
		this.m_Camera = this.GetComponent<Camera>();
		m_CmdBuf = new CommandBuffer();
		m_CmdBuf.name = "PopcornFX Rendering";
		m_Camera.AddCommandBuffer((CameraEvent)PKFxManager.m_GlobalConf.globalEventSetting, m_CmdBuf);
	}

	//----------------------------------------------------------------------------

#region Rendering/Command buffer setup methods

	private static bool ResolveDepthShaderAndTextureFormat()
	{
		// Do this only once.
		if (g_isDepthResolved)
			return false;
		g_DepthShaderName = "Hidden/PKFx Depth Copy";
		if (!Shader.Find(g_DepthShaderName).isSupported || !SystemInfo.SupportsRenderTextureFormat(g_DepthFormat))
		{
			Debug.LogWarning("[PKFX] " + g_DepthShaderName + " shader or " + g_DepthFormat + " texture format not supported.");
			g_DepthShaderName = "Hidden/PKFx Depth Copy to Color";
			g_DepthFormat = RenderTextureFormat.RFloat;
			if (!SystemInfo.SupportsRenderTextureFormat(g_DepthFormat))
			{
				Debug.LogWarning("[PKFX] " + g_DepthFormat + " fallback texture format not supported.");
				g_DepthFormat = RenderTextureFormat.RHalf;
				Debug.LogWarning("[PKFX] Resorting to " + g_DepthFormat + " (may produce artefacts).");
			}
			Debug.LogWarning("[PKFX] Falling back to " + g_DepthShaderName + " shader / " + g_DepthFormat + " texture format.");
		}
		g_isDepthResolved = true;
		return true;
	}

	//----------------------------------------------------------------------------

	protected bool SetupDepthGrab()
	{
		bool firstAttempt = ResolveDepthShaderAndTextureFormat();
		if (m_DepthGrabMat == null)
		{
			if (!SystemInfo.SupportsRenderTextureFormat(g_DepthFormat))
			{
				if (firstAttempt)
				{
					Debug.LogError("[PKFX] " + g_DepthFormat + " texture format not supported.");
					Debug.LogError("[PKFX] Soft particles/distortion disabled.");
				}
				m_IsDepthCopyEnabled = false;
				return false;
			}
			m_DepthGrabMat = new Material(Shader.Find(g_DepthShaderName));
			if (m_DepthGrabMat == null)
			{
				if (firstAttempt)
				{
					Debug.LogError("[PKFX] Depth copy shader not found.");
					Debug.LogError("[PKFX] Soft particles/distortion disabled.");
				}
				m_IsDepthCopyEnabled = false;
				return false;
			}
		}

		if (m_DepthRT == null)
		{
			m_DepthRT = new RenderTexture(m_Camera.pixelWidth,
			                              m_Camera.pixelHeight, (int)m_DepthGrabFormat, g_DepthFormat);
		}
		if (!m_DepthRT.IsCreated())
			m_DepthRT.Create();

		if (m_CmdBufDepthGrabber == null)
		{
			m_CmdBufDepthGrabber = new CommandBuffer();
			m_CmdBufDepthGrabber.name = "PopcornFX Depth Grabber";
		}
		if (m_Camera.actualRenderingPath == RenderingPath.DeferredLighting || m_Camera.actualRenderingPath == RenderingPath.DeferredShading)
			m_Camera.AddCommandBuffer(CameraEvent.AfterFinalPass, m_CmdBufDepthGrabber);
		else
			m_Camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, m_CmdBufDepthGrabber);
		DepthRT = m_DepthRT.GetNativeTexturePtr();
		m_Camera.depthTextureMode |= DepthTextureMode.Depth;
		m_IsDepthCopyEnabled = true;
		return true;
	}

	//----------------------------------------------------------------------------

	void ReleaseDepthGrabResources()
	{
		if (m_CmdBufDepthGrabber != null)
		{
			m_CmdBufDepthGrabber.Clear();
			m_Camera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, m_CmdBufDepthGrabber);
			m_Camera.RemoveCommandBuffer(CameraEvent.AfterFinalPass, m_CmdBufDepthGrabber);
			m_CmdBufDepthGrabber = null;
		}
		if (m_DepthRT != null)
		{
			m_DepthRT.Release();
			m_DepthRT = null;
		}
		DepthRT = IntPtr.Zero;
		m_IsDepthCopyEnabled = false;
    }

	//----------------------------------------------------------------------------

	protected bool SetupDistortionPass()
	{
		if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBFloat))
		{
			m_DistortionRT = new RenderTexture(m_Camera.pixelWidth,
												m_Camera.pixelHeight,
												0,
												RenderTextureFormat.ARGBFloat,
												RenderTextureReadWrite.sRGB);
		}
		else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
		{
			m_DistortionRT = new RenderTexture(m_Camera.pixelWidth,
												m_Camera.pixelHeight,
												0,
												RenderTextureFormat.ARGBHalf,
												RenderTextureReadWrite.sRGB);
		}
		else
		{
			Debug.LogError("[PKFX] This device does not support ARGBFloat nor ARGBHalf render texture formats...");
			Debug.LogError("[PKFX] Distortion disabled.");
			m_EnableDistortion = false;
			m_EnableBlur = false;
		}
		if (m_DistortionRT != null && !m_DistortionRT.IsCreated())
			m_DistortionRT.Create();
		if (m_DistortionMat == null)
		{
			m_DistortionMat = new Material(Shader.Find("Hidden/PKFx Distortion"));
			if (m_DistortionMat == null)
			{
				Debug.LogError("[PKFX] Failed to load FxDistortionEffect shader...");
				Debug.LogError("[PKFX] Distortion disabled.");
				m_EnableDistortion = false;
				m_EnableBlur = false;
			}
		}
		if (m_DistBlurMat == null)
		{
			m_DistBlurMat = new Material(Shader.Find("Hidden/PKFx Blur Shader for Distortion Pass"));
			if (m_DistBlurMat == null)
			{
				Debug.LogError("[PKFX] Failed to load blur shader...");
				Debug.LogError("[PKFX] Distortion blur disabled.");
				m_EnableBlur = false;
			}
		}
		if (m_Camera.actualRenderingPath != RenderingPath.Forward)
		{
			m_CmdBufDisto = new CommandBuffer();
			m_CmdBufDisto.name = "PopcornFX Distortion Post-Effect";
			m_Camera.AddCommandBuffer(CameraEvent.BeforeImageEffects, m_CmdBufDisto);
		}
		return m_EnableDistortion;
	}

	//----------------------------------------------------------------------------

	void ReleaseDistortionResources()
	{
		if (m_CmdBufDisto != null)
		{
			m_Camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, m_CmdBufDisto);
			m_CmdBufDisto = null;
		}
		if (m_DistortionRT != null)
		{
			m_DistortionRT.Release();
			m_DistortionRT = null;
		}
		m_EnableDistortion = false;
	}

	//----------------------------------------------------------------------------

	void SetupRendering()
	{
		// Depth pass
		if (m_CmdBufDepthGrabber != null)
			m_CmdBufDepthGrabber.Clear();
		if ((m_EnableDistortion || m_EnableSoftParticles) && // Either require depth
			(m_CmdBufDepthGrabber != null || SetupDepthGrab())) // Setup if not already
		{
			if (m_DepthRT != null && !m_DepthRT.IsCreated()) // DX9 lost device
			{
				m_DepthRT.Release();
				m_DepthRT = new RenderTexture(m_Camera.pixelWidth,
								m_Camera.pixelHeight, (int)m_DepthGrabFormat, g_DepthFormat);
				if (!m_DepthRT.IsCreated())
					m_DepthRT.Create();
				DepthRT = m_DepthRT.GetNativeTexturePtr();
			}
			else
			{
				if (m_Camera.actualRenderingPath == RenderingPath.Forward && !m_FlipRendering &&
					SystemInfo.graphicsDeviceVersion.Contains("Direct3D"))
					m_DepthGrabMat.SetFloat("_Flip", 1f);
				m_CmdBufDepthGrabber.Blit((Texture)m_DepthRT, m_DepthRT, m_DepthGrabMat);
			}
		}
		else // disabled or failed to setup
		{
			if (m_CmdBufDepthGrabber != null)
			{
				m_CmdBufDepthGrabber.Clear();
				m_Camera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, m_CmdBufDepthGrabber);
				m_Camera.RemoveCommandBuffer(CameraEvent.AfterFinalPass, m_CmdBufDepthGrabber);
				m_CmdBufDepthGrabber = null;
				DepthRT = IntPtr.Zero;
			}
		}

		// Distortion PostFX
		if (m_CmdBufDisto != null)
			m_CmdBufDisto.Clear();
		if (m_Camera.actualRenderingPath != RenderingPath.Forward &&
			m_EnableDistortion &&
			((m_CmdBufDisto != null && m_DistortionRT != null) || SetupDistortionPass()))
		{
			int tmpID = Shader.PropertyToID("PKFxDistTmp");
			RenderTargetIdentifier tmpRTID = new RenderTargetIdentifier(tmpID);
			m_CmdBufDisto.GetTemporaryRT(tmpID, -1, -1, -1, FilterMode.Trilinear, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
			m_DistortionMat.SetTexture("_DistortionTex", m_DistortionRT);
			m_CmdBufDisto.Blit(BuiltinRenderTextureType.CameraTarget, tmpRTID, m_DistortionMat);

			if (m_EnableBlur)
			{
				m_DistBlurMat.SetTexture("_DistortionTex", m_DistortionRT);
				m_DistBlurMat.SetFloat("_BlurFactor", m_BlurFactor);
				m_CmdBufDisto.Blit(tmpRTID, BuiltinRenderTextureType.CameraTarget, m_DistBlurMat);
			}
			else
				m_CmdBufDisto.Blit(tmpRTID, BuiltinRenderTextureType.CameraTarget);
		}
		m_CmdBuf.Clear();

		// Regular rendering
		m_CmdBuf.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
		m_CmdBuf.IssuePluginEvent(PKFxManager.GetRenderEventFunc(), (int)((UInt32)m_CurrentCameraID | PKFxManager.POPCORN_MAGIC_NUMBER));
		//m_CmdBuf.Blit((Texture)m_DepthRT,BuiltinRenderTextureType.CameraTarget);

		if (m_EnableDistortion)
		{
			// Distortion rendering
			m_CmdBuf.SetRenderTarget(m_DistortionRT);
			m_CmdBuf.ClearRenderTarget(false, true, Color.black);
			m_CmdBuf.IssuePluginEvent(PKFxManager.GetRenderEventFunc(), (int)((UInt32)m_CurrentCameraID | PKFxManager.POPCORN_MAGIC_NUMBER | 0x00002000));
		}

		m_PrevScreenWidth = this.m_Camera.pixelWidth;
		m_PrevScreenHeight = this.m_Camera.pixelHeight;
	}

#endregion

	//----------------------------------------------------------------------------

	void UpdateFrame()
	{
		m_CameraDescription.DT = Time.smoothDeltaTime;
		m_TimeMultiplier = Mathf.Max(m_TimeMultiplier, 0.0f);
		m_TimeMultiplier = Mathf.Min(m_TimeMultiplier, 8.0f);
		m_CameraDescription.DT *= m_TimeMultiplier;
		m_CameraDescription.NearClip = this.m_Camera.nearClipPlane;
		m_CameraDescription.FarClip = this.m_Camera.farClipPlane;
		m_CameraDescription.LODBias = m_TextureLODBias;
		m_CameraDescription.DepthBpp = (int)m_DepthGrabFormat;

		// Set the camera flags
		m_CameraDescription.Flags = 0;
		m_CameraDescription.Flags |= m_UseDepthGrabToZTest ? (int)PKFxManager.CamFlags.UseDepthGrabberTexture : 0;
		// We need to recreate the depth texture on the native side when "Use Depth Grab To Z Test" is enabled
		if (this.m_Camera.pixelWidth != m_PrevScreenWidth || this.m_Camera.pixelHeight != m_PrevScreenHeight)
		{
			m_CameraDescription.Flags |= (int)PKFxManager.CamFlags.ScreenResolutionChanged;
			// If the screen resolution has changed, we remove the setup of the depth grab and of the distortion
			if (m_CmdBufDepthGrabber != null)
			{
				if (m_Camera.actualRenderingPath == RenderingPath.DeferredLighting || m_Camera.actualRenderingPath == RenderingPath.DeferredShading)
					m_Camera.RemoveCommandBuffer(CameraEvent.AfterFinalPass, m_CmdBufDepthGrabber);
				else
					m_Camera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, m_CmdBufDepthGrabber);
				m_CmdBufDepthGrabber.Clear();
				m_CmdBufDepthGrabber = null;
			}
			if (m_DepthRT != null)
			{
				DepthRT = IntPtr.Zero;
				m_DepthRT.Release();
				m_DepthRT = null;
			}

			if (m_DistortionRT != null)
			{
				m_DistortionRT.Release();
				m_DistortionRT = null;
			}
			if (m_CmdBufDisto != null)
			{
				m_Camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, m_CmdBufDisto);
				m_CmdBufDisto.Clear();
				m_CmdBufDisto = null;
			}
			else if (m_EnableDistortion && m_DistortionRT == null)
			{
				SetupDistortionPass();
				if (m_Camera.actualRenderingPath == RenderingPath.Forward)
				{
					PKFxDistortionEffect distoPostFx = GetComponent<PKFxDistortionEffect>();
					if (distoPostFx != null)
						distoPostFx._DistortionRT = m_DistortionRT;
				}
			}
			// We then update the prev screen size
			m_PrevScreenWidth = this.m_Camera.pixelWidth;
			m_PrevScreenHeight = this.m_Camera.pixelHeight;
		}

#if UNITY_STEREOSCOPIC_EYE
		PKFxManager.LogicalUpdate(m_CameraDescription.DT);
		if (!m_Camera.stereoEnabled)
		{
			UpdateViewMatrix();
			UpdateProjectionMatrix();
			m_CurrentCameraID = m_CameraID;
			PKFxManager.UpdateCamDesc(m_CurrentCameraID, m_CameraDescription, false);
		}
		else
		{
			// stereo-cam's first eye.
			UpdateViewMatrix(true, Camera.StereoscopicEye.Left);
			UpdateProjectionMatrix(true, Camera.StereoscopicEye.Left);
			m_CurrentCameraID = m_CameraID;
			PKFxManager.UpdateCamDesc(m_CurrentCameraID, m_CameraDescription, false);
			// second eye.
			UpdateViewMatrix(true, Camera.StereoscopicEye.Right);
			UpdateProjectionMatrix(true, Camera.StereoscopicEye.Right);
			m_CurrentCameraID = m_VRReservedID;
			PKFxManager.UpdateCamDesc(m_CurrentCameraID, m_CameraDescription, false);
		}
#else
		if (m_Camera.stereoEnabled)
		{
			Debug.LogError("[PKFX] VR is not supported on Unity 5.2 and 5.3");
			return;
		}
		m_CurrentCameraID = m_CameraID;
		UpdateViewMatrix();
		UpdateProjectionMatrix();
		PKFxManager.LogicalUpdate(m_CameraDescription.DT);
		PKFxManager.UpdateCamDesc(m_CurrentCameraID, m_CameraDescription, false);
#endif
	}

	//----------------------------------------------------------------------------

#if UNITY_STEREOSCOPIC_EYE
	void UpdateViewMatrix(bool isVR = false, Camera.StereoscopicEye eye = Camera.StereoscopicEye.Left)
	{
		if (!isVR)
			m_CameraDescription.ViewMatrix = this.m_Camera.worldToCameraMatrix;
		else
			m_CameraDescription.ViewMatrix = this.m_Camera.GetStereoViewMatrix(eye);
	}
#else
	void UpdateViewMatrix()
	{
		m_CameraDescription.ViewMatrix = this.m_Camera.worldToCameraMatrix;
	}
#endif

	//----------------------------------------------------------------------------

#if UNITY_STEREOSCOPIC_EYE
	void UpdateProjectionMatrix(bool isVR = false, Camera.StereoscopicEye eye = Camera.StereoscopicEye.Left)
	{
#if UNITY_2017
        bool isRenderingToTexture = m_FlipRendering;
#else
        bool isRenderingToTexture = m_FlipRendering
            || this.m_Camera.actualRenderingPath == RenderingPath.DeferredLighting
            || this.m_Camera.actualRenderingPath == RenderingPath.DeferredShading;
#endif
		Matrix4x4 projectionMatrix;
		if (!isVR)
			projectionMatrix = this.m_Camera.projectionMatrix;
		else
			projectionMatrix = this.m_Camera.GetStereoProjectionMatrix(eye);

		m_CameraDescription.ProjectionMatrix = GL.GetGPUProjectionMatrix(projectionMatrix, isRenderingToTexture);
	}
#else
	void UpdateProjectionMatrix()
	{
		bool isRenderingToTexture = m_FlipRendering
			|| this.m_Camera.actualRenderingPath == RenderingPath.DeferredLighting
			|| this.m_Camera.actualRenderingPath == RenderingPath.DeferredShading;

		m_CameraDescription.ProjectionMatrix = GL.GetGPUProjectionMatrix(this.m_Camera.projectionMatrix, isRenderingToTexture);
	}
#endif
	//----------------------------------------------------------------------------

	void OnPreCull()
	{
		UpdateFrame();
	}

	void OnPreRender()
	{
		if (!PKFxManager.m_PackLoaded)
			return;
		if (m_CurrentFrameID != m_LastUpdateFrameID)
		{
			m_CurrentCameraID = m_CameraID;
			SetupRendering();
			m_LastUpdateFrameID = m_CurrentFrameID;
		}
		else
		{
			m_CurrentCameraID = m_VRReservedID;
			SetupRendering();
		}

		if (m_LastFrameCount != Time.frameCount)
		{
			PKFxManager.UpdateParticles(m_CameraDescription);
			m_LastFrameCount = Time.frameCount;
		}
		PKFxManager.Render(m_CurrentCameraID);
	}
	//----------------------------------------------------------------------------

	// This is for the distortion cam.
	void Update()
	{
		m_CurrentFrameID++;
	}

	//----------------------------------------------------------------------------

	protected void OnDestroy()
	{
		ReleaseDepthGrabResources();
		ReleaseDistortionResources();
	}
}
