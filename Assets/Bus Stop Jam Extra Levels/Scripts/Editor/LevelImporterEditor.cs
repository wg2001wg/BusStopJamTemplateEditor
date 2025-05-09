using UnityEngine;
using UnityEditor;

namespace Watermelon.BusStop
{
    [CustomEditor(typeof(LevelsImporter))]
    public class LevelImporterEditor : Editor
    {
        private const string LEVELS_TO_IMPORT_PROPERTY_NAME = "levelsToImport";
        private const string LEVEL_DATABASE_PROPERTY_NAME = "levelDatabase";
        private const string LEVELS_PROPERTY_NAME = "levels";
        private SerializedProperty levelsToImportProperty;
        private SerializedProperty levelDatabaseProperty;
        private string directory;
        private bool displayDirectory;

        public void OnEnable()
        {
            levelsToImportProperty = serializedObject.FindProperty(LEVELS_TO_IMPORT_PROPERTY_NAME);
            levelDatabaseProperty = serializedObject.FindProperty(LEVEL_DATABASE_PROPERTY_NAME);

            if (levelDatabaseProperty.objectReferenceValue == null)
            {
                levelDatabaseProperty.objectReferenceValue = EditorUtils.GetAsset<LevelDatabase>();
            }

            GetDirectory();
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(levelsToImportProperty);

            if (levelDatabaseProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Levels Database is not found. Please assign the database below. New levels will be imported in this database.", MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("New levels will be imported in the database below.", MessageType.Info);

            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(levelDatabaseProperty);

            if (EditorGUI.EndChangeCheck())
            {
                GetDirectory();
            }

            if (displayDirectory)
            {
                EditorGUILayout.HelpBox("Levels will be moved to the folder below.", MessageType.Info);
                directory = EditorGUILayout.TextField("Directory", directory);
            }

            EditorGUI.BeginDisabledGroup(levelDatabaseProperty.objectReferenceValue == null);

            if (GUILayout.Button("ImportLevels"))
            {
                ImportLevels();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        private void GetDirectory()
        {
            displayDirectory = false;

            if (levelDatabaseProperty.objectReferenceValue == null)
            {
                return;
            }

            SerializedObject levelDatabaseSerializedObject = new SerializedObject(levelDatabaseProperty.objectReferenceValue);
            SerializedProperty levelsProperty = levelDatabaseSerializedObject.FindProperty(LEVELS_PROPERTY_NAME);

            if (levelsProperty.arraySize == 0)
            {
                return;
            }

            string fullPath = AssetDatabase.GetAssetPath(levelsProperty.GetArrayElementAtIndex(0).objectReferenceValue);
            directory = fullPath.Substring(0, fullPath.LastIndexOf('/'));
            displayDirectory = true;
        }

        private void ImportLevels()
        {
            SerializedObject levelDatabaseSerializedObject = new SerializedObject(levelDatabaseProperty.objectReferenceValue);
            SerializedProperty levelsProperty = levelDatabaseSerializedObject.FindProperty(LEVELS_PROPERTY_NAME);
            int newLevelIndex;
            GUID newLevelGUID;
            string moveResult;
            string oldPath;
            string newPath;
            int lastIndex;

            for (int i = 0; i < levelsToImportProperty.arraySize; i++)
            {
                newLevelGUID = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(levelsToImportProperty.GetArrayElementAtIndex(i).objectReferenceValue));

                for (int j = 0; j < levelsProperty.arraySize; j++)
                {
                    if (AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(levelsProperty.GetArrayElementAtIndex(j).objectReferenceValue)) == newLevelGUID)
                    {
                        EditorUtility.DisplayDialog("Info", "Levels already integrated into the project. Operation canceled.", "Ok");
                        return;
                    }
                }
            }


            for (int i = 0; i < levelsToImportProperty.arraySize; i++)
            {
                newLevelIndex = levelsProperty.arraySize;
                levelsProperty.arraySize++;
                levelsProperty.GetArrayElementAtIndex(newLevelIndex).objectReferenceValue = levelsToImportProperty.GetArrayElementAtIndex(i).objectReferenceValue;

                if (displayDirectory)
                {
                    oldPath = AssetDatabase.GetAssetPath(levelsProperty.GetArrayElementAtIndex(newLevelIndex).objectReferenceValue);
                    lastIndex = oldPath.LastIndexOf('/');
                    newPath = directory + oldPath.Substring(lastIndex, oldPath.Length - lastIndex);
                    moveResult = AssetDatabase.ValidateMoveAsset(oldPath, newPath);

                    if (moveResult == "")
                    {
                        AssetDatabase.MoveAsset(oldPath, newPath);
                    }
                    else
                    {
                        Debug.LogError($"Couldn't move {oldPath} because {moveResult}");
                    }
                }
            }

            levelDatabaseSerializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Info", "Levels successfully integrated into the project!", "Ok");
        }
    }
}