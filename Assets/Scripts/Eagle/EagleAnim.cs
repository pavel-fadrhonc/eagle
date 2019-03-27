using System.Collections.Generic;
using UnityEngine;

namespace EagleProject
{
    public class EagleAnim : MonoBehaviour
    {
        public struct FrameSet
        {
            public FrameSet(int startFrame, int endFrame)
            {
                this.startFrame = startFrame;
                this.endFrame = endFrame;
            }
            
            public int startFrame;
            public int endFrame;

            public static FrameSet Lerp(FrameSet fs1, FrameSet fs2, float t)
            {
                return new FrameSet((int) Mathf.Lerp(fs1.startFrame, fs2.startFrame, t),
                    (int) Mathf.Lerp(fs1.endFrame, fs2.endFrame, t));
            }
        }
        
        
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