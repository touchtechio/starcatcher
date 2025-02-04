using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(MIDISettings))]
[CanEditMultipleObjects ()]
public class MIDISettingsEditor : Editor
{
	MIDISettings o;
	 
	private string[] channelMaskValues = new string[16] {
		"1",
		"2",
		"3",
		"4",
		"5",
		"6",
		"7",
		"8",
		"9",
		"10",
		"11",
		"12",
		"13",
		"14",
		"15",
		"16"
	};

	public void OnEnable ()
	{
		o = target as MIDISettings;

	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();

		DrawDefaultInspector ();

		EditorGUI.BeginChangeCheck ();

		EditorGUILayout.Space ();

		EditorGUILayout.LabelField ("BASS NET - License", EditorStyles.boldLabel);

		o.userName = EditorGUILayout.TextField ("Email", o.userName);
		o.password = EditorGUILayout.PasswordField ("Reg. Key", o.password);

		EditorGUILayout.Space ();

		EditorGUILayout.LabelField ("Channel Masks", EditorStyles.boldLabel);

		o.synthChannelMask = EditorGUILayout.MaskField ("Synthesizer Mask", o.synthChannelMask, channelMaskValues);
		o.channelMask = EditorGUILayout.MaskField ("MidiOut Mask", o.channelMask, channelMaskValues);

		if (GUI.changed || EditorGUI.EndChangeCheck ()) {
			serializedObject.ApplyModifiedProperties ();
			EditorUtility.SetDirty (o);
		}
	}
}