﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LandscapeDesignTool.Editor.WindowTabs
{
    public class TabRegulationAreaGenerate
    {
        bool _isCreatingContour = false;

        private int selectedIndex;

        RegulationArea regulationArea;

        private EditorWindow _parentWindow;
        private GUIStyle _labelStyle;
        private Vector2 _scrollPosition = Vector2.zero;

        public TabRegulationAreaGenerate(EditorWindow parentWindow)
        {
            _parentWindow = parentWindow;
        }

        public void Draw(GUIStyle labelStyle)
        {
            LDTTools.CheckTag("RegulationArea");

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            _labelStyle = labelStyle;

            DrawCreateRegulation();
            DrawEditRegulationArea();
            DrawDeleteRegulationArea();

            EditorGUILayout.EndScrollView();
        }

        private void DrawCreateRegulation()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("<size=15>規制エリア作成</size>", _labelStyle);

            GUI.enabled = true;

            if (_isCreatingContour)
            {
                DrawCreateContourButton();
                return;
            }
            
            GUI.color = Color.white;
            if (GUILayout.Button("新規規制エリア作成..."))
            {
                _isCreatingContour = true;
                GameObject go = new GameObject();
                go.layer = LayerMask.NameToLayer("RegulationArea");
                go.name = LDTTools.GetNumberWithTag("RegulationArea", "規制エリア");
                go.tag = "RegulationArea";
                Selection.activeObject = go;
                regulationArea = go.AddComponent<RegulationArea>();
                regulationArea.SetAreaColor(new Color(0.8f, 0.3f, 0.3f, 0.4f));
                _parentWindow.Repaint();
            }
        }

        private void DrawCreateContourButton()
        {
            RegulationAreaEditor.Active.IsEditMode = true;
            EditorGUILayout.HelpBox("地面をクリックして頂点を追加してください。", MessageType.Info);

            GUILayout.BeginHorizontal();

            GUI.color = Color.green;
            if (GUILayout.Button("完了"))
            {
                _isCreatingContour = false;

                if (regulationArea.Vertices.Count > 2)
                {
                    regulationArea.GenMesh();
                    RegulationAreaEditor.Active.IsEditMode = false;
                }
                else
                {
                    Object.DestroyImmediate(regulationArea.gameObject);
                    RegulationAreaEditor.Active = null;
                }

                _parentWindow.Repaint();
            }

            GUI.color = Color.white;
            if (GUILayout.Button("取り消し"))
            {
                _isCreatingContour = false;

                Object.DestroyImmediate(regulationArea.gameObject);
                RegulationAreaEditor.Active = null;

                _parentWindow.Repaint();
            }

            GUILayout.EndHorizontal();
        }

        private void DrawEditRegulationArea()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<size=15>規制エリア編集</size>", _labelStyle);

            DrawRegurationAreaList();

            if (RegulationAreaEditor.Active == null)
                return;

            EditorGUILayout.Space();

            RegulationAreaEditor.Active.OnInspectorGUI();
        }

        void DrawRegurationAreaList()
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("RegulationArea");

            if (objects.Length == 0)
                return;

            var popupElements = new List<string>();
            for (int i = 0; i < objects.Length; ++i)
            {
                popupElements.Add(objects[i].name);

                // アクティブオブジェクトが外部から変更された際(ヒエラルキーからオブジェクトを選択した際)に表示を更新
                if (Selection.activeGameObject == objects[i])
                    selectedIndex = i;
            }

            if (selectedIndex >= popupElements.Count)
                selectedIndex = popupElements.Count - 1;

            var boldStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold
            };
            EditorGUILayout.LabelField("編集対象", boldStyle);

            selectedIndex = EditorGUILayout.Popup("", selectedIndex, popupElements.ToArray());

            Selection.activeGameObject = objects[selectedIndex];
        }

        private void DrawDeleteRegulationArea()
        {
            if (RegulationAreaEditor.Active == null)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<size=15>規制エリア削除</size>", _labelStyle);

            GUI.color = Color.red;
            if (GUILayout.Button("選択中の規制エリアを削除"))
            {
                Object.DestroyImmediate(RegulationAreaEditor.Active.Target.gameObject);
                selectedIndex = Mathf.Max(selectedIndex - 1, 0);
            }

            GUI.color = Color.white;
        }

        public void OnSceneGUI()
        {
            if (!_isCreatingContour)
                return;

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            // クリック時に頂点生成を行う
            var ev = Event.current;
            if (!(ev.type == EventType.MouseDown && !ev.shift && !ev.control && !ev.alt))
                return;

            RaycastHit[] hits;
            int layerMask = 1 << 31;
            Vector3 mousePosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

            hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);
            if (hits == null)
                return;

            regulationArea.AddVertex(hits[0].point);
        }
    }
}