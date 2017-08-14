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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class PKFxPackDependent : MonoBehaviour 
{
	private IEnumerator _WaitForPack()
	{
		do
		{
			yield return null;
		}	
		while (!PKFxManager.m_PackCopied);
	}

	//----------------------------------------------------------------------------

	private IEnumerator _WaitForPackLoaded()
	{
		do
		{
			yield return null;
		}	
		while (!PKFxManager.m_PackLoaded);
	}
	
	//----------------------------------------------------------------------------
	
	public Coroutine WaitForPack(bool isLoaded)
	{
		if (isLoaded)
			return StartCoroutine("_WaitForPack");
		else
			return StartCoroutine("_WaitForPackLoaded");
	}
	
		
	//----------------------------------------------------------------------------

	private bool		checkHash(KeyValuePair<string, string> entry, List<KeyValuePair<string, string> > deviceContent)
	{
		foreach (KeyValuePair<string, string> file in deviceContent)
		{
			if (file.Key == entry.Key)
				return file.Value == entry.Value;
		}
		return false;
	}
	
	//----------------------------------------------------------------------------
	
	public virtual void BaseInitialize()
	{
		if (!PKFxManager.m_PackLoaded)
		{
			PKFxManager.Startup();
			StartCoroutine("CopyPackAsyncOnAndroid");
			WaitForPack(false);

			if (Application.platform != RuntimePlatform.Android)
			{
				if (!PKFxManager.TryLoadPackRelative())
					PKFxManager.LoadPack(PKFxManager.m_PackPath + "/PackFx");
			}
	
			PKFxManager.m_PackLoaded = true;
		}
	}
	
	//----------------------------------------------------------------------------
	
	void	Start()
	{
		BaseInitialize();
	}
	
	//----------------------------------------------------------------------------
	
	IEnumerator CopyPackAsyncOnAndroid()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			string 		fromPath = Application.streamingAssetsPath + "/";
			string 		toPath = Application.persistentDataPath + "/";
			WWW			www1;
			List<KeyValuePair<string, string> >	archiveContent = new List<KeyValuePair<string, string>>();
			
			// Open Index archive size
			www1 = new WWW(fromPath + "Index.txt");
			yield return www1;

			byte[] indexOf = www1.bytes;
			string[] lines = Encoding.ASCII.GetString(indexOf).Split('\n');
			
			foreach (string line in lines)
			{
				if (line.Length > 1)
				{
					string fileName = line.Substring(0, line.LastIndexOf(':'));
					string fileHash = line.Substring(line.LastIndexOf(':'));
					archiveContent.Add(new KeyValuePair<string, string>(fileName, fileHash));
				}
			}
			
			// Open Index device size if it exists
			if (File.Exists(toPath + "Index.txt"))
			{
				List<KeyValuePair<string, string> >	deviceContent = new List<KeyValuePair<string, string>>();

				StreamReader sr = new StreamReader(toPath + "Index.txt");
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					string fileName = line.Substring(0, line.LastIndexOf(':'));
					string fileHash = line.Substring(line.LastIndexOf(':'));
					deviceContent.Add(new KeyValuePair<string, string>(fileName, fileHash));					
				}
				sr.Close();
				// for each file in the archive index, check existence and hash device size.
				// copy if absent, overwrite if different.
				foreach (KeyValuePair<string, string> entry in archiveContent)
				{
					if (!checkHash(entry, deviceContent))
					{
						Debug.Log("[PKFX] " + fromPath + entry.Key + " : file doesn't exist or hash differs");
						www1 = new WWW(fromPath + entry.Key);
						yield return www1;
						string dir = new string((toPath + entry.Key).ToCharArray(), 0, toPath.Length + entry.Key.LastIndexOf('/'));
						if (!Directory.Exists(dir))
							Directory.CreateDirectory(dir);
						File.WriteAllBytes(toPath + entry.Key, www1.bytes);
					}
				}
			}
			// Else, copy the whole pack
			else
			{
				Debug.Log("[PKFX] No pack index found, copy everything!");
				foreach (KeyValuePair<string, string> entry in archiveContent)
				{
					www1 = new WWW(fromPath + entry.Key);
					yield return www1;
					string dir = new string((toPath + entry.Key).ToCharArray(), 0, toPath.Length + entry.Key.LastIndexOf('/'));
					if (!Directory.Exists(dir))
						Directory.CreateDirectory(dir);
					File.WriteAllBytes(toPath + entry.Key, www1.bytes);
				}
			}
			
			File.WriteAllBytes(toPath + "Index.txt", indexOf);
			PKFxManager.LoadPack(Application.persistentDataPath + "/PackFx/");
			PKFxManager.m_PackLoaded = true;
		}
		PKFxManager.m_PackCopied = true;
		yield return null;
	}
}
