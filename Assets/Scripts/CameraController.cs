using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Singleton
    public static CameraController instance;
    #endregion

    #region Data
    [Space, Header("Data")]
    [SerializeField] private CameraInputData camInputData = null;

    #endregion

    #region Settings
    [Space, Header("Look Settings")]
    [SerializeField] private Vector2 sensitivity = Vector2.zero;
    [SerializeField] private Vector2 smoothAmount = Vector2.zero;
    [SerializeField] private Vector2 lookAngleClamp = Vector2.zero;
    #endregion

    #region Variables
    private float yaw;
    private float pitch;

    private float desiredYaw;
    private float desiredPitch;

    public bool lockRotation = false;
    #endregion

    #region References
    private Transform pitchTransform = null;
    private Camera cam = null;
    #endregion

    #region BuiltIn Methods
    private void Awake()
    {
        instance = this;
        GetReferences();
        InitVariables();
        LockCursor();
    }

    private void LateUpdate()
    {
        if (!lockRotation)
        {
            CalculateRotation();
            SmoothRotation();
            ApplyRotation();
        }
    }
    #endregion

    #region Custom Methods

    public void ToggleCursor()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void GetReferences()
    {
        pitchTransform = transform.GetChild(0).transform;
        cam = GetComponentInChildren<Camera>();
    }

    private void InitVariables()
    {
        yaw = transform.eulerAngles.y;
        desiredYaw = yaw;
        lockRotation = false;
    }

    private void CalculateRotation()
    {
        desiredYaw += camInputData.InputVector.x * sensitivity.x * Time.deltaTime;
        desiredPitch += camInputData.InputVector.y * sensitivity.y * Time.deltaTime;

        desiredPitch = Mathf.Clamp(desiredPitch, lookAngleClamp.x, lookAngleClamp.y);
    }

    private void SmoothRotation()
    {
        yaw = Mathf.Lerp(yaw, desiredYaw, smoothAmount.x * Time.deltaTime);
        pitch = Mathf.Lerp(pitch, desiredPitch, smoothAmount.y * Time.deltaTime);
    }

    private void ApplyRotation()
    {
        transform.eulerAngles = new Vector3(0f, yaw, 0f);
        pitchTransform.localEulerAngles = new Vector3(-pitch, 0f, 0f);
    }
    #endregion
}
