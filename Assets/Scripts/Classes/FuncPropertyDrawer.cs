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
        EditorGUI.PropertyField(position, property, label, true);
        EditorGUI.PropertyField(position, property, label, true);
        Debug.Log(base.fieldInfo);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        type = (FuncType)EditorGUI.EnumPopup(position, type);
        if (type != oldtype) {
            if (type == FuncType.Null) {
                property.managedReferenceValue = null;
            }
            else {
                System.Type t = System.Type.GetType(type.ToString());
                property.managedReferenceValue = System.Activator.CreateInstance(t);
            }
        }
        oldtype = type;
        //property.managedReferenceValue = new Linear();
        /*if ( == null) {
            var amountRect = new Rect(position.x, position.y, 30, position.height);
            EditorGUI.TextField(amountRect, "test");
        }*/
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if(type==FuncType.Null)
            return base.GetPropertyHeight(property, label);
        return 200;
    }
}
