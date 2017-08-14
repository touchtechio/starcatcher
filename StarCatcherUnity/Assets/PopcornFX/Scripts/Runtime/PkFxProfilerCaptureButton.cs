//----------------------------------------------------------------------------
// Created on Wed Jun 3 18:48:15 2015 Valentin Bas
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

public class PkFxProfilerCaptureButton : MonoBehaviour
{
	public int		FrameCountToCapture = 10;

	private bool	_InCapture = false;
	private int		_FrameCaptured = 0;

	void OnGUI()
	{
		if (!_InCapture && GUI.Button(new Rect(10, 10, 500, 150), "Profiler capture"))
		{
			_InCapture = true;
			PKFxManager.ProfilerSetEnable(true);
		}
	}

	//----------------------------------------------------------------------------

	void OnPostRender()
	{
		if (_InCapture)
		{
			_FrameCaptured++;
			if (_FrameCaptured >= FrameCountToCapture)
			{
				_FrameCaptured = 0;
				PKFxManager.WriteProfileReport(Application.persistentDataPath + "/ProfileReport.pkpr");
				Debug.Log("[PKFX] Profiling report written to " + Application.persistentDataPath + "/ProfileReport.pkpr");
				_InCapture = false;
				PKFxManager.ProfilerSetEnable(false);
			}
		}
	}
}
