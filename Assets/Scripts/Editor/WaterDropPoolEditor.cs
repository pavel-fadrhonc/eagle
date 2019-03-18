using UnityEditor;
using UnityEngine;

namespace Eagle.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(WaterDropPool))]
    public class WaterDropPoolEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var gen = GUILayout.Button("Generate");
            if (gen)
            {
                var pool = target as WaterDropPool;
                pool.ClearWaterDrops();
                pool.Generate();
            }
            
        }
    }
}