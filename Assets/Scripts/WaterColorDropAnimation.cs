using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Superbest_random;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaterColorDropAnimation : MonoBehaviour
{
    public struct LayerAnimData
    {
        public float duration;
        public float normTime;

        public float maxScale;
        public float startAngle;
        public float targetAngle;
    }

    public event Action<WaterColorDropAnimation> AnimationFinishedEvent;

    public float animDuration;
    public float maxScale = 4f;
    
    [Tooltip("Random deviation in percent for scale, lower bound")]
    public float scaleVariationMin = 0.1f;
    [Tooltip("Random deviation in percent for scale, upper bound")]
    public float scaleVariationMax = 0.1f;    
    
    [Tooltip("Random deviation in percent for duration, lower bound")]
    public float durationVariationMin = 0.1f;
    [Tooltip("Random deviation in percent for duration, upper bound")]
    public float durationVariationMax = 0.3f;
    

    [Tooltip("In degrees")]
    public float rotationAngleVarMin = -5f;
    public float rotationAngleVarMax = 5f;

    public AnimationCurve scaleCurve;

    public bool animate;

    private WaterColorDrop _waterColorDrop;

    public bool AnimRunning { get; set; }

    private void Awake()
    {
        _waterColorDrop = GetComponent<WaterColorDrop>();
    }

    private void Update()
    {
        if (animate)
        {
            Animate();
            animate = false;
        }
    }

    public void Animate()
    {
        StartCoroutine(AnimateCoroutine());
    }
    
    private IEnumerator AnimateCoroutine()
    {
        float animTime = 0f;
        var layers = _waterColorDrop.Layers;
        var layerAnimData = new List<LayerAnimData>();
        var gaussRandom = new System.Random(UnityEngine.Random.Range(0, 10000));

        AnimRunning = true;
        //
        for (int i = 0; i < layers.Count; i++)
        {
            //var variation = UnityEngine.Random.Range(variationMin, variationMax);
            var variationDuration = Random.Range(0f,1f) * (durationVariationMax - durationVariationMin) + durationVariationMin;
            var variationScale = Random.Range(0f,1f) * (durationVariationMax - durationVariationMin) + durationVariationMin;
            var targetRandAngle = Random.Range(0f,1f) * (rotationAngleVarMax - rotationAngleVarMin) + rotationAngleVarMin;

            layerAnimData.Add(new LayerAnimData()
            {
                duration = animDuration * (1 - variationDuration),
                maxScale = maxScale * (1 - variationScale),
                startAngle = 0,
                targetAngle = (float) targetRandAngle
            });

            layers[i].transform.localScale = Vector3.zero;
            layers[i].transform.localRotation = Quaternion.identity;            
        }

        var layersActive = true;
        while (layersActive)
        {
            animTime += Time.deltaTime;

            for (int layerIdx = 0; layerIdx < layers.Count; layerIdx++)
            {
                var layer = layers[layerIdx];
                var layerData = layerAnimData[layerIdx];

                layersActive = false;
                if (animTime < layerData.duration)
                {
                    layerData.normTime = animTime / layerData.duration;
                    var scale = scaleCurve.Evaluate(layerData.normTime) * layerData.maxScale;
                    layer.transform.localScale = new Vector3(scale, scale, scale);
                    layer.transform.localRotation = Quaternion.Euler(0,0, Mathf.Lerp(layerData.startAngle, layerData.targetAngle,
                        layerData.normTime)) ;

                    layersActive = true;
                }
            }

            yield return null;
        }

        AnimationFinishedEvent?.Invoke(this);
        AnimRunning = false;
    }
}
