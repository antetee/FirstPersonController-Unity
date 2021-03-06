using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBreathing : MonoBehaviour
{
    #region Variables
    [Space, Header("Data")]
    [SerializeField] private PerlinNoiseData data = null;

    [Space, Header("Axis")]
    [SerializeField] private bool x = true;
    [SerializeField] private bool y = true;
    [SerializeField] private bool z = true;

    private PerlinNoiseScroller perlinNoiseScroller;
    private Vector3 finalRot;
    private Vector3 finalPos;
    #endregion

    #region BuiltIn Methods
    private void Start()
    {
        perlinNoiseScroller = new PerlinNoiseScroller(data);
    }

    private void LateUpdate()
    {
        if (data != null)
        {
            perlinNoiseScroller.UpdateNoise();

            Vector3 posOffset = Vector3.zero;
            Vector3 rotOffset = Vector3.zero;

            switch (data.transformTarget)
            {
                case TransformTarget.Position:
                    {
                        if (x)
                            posOffset.x += perlinNoiseScroller.Noise.x;

                        if (y)
                            posOffset.y += perlinNoiseScroller.Noise.y;

                        if (z)
                            posOffset.z += perlinNoiseScroller.Noise.z;

                        finalPos.x = x ? posOffset.x : transform.localPosition.x;
                        finalPos.y = y ? posOffset.y : transform.localPosition.y;
                        finalPos.z = z ? posOffset.z : transform.localPosition.z;

                        transform.localPosition = finalPos;
                        break;
                    }

                case TransformTarget.Rotation:
                    {
                        if (x)
                            rotOffset.x = perlinNoiseScroller.Noise.x;

                        if (y)
                            rotOffset.y = perlinNoiseScroller.Noise.y;

                        if (z)
                            rotOffset.z = perlinNoiseScroller.Noise.z;

                        finalRot.x = x ? rotOffset.x : transform.localEulerAngles.x;
                        finalRot.y = y ? rotOffset.y : transform.localEulerAngles.y;
                        finalRot.z = z ? rotOffset.z : transform.localEulerAngles.z;

                        transform.localEulerAngles = finalRot;

                        break;
                    }

                case TransformTarget.Both:
                    {
                        if (x)
                        {
                            posOffset.x += perlinNoiseScroller.Noise.x;
                            rotOffset.x += perlinNoiseScroller.Noise.x;
                        }

                        if (y)
                        {
                            posOffset.y += perlinNoiseScroller.Noise.y;
                            rotOffset.y += perlinNoiseScroller.Noise.y;
                        }

                        if (z)
                        {
                            posOffset.z += perlinNoiseScroller.Noise.z;
                            rotOffset.z += perlinNoiseScroller.Noise.z;
                        }

                        finalPos.x = x ? posOffset.x : transform.localPosition.x;
                        finalPos.y = y ? posOffset.y : transform.localPosition.y;
                        finalPos.z = z ? posOffset.z : transform.localPosition.z;


                        finalRot.x = x ? rotOffset.x : transform.localEulerAngles.x;
                        finalRot.y = y ? rotOffset.y : transform.localEulerAngles.y;
                        finalRot.z = z ? rotOffset.z : transform.localEulerAngles.z;

                        transform.localPosition = finalPos;
                        transform.localEulerAngles = finalRot;
                        break;
                    }

            }
        }
    }
    #endregion
}
