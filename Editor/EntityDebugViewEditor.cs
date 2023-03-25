using System.Collections.Generic;
using UnityEditor;

namespace Lekret.Ecs.Editor
{
    [CustomEditor(typeof(EntityDebugView))]
    public class EntityDebugViewEditor : UnityEditor.Editor
    {
        private EntityDebugView _target;
        private readonly List<object> _componentsBuffer = new();

        private void OnEnable()
        {
            _target = (EntityDebugView) target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField($"{_target.Entity}");

            var entity = _target.Entity;
            entity.GetAll(_componentsBuffer);
            foreach (var component in _componentsBuffer)
            {
                EditorGUILayout.LabelField(component?.ToString() ?? "Null");
            }
        }
    }
}