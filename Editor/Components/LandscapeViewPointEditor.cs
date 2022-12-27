using UnityEditor;
using UnityEngine;

namespace LandscapeDesignTool.Editor
{
    [CustomEditor(typeof(LandscapeViewPoint))]
    public class LandScapeViewPointEditor : UnityEditor.Editor
    {
        public static LandScapeViewPointEditor Active;

        public LandscapeViewPoint Target => target as LandscapeViewPoint;
        
        public override void OnInspectorGUI()
        {
            Active = this;

            Target.gameObject.name = EditorGUILayout.TextField("���_�ꖼ", Target.gameObject.name);

            Target.Fov = EditorGUILayout.FloatField("����p", Target.Fov);
            Target.Camera.fieldOfView = Target.Fov;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
