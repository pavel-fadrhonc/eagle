using UnityEngine;
using UnityEngine.Serialization;

namespace EagleProject
{
    public class EagleMovement : MonoBehaviour
    {
        [Tooltip("0 padding - the center of eagle can be exactly at the end of screen, 0.1 - 10% off etc.")]
        public float screenPadding;

        public float speedScale;
        
        public Vector2 defaultMovementSpeed;
        public Vector2 speedBoostVertical;
        public Vector2 speedBoostHorizontal;

        private Vector2 _currentMovementSpeed;

        private void Awake()
        {
            _currentMovementSpeed = defaultMovementSpeed;
        }

        private void Update()
        {
            transform.Translate(_currentMovementSpeed * Time.deltaTime, Space.World);
        }

        public void LerpMovementSpeed(Vector2 t)
        {
            _currentMovementSpeed = new Vector2()
            {
                x = Mathf.Lerp(defaultMovementSpeed.x, 
                    t.x < 0 ? defaultMovementSpeed.x + speedBoostHorizontal.x : defaultMovementSpeed.x + speedBoostHorizontal.y, 
                    Mathf.Abs(t.x)) * speedScale,
                y = Mathf.Lerp(defaultMovementSpeed.y,
                    t.y < 0 ? defaultMovementSpeed.y + speedBoostVertical.x : defaultMovementSpeed.y + speedBoostVertical.y,
                    Mathf.Abs(t.y)) * speedScale,
            };
        }
    }
}