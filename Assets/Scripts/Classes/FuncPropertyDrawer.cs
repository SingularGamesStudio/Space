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
    //TODO
    enum FuncType {
        Null,
        Linear,
        Sum,
        Coord,
		Perlin,
    }
    private bool functionsInitialized = false;
    private void InitializeFunctions()
    {
        if (functionsInitialized)
            return;
        functionsInitialized = true;
		
		//TODO
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        Initialize(property);
        InitializeFunctions();
        FuncType oldtype;
        FuncType type;
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
            ForceInitialize(property);
        }
		if (property.isExpanded) {
            
			EditorGUI.PropertyField(position, property, true);
		}
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		//TODO:fix this, now returns too much
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
