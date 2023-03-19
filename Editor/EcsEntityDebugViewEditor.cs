using System.Collections.Generic;
using UnityEditor;

namespace Lekret.Ecs.Editor
{
    [CustomEditor(typeof(EcsEntityDebugView))]
    public class EcsEntityDebugViewEditor : UnityEditor.Editor
    {
        private EcsEntityDebugView _target;
        private readonly List<object> _componentsBuffer = new();

        private void OnEnable()
        {
            _target = (EcsEntityDebugView) target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField($"{_target.Entity}");

            var entity = _target.Entity;
            entity.GetAll(_componentsBuffer);
            foreach (var component in _componentsBuffer)
            {
                EditorGUILayout.LabelField($"Component: {component.GetType()}");
            }
        }
    }
}