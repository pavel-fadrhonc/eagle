using System.Collections.Generic;
using UnityEngine;

namespace EagleProject
{
    public class EagleAnim : MonoBehaviour
    {
        public List<Sprite> frames;
        public int fps;

        private float _deltaTime;

        private int _currentSpriteIdx;
        private float _nextFrameTime;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            _deltaTime = 1.0f / fps;
        }

        private void Update()
        {
            if (Time.time > _nextFrameTime)
            {
                _spriteRenderer.sprite = frames[_currentSpriteIdx++ % frames.Count];
                _nextFrameTime += _deltaTime;
            }
        }
    }
}