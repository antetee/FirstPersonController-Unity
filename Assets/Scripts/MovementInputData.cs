using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementInputData", menuName = "Data/ MovementInputData")]
public class MovementInputData : ScriptableObject
{
    #region Variables
    private Vector2 m_inputVector;

    private bool m_isRunning;
    private bool m_isCrouching;

    private bool m_crouchClicked;
    private bool m_jumpClicked;

    private bool m_runClicked;
    private bool m_runReleased;
    #endregion

    #region Properties
    public Vector2 InputVector => m_inputVector;
    public bool HasInput => m_inputVector != Vector2.zero;

    public float InputVectorX
    {
        set => m_inputVector.x = value;
    }

    public float InputVectorY
    {
        set => m_inputVector.y = value;
    }

    public bool IsRunning
    {
        get => m_isRunning;
        set => m_isRunning = value;
    }

    public bool IsCrouching
    {
        get => m_isCrouching;
        set => m_isCrouching = value;
    }

    public bool CrouchClicked
    {
        get => m_crouchClicked;
        set => m_crouchClicked = value;
    }

    public bool JumpClicked
    {
        get => m_jumpClicked;
        set => m_jumpClicked = value;
    }

    public bool RunClicked
    {
        get => m_runClicked;
        set => m_runClicked = value;
    }

    public bool RunReleased
    {
        get => m_runReleased;
        set => m_runReleased = value;
    }
    #endregion

    #region Custom Methods
    public void ResetInput()
    {
        m_inputVector = Vector2.zero;

        m_isRunning = false;
        m_isCrouching = false;

        m_crouchClicked = false;
        m_jumpClicked = false;
        m_runClicked = false;
        m_runReleased = false;
    }

    #endregion
}
