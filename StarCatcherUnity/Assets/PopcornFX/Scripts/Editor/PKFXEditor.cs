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
using UnityEditor;
using System;
using System.Collections;

public static class PKFxEditor : object 
{
	#region Utilities

	private static int		ftoi(float fff)
	{
		return BitConverter.ToInt32(BitConverter.GetBytes(fff), 0);
	}

	//----------------------------------------------------------------------------

	private static float	itof(int i)
	{
		return BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
	}

	//----------------------------------------------------------------------------

	private static bool		HasMin(int flag)
	{
		return (flag & (int)PKFxManager.EAttrDescFlag.HasMin) != 0;
	}

	//----------------------------------------------------------------------------

	private static bool		HasMax(int flag)
	{
		return (flag & (int)PKFxManager.EAttrDescFlag.HasMax) != 0;
	}

	//----------------------------------------------------------------------------

	private static Vector2	Minf(Vector2 v, SerializedProperty x, SerializedProperty y)
	{
		v.x = Mathf.Min(v.x, x.floatValue);
		v.y = Mathf.Min(v.y, y.floatValue);
		return v;
	}

	private static Vector3	Minf(Vector3 v, SerializedProperty x, SerializedProperty y, SerializedProperty z)
	{
		v.x = Mathf.Min(v.x, x.floatValue);
		v.y = Mathf.Min(v.y, y.floatValue);
		v.z = Mathf.Min(v.z, z.floatValue);
		return v;
	}

	private static Vector4	Minf(Vector4 v, SerializedProperty x, SerializedProperty y, SerializedProperty z, SerializedProperty w)
	{
		v.x = Mathf.Min(v.x, x.floatValue);
		v.y = Mathf.Min(v.y, y.floatValue);
		v.z = Mathf.Min(v.z, z.floatValue);
		v.w = Mathf.Min(v.w, w.floatValue);
		return v;
	}

	//----------------------------------------------------------------------------

	private static Vector2	Maxf(Vector2 v, SerializedProperty x, SerializedProperty y)
	{
		v.x = Mathf.Max(v.x, x.floatValue);
		v.y = Mathf.Max(v.y, y.floatValue);
		return v;
	}
	
	private static Vector3	Maxf(Vector3 v, SerializedProperty x, SerializedProperty y, SerializedProperty z)
	{
		v.x = Mathf.Max(v.x, x.floatValue);
		v.y = Mathf.Max(v.y, y.floatValue);
		v.z = Mathf.Max(v.z, z.floatValue);
		return v;
	}
	
	private static Vector4	Maxf(Vector4 v, SerializedProperty x, SerializedProperty y, SerializedProperty z, SerializedProperty w)
	{
		v.x = Mathf.Max(v.x, x.floatValue);
		v.y = Mathf.Max(v.y, y.floatValue);
		v.z = Mathf.Max(v.z, z.floatValue);
		v.w = Mathf.Max(v.w, w.floatValue);
		return v;
	}

	//----------------------------------------------------------------------------

	private static float Min(SerializedProperty p, SerializedProperty min)
	{
		return itof(Mathf.Min(ftoi(p.floatValue), ftoi(min.floatValue)));
	}

	//----------------------------------------------------------------------------

	private static float Max(SerializedProperty p, SerializedProperty max)
	{
		return itof(Mathf.Max(ftoi(p.floatValue), ftoi(max.floatValue)));
	}

	#endregion

	//----------------------------------------------------------------------------

	private static float	FloatSlider(SerializedProperty fP, SerializedProperty fPMin, SerializedProperty fPMax, string name, string description)
	{
		if (!string.IsNullOrEmpty(name))
			return EditorGUILayout.Slider(new GUIContent(name, description), fP.floatValue, fPMin.floatValue, fPMax.floatValue);
		else
			return EditorGUILayout.Slider((fP.floatValue), (fPMin.floatValue), (fPMax.floatValue));
	}

	//----------------------------------------------------------------------------

	private static float	IntSlider(SerializedProperty fP, SerializedProperty fPMin, SerializedProperty fPMax, string name, string description)
	{
		if (!string.IsNullOrEmpty(name))
			return itof(EditorGUILayout.IntSlider(new GUIContent(name, description), ftoi(fP.floatValue), ftoi(fPMin.floatValue), ftoi(fPMax.floatValue)));
		else
			return itof(EditorGUILayout.IntSlider(ftoi(fP.floatValue), ftoi(fPMin.floatValue), ftoi(fPMax.floatValue)));
	}

	//----------------------------------------------------------------------------

	private static float	MinFloatField(SerializedProperty fP, SerializedProperty fPMin, string name, string description)
	{
		return Mathf.Max(EditorGUILayout.FloatField(new GUIContent(name, description), fP.floatValue), fPMin.floatValue);
	}

