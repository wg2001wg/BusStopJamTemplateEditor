﻿using UnityEngine;
using UnityEditor;

namespace Watermelon
{
    [CustomPropertyDrawer(typeof(UIScaleAnimation))]
    public class UIScaleAnimationPropertyDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float x = position.x;
            float y = position.y;
            float width = (position.width - EditorGUIUtility.labelWidth);
            float height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PrefixLabel(new Rect(x, y, EditorGUIUtility.labelWidth, height), label);
            EditorGUI.PropertyField(new Rect(x + EditorGUIUtility.labelWidth, y, width, height), property.FindPropertyRelative("transform"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}
