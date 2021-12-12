using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseScroller : MonoBehaviour
{
    #region Variables
    PerlinNoiseData data;

    Vector3 noiseOffset;
    Vector3 noise;
    #endregion

    #region Properties
    public Vector3 Noise => noise;
    #endregion

    #region Custom Methods
    public PerlinNoiseScroller(PerlinNoiseData _data)
    {
        data = _data;

        float _rand = 32f;

        noiseOffset.x = Random.Range(0f, _rand);
        noiseOffset.y = Random.Range(0f, _rand);
        noiseOffset.z = Random.Range(0f, _rand);
    }

    public void UpdateNoise()
    {
        float _scrollOffset = Time.deltaTime * data.frequency;

        noiseOffset.x += _scrollOffset;
        noiseOffset.y += _scrollOffset;
        noiseOffset.z += _scrollOffset;

        noise.x = Mathf.PerlinNoise(noiseOffset.x, 0f);
        noise.y = Mathf.PerlinNoise(noiseOffset.x, 1f);
        noise.z = Mathf.PerlinNoise(noiseOffset.x, 2f);

        noise -= Vector3.one * 0.5f;
        noise *= data.amplitude;
    }
    #endregion
}
