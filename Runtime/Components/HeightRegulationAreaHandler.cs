using UnityEngine;
using UnityEditor;

namespace LandscapeDesignTool
{
    public class HeightRegulationAreaHandler : MonoBehaviour
    {
        [SerializeField] float areaHeight = 10;
        [SerializeField] float areaDiameter = 10;
        [SerializeField] Vector3 targetPoint= Vector3.zero;
        [SerializeField] Color areaColor;
        
        /// <summary>
        /// 高さ規制は円柱で表示されますが、その円柱の高さです。
        /// この円柱の高さのうち、規制の高さ分が地面の上に出て、残りは地面の下に埋まります。
        /// 広い範囲を指定しても円柱が浮かないように長めにします。
        /// </summary>
        private const float heightRegulationDisplayLength = 3000f;
        

        // Update is called once per frame
        void Update()
        {

        }
        private void OnDrawGizmosSelected()
        {

            Gizmos.color = Color.blue;

            int n = 0;
            float size = 50f;

            Vector3 v0 = new Vector3(targetPoint.x, targetPoint.y, targetPoint.z);
            Gizmos.DrawCube(v0, new Vector3(10, 10, 10));
        }

        public void SetHeight( float h)
        {
            areaHeight = h;
        }
        public float GetHeight()
        {
            return areaHeight;
        }
        public void SetDiameter(float r)
        {
            areaDiameter = r;
        }
        public float GetDiameter()
        {
            return areaDiameter;
        }
        public void SetPoint( Vector3 p)
        {
            targetPoint = new Vector3(p.x, p.y, p.z);
        }
        public Vector3 GetPoint()
        {
            return targetPoint;
        }
        public void SetColor(Color c)
        {
            areaColor = c;
        }
        public Color GetColor()
        {
            return areaColor;
        }


#if UNITY_EDITOR
            [CustomEditor(typeof(HeightRegulationAreaHandler))]
        [CanEditMultipleObjects]
        public class HeightRegurationAreaEditor : Editor
        {

            Color _areaColor = new Color(0, 1, 1, 0.5f);
            float _height;
            bool _pointing = false;
            float _radius = 10;
            private void Awake()
            {
                _height = Selection.activeGameObject.GetComponent<HeightRegulationAreaHandler>().areaHeight;
                _radius = Selection.activeGameObject.GetComponent<HeightRegulationAreaHandler>().areaDiameter;
            }

            public override void OnInspectorGUI()
            {
                var regulation = (HeightRegulationAreaHandler)target;
                EditorGUILayout.LabelField($"高さ: {regulation.areaHeight}");
                EditorGUILayout.LabelField($"直径: {regulation.areaDiameter}");
            }
        }
        
        public static void SetupRegulationArea(HeightRegulationAreaHandler regulationArea, float diameter, Color color, float height)
        {
            // Unityのデフォルト円柱は高さが2mであることに注意
            // regulationArea.transform.localScale = new Vector3(_heightAreaRadius, _heightAreaHeight / 2f, _heightAreaRadius);
            regulationArea.transform.localScale =
                new Vector3(diameter, heightRegulationDisplayLength / 2f, diameter);
            
            regulationArea.SetColor(color);
            regulationArea.SetHeight(height);
            regulationArea.SetDiameter(diameter);

            var targetPoint = regulationArea.GetPoint();
            regulationArea.transform.position = new Vector3(targetPoint.x, targetPoint.y - heightRegulationDisplayLength / 2f + height, targetPoint.z);
            Material mat = LDTTools.MakeMaterial(color);
            regulationArea.GetComponent<Renderer>().material = mat;
        }
#endif
    }
}
