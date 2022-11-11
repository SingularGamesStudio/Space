using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Func))]
public class FuncPropertyDrawer : PropertyDrawer
{
    enum FuncType {
        Null,
        Linear,
        Sum,
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        FuncType oldtype;
        FuncType type;
        Func Inst = (Func)GetPropertyInstance(property);
        Rect ArrowPosition = new Rect(position.min.x, position.min.y+3, EditorGUIUtility.labelWidth, 15);
        if (Inst != null) {
			oldtype = (FuncType)Enum.Parse(typeof(FuncType), Inst.GetType().ToString());
            property.isExpanded = EditorGUI.Foldout(ArrowPosition, property.isExpanded, label, true);
		} else {
			oldtype = FuncType.Null;
            EditorGUI.LabelField(ArrowPosition, label);
            property.isExpanded = false;
		}
		Rect MenuPosition = new Rect(position.min.x + EditorGUIUtility.labelWidth, position.min.y, position.width-EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        type = (FuncType)EditorGUI.EnumPopup(MenuPosition, oldtype);
        if (type != oldtype) {
            if (type == FuncType.Null) {
                property.managedReferenceValue = null;
            }
            else {
                Type t = Type.GetType(type.ToString());
                property.managedReferenceValue = Activator.CreateInstance(t);
            }
        }
		if (property.isExpanded) {
            
			EditorGUI.PropertyField(position, property, true);
            /*if (Inst.argCnt > 0) {
                EditorGUI.indentLevel++;
                SerializedProperty arg1 = property.FindPropertyRelative("arg1");
                Debug.Log(arg1.type+" "+property.type);
                Rect Arg1Position = new Rect(position.min.x, position.min.y + EditorGUI.GetPropertyHeight(property), position.width, EditorGUI.GetPropertyHeight(arg1));
                EditorGUI.PropertyField(Arg1Position, arg1, true);
                if (Inst.argCnt > 1) {
                    SerializedProperty arg2 = property.FindPropertyRelative("arg2");
                    Rect Arg2Position = new Rect(position.min.x, position.min.y + EditorGUI.GetPropertyHeight(property)+ EditorGUI.GetPropertyHeight(arg1),
                        position.width, EditorGUI.GetPropertyHeight(arg2));
                    EditorGUI.PropertyField(Arg2Position, arg2, true);
                }
                EditorGUI.indentLevel--;
            }*/
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
		
		if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;
        float sum = EditorGUI.GetPropertyHeight(property);
        try {
            Func Inst = (Func)GetPropertyInstance(property);
            if (Inst.argCnt > 0) {
                sum += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("arg1"));
                if (Inst.argCnt > 1) {
                    sum += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("arg2"));
                }
            }
        }
        catch (Exception) {
            //TODO: Fix error on startup
        }
        return sum;
    }
}
