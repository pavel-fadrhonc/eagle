using UnityEngine;
using System.Collections;
using Superbest_random;

public class GaussianTest : MonoBehaviour
{
    private const int NUM_SAMPLES = 1000;

    // Use this for initialization
    System.Random m_GaussianRandom;
    void Start()
    {
        m_GaussianRandom  = new System.Random(UnityEngine.Random.Range(0, 10000));

        float maxRand = 0;
        float minRand = 0;

        for (int i = 0; i < NUM_SAMPLES; i++)
        {
            var rand = m_GaussianRandom.NextGaussian();

            maxRand = Mathf.Max((float) rand, (float) maxRand);
            minRand = Mathf.Min((float) rand, (float) minRand);

        }

        Debug.Log("Generated " + NUM_SAMPLES + " of random gaussian numers. Max: " + maxRand + " Min " + minRand);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
