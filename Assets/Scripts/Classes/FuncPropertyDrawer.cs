using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Func))]
public class FuncPropertyDrawer : PropertyDrawer
{
    enum FuncType {
        Null,
        Linear,
        Sum,
    }
    FuncType oldtype = FuncType.Null;
    FuncType type = FuncType.Null;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		object Inst = GetPropertyInstance(property);
		if (Inst != null) {
			oldtype = (FuncType)Enum.Parse(typeof(FuncType), Inst.GetType().ToString());
			property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
		} else {
			oldtype = FuncType.Null;
			property.isExpanded = false;
		}
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        type = (FuncType)EditorGUI.EnumPopup(position, oldtype);
        if (type != oldtype) {
            if (type == FuncType.Null) {
                property.managedReferenceValue = null;
            }
            else {
                System.Type t = System.Type.GetType(type.ToString());
                property.managedReferenceValue = System.Activator.CreateInstance(t);
            }
        }
		if(Inst!=null)
		((Func)Inst).PropertySize = EditorGUIUtility.singleLineHeight;
		if (property.isExpanded) {
			EditorGUI.indentLevel++;
			Rect pos1 = position;
			pos1.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(pos1, property, label, true);
			((Func)Inst).PropertySize +=base.GetPropertyHeight(property, label);
			EditorGUI.indentLevel--;
		}
	}
	public System.Object GetPropertyInstance(SerializedProperty property)
	{

		string path = property.propertyPath;

		System.Object obj = property.serializedObject.targetObject;
		var type = obj.GetType();

		var fieldNames = path.Split('.');
		for (int i = 0; i < fieldNames.Length; i++) {
			var info = type.GetField(fieldNames[i]);
			if (info == null)
				break;

			// Recurse down to the next nested object.
			obj = info.GetValue(obj);
			type = info.FieldType;
		}

		return obj;
	}
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		object Inst = GetPropertyInstance(property);
		if (type==FuncType.Null)
            return base.GetPropertyHeight(property, label);
		Debug.Log(((Func)Inst).PropertySize);
		return ((Func)Inst).PropertySize;
	}
}
