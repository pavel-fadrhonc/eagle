using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class Windzone : MonoBehaviour
    {
        public AnimationCurve forceCurve;
        public float maxForce;
        public Vector2 windDirectionVector;
        public float roundDuration;

        private List<Rigidbody2D> _rbs  = new List<Rigidbody2D>();

        private float _roundTime;

        private void OnTriggerEnter2D(Collider2D other)
        {
            var rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                _rbs.Add(rb);                
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                _rbs.Remove(rb);                
            }
        }

        private void FixedUpdate()
        {
            _roundTime = Mathf.Repeat(Time.time, roundDuration);
            
            foreach (var rb in _rbs)
            {
                rb.AddForce(windDirectionVector.normalized * Time.deltaTime * maxForce * forceCurve.Evaluate(_roundTime / roundDuration));
            }
        }
    }
    
    #if UNITY_EDITOR

    public class WindZoneEditor : Editor
    {
        private void OnSceneGUI()
        {
            var targetCol = (target as Windzone).GetComponent<Collider>();
            //targetCol.bounds.
            
            Handles.color = Color.red;
            var colCenter = targetCol.bounds.center;
            var extent = targetCol.bounds.extents;
            var A = colCenter - extent;
            var B = colCenter - new Vector3(extent.x, -extent.y, extent.z);
            var C = colCenter - new Vector3(-extent.x, -extent.y, extent.z);
            var D = colCenter - new Vector3(-extent.x, extent.y, extent.z);
                
            Handles.DrawLine(A, B);
            Handles.DrawLine(B, C);
            Handles.DrawLine(C, D);
            Handles.DrawLine(D, A);
            
        }
    }
    
    #endif
}