	//----------------------------------------------------------------------------

	private static float	MaxFloatField(SerializedProperty fP, SerializedProperty fPMax, string name, string description)
	{
		return Mathf.Min(EditorGUILayout.FloatField(new GUIContent(name, description), fP.floatValue), fPMax.floatValue);

	}

	//----------------------------------------------------------------------------

	private static float	IntField(SerializedProperty v)
	{
		return itof(EditorGUILayout.IntField(ftoi(v.floatValue)));
	}

	//----------------------------------------------------------------------------

	public static SerializedProperty AttributeField(SerializedProperty attr)
	{
		SerializedProperty m_Name = attr.FindPropertyRelative("m_Descriptor.Name");
		SerializedProperty m_Description = attr.FindPropertyRelative("m_Descriptor.Description");
		SerializedProperty m_Type = attr.FindPropertyRelative("m_Descriptor.Type");
		SerializedProperty m_Flag = attr.FindPropertyRelative("m_Descriptor.MinMaxFlag");
		SerializedProperty m_Value0 = attr.FindPropertyRelative("m_Value0");
		SerializedProperty m_Value1 = attr.FindPropertyRelative("m_Value1");
		SerializedProperty m_Value2 = attr.FindPropertyRelative("m_Value2");
		SerializedProperty m_Value3 = attr.FindPropertyRelative("m_Value3");
		SerializedProperty m_MinValue0 = attr.FindPropertyRelative("m_Descriptor.MinValue0");
		SerializedProperty m_MinValue1 = attr.FindPropertyRelative("m_Descriptor.MinValue1");
		SerializedProperty m_MinValue2 = attr.FindPropertyRelative("m_Descriptor.MinValue2");
		SerializedProperty m_MinValue3 = attr.FindPropertyRelative("m_Descriptor.MinValue3");
		SerializedProperty m_MaxValue0 = attr.FindPropertyRelative("m_Descriptor.MaxValue0");
		SerializedProperty m_MaxValue1 = attr.FindPropertyRelative("m_Descriptor.MaxValue1");
		SerializedProperty m_MaxValue2 = attr.FindPropertyRelative("m_Descriptor.MaxValue2");
		SerializedProperty m_MaxValue3 = attr.FindPropertyRelative("m_Descriptor.MaxValue3");

		switch(m_Type.intValue)
		{
		case (int)PKFxManager.BaseType.Float:
			if (HasMin(m_Flag.intValue) && HasMax(m_Flag.intValue))
				m_Value0.floatValue = FloatSlider(m_Value0, m_MinValue0, m_MaxValue0, m_Name.stringValue, m_Description.stringValue);
			else if (HasMin(m_Flag.intValue))
				m_Value0.floatValue = MinFloatField(m_Value0, m_MinValue0, m_Name.stringValue, m_Description.stringValue);
			else if (HasMax(m_Flag.intValue))
				m_Value0.floatValue = MaxFloatField(m_Value0, m_MaxValue0, m_Name.stringValue, m_Description.stringValue);
			else
				m_Value0.floatValue = EditorGUILayout.FloatField(new GUIContent(m_Name.stringValue, m_Description.stringValue), m_Value0.floatValue);
			break;
		case (int)PKFxManager.BaseType.Float2:
			if (HasMin(m_Flag.intValue) && HasMax(m_Flag.intValue))
			{
				EditorGUILayout.PrefixLabel(new GUIContent(m_Name.stringValue, m_Description.stringValue));
				EditorGUI.indentLevel++;
				m_Value0.floatValue = FloatSlider(m_Value0, m_MinValue0, m_MaxValue0, "X", m_Name.stringValue);
				m_Value1.floatValue = FloatSlider(m_Value1, m_MinValue1, m_MaxValue1, "Y", m_Name.stringValue);
				EditorGUI.indentLevel--;
			}
			else
			{
				Vector2 tmp2 = new Vector2((m_Value0.floatValue), (m_Value1.floatValue));
				tmp2 = EditorGUILayout.Vector2Field(new GUIContent(m_Name.stringValue, m_Description.stringValue), tmp2);
				if (HasMin(m_Flag.intValue))
					tmp2 = Maxf(tmp2, m_MinValue0, m_MinValue1);
				if (HasMax(m_Flag.intValue))
					tmp2 = Minf(tmp2, m_MaxValue0, m_MaxValue1);
				m_Value0.floatValue = tmp2.x;
				m_Value1.floatValue = tmp2.y;
			}
			break;
		case (int)PKFxManager.BaseType.Float3:
			if (HasMin(m_Flag.intValue) && HasMax(m_Flag.intValue))
			{
				EditorGUILayout.PrefixLabel(new GUIContent(m_Name.stringValue, m_Description.stringValue));
				EditorGUI.indentLevel++;
				m_Value0.floatValue = FloatSlider(m_Value0, m_MinValue0, m_MaxValue0, "X", m_Name.stringValue);
				m_Value1.floatValue = FloatSlider(m_Value1, m_MinValue1, m_MaxValue1, "Y", m_Name.stringValue);
				m_Value2.floatValue = FloatSlider(m_Value2, m_MinValue2, m_MaxValue2, "Z", m_Name.stringValue);
				EditorGUI.indentLevel--;
			}
			else
			{
				Vector3 tmp3 = new Vector3((m_Value0.floatValue), (m_Value1.floatValue), (m_Value2.floatValue));
				tmp3 = EditorGUILayout.Vector3Field(new GUIContent(m_Name.stringValue, m_Description.stringValue), tmp3);
				if (HasMin(m_Flag.intValue))
					tmp3 = Maxf(tmp3, m_MinValue0, m_MinValue1, m_MinValue2);
				if (HasMax(m_Flag.intValue))
					tmp3 = Minf(tmp3, m_MaxValue0, m_MaxValue1, m_MaxValue2);
				m_Value0.floatValue = (tmp3.x);
				m_Value1.floatValue = (tmp3.y);
				m_Value2.floatValue = (tmp3.z);
			}
			break;
		case (int)PKFxManager.BaseType.Float4:
			if (HasMin(m_Flag.intValue) && HasMax(m_Flag.intValue))
			{
				EditorGUILayout.PrefixLabel(new GUIContent(m_Name.stringValue, m_Description.stringValue));
				EditorGUI.indentLevel++;
				m_Value0.floatValue = FloatSlider(m_Value0, m_MinValue0, m_MaxValue0, "X", m_Name.stringValue);
				m_Value1.floatValue = FloatSlider(m_Value1, m_MinValue1, m_MaxValue1, "Y", m_Name.stringValue);
				m_Value2.floatValue = FloatSlider(m_Value2, m_MinValue2, m_MaxValue2, "Z", m_Name.stringValue);
				m_Value3.floatValue = FloatSlider(m_Value3, m_MinValue3, m_MaxValue3, "W", m_Name.stringValue);
				EditorGUI.indentLevel--;
			}
			else
			{
				Vector4 tmp4 = new Vector4((m_Value0.floatValue), (m_Value1.floatValue), (m_Value2.floatValue), (m_Value3.floatValue));
				tmp4 = EditorGUILayout.Vector4Field(m_Name.stringValue, tmp4);
				if (HasMin(m_Flag.intValue))
					tmp4 = Maxf(tmp4, m_MinValue0, m_MinValue1, m_MinValue2, m_MinValue3);
				if (HasMax(m_Flag.intValue))
					tmp4 = Minf(tmp4, m_MaxValue0, m_MaxValue1, m_MaxValue2, m_MaxValue3);
				m_Value0.floatValue = (tmp4.x);
				m_Value1.floatValue = (tmp4.y);
				m_Value2.floatValue = (tmp4.z);
				m_Value3.floatValue = (tmp4.w);
			}
			break;

		case (int)PKFxManager.BaseType.Int:
			if (HasMin(m_Flag.intValue) && HasMax(m_Flag.intValue))
				m_Value0.floatValue = IntSlider(m_Value0, m_MinValue0, m_MaxValue0, m_Name.stringValue, m_Description.stringValue);
			else
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(new GUIContent(m_Name.stringValue, m_Description.stringValue));
				m_Value0.floatValue = IntField(m_Value0);
				if (HasMin(m_Flag.intValue))
					m_Value0.floatValue = Max(m_Value0, m_MinValue0);
				if (HasMax(m_Flag.intValue))
					m_Value0.floatValue = Min(m_Value0, m_MaxValue0);
				EditorGUILayout.EndHorizontal();
			}
			break;
		case (int)PKFxManager.BaseType.Int2:
			if (HasMin(m_Flag.intValue) && HasMax(m_Flag.intValue))
			{
				EditorGUILayout.PrefixLabel(new GUIContent(m_Name.stringValue, m_Description.stringValue));
				EditorGUI.indentLevel++;
				m_Value0.floatValue = IntSlider(m_Value0, m_MinValue0, m_MaxValue0, "X", m_Name.stringValue);
				m_Value1.floatValue = IntSlider(m_Value1, m_MinValue1, m_MaxValue1, "Y", m_Name.stringValue);
				EditorGUI.indentLevel--;
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(new GUIContent(m_Name.stringValue, m_Description.stringValue));
				GUILayout.Label("X");
				m_Value0.floatValue = IntField(m_Value0);
				GUILayout.Label("Y");
				m_Value1.floatValue = IntField(m_Value1);
				if (HasMin(m_Flag.intValue))
				{
					m_Value0.floatValue = Max(m_Value0, m_MinValue0);
					m_Value1.floatValue = Max(m_Value1, m_MinValue1);
				}
				if (HasMax(m_Flag.intValue))
				{
					m_Value0.floatValue = Min(m_Value0, m_MaxValue0);
					m_Value1.floatValue = Min(m_Value1, m_MaxValue1);
				}
				EditorGUILayout.EndHorizontal();
			}
			break;
		case (int)PKFxManager.BaseType.Int3:
			if (HasMin(m_Flag.intValue) && HasMax(m_Flag.intValue))
			{
				EditorGUILayout.PrefixLabel(new GUIContent(m_Name.stringValue, m_Description.stringValue));
				EditorGUI.indentLevel++;
				m_Value0.floatValue = IntSlider(m_Value0, m_MinValue0, m_MaxValue0, "X", m_Name.stringValue);
				m_Value1.floatValue = IntSlider(m_Value1, m_MinValue1, m_MaxValue1, "Y", m_Name.stringValue);
				m_Value2.floatValue = IntSlider(m_Value2, m_MinValue2, m_MaxValue2, "Z", m_Name.stringValue);
				EditorGUI.indentLevel--;
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(new GUIContent(m_Name.stringValue, m_Description.stringValue));
				GUILayout.Label("X");
				m_Value0.floatValue = IntField(m_Value0);
				GUILayout.Label("Y");
				m_Value1.floatValue = IntField(m_Value1);
				GUILayout.Label("Z");
				m_Value2.floatValue = IntField(m_Value2);
				if (HasMin(m_Flag.intValue))
				{
					m_Value0.floatValue = Max(m_Value0, m_MinValue0);
					m_Value1.floatValue = Max(m_Value1, m_MinValue1);
					m_Value2.floatValue = Max(m_Value2, m_MinValue2);
				}
				if (HasMax(m_Flag.intValue))
				{
					m_Value0.floatValue = Min(m_Value0, m_MaxValue0);
					m_Value1.floatValue = Min(m_Value1, m_MaxValue1);
					m_Value2.floatValue = Min(m_Value2, m_MaxValue2);
				}
				EditorGUILayout.EndHorizontal();
			}
			break;
		case (int)PKFxManager.BaseType.Int4:
			if (HasMin(m_Flag.intValue) && HasMax(m_Flag.intValue))
			{
				EditorGUILayout.PrefixLabel(new GUIContent(m_Name.stringValue, m_Description.stringValue));
				EditorGUI.indentLevel++;
				m_Value0.floatValue = IntSlider(m_Value0, m_MinValue0, m_MaxValue0, "X", m_Name.stringValue);
				m_Value1.floatValue = IntSlider(m_Value1, m_MinValue1, m_MaxValue1, "Y", m_Name.stringValue);
				m_Value2.floatValue = IntSlider(m_Value2, m_MinValue2, m_MaxValue2, "Z", m_Name.stringValue);
				m_Value3.floatValue = IntSlider(m_Value3, m_MinValue3, m_MaxValue3, "W", m_Name.stringValue);
				EditorGUI.indentLevel--;
			}
			else
			{
				EditorGUILayout.PrefixLabel(new GUIContent(m_Name.stringValue, m_Description.stringValue));
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("X");
				m_Value0.floatValue = IntField(m_Value0);
				GUILayout.Label("Y");
				m_Value1.floatValue = IntField(m_Value1);
				GUILayout.Label("Z");
				m_Value2.floatValue = IntField(m_Value2);
				GUILayout.Label("W");
				m_Value3.floatValue = IntField(m_Value3);
				if (HasMin(m_Flag.intValue))
				{
					m_Value0.floatValue = Max(m_Value0, m_MinValue0);
					m_Value1.floatValue = Max(m_Value1, m_MinValue1);
					m_Value2.floatValue = Max(m_Value2, m_MinValue2);
					m_Value3.floatValue = Max(m_Value3, m_MinValue3);
				}
				if (HasMax(m_Flag.intValue))
				{
					m_Value0.floatValue = Min(m_Value0, m_MaxValue0);
					m_Value1.floatValue = Min(m_Value1, m_MaxValue1);
					m_Value2.floatValue = Min(m_Value2, m_MaxValue2);
					m_Value3.floatValue = Min(m_Value3, m_MaxValue3);
				}
				EditorGUILayout.EndHorizontal();
			}
			break;
		}
		return attr;
	}
}
