//----------------------------------------------------------------------------
// Created on Tue Jul 8 17:59:09 2014 Raphael Thoulouze
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

internal class PKFxDistortionEffect : MonoBehaviour {
	
	public Material			m_MaterialDistortion;
	public Material			m_MaterialBlur;
	public float			m_BlurFactor;
	private RenderTexture	m_TmpRT;
	public RenderTexture	_DistortionRT;
	
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (_DistortionRT == null || !_DistortionRT.IsCreated() || m_MaterialDistortion == null)
		{
			Graphics.Blit(source, destination);
			Debug.LogError("[PKFX] Distortion effect setup failed.");
			return ;
		}
		if (m_MaterialBlur == null)
		{
			this.m_MaterialDistortion.SetTexture("_DistortionTex", this._DistortionRT);
			Graphics.Blit(source, destination, this.m_MaterialDistortion);
			return ;
		}
		this.m_MaterialBlur.SetTexture("_DistortionTex", this._DistortionRT);
		this.m_MaterialBlur.SetFloat("_BlurFactor", m_BlurFactor);
		this.m_MaterialDistortion.SetTexture("_DistortionTex", this._DistortionRT);
		m_TmpRT = RenderTexture.GetTemporary(source.width, source.height, source.volumeDepth, source.format);

		Graphics.Blit(source, m_TmpRT, this.m_MaterialDistortion);
		Graphics.Blit(m_TmpRT, destination, this.m_MaterialBlur);

		RenderTexture.ReleaseTemporary(m_TmpRT);
	}
}
