using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransformTarget
{
    Position,
    Rotation,
    Both
}


[CreateAssetMenu(fileName = "PerlinNoiseData", menuName = "Data/PerlinNoiseData")]
public class PerlinNoiseData : ScriptableObject
{
    #region Variables
    public TransformTarget transformTarget;

    [Space]
    public float amplitude;
    public float frequency;
    #endregion
}
