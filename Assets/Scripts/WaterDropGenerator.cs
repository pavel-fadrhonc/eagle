using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Superbest_random;
using Random = UnityEngine.Random;

/// <summary>
/// Generates some amount of water drop in viewport in some interval
/// </summary>
public class WaterDropGenerator : MonoBehaviour
{
    public List<Color> colorPool = new List<Color>();
    public Vector2 sizeSpan;
    public Vector2 intervalSpan;

    private List<Tuple<WaterColorDrop, WaterColorDropAnimation>> m_activeDropAnims = new List<Tuple<WaterColorDrop, WaterColorDropAnimation>>();

    // Use this for initialization
    void Start()
    {
        m_nextSpawnTime = Random.Range(intervalSpan.x, intervalSpan.y);

        m_Random = new System.Random(Random.Range(0,10000));
    }

    private float m_nextSpawnTime;
    void Update()
    {
        if (Time.time > m_nextSpawnTime)
        {
            Spawn();
            m_nextSpawnTime = m_nextSpawnTime + Random.Range(intervalSpan.x, intervalSpan.y);
        }

        CheckForInactiveDrops();
    }

    private void CheckForInactiveDrops()
    {
        foreach (var waterColorDrop in m_activeDropAnims)
        {
            var rend = waterColorDrop.Item1.Layers[0].GetComponent<Renderer>();
            if (!rend.isVisible && !waterColorDrop.Item2.AnimRunning)
            {
                WaterDropPool.Instance.Return(waterColorDrop.Item1);
            }
        }
    }

    private System.Random m_Random;
    private void Spawn()
    {
        var waterDrop = WaterDropPool.Instance.Get();

        if (waterDrop == null)
        {
            // darn, out of water drops, try next time...
            return;
        }

        m_activeDropAnims.Add(new Tuple<WaterColorDrop, WaterColorDropAnimation>(waterDrop, waterDrop.gameObject.GetComponent<WaterColorDropAnimation>()));

        // find a position
        var randX = (float) (m_Random.NextGaussian() + 3) / 6f;
        var randY = (float) (m_Random.NextGaussian() + 3) / 6f;

        var worldPos = Camera.main.ViewportToWorldPoint(new Vector3(randX, randY, 0));
        worldPos.z = 0;

        // get color
        var color = colorPool[Random.Range(0, colorPool.Count)];

        // get a size
        var size = Random.Range(sizeSpan.x, sizeSpan.y);

        waterDrop.transform.position = worldPos;
        waterDrop.Color = color;
        var anim = waterDrop.GetComponent<WaterColorDropAnimation>();

        anim.maxScale = size;
        //anim.AnimationFinishedEvent += OnAnimationEventFinished;
        anim.Animate();
    }

    private void OnAnimationEventFinished(WaterColorDropAnimation anim)
    {
        anim.AnimationFinishedEvent -= OnAnimationEventFinished;
        WaterDropPool.Instance.Return(anim.gameObject.GetComponent<WaterColorDrop>());
    }
}
