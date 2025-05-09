using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Watermelon
{
    public static class LevelEditorUtils
    {
        public static IEnumerable<SerializedProperty> GetLevelEditorProperies(SerializedObject serializedObject)
        {
            Type targetType = serializedObject.targetObject.GetType();

            IEnumerable<FieldInfo> fieldInfos = targetType.GetFields(ReflectionUtils.FLAGS_INSTANCE).Where(x => x.GetCustomAttribute<LevelEditorSetting>() != null);

            foreach (var field in fieldInfos)
            {
                SerializedProperty serializedProperty = serializedObject.FindProperty(field.Name);
                if (serializedProperty != null)
                    yield return serializedProperty;
            }
        }

        public static IEnumerable<SerializedProperty> GetLevelEditorProperies(SerializedProperty serializedProperty)
        {
            if (serializedProperty.propertyType == SerializedPropertyType.Generic)
            {
                Type targetType = serializedProperty.boxedValue.GetType();
                IEnumerable<FieldInfo> fieldInfos = targetType.GetFields(ReflectionUtils.FLAGS_INSTANCE).Where(x => x.GetCustomAttribute<LevelEditorSetting>() != null);
                foreach (var field in fieldInfos)
                {
                    SerializedProperty subProperty = serializedProperty.FindPropertyRelative(field.Name);
                    if (subProperty != null)
                        yield return subProperty;
                }
            }
        }

        public static IEnumerable<SerializedProperty> GetUnmarkedProperties(SerializedObject serializedObject)
        {
            Type targetType = serializedObject.targetObject.GetType();

            IEnumerable<FieldInfo> fieldInfos = targetType.GetFields(ReflectionUtils.FLAGS_INSTANCE).Where(x => x.GetCustomAttribute<LevelEditorSetting>() == null);

            foreach (var field in fieldInfos)
            {
                SerializedProperty serializedProperty = serializedObject.FindProperty(field.Name);
                if (serializedProperty != null)
                    yield return serializedProperty;
            }
        }

        public static IEnumerable<SerializedProperty> GetUnmarkedProperties(SerializedProperty serializedProperty)
        {
            if (serializedProperty.propertyType == SerializedPropertyType.Generic)
            {
                Type targetType = serializedProperty.boxedValue.GetType();
                IEnumerable<FieldInfo> fieldInfos = targetType.GetFields(ReflectionUtils.FLAGS_INSTANCE).Where(x => x.GetCustomAttribute<LevelEditorSetting>() == null);
                foreach (var field in fieldInfos)
                {
                    SerializedProperty subProperty = serializedProperty.FindPropertyRelative(field.Name);
                    if (subProperty != null)
                        yield return subProperty;
                }
            }
        }

    }
}
