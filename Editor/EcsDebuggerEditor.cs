using UnityEditor;

namespace ECS.Editor
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
            EditorGUILayout.LabelField($"Entities count: {_target.World.EntitiesCount}");
        }
    }
}