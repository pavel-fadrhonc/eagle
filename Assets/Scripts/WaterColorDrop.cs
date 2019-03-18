using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Superbest_random;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class WaterColorDrop : MonoBehaviour
{
    [Tooltip("how much vertices should the circle be divided into")]
    public int startVerticesCount;
    public float circleRadius = 1f;

    public int numDeformationsInitial = 2;
    [Tooltip("Applied on top of initial.")]
    public int numDeformationsVariation = 3;

    public int numLayers = 5;

    public Material usedMaterial;

    [SerializeField]
    private Color m_Color;

    public Color Color
    {
        get { return m_Color; }
        set
        {
            m_Color = value;
            foreach (var layer in Layers) 
            {
                var rend = layer.GetComponent<MeshRenderer>();
                rend.material.color = value;
            }
        }
    }

    public float gauss_um_initial = 0f;
    public float gauss_sigma_initial = 1f;

    public float gauss_um_addition = 0f;
    public float gauss_sigma_additional = 1f;

    public bool interleaveLayers = false;
    public int interleaveLayersStep = 5;

    public bool regenerate = false;

    public List<GameObject> Layers
    {
        get { return _layers; }
    }
    
    [HideInInspector] [SerializeField] private List<GameObject> _layers = new List<GameObject>();

    public void Update()
    {
        if (regenerate)
        {       
            Regenerate();
            regenerate = false;
        }
    }

    private float _regenDuration;
    private float _deformateDuration;
    public void Regenerate()
    {
        Stopwatch regenStopWatch = new Stopwatch();
        regenStopWatch.Start();
        
        Stopwatch deformatePolyStopWatch = new Stopwatch();
        
        // Create Vector2 vertices
        Vector2[] vertices2D = new Vector2[startVerticesCount];

        float angleStep = (Mathf.PI * 2) / startVerticesCount;
        for (int i = 0; i < startVerticesCount; i++)
        {
            float x = Mathf.Sin(i * angleStep) * circleRadius;
            float y = Mathf.Cos(i * angleStep) * circleRadius;

            vertices2D[i] = new Vector2(x, y);
        }

        deformatePolyStopWatch.Start();
        for (int defIdx = 0; defIdx < numDeformationsInitial; defIdx++)
        {
            vertices2D = DeformatePolygon(vertices2D, gauss_um_initial, gauss_sigma_initial);
        }
        deformatePolyStopWatch.Stop();
        
        for (int layerIdx = 0; layerIdx < numLayers; layerIdx++)
        {
            GameObject layerGo;
            MeshRenderer layerMeshRend;
            MeshFilter layerMeshFilter;
            if (layerIdx >= _layers.Count)
            {
                layerGo = new GameObject($"layer {layerIdx}");
                layerMeshRend = layerGo.AddComponent<MeshRenderer>();
                layerMeshFilter = layerGo.AddComponent<MeshFilter>();
                layerGo.transform.SetParent(transform);
                layerGo.transform.localPosition = Vector3.zero;
                layerMeshRend.sortingOrder = layerIdx / interleaveLayersStep;
                _layers.Add(layerGo);
            }
            else
            {
                layerGo = _layers[layerIdx];
                layerMeshRend = layerGo.GetComponent<MeshRenderer>();
                layerMeshFilter = layerGo.GetComponent<MeshFilter>();
                layerGo.transform.localScale = Vector3.one;
            }

            var layerVerts = vertices2D.Clone() as Vector2[];

            deformatePolyStopWatch.Start();
            for (int addDefIdx = 0; addDefIdx < numDeformationsVariation; addDefIdx++)
            {
                layerVerts = DeformatePolygon(layerVerts, gauss_um_addition, gauss_sigma_additional);
            }
            deformatePolyStopWatch.Stop();

            // Use the triangulator to get indices for creating triangles
            Triangulator tr = new Triangulator(layerVerts);
            int[] indices = tr.Triangulate();

            // Create the Vector3 vertices
            Vector3[] vertices = new Vector3[layerVerts.Length];
            for (int i = 0; i < layerVerts.Length; i++)
            {
                vertices[i] = new Vector3(layerVerts[i].x, layerVerts[i].y, 0);
            }

            // Create the mesh
            Mesh msh = new Mesh();
            msh.vertices = vertices;
            msh.triangles = indices;
            msh.RecalculateNormals();
            msh.RecalculateBounds();

            layerMeshFilter.mesh = msh;

            var mat = new Material(usedMaterial);
            mat.color = Color;

            layerMeshRend.material = mat;
        }
        
        regenStopWatch.Stop();
        Debug.Log($"{gameObject.name} regenerate duration: {regenStopWatch.Elapsed}");
        Debug.Log($"{gameObject.name} deformate Polygon duration: {deformatePolyStopWatch.Elapsed}");
    }

    private Vector2[] DeformatePolygon(Vector2[] verts, float gauss_mu = 0f, float gauss_sigma = 1f)
    {
        var arrSize = verts.Length;
        var retArr = new Vector2[arrSize * 2];

        for (int i = 0; i < arrSize; i++)
        {
            var vert1 = verts[i];

            Vector2 vert2;
            if (i == arrSize - 1)
            {
                vert2 = verts[0];
            }
            else
            {
                vert2 = verts[i + 1];
            }

            Vector2 edgeVect = (vert2 - vert1);
            Vector2 midPoint = vert1 + edgeVect * 0.5f;

            float halfEdgeMagn = edgeVect.magnitude * 0.5f;

            var randx = new System.Random(Random.Range(0,1000)).NextGaussian(gauss_mu, gauss_sigma) ;
            var randy = new System.Random(Random.Range(0, 1000)).NextGaussian(gauss_mu, gauss_sigma);

            var coordx = randx * halfEdgeMagn + midPoint.x;
            var coordy = randy * halfEdgeMagn + midPoint.y;

            var newVert = new Vector2((float)coordx, (float)coordy);

            if (i == 0)
            {
                retArr[i * 2] = vert1;
            }
            
            retArr[i * 2 + 1] = newVert;
            if (i < arrSize - 1) // last vertex gets processed in arrSize - 1 cycle
            {
                retArr[(i + 1) * 2] = vert2;
            }
        }

        return retArr;
    }
}
