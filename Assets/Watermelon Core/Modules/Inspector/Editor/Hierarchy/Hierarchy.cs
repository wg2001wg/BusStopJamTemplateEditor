using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;
using static System.Linq.Expressions.Expression;

namespace Watermelon
{
    public class Hierarchy
    {
        private static Type sceneHierarchyWindowType;
        private static Type sceneHierarchyType;
        private static Type treeViewControllerType;
        private static Type treeViewGUIType;

        private static PropertyInfo sceneHierarchyProperty;
        private static FieldInfo hierarchyTreeViewField;
        private static PropertyInfo treeViewGUIProperty;

        private static FieldInfo iconWidthField;
        private static FieldInfo iconSpaceField;

        private EditorWindow window;

        private object sceneHierarchy;
        private object treeViewController;
        private object treeViewGUI;

        private readonly float defaultIconWidth;
        private readonly float defaultSpaceBeforeIcon;

        private static PropertyInfo lastInteractedHierarchyWindow;
        private static Func<object> getLastInteractedHierarchyWindow;
        private static Dictionary<object, Hierarchy> hierarchies = new Dictionary<object, Hierarchy>();

        [InitializeOnLoadMethod]
        private static void PrepareData()
        {
            sceneHierarchyWindowType = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            sceneHierarchyProperty = sceneHierarchyWindowType.GetProperty("sceneHierarchy");

            sceneHierarchyType = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchy");
            hierarchyTreeViewField = sceneHierarchyType.GetField("m_TreeView", BindingFlags.NonPublic | BindingFlags.Instance);

            treeViewControllerType = typeof(TreeViewState).Assembly.GetType("UnityEditor.IMGUI.Controls.TreeViewController");
            treeViewGUIProperty = treeViewControllerType.GetProperty("gui");

            treeViewGUIType = typeof(UnityEditor.IMGUI.Controls.TreeView).Assembly.GetType("UnityEditor.IMGUI.Controls.TreeViewGUI");

            iconWidthField = treeViewGUIType.GetField("k_IconWidth");
            iconSpaceField = treeViewGUIType.GetField("k_SpaceBetweenIconAndText");

            lastInteractedHierarchyWindow = sceneHierarchyWindowType.GetProperty("lastInteractedHierarchyWindow", BindingFlags.Public | BindingFlags.Static);
            getLastInteractedHierarchyWindow = Lambda<Func<object>>(Property(null, lastInteractedHierarchyWindow)).Compile();
        }

        public Hierarchy(EditorWindow window)
        {
            this.window = window;

            sceneHierarchy = sceneHierarchyProperty.GetValue(window);
            treeViewController = hierarchyTreeViewField.GetValue(sceneHierarchy);
            treeViewGUI = treeViewGUIProperty.GetValue(treeViewController);

            defaultIconWidth = (float)iconWidthField.GetValue(treeViewGUI);
            defaultSpaceBeforeIcon = (float)iconSpaceField.GetValue(treeViewGUI);

            SetIconWidth(0, 18);
        }

        private void SetIconWidth(float iconWidth, float spaceBeforeIcon)
        {
            iconWidthField.SetValue(treeViewGUI, iconWidth);
            iconSpaceField.SetValue(treeViewGUI, spaceBeforeIcon);
        }

        private void ResetIconWidth()
        {
            SetIconWidth(defaultIconWidth, defaultSpaceBeforeIcon);
        }

        public void DrawElementGUI(int instanceID, Rect selectionRect)
        {
            GameObject instanceObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (!instanceObject) return;

            Texture texture = EditorCustomHierarchy.GetTexture(instanceObject);

            if (texture == null) return;
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                ResetIconWidth();

                return;
            }

            SetIconWidth(0, 18);

            Rect iconRect = new Rect(selectionRect) { width = 16, height = 16 };
            iconRect.y += (iconRect.height - 16) / 2;

            using(new ColorScope(EditorCustomStyles.HIERARCHY_COLOR))
            {
                GUI.DrawTexture(iconRect, texture);
            }
        }

        public static Hierarchy GetLastHierarchy()
        {
            object lastHierarchyWindow = getLastInteractedHierarchyWindow();

            if (!hierarchies.TryGetValue(lastHierarchyWindow, out var hierarchy))
            {
                hierarchy = new Hierarchy(lastHierarchyWindow as EditorWindow);
                hierarchies.Add(lastHierarchyWindow, hierarchy);
            }

            return hierarchy;
        }

        public static void ClearHierarchies()
        {
            if (hierarchies.Count == 0) return;

            foreach(Hierarchy hierarchy in hierarchies.Values)
            {
                if(hierarchy != null)
                {
                    hierarchy.ResetIconWidth();
                }
            }

            hierarchies.Clear();
        }
    }
}
