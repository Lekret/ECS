using UnityEditor;

namespace Lekret.Ecs.Editor
{
    [CustomEditor(typeof(EcsDebugger))]
    public class EcsDebuggerEditor : UnityEditor.Editor
    {
        private EcsDebugger _target;

        private void OnEnable()
        {
            _target = (EcsDebugger) target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField($"Entity count: {_target.Manager.EntitiesCount}");
        }
    }
}