using System;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Serialization;

namespace EagleProject
{
    [Serializable]
    public struct EagleSettings
    {
        public EagleAnim.FrameSet FrameSet;
        [Tooltip("In Radians")]
        public float Angle;
        public float FpsScale;
        [FormerlySerializedAs("AnimType")] public EagleAnim.AnimLoopType animLoopType;

        public static EagleSettings Lerp(EagleSettings es1, EagleSettings es2, float t)
        {
            return new EagleSettings()
            {
                FrameSet = EagleAnim.FrameSet.Lerp(es1.FrameSet, es2.FrameSet, t),
                Angle = Mathf.Lerp(es1.Angle, es2.Angle, t),
                FpsScale = Mathf.Lerp(es1.FpsScale, es2.FpsScale, t),
                animLoopType = t > 0.5 ? es2.animLoopType : es1.animLoopType
            };
        }
    }
    
    public class EagleControl : MonoBehaviour
    {
        [Header("X == 1 control values")]
        public EagleSettings Settings_X_POS1 =new EagleSettings()
        {
            FrameSet = new EagleAnim.FrameSet()
            {
                endFrame = 1,
                startFrame = 12
            },
            Angle = 0f,
            FpsScale = 1.2f,
            animLoopType = EagleAnim.AnimLoopType.Loop
        };
     
        [Space]        
        [Header("X == -1 control values")]
        public EagleSettings Settings_X_NEG1 =new EagleSettings()
        {
            FrameSet = new EagleAnim.FrameSet()
            {
                endFrame = 6,
                startFrame = 12
            },
            Angle = 30f,
            FpsScale = 1.5f,
            animLoopType = EagleAnim.AnimLoopType.Clamp
        };        
        
        [Space]
        [Header("Y == 1 control values")]
        public EagleSettings Settings_Y_POS1 =new EagleSettings()
        {
            FrameSet = new EagleAnim.FrameSet()
            {
                endFrame = 1,
                startFrame = 6
            },
            Angle = 0f,
            FpsScale = 1.5f,
            animLoopType = EagleAnim.AnimLoopType.PingPong
        };
        
        [Space]
        [Header("Y == -1 control values")]
        public EagleSettings Settings_Y_NEG1 = new EagleSettings()
        {
            FrameSet = new EagleAnim.FrameSet()
            {
                endFrame = 5,
                startFrame = 5
            },
            Angle = -30f,
            FpsScale = 1f,
            animLoopType = EagleAnim.AnimLoopType.Clamp
        };
        
        [Space]
        [Header("X == 0, Y == 0 control values")]
        public EagleSettings Settings_Y_0_X_0 = new EagleSettings()
        {
            FrameSet = new EagleAnim.FrameSet()
            {
                endFrame = 5,
                startFrame = 12
            },
            Angle = 0f,
            FpsScale = 0.7f,
            animLoopType = EagleAnim.AnimLoopType.PingPong
        };

        private EagleAnim _eagleAnim;
        private EagleMovement _eagleMovement;
        private Vector2 _inputCircular;
        
        private void Awake()
        {
            _eagleAnim = GetComponent<EagleAnim>();
            _eagleMovement = GetComponent<EagleMovement>();
            
            _eagleAnim.SetFrameSet(Settings_Y_0_X_0.FrameSet);
        }

        private void Update()
        {
            // get input
            var input = new Vector2(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")
            );

            // map square input to circle, to maintain uniform speed in all directions
            _inputCircular = new Vector2(
                input.x * Mathf.Sqrt(1 - input.y * input.y * 0.5f),
                input.y * Mathf.Sqrt(1 - input.x * input.x * 0.5f)
            );

            EagleAnim.FrameSet currentFrameSet;
            EagleAnim.AnimLoopType currentAnimLoopType = EagleAnim.AnimLoopType.PingPong;
            float currentFpsScale;
            float currentAngle = 0;

            EagleSettings currentSettings = Settings_Y_0_X_0;
            float tx = _inputCircular.x;
            float ty = _inputCircular.y;
            
            // split it into quadrants
            //    |    2    |    1    |
            //    |_________|_________|
            //    |         |         |
            //    |    3    |    4    |
            float t;
            if (tx > 0)
            {
                if (ty > 0)
                { // 1
                    t = Vector2.Angle(Vector2.right, _inputCircular) / 90f;
                    currentSettings = EagleSettings.Lerp(Settings_X_POS1, Settings_Y_POS1, t);
                }
                else
                { // 4
                    t = Vector2.Angle(Vector2.down, _inputCircular) / 90f;
                    currentSettings = EagleSettings.Lerp(Settings_Y_NEG1, Settings_X_POS1, t);
                }
            }
            else
            {
                if (ty > 0)
                { // 2
                    t = Vector2.Angle(Vector2.up, _inputCircular) / 90f;
                    currentSettings = EagleSettings.Lerp(Settings_Y_POS1, Settings_X_NEG1, t);
                }
                else
                { // 3
                    t = Vector2.Angle(Vector2.left, _inputCircular) / 90f;
                    currentSettings = EagleSettings.Lerp(Settings_X_NEG1, Settings_Y_NEG1, t);
                }                
            }
            
            currentSettings = EagleSettings.Lerp(Settings_Y_0_X_0, currentSettings, _inputCircular.magnitude);
            
//            // lerp on x first, then on Y, but in practise it doesn't matter
//            if (input.x < 0)
//            {
//                currentFrameSet = EagleAnim.FrameSet.Lerp(Settings_Y_0_X_0.FrameSet, Settings_X_NEG1.FrameSet, tx);
//                currentFpsScale = Mathf.Lerp(Settings_Y_0_X_0.FpsScale, Settings_X_NEG1.FpsScale,tx);
//                currentAngle = Mathf.Lerp(Settings_Y_0_X_0.Angle, Settings_X_NEG1.Angle, tx);
//            }
//            else
//            {
//                currentFrameSet = EagleAnim.FrameSet.Lerp(Settings_Y_0_X_0.FrameSet, Settings_X_POS1.FrameSet, tx);
//                currentFpsScale = Mathf.Lerp(Settings_Y_0_X_0.FpsScale, Settings_X_POS1.FpsScale,tx);
//                currentAngle = Mathf.Lerp(Settings_Y_0_X_0.Angle, Settings_X_POS1.Angle, tx);
//            }            
//            if (input.y < 0)
//            {
//                currentFrameSet = EagleAnim.FrameSet.Lerp(currentFrameSet, Settings_Y_NEG1.FrameSet, ty);
//                currentFpsScale = Mathf.Lerp(currentFpsScale, Settings_Y_NEG1.FpsScale,ty);
//                currentAngle = Mathf.Lerp(currentAngle, Settings_Y_NEG1.Angle, ty);
//                //currentAnimLoopType = Settings_Y_NEG1.animLoopType;
//            }
//            else
//            {
//                currentFrameSet = EagleAnim.FrameSet.Lerp(currentFrameSet, Settings_Y_POS1.FrameSet, ty);
//                currentFpsScale = Mathf.Lerp(currentFpsScale, Settings_Y_POS1.FpsScale,ty);
//                currentAngle = Mathf.Lerp(currentAngle, Settings_Y_POS1.Angle, ty);
//                //currentAnimLoopType = Settings_Y_POS1.animLoopType;
//            }
            
            _eagleAnim.SetFrameSet(currentSettings.FrameSet);
            _eagleAnim.SetAnimType(currentSettings.animLoopType);
            _eagleAnim.SetFpsScale(currentSettings.FpsScale);
            _eagleMovement.LerpMovementSpeed(_inputCircular);
            transform.rotation = Quaternion.Euler(0,0, currentSettings.Angle);
        }
    }
}