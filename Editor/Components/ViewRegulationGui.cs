﻿using PLATEAU.CityGML;
using UnityEditor;
using UnityEngine;
using Material = UnityEngine.Material;
using Object = UnityEngine.Object;

namespace LandscapeDesignTool.Editor
{
    public class ViewRegulationGUI
    {
        int selectIndex = 0;
        bool selectingTarget = false;
        GameObject vpgroup;
        Color _areaColor = new Color(0, 1, 0, 0.2f);
        Color _areaInvalidColor = new Color(1, 0, 0, 0.2f);
        private float _wsize;
        private float _hsize;
        float _interval = 3.0f;
        private Vector3 _prevPosition;
        private const string ObjNameLineOfSight = "LineOfSight";
        private const string ObjNameCoveringMesh = "CoveringMesh";

        public ViewRegulationGUI(ViewRegulation target)
        {
            _wsize = target.screenWidth;
            _hsize = target.screenHeight;
            _prevPosition = target.transform.position;
        }

        public void Draw(ViewRegulation target)
        {
            var style = new GUIStyle(EditorStyles.label);
            style.richText = true;
            // this.serializedObject.Update();

            SceneView sceneView = SceneView.lastActiveSceneView;

            EditorGUILayout.HelpBox("視点場を選択して眺望対象をシーン内で選択してください", MessageType.Info);
            _wsize = EditorGUILayout.FloatField("眺望対象での横サイズ(m)", _wsize);
            _hsize = EditorGUILayout.FloatField("眺望対象での縦サイズ(m)", _hsize);
            _areaColor = EditorGUILayout.ColorField("色の設定", _areaColor);
            _areaInvalidColor = EditorGUILayout.ColorField("規制色の設定", _areaInvalidColor);
            _interval = EditorGUILayout.FloatField("障害物の判定間隔(m)", _interval);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<size=12>視点場</size>", style);

            vpgroup = Object.FindObjectOfType<LandscapeViewPointGroup>().gameObject;

            if (vpgroup == null || vpgroup.transform.childCount == 0)
            {
                EditorGUILayout.HelpBox("視点場を作成してください", MessageType.Error);
                return;
            }

            string[] options = new string[vpgroup.transform.childCount];
            for (int i = 0; i < vpgroup.transform.childCount; i++)
            {
                LandscapeViewPoint vp = vpgroup.transform.GetChild(i).GetComponent<LandscapeViewPoint>();
                options[i] = vp.Name;
            }
            selectIndex = EditorGUILayout.Popup(selectIndex, options);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<size=12>眺望対象</size>", style);
            GUI.color = selectingTarget == false
                ? Color.white
                : Color.green;
            if (GUILayout.Button("眺望対象の選択"))
            {
                sceneView.Focus();
                selectingTarget = true;
            }
        }

        public enum SurfaceType
        {
            Opaque,
            Transparent
        }

        private Vector3 targetPosition;

        public void OnSceneGUI(ViewRegulation target)
        {
            EditorGUI.BeginChangeCheck();
            var trans = target.transform;
            var posStart = trans.position;
            Vector3 posEnd = Handles.PositionHandle(targetPosition, Quaternion.identity);
            float lineLength = (posEnd - posStart).magnitude;
            
            Handles.Label(posStart, "視線 始点");
            if ((posStart - _prevPosition).sqrMagnitude > 0.00001)
            {
                CreateViewRegulation(target, posStart, posEnd, lineLength);
            }
            
            Handles.Label(targetPosition, "視線 終点");

            if (EditorGUI.EndChangeCheck())
            {
                if (targetPosition != posEnd)
                    CreateViewRegulation(target, posStart, posEnd, lineLength);
                targetPosition = posEnd;
            }

            if(!selectingTarget) return;

            var ev = Event.current;

            RaycastHit hit;
            if (ev.type == EventType.KeyUp && ev.keyCode == KeyCode.LeftShift)
            {

                Transform origin = vpgroup.transform.GetChild(selectIndex);
                Vector3 originPoint = origin.position;
                Vector3 mousePosition = Event.current.mousePosition;


                Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Debug.Log(hit.collider.name);
                    Debug.Log(hit.collider.bounds.center);

                    Vector3 targetPoint = hit.collider.bounds.center;

                    selectingTarget = false;
                    float length = Vector3.Distance(originPoint, targetPoint);

                    CreateViewRegulation(target, originPoint, targetPoint, length);
                }
                ev.Use();
            }

            _prevPosition = posStart;
        }

        void CreateViewRegulation(ViewRegulation targetViewRegulation, Vector3 originPoint, Vector3 targetPoint, float length)
        {
            CreateCoveringMesh(targetViewRegulation, originPoint, targetPosition, length);
            CreateLineOfSight(targetViewRegulation, originPoint, targetPosition);
        }

