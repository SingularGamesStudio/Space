using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Func))]
public class FuncPropertyDrawer : NestablePropertyDrawer
{
    enum FuncType {
        Null,
        Linear,
        Sum,
        Coord,
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        FuncType oldtype;
        FuncType type;
        //Debug.Log(GetPropertyInstance(property));
        Func Inst = (Func)propertyObject;
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
            Initialize(property);
        }
		if (property.isExpanded) {
            
			EditorGUI.PropertyField(position, property, true);
		}
    }
    /*public System.Object GetPropertyInstance(SerializedProperty property)
	{

		string path = property.propertyPath;

		System.Object obj = property.serializedObject.targetObject;
		var type = obj.GetType();

		var fieldNames = path.Split('.');
        Debug.Log("Path: "+path);
        Debug.Log(type);
		for (int i = 0; i < fieldNames.Length; i++) {
			var info = type.GetField(fieldNames[i]);
			if (info == null)
				break;

			// Recurse down to the next nested object.
			obj = info.GetValue(obj);
            type = info.FieldType;
            Debug.Log(obj.GetType() +" "+ obj.GetType().IsArray);
            if (obj.GetType().IsArray) {
                var index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
                obj = ((System.Object[])obj)[index];
            }
            
            Debug.Log(type);
        }

		return obj;
	}*/

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		
		if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;
        float sum = EditorGUI.GetPropertyHeight(property);
        try {
            Func Inst = (Func)propertyObject;
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
