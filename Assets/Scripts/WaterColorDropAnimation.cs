using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Superbest_random;
using UnityEngine;

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

    public float animDuration;
    public float maxScale = 4f;

    public float variationMin;
    public float variationMax;

    [Tooltip("In degrees")]
    public float rotationAngleVarMin = -5f;
    public float rotationAngleVarMax = 5f;

    public AnimationCurve scaleCurve;

    public bool animate;

    [Header("Obsolete")]
    [Tooltip("How much variation in duration can there be. min: all layers will have same duration [0,1] max: layers can have 0-duration length")]
    public float durationVariationMin = 0.1f;
    public float durationVariationMax = 0.3f;

    public float scaleVariationMin = 0.1f;
    public float scaleVariationMax = 0.1f;

    private WaterColorDrop _waterColorDrop;

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

        for (int i = 0; i < layers.Count; i++)
        {
            //var variation = UnityEngine.Random.Range(variationMin, variationMax);
            var variation = (float) (gaussRandom.NextGaussian() + 1f) * 0.5f * (variationMax - variationMin) + variationMin;
            var targetRandAngle = (gaussRandom.NextGaussian() + 1f) * 0.5f * (rotationAngleVarMax - rotationAngleVarMin) + rotationAngleVarMin;

            layerAnimData.Add(new LayerAnimData()
            {
                duration = animDuration * (1 - variation),
                maxScale = maxScale * (1 - variation),
                startAngle = layers[i].transform.localRotation.eulerAngles.z,
                targetAngle = (float) targetRandAngle
            });

            layers[i].transform.localScale = Vector3.zero;
            layers[i].transform.localRotation = Quaternion.identity;            
        }

        while (animTime < animDuration)
        {
            animTime += Time.deltaTime;

            var layersActive = false;

            for (int layerIdx = 0; layerIdx < layers.Count; layerIdx++)
            {
                var layer = layers[layerIdx];
                var layerData = layerAnimData[layerIdx];

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

            if (!layersActive)
                yield break;

            yield return null;
        }
    }
}
