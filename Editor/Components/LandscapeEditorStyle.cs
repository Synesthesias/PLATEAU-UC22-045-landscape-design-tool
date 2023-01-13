﻿using UnityEditor;
using UnityEngine;

namespace LandscapeDesignTool.Editor
{
    public static class LandscapeEditorStyle
    {
        private static readonly GUIStyle styleLabelRichText = new GUIStyle(EditorStyles.label)
        {
            richText = true
        };
        
        public static void Header(string text)
        {
            EditorGUILayout.LabelField($"<size=15>{text}</size>", styleLabelRichText);
        }
    }
}