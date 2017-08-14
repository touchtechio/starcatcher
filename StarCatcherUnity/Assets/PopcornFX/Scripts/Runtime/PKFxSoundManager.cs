//----------------------------------------------------------------------------
// Created on Tue Mar 31 18:35:30 2015 Valentin Bas
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
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Collections.Generic;
using AOT;

public class PKFxSoundManager : MonoBehaviour
{
	private delegate void VoidFPtr(IntPtr ptr);
	public delegate void StartSoundDelegate(PKFxManager.SoundDescriptor soundDesc);

	private static StartSoundDelegate   m_onStartSoundDelegate = null;
	private static List<AudioSource>    m_spawnedSound = new List<AudioSource>();

	//----------------------------------------------------------------------------

	void Awake()
	{
	}

	//----------------------------------------------------------------------------

	void Update()
	{
		if (m_onStartSoundDelegate == null && m_spawnedSound != null)
		{
			for (int i = m_spawnedSound.Count - 1; i >= 0; i--)
			{
				AudioSource it = m_spawnedSound[i];
				if (!it.isPlaying)
				{
					Destroy(it.gameObject);
					m_spawnedSound.RemoveAt(i);
				}
			}
		}
	}

	//----------------------------------------------------------------------------

	[MonoPInvokeCallback(typeof(VoidFPtr))]
	public static void OnStartSound(IntPtr actionFactorySound)
	{
		PKFxManager.S_SoundDescriptor desc = (PKFxManager.S_SoundDescriptor)Marshal.PtrToStructure(actionFactorySound, typeof(PKFxManager.S_SoundDescriptor));

		PKFxManager.SoundDescriptor sound = new PKFxManager.SoundDescriptor(desc);

		if (m_onStartSoundDelegate != null)
		{
			m_onStartSoundDelegate(sound);
			return;
		}

		string soundPath = "PKFxSounds/" + Path.ChangeExtension(sound.Path, null);

		AudioClip clip = Resources.Load(soundPath) as AudioClip;
		if (clip != null)
		{
			GameObject soundGo = new GameObject("FxSound");
			if (soundGo != null)
			{
				soundGo.transform.position = sound.WorldPosition;
				AudioSource soundSource = soundGo.AddComponent<AudioSource>();
				if (soundSource != null)
				{
					soundSource.clip = clip;
					soundSource.Play();
					soundSource.volume = sound.Volume;
					soundSource.time = sound.StartTimeOffsetInSeconds;
					soundSource.spatialBlend = 1.0f;
                    if (sound.PlayTimeInSeconds != 0.0f)
                        Destroy(soundSource, sound.PlayTimeInSeconds);
                    else
                        m_spawnedSound.Add(soundSource);
				}
			}
		}
		else
			Debug.LogError("[PKFX] Could not load sound layer " + soundPath);
	}

	//----------------------------------------------------------------------------

	public static void RegisterCustomHandler(StartSoundDelegate customDelegate)
	{
		m_onStartSoundDelegate = customDelegate;
	}
}
