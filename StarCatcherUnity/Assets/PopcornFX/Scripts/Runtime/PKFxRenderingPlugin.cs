//----------------------------------------------------------------------------
// Created on Thu Oct 10 16:25:15 2013 Raphael Thoulouze
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
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

//----------------------------------------------------------------------------

public class PKFxRenderingPlugin : PKFxCamera
{
	private PKFxDistortionEffect m_PostFx = null;

	// Exposed in "Advanced" Editor
	[Tooltip("Shows settings likely to impact performance.")][HideInInspector]
	public bool						m_ShowAdvancedSettings = false;
	[Tooltip("Loads a user-defined mesh to be used for particles world collisions.")][HideInInspector]
	public bool						m_UseSceneMesh = false;
	[Tooltip("Name of the scene mesh relative to the PackFx directory.")][HideInInspector]
	public string					m_SceneMeshPkmmPath = "Meshes/UnityScene.pkmm";

	public List<PkFxCustomShader>				m_BoundShaders;
	
	//----------------------------------------------------------------------------

	IEnumerator	Start()
	{
		base.BaseInitialize();
		yield return WaitForPack(true);

		if (Application.isEditor && QualitySettings.desiredColorSpace != ColorSpace.Linear)
			Debug.LogWarning("[PKFX] Current rendering not in linear space. " +
				"Colors may not be accurate.\n" +
				"To properly set the color space, go to \"Player Settings\">\"Other Settings\">\"Color Space\"");

		m_IsDepthCopyEnabled = (m_EnableSoftParticles || m_EnableDistortion);

		if (m_EnableDistortion && !SystemInfo.supportsImageEffects)
		{
			Debug.LogWarning("[PKFX] Image effects not supported, distortions disabled.");
			m_EnableDistortion = false;
		}

		if (m_UseSceneMesh && m_SceneMeshPkmmPath.Length > 0)
		{
			if (PKFxManager.LoadPkmmAsSceneMesh(m_SceneMeshPkmmPath))
				Debug.Log ("[PKFX] Scene Mesh loaded");
			else
				Debug.LogError("[PKFX] Failed to load mesh " + m_SceneMeshPkmmPath + " as scene mesh");
		}

		if (m_EnableDistortion || m_EnableSoftParticles)
			SetupDepthGrab();
		if (m_IsDepthCopyEnabled && m_EnableDistortion && m_Camera.actualRenderingPath == RenderingPath.Forward)
		{
			SetupDistortionPass();
			if (m_EnableDistortion && m_Camera.actualRenderingPath == RenderingPath.Forward)
			{
				m_PostFx = gameObject.AddComponent<PKFxDistortionEffect>();
				m_PostFx.m_MaterialDistortion = m_DistortionMat;
				m_PostFx.m_MaterialBlur = m_EnableBlur ? m_DistBlurMat : null;
				m_PostFx.m_BlurFactor = m_BlurFactor;
				m_PostFx._DistortionRT = m_DistortionRT;
				m_PostFx.hideFlags = HideFlags.HideAndDontSave;
			}
		}
		if (m_BoundShaders != null)
		{
			for (int iShader = 0; iShader < m_BoundShaders.Count; ++iShader)
			{
				if (m_BoundShaders[iShader] != null && !string.IsNullOrEmpty(m_BoundShaders[iShader].m_ShaderName))
				{
					if (!string.IsNullOrEmpty(m_BoundShaders[iShader].m_ShaderGroup))
					{
						m_BoundShaders[iShader].m_LoadedShaderId = PKFxManager.LoadShader(m_BoundShaders[iShader].GetDesc());
						m_BoundShaders[iShader].UpdateShaderConstants(true);
					}
					else
					{
						Debug.LogWarning("[PKFX] " + m_BoundShaders[iShader].m_ShaderName + " has no ShaderGroup, it will not be loaded");
					}
				}
			}
		}
	}

	//----------------------------------------------------------------------------

	void Update()
	{
		m_CurrentFrameID++;
	}

	//----------------------------------------------------------------------------

	void LateUpdate()
	{
		if (m_BoundShaders != null)
		{
			for (int iShader = 0; iShader < m_BoundShaders.Count; ++iShader)
			{
				if (m_BoundShaders[iShader] != null && !string.IsNullOrEmpty(m_BoundShaders[iShader].m_ShaderName))
				{
					if (!string.IsNullOrEmpty(m_BoundShaders[iShader].m_ShaderGroup))
					{
						m_BoundShaders[iShader].UpdateShaderConstants(false);
					}
				}
			}
		}
	}

	//----------------------------------------------------------------------------
}
