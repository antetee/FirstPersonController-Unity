using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region References
    [SerializeField] private MovementInputData movementInputData = null;
    [SerializeField] private CameraInputData cameraInputData = null;
    #endregion

    #region BuiltIn Methods
    private void Start()
    {
        cameraInputData.ResetInput();
        movementInputData.ResetInput();
    }

    private void Update()
    {
        GetMovementInputData();
        GetCameraInput();
    }
    #endregion

    #region Custom Methods
    private void GetMovementInputData()
    {
        movementInputData.InputVectorX = Input.GetAxisRaw("Horizontal");
        movementInputData.InputVectorY = Input.GetAxisRaw("Vertical");

        movementInputData.RunClicked = Input.GetKeyDown(KeyCode.LeftShift);
        movementInputData.RunReleased = Input.GetKeyUp(KeyCode.LeftShift);

        if (movementInputData.RunClicked)
            movementInputData.IsRunning = true;

        if (movementInputData.RunReleased)
            movementInputData.IsRunning = false;

        movementInputData.JumpClicked = Input.GetKeyDown(KeyCode.Space);
        movementInputData.CrouchClicked = Input.GetKeyDown(KeyCode.LeftControl);
    }

    private void GetCameraInput()
    {
        cameraInputData.InputVectorX = Input.GetAxis("Mouse X");
        cameraInputData.InputVectorY = Input.GetAxis("Mouse Y");

        cameraInputData.ZoomClicked = Input.GetMouseButtonDown(1);
        cameraInputData.ZoomReleased = Input.GetMouseButtonUp(1);
    }
    #endregion
}
