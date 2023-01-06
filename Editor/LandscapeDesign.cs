using LandscapeDesignTool.Editor.WindowTabs;
using UnityEngine;
using UnityEditor;


namespace LandscapeDesignTool.Editor
{
#if UNITY_EDITOR
    public class LandscapeDesign : EditorWindow
    {

        private readonly string[] _tabToggles =
            { "���_��쐬", "�K���G���A�쐬", "���]�K���쐬", "�����K���G���A�쐬", "ShapeFile�Ǎ�", "ShapeFile�����o��" };

        private int _tabIndex;
        private readonly TabViewPointGenerate _tabViewPointGenerate;
        private readonly TabRegulationAreaGenerate _tabRegulationAreaGenerate;
        private readonly TabShapefileLoad _tabShapefileLoad = new TabShapefileLoad();
        private readonly TabHeightRegulationGenerate _tabHeightRegulationGenerate = new TabHeightRegulationGenerate();
        private readonly TabRegulationAreaExport _tabRegulationAreaExport = new TabRegulationAreaExport();

        private readonly TabViewportRegulationGenerate _tabViewportRegulationGenerate =
            new TabViewportRegulationGenerate();

        public LandscapeDesign()
        {
            _tabViewPointGenerate = new TabViewPointGenerate(this);
            _tabRegulationAreaGenerate = new TabRegulationAreaGenerate(this);
        }
        private void Awake()
        {

            LDTTools.SetUI();
        }


        [MenuItem("PLATEAU/�i�ς܂��Â���/�i�όv��")]
        public static void ShowWindow()
        {
            TagAdder.ConfigureTags();
            EditorWindow.GetWindow(typeof(LandscapeDesign), true, "�i�όv����");
        }

        void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            switch (_tabIndex)
            {
                case 0:
                    //_tabViewPointGenerate.OnSceneGUI();
                    break;
                case 1:
                    _tabRegulationAreaGenerate.OnSceneGUI();
                    break;
                case 2:
                    _tabViewportRegulationGenerate.OnSceneGUI();
                    break;
                case 3:
                    _tabHeightRegulationGenerate.OnSceneGUI();
                    break;
            }
        }

        private void Update()
        {
            _tabViewPointGenerate.Update();
        }

        private void OnGUI()
        {
            var style = new GUIStyle(EditorStyles.label);
            style.richText = true;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                _tabIndex = GUILayout.Toolbar(_tabIndex, _tabToggles, new GUIStyle(EditorStyles.toolbarButton),
                    GUI.ToolbarButtonSize.FitToContents);
            }

            switch (_tabIndex)
            {
                case 0:
                    _tabViewPointGenerate.Draw(style);
                    break;
                case 1:
                    _tabRegulationAreaGenerate.Draw(style);
                    break;
                case 2:
                    _tabViewportRegulationGenerate.Draw(style);
                    break;
                case 3:
                    _tabHeightRegulationGenerate.Draw(style);
                    break;
                case 4:
                    _tabShapefileLoad.Draw(style);
                    break;
                case 5:
                    _tabRegulationAreaExport.Draw(style);
                    break;
            }
        }
    }
}


#endif