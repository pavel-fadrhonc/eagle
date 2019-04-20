using UnityEngine;

namespace DefaultNamespace
{
    public class PhysicsMovement2D : MonoBehaviour
    {
        public Vector2 moveVector;

        private Rigidbody2D _rb;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            _rb.MovePosition((Vector2) transform.position + (moveVector * Time.fixedDeltaTime));
        }
    }
}