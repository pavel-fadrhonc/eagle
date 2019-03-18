using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Random = UnityEngine.Random;

public class WaterDropPool : SceneSingleton<WaterDropPool>
{
    [Tooltip("number of waterdrops in pool")]
    public int numDrops;

    public bool generateOnStart;
    
    public WaterColorDrop waterDropPrefab;

    [HideInInspector] [SerializeField] private List<WaterColorDrop> _waterDropPool = new List<WaterColorDrop>();
    [HideInInspector] [SerializeField] private List<WaterColorDrop> _activeWaterDrops = new List<WaterColorDrop>();
    [HideInInspector] [SerializeField] private List<WaterColorDrop> _inactiveWaterDrops = new List<WaterColorDrop>();

    private void Start()
    {
        if (generateOnStart)
        {
            ClearWaterDrops();
            Generate();
        }
    }

    public WaterColorDrop Get()
    {
        if (_inactiveWaterDrops.Count == 0)
        {
            Debug.LogError("No more free water drops in pool!");
            return null;
        }

        var retDrop = _inactiveWaterDrops[Random.Range(0, _inactiveWaterDrops.Count)];
        _inactiveWaterDrops.Remove(retDrop);
        _activeWaterDrops.Add(retDrop);
        retDrop.gameObject.SetActive(true);

        return retDrop;
    }

    public void Return(WaterColorDrop drop)
    {
        _activeWaterDrops.Remove(drop);

        if (!_inactiveWaterDrops.Contains(drop))
            _inactiveWaterDrops.Add(drop);

        drop.gameObject.SetActive(false);
    }

    public void Generate()
    {
        StartCoroutine(GenerateCor());
    }

    public void ClearWaterDrops()
    {
        foreach (var drop in _waterDropPool)
        {
            if (Application.isEditor)
                DestroyImmediate(drop.gameObject);
            else
                Destroy(drop.gameObject);
        }
        
        _waterDropPool.Clear();
        _inactiveWaterDrops.Clear();
        _activeWaterDrops.Clear();
    }
    
    private IEnumerator GenerateCor()
    {
        for (int i = 0; i < numDrops; i++)
        {
            var dropGO = Instantiate(waterDropPrefab.gameObject, transform);
            dropGO.name = "WaterDrop_" + i;
            dropGO.transform.SetParent(transform);
            var colorDrop = dropGO.GetComponent<WaterColorDrop>();
            _waterDropPool.Add(colorDrop);
            _inactiveWaterDrops.Add(colorDrop);

            colorDrop.Regenerate();
        }

        yield return null;

        foreach (var waterColorDrop in _waterDropPool)
        {
            waterColorDrop.gameObject.SetActive(false);
        }
    }
}
