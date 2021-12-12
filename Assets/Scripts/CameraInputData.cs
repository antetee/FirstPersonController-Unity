using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraInputData", menuName = "Data/CameraInputData")]
public class CameraInputData : ScriptableObject
{
    #region Variables
    private Vector2 inputVector;
    private bool isZooming;
    private bool zoomClicked;
    private bool zoomReleased;
    #endregion

    #region Properties
    public Vector2 InputVector => inputVector;

    public float InputVectorX
    {
        set => inputVector.x = value;
    }

    public float InputVectorY
    {
        set => inputVector.y = value;
    }

    public bool IsZooming
    {
        get => isZooming;
        set => isZooming = value;
    }

    public bool ZoomClicked
    {
        get => zoomClicked;
        set => zoomClicked = value;
    }

    public bool ZoomReleased
    {
        get => zoomReleased;
        set => zoomReleased = value;
    }
    #endregion

    #region Custom Methods
    public void ResetInput()
    {
        inputVector = Vector2.zero;
        isZooming = false;
        zoomClicked = false;
        zoomReleased = false;
    }
    #endregion
}
