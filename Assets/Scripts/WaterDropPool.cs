using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterDropPool : SceneSingleton<WaterDropPool>
{
    [Tooltip("number of waterdrops in pool")]
    public int numDrops;

    public WaterColorDrop waterDropPrefab;

    private List<WaterColorDrop> m_WaterDropPool = new List<WaterColorDrop>();
    private List<WaterColorDrop> m_ActiveWaterDrops = new List<WaterColorDrop>();
    private List<WaterColorDrop> m_InactiveWaterDrops = new List<WaterColorDrop>();

    private void Start()
    {
        Debug.Log("starting generating the water drops");
        var startTime = Time.time;

        StartCoroutine(Generate());

        Debug.Log(numDrops + " drops generated in " + (Time.time - startTime) + " seconds");
    }

    public WaterColorDrop Get()
    {
        if (m_InactiveWaterDrops.Count == 0)
        {
            Debug.LogError("No more free water drops in pool!");
            return null;
        }

        var retDrop = m_InactiveWaterDrops[Random.Range(0, m_InactiveWaterDrops.Count)];
        m_InactiveWaterDrops.Remove(retDrop);
        m_ActiveWaterDrops.Add(retDrop);
        retDrop.gameObject.SetActive(true);

        return retDrop;
    }

    public void Return(WaterColorDrop drop)
    {
        m_ActiveWaterDrops.Remove(drop);

        if (!m_InactiveWaterDrops.Contains(drop))
            m_InactiveWaterDrops.Add(drop);

        drop.gameObject.SetActive(false);
    }

    private IEnumerator Generate()
    {
        for (int i = 0; i < numDrops; i++)
        {
            var dropGO = Instantiate(waterDropPrefab.gameObject, transform);
            dropGO.name = "WaterDrop_" + i;
            dropGO.transform.SetParent(transform);
            var colorDrop = dropGO.GetComponent<WaterColorDrop>();
            m_WaterDropPool.Add(colorDrop);
            m_InactiveWaterDrops.Add(colorDrop);

            colorDrop.Regenerate();
        }

        yield return null;

        foreach (var waterColorDrop in m_WaterDropPool)
        {
            waterColorDrop.gameObject.SetActive(false);
        }
    }
}