        /// <summary>
        /// 選択判定のためのメッシュを生成します。
        /// </summary>
        private void CreateCoveringMesh(ViewRegulation target, Vector3 originPoint, Vector3 targetPoint, float length)
        {
            // 以前の CoveringMesh があれば削除します。
            var trans = target.transform;
            for (int i = 0; i < trans.childCount; i++)
            {
                var child = trans.GetChild(i);
                if (child.name == ObjNameCoveringMesh)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
            
            Vector3[] vertex = new Vector3[6];
            vertex[0] = new Vector3(0, 0, 0);
            vertex[1] = new Vector3(-_wsize / 2.0f, -_hsize / 2.0f, length);
            vertex[2] = new Vector3(-_wsize / 2.0f, _hsize / 2.0f, length);
            vertex[3] = new Vector3(_wsize / 2.0f, _hsize / 2.0f, length);
            vertex[4] = new Vector3(_wsize / 2.0f, -_hsize / 2.0f, length);
            vertex[5] = new Vector3(0, 0, length);

            int[] idx = {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 1,
                5, 2, 1,
                5, 3, 2,
                5, 4, 3,
                5, 1, 4 };

            GameObject go = new GameObject(ObjNameCoveringMesh);
            go.layer = LayerMask.NameToLayer("RegulationArea");

            var mf = go.GetComponent<MeshFilter>();
            if (mf == null)
                mf = go.AddComponent<MeshFilter>();

            var mesh = new Mesh();
            mesh.vertices = vertex;
            mesh.triangles = idx;

            var mr = go.GetComponent<MeshRenderer>();
            if (mr == null)
                mr = go.AddComponent<MeshRenderer>();

            Material material = LDTTools.MakeMaterial(_areaColor);

            mr.sharedMaterial = material;
            mf.mesh = mesh;

            go.transform.position = originPoint;
            go.transform.LookAt(targetPoint, Vector3.up);
            go.transform.parent = ((ViewRegulation)target).transform;
            mr.enabled = false;
        }

        void ClearLineOfSight(ViewRegulation target)
        {
            var root = (target).transform;
            for (int i = 0; i < root.childCount; ++i)
            {
                var trans = root.GetChild(i);
                string childName = trans.name;
                if (childName == ObjNameLineOfSight)
                {
                    Object.DestroyImmediate(trans.gameObject);
                }
            }
        }

        float CreateLineOfSight(ViewRegulation target, Vector3 origin, Vector3 destination)
        {
            ClearLineOfSight(target);

            var obj = new GameObject(ObjNameLineOfSight);
            obj.transform.parent = ((ViewRegulation)target).transform;

            float result = -1;

            int divx = (int)(_wsize / _interval);
            int divy = (int)(_hsize / _interval);

            for (int i = 0; i < divx + 1; i++)
            {
                for (int j = 0; j < divy + 1; j++)
                {
                    float x = destination.x - (_wsize / 2.0f) + _interval * i;
                    float y = destination.y - (_hsize / 2.0f) + _interval * j;
                    Vector3 d = new Vector3(x, y, destination.z);
                    RaycastHit hit;

                    if (RaycastBuildings(target, origin, d, out hit))
                    {
                        DrawLine(origin, hit.point, obj, _areaColor);
                        DrawLine(hit.point, d, obj, _areaInvalidColor);
                    }
                    else
                    {
                        DrawLine(origin, d, obj, _areaColor);
                    }
                }
            }

            return result;
        }

        void DrawLine(Vector3 origin, Vector3 distination, GameObject parent, Color col)
        {
            Vector3[] point = new Vector3[2];
            point[0] = origin;
            point[1] = distination;

            GameObject go = new GameObject("ViewRegurationAreaByLine");
            go.layer = LayerMask.NameToLayer("RegulationArea");

            LineRenderer lineRenderer = go.AddComponent<LineRenderer>();

            lineRenderer.SetPositions(point);
            lineRenderer.positionCount = point.Length;
            lineRenderer.startWidth = 1.0f;
            lineRenderer.endWidth = 1.0f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

            lineRenderer.startColor = col;
            lineRenderer.endColor = col;

            go.transform.parent = parent.transform;
        }

        bool RaycastBuildings(ViewRegulation target, Vector3 origin, Vector3 destination, out RaycastHit hitInfo)
        {
            bool result = false;

            hitInfo = new RaycastHit();

            Vector3 direction = (destination - origin).normalized;

            RaycastHit[] hits;
            hits = Physics.RaycastAll(origin, direction, 10000);

            float minDistance = float.MaxValue;
            if (hits.Length <= 0)
                return result;

            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.name == target.name)
                    continue;

                int layerIgnoreRaycast = LayerMask.NameToLayer("RegulationArea");

                if (hit.collider.gameObject.layer == layerIgnoreRaycast)
                    continue;

                result = true;

                if (hit.distance >= minDistance)
                    continue;

                hitInfo = hit;
                minDistance = hit.distance;
            }

            return result;
        }
    }
}