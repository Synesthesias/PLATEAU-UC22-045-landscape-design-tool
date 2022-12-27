﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LandscapeDesignTool.Editor.WindowTabs
{
    public class TabRegulationAreaExport
    {
        string _regulationAreaExportPath = "";

        public void Draw(GUIStyle labelStyle)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<size=15>ShapeFile出力</size>", labelStyle);

            string[] options = { "規制エリア", "高さ規制エリア", "眺望規制エリア" };

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField("エクスポート先", _regulationAreaExportPath);
            }

            if (GUILayout.Button("エクスポート先選択"))
            {
                var selectedPath = EditorUtility.SaveFilePanel("保存先", "", "Shapefile", "shp");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    _regulationAreaExportPath = selectedPath;
                }
            }

            if (GUILayout.Button("規制エリア出力"))
            {
                List<List<Vector2>> contours = new List<List<Vector2>>();

                GameObject[] objects = GameObject.FindGameObjectsWithTag("RegulationArea");
                int objCount = objects.Length;
                string[] types = new string[objCount];
                Color[] cols = new Color[objCount];
                float[] heights = new float[objCount];
                Vector2[,] v2 = new Vector2[objCount, 2];
                for (int i = 0; i < objCount; i++)
                {
                    if (objects[i].GetComponent<RegulationArea>())
                    {
                        List<Vector2> p = new List<Vector2>();
                        RegulationArea obj =
                            objects[i].GetComponent<RegulationArea>();
                        types[i] = "PolygonArea";
                        heights[i] = obj.GetHeight();
                        cols[i] = obj.GetAreaColor();
                        v2[i, 0] = new Vector2(0, 0);
                        v2[i, 1] = new Vector2(0, 0);

                        List<Vector2> cnt = obj.GetVertex2D();
                        contours.Add(cnt);
                    }
                }

                LDTTools.WriteShapeFile(_regulationAreaExportPath, "RegurationArea", types, cols, heights, v2,
                    contours);
            }
        }
    }
}