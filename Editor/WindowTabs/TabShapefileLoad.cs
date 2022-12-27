﻿using UnityEditor;
using UnityEngine;

namespace LandscapeDesignTool.Editor.WindowTabs
{
    /// <summary>
    /// Shapefile読込のタブを描画します。
    /// </summary>
    public class TabShapefileLoad
    {
        private readonly ShapeFileEditorHelper _shapeFileEditorHelper = new ShapeFileEditorHelper();

        public void Draw(GUIStyle labelStyle)
        {
            _shapeFileEditorHelper.DrawGui();
        }
    }
}