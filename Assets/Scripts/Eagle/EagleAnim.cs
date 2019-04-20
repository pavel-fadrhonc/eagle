using System;
using System.Collections.Generic;
using UnityEngine;

namespace EagleProject
{
    public class EagleAnim : MonoBehaviour
    {
        [Serializable]
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

        public enum AnimLoopType
        {
            PingPong,
            Clamp,
            Loop
        }
        
        public List<Sprite> frames;
        public int fps;

        private float _fpsScale;
        private float _deltaTime;
        private int _frameDirection;
        private AnimLoopType _animLoopType;

        private int _currentSpriteIdx;
        private float _nextFrameTime;
        private SpriteRenderer _spriteRenderer;
        private FrameSet _currentFrameSet;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            _deltaTime = 1.0f / fps;
        }

        private void Update()
        {
            if (Time.time > _nextFrameTime)
            {
                _spriteRenderer.sprite = frames[_currentSpriteIdx % frames.Count];
                _nextFrameTime += _deltaTime;

                if (_frameDirection < 0)
                {
                    if (_currentSpriteIdx > _currentFrameSet.startFrame)
                        _currentSpriteIdx--;
                    //_animLoopType == AnimLoopType.PingPong
                    else if (_currentSpriteIdx < _currentFrameSet.startFrame )
                    {
                        _currentSpriteIdx++;
                        _frameDirection = 1;                        
                    }
                    else if (_currentSpriteIdx == _currentFrameSet.startFrame)
                    {
                        if (_animLoopType == AnimLoopType.PingPong)
                        {
                            if (_currentSpriteIdx < _currentFrameSet.endFrame)
                            {
                                _currentSpriteIdx++;
                                _frameDirection = 1;                                                                                    
                            }
                        }                            
                        else if (_animLoopType == AnimLoopType.Loop)
                        {
                            _currentSpriteIdx = _currentFrameSet.endFrame;
                        }
                        // if clamp or startframe == endfame, we don't change the frame
                    }
                }
                else
                {
                    if (_currentSpriteIdx < _currentFrameSet.endFrame)
                        _currentSpriteIdx++;
                    else if (_currentSpriteIdx > _currentFrameSet.startFrame )
                    {
                        _currentSpriteIdx--;
                        _frameDirection = -1;                        
                    }
                    else if (_currentSpriteIdx == _currentFrameSet.endFrame)
                    {
                        if (_animLoopType == AnimLoopType.PingPong)
                        {
                            if (_currentSpriteIdx > _currentFrameSet.startFrame)
                            {
                                _currentSpriteIdx--;
                                _frameDirection = -1;                                                                                    
                            }
                        }                            
                        else if (_animLoopType == AnimLoopType.Loop)
                        {
                            _currentSpriteIdx = _currentFrameSet.startFrame;
                        }
                        // if clamp or startframe == endfame, we don't change the frame
                    }
                }
            }
        }

        public void SetFpsScale(float scale)
        {
            _fpsScale = scale;
            _deltaTime = 1.0f / (fps * scale);
        }
        
        public void SetFrameSet(FrameSet fs)
        {
            _currentFrameSet = fs;
        }

        public void SetAnimType(AnimLoopType at)
        {
            _animLoopType = at;
        }
    }
}