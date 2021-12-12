using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class PlayerController : MonoBehaviour
{
    #region Singleton
    public static PlayerController instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    #region Variables
    #region Data
    [Header("Data")]
    [SerializeField] private MovementInputData movementInputData;
    [SerializeField] private HeadBobData headBobData = null;
    #endregion

    #region Movement
    [Header("Movement"), Space]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float jumpSpeed;
    [Range(0f, 1f)] [SerializeField] private float moveBackwardsSpeedPercent = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float moveSideSpeedPercent = 0.75f;
    #endregion

    #region Sprint Settings
    [Header("Sprint Settings"), Space]
    [Range(-1f, 1f)] [SerializeField] private float canRunThreshold = 0.8f;
    [SerializeField] private AnimationCurve sprintTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private bool isDecreasingStamina = false;
    public bool isRunning = false;
    #endregion

    #region Crouch Settings
    [Header("Crouch Settings"), Space]
    [Range(0.2f, 0.9f)] [SerializeField] private float crouchPercent = 0.6f;
    [SerializeField] private float crouchTransitionDuration = 1f;
    [SerializeField] private AnimationCurve crouchTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    #endregion

    #region Landing Settings
    [Header("Landing Settings"), Space]
    [Range(0.05f, 0.5f)] [SerializeField] private float lowLandAmount = 0.1f;
    [Range(0.2f, 0.9f)] [SerializeField] private float highLandAmount = 0.1f;
    [SerializeField] private float landTimer = 0.5f;
    [SerializeField] private float landDuration = 1f;
    [SerializeField] private AnimationCurve landCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    #endregion

    #region Gravity Settings
    [Header("Gravity Settings"), Space]
    [SerializeField] private float gravityMultiplier = 2.5f;
    [SerializeField] private float stickToGroundForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [Range(0f, 1f)] [SerializeField] private float rayLength = 0.1f;
    [Range(0.01f, 1f)] [SerializeField] private float raySphereRadius = 0.1f;
    #endregion

    #region Wall Settings
    [Header("Wall Settings"), Space]
    [SerializeField] private LayerMask obstacleLayers;
    [Range(0f, 1f)] [SerializeField] private float rayObstacleLength = 0.1f;
    [Range(0.01f, 1f)] [SerializeField] private float rayObstacleSphereRadius = 0.1f;
    #endregion

    #region Smooth Settings
    [Header("Smooth Settings"), Space]
    [Range(1f, 100f)] [SerializeField] private float smoothRotateSpeed = 5f;
    [Range(1f, 100f)] [SerializeField] private float smoothInputSpeed = 5f;
    [Range(1f, 100f)] [SerializeField] private float smoothVelocitySpeed = 5f;
    [Range(1f, 100f)] [SerializeField] private float smoothFinalDirectionSpeed = 5f;
    [Range(1f, 100f)] [SerializeField] private float smoothHeadBobSpeed = 5f;
    #endregion

    #region Camera Turning
    [Header("Camera Turning"), Space]
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    #endregion

    #region Debug
    [Header("Debug")]
    public bool lockMovement = false;

    [SerializeField] private Vector2 inputVector;
    [SerializeField] private Vector2 smoothInputVector;
    [Space]
    [SerializeField] private Vector3 finalMoveDir;
    [SerializeField] private Vector3 smoothFinalMoveDir;
    [Space]
    [SerializeField] private Vector3 finalMoveVector;
    [Space]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float smoothCurrentSpeed;
    [SerializeField] private float finalSmoothCurrentSpeed;
    [SerializeField] private float walkSprintSpeedDifference;
    [Space]
    [SerializeField] private float finalRayLenght;
    [SerializeField] private bool hitWall;
    [SerializeField] private bool isControllerGrounded;
    [SerializeField] private bool previouslyGrounded;
    [Space]
    [SerializeField] private float initHeight;
    [SerializeField] private float crouchHeight;
    [SerializeField] private Vector3 initCenter;
    [SerializeField] private Vector3 crouchCenter;
    [Space]
    [SerializeField] private float initCamHeight;
    [SerializeField] private float crouchCamHeight;
    [SerializeField] private float crouchStandHeightDifference;
    [SerializeField] private bool duringCrouchAnimation;
    [SerializeField] private bool duringSprintAnimation;
    [Space]
    [SerializeField] private float inAirTimer;
    [Space]
    [SerializeField] private float inputVectorMagnitude;
    [SerializeField] private float smoothInputVectorMagnitude;
    [Space]
    [Range(1f, 100f)] [SerializeField] private float smoothInputMagnitudeSpeed = 5f;
    RaycastHit hitInfo;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 direction = Vector3.zero;

    #endregion
    #endregion

    #region References
    [Header("References"), Space]
    private CharacterController controller;
    private MovementInputData input;
    private HeadBob headBob;
    private CameraController camController;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform yawTransform;

    private IEnumerator crouchRoutine;
    private IEnumerator landRoutine;
    #endregion

    #region BuiltIn Methods
    private void Start()
    {
        GetReferences();
        InitVariables();
    }

    private void Update()
    {
        if (!lockMovement)
        {
            if (controller)
            {
                CheckIfGrounded();
                CheckIfWall();

                SmoothInput();
                SmoothSpeed();
                SmoothDir();

                SmoothInputMagnitude();

                CalculateMovement();
                CalculateSpeed();
                CalculateFinalMovement();

                HandleCrouch();
                HandleHeadBob();
                HandleRunFOV();
                HandleRunStamina();
                HandleLanding();

                ApplyGravity();
                ApplyMovement();
            }
        }
    }
    #endregion

    #region Custom Methods

    #region Smoothing Methods

    private void SmoothInput()
    {
        inputVector = movementInputData.InputVector.normalized;
        smoothInputVector = Vector2.Lerp(smoothInputVector, inputVector, Time.deltaTime * smoothInputSpeed);
    }

    private void SmoothSpeed()
    {
        smoothCurrentSpeed = Mathf.Lerp(smoothCurrentSpeed, currentSpeed, Time.deltaTime * smoothVelocitySpeed);

        if (movementInputData.IsRunning && CanRun())
        {
            float walkSprintPercent = Mathf.InverseLerp(walkSpeed, sprintSpeed, smoothCurrentSpeed);
            finalSmoothCurrentSpeed = sprintTransitionCurve.Evaluate(walkSprintPercent) * walkSprintSpeedDifference + walkSpeed;
        }
        else
        {
            finalSmoothCurrentSpeed = smoothCurrentSpeed;
        }
    }

    private void SmoothDir()
    {
        smoothFinalMoveDir = Vector3.Lerp(smoothFinalMoveDir, finalMoveDir, Time.deltaTime * smoothFinalDirectionSpeed);
        Debug.DrawRay(transform.position, smoothFinalMoveDir, Color.yellow);
    }

    private void SmoothInputMagnitude()
    {
        inputVectorMagnitude = inputVector.magnitude;
        smoothInputVectorMagnitude = Mathf.Lerp(smoothInputVectorMagnitude, inputVectorMagnitude, Time.deltaTime * smoothInputMagnitudeSpeed);
    }
    #endregion

    #region Movement Calculation Methods
    private void CalculateMovement()
    {
        Vector3 horizontalRaw = transform.right * smoothInputVector.x;
        Vector3 verticalRaw = transform.forward * smoothInputVector.y;

        Vector3 desiredDir = verticalRaw + horizontalRaw;
        Vector3 flattenDir = FlattenVectorOnSlopes(desiredDir);

        finalMoveDir = flattenDir;

        #region Old Code
        /*
        direction = new Vector3(horizontalRaw, 0, verticalRaw);

        direction = direction.normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            moveDirection *= currentSpeed;
        }
        */
        #endregion
    }

    private Vector3 FlattenVectorOnSlopes(Vector3 vectorToFlat)
    {
        if (isControllerGrounded)
            vectorToFlat = Vector3.ProjectOnPlane(vectorToFlat, hitInfo.normal);

        return vectorToFlat;
    }

    private void CalculateSpeed()
    {
        currentSpeed = movementInputData.IsRunning && CanRun() ? sprintSpeed : walkSpeed;
        currentSpeed = movementInputData.IsCrouching ? crouchSpeed : currentSpeed;
        currentSpeed = !movementInputData.HasInput ? 0f : currentSpeed;
    }

    private void CalculateFinalMovement()
    {
        float _smoothInputVectorMagnitude = smoothInputVectorMagnitude;
        Vector3 finalVector = smoothFinalMoveDir * finalSmoothCurrentSpeed * smoothInputVectorMagnitude;

        finalMoveVector.x = finalVector.x;
        finalMoveVector.z = finalVector.z;

        if (controller.isGrounded)
            finalMoveVector.y += finalVector.y;
    }

    #endregion

    #region Crouch Methods

    private void HandleCrouch()
    {
        if (movementInputData.CrouchClicked && isControllerGrounded)
        {
            InvokeCrouchRoutine();
        }
    }

    private void InvokeCrouchRoutine()
    {
        if (landRoutine != null)
            StopCoroutine(landRoutine);

        if (crouchRoutine != null)
            StopCoroutine(crouchRoutine);

        crouchRoutine = CrouchRoutine();
        StartCoroutine(crouchRoutine);
    }

    private IEnumerator CrouchRoutine()
    {
        duringCrouchAnimation = true;

        float percent = 0f;
        float smoothPercent = 0f;
        float speed = 1f / crouchTransitionDuration;

        float currentHeight = controller.height;
        Vector3 currentCenter = controller.center;

        float desiredHeight = movementInputData.IsCrouching ? initHeight : crouchHeight;
        Vector3 desiredCenter = movementInputData.IsCrouching ? initCenter : crouchCenter;

        Vector3 camPos = yawTransform.localPosition;
        float camCurrentHeight = camPos.y;
        float camDesiredHeight = movementInputData.IsCrouching ? initCamHeight : crouchCamHeight;

        movementInputData.IsCrouching = !movementInputData.IsCrouching;
        headBob.CurrentStateHeight = movementInputData.IsCrouching ? crouchCamHeight : initCamHeight;

        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            smoothPercent = crouchTransitionCurve.Evaluate(percent);

            controller.height = Mathf.Lerp(currentHeight, desiredHeight, smoothPercent);
            controller.center = Vector3.Lerp(currentCenter, desiredCenter, smoothPercent);

            camPos.y = Mathf.Lerp(camCurrentHeight, camDesiredHeight, smoothPercent);
            yawTransform.localPosition = camPos;

            yield return null;
        }

        duringCrouchAnimation = false;
    }

    #endregion

    #region Landing Methods
    private void HandleLanding()
    {
        if (!previouslyGrounded && isControllerGrounded)
            InvokeLandingRoutine();
    }

    private void InvokeLandingRoutine()
    {
        if (landRoutine != null)
            StopCoroutine(landRoutine);

        landRoutine = LandingRoutine();
        StartCoroutine(landRoutine);
    }

    private IEnumerator LandingRoutine()
    {
        float percent = 0f;
        float landAmount = 0f;

        float speed = 1f / landDuration;

        Vector3 localPos = yawTransform.localPosition;
        float initLandHeight = localPos.y;

        landAmount = inAirTimer > landTimer ? highLandAmount : lowLandAmount;

        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            float desiredY = landCurve.Evaluate(percent) * landAmount;

            localPos.y = initLandHeight + desiredY;
            yawTransform.localPosition = localPos;

            yield return null;
        }
    }

    #endregion

    #region HeadBob
    private void HandleHeadBob()
    {
        if (movementInputData.HasInput && isControllerGrounded && !hitWall)
        {
            if (!duringCrouchAnimation)
            {
                headBob.ScrollHeadBob(movementInputData.IsRunning && CanRun(), movementInputData.IsCrouching, movementInputData.InputVector);
                yawTransform.localPosition = Vector3.Lerp(yawTransform.localPosition, (Vector3.up * headBob.CurrentStateHeight) + headBob.FinalOffset, Time.deltaTime * smoothHeadBobSpeed);
            }
        }
        else
        {
            if (!headBob.Resetted)
            {
                headBob.ResetHeadBob();
            }

            if (!duringCrouchAnimation)
                yawTransform.localPosition = Vector3.Lerp(yawTransform.localPosition, new Vector3(0f, headBob.CurrentStateHeight, 0), Time.deltaTime * smoothHeadBobSpeed);
        }
    }
    #endregion

    #region Checks
    private void CheckIfGrounded()
    {
        Vector3 origin = transform.position + controller.center;

        bool hitGround = Physics.SphereCast(origin, raySphereRadius, Vector3.down, out hitInfo, finalRayLenght, groundLayer);
        Debug.DrawRay(origin, Vector3.down * (finalRayLenght), Color.red);

        isControllerGrounded = hitGround ? true : false;
    }

    private void CheckIfWall()
    {
        Vector3 origin = transform.position + controller.center;
        RaycastHit wallInfo;

        bool _hitWall = false;

        if (movementInputData.HasInput && finalMoveDir.sqrMagnitude > 0)
            hitWall = Physics.SphereCast(origin, rayObstacleSphereRadius, direction, out wallInfo, rayObstacleLength, obstacleLayers);
        Debug.DrawRay(origin, direction * rayObstacleLength, Color.blue);

        hitWall = _hitWall ? true : false;
    }

    private bool CanRun()
    {
        Vector3 normalizedDirection = Vector3.zero;

        if (smoothFinalMoveDir != Vector3.zero)
            normalizedDirection = smoothFinalMoveDir.normalized;

        float dot = Vector3.Dot(transform.forward, normalizedDirection);
        return dot >= canRunThreshold && !movementInputData.IsCrouching ? true : false;
    }
    #endregion

    #region Sprint Methods
    private void HandleRunFOV()
    {
        if (movementInputData.HasInput && isControllerGrounded && !hitWall)
        {
            if (movementInputData.RunClicked && CanRun())
            {
                duringSprintAnimation = true;
            }

            if (movementInputData.IsRunning && CanRun() && !duringSprintAnimation)
            {
                duringSprintAnimation = true;
            }

            if (movementInputData.RunReleased || !movementInputData.HasInput || hitWall)
            {
                if (duringSprintAnimation)
                {
                    duringSprintAnimation = false;
                }
            }
        }
    }

    private void HandleRunStamina()
    {
        if (movementInputData.HasInput && isControllerGrounded && !hitWall)
        {
            if (movementInputData.RunClicked && CanRun())
            {
                isRunning = true;
            }

            if (movementInputData.IsRunning && CanRun() && !duringSprintAnimation)
            {

            }

            if (movementInputData.RunReleased || !movementInputData.HasInput || hitWall)
            {
                isRunning = false;
                isDecreasingStamina = false;
                StopAllCoroutines();
            }
        }
    }
    #endregion

    #region Jumping Methods
    private void HandleJump()
    {
        if (movementInputData.JumpClicked && !movementInputData.IsCrouching)
        {
            finalMoveVector.y = jumpSpeed;

            previouslyGrounded = true;
            isControllerGrounded = false;
        }
    }

    #endregion

    #region Apply Methods
    private void ApplyMovement()
    {
        Vector3 normalizedRawMoveDir = inputVector.normalized;

        float desiredAngle = Mathf.Atan2(normalizedRawMoveDir.x, normalizedRawMoveDir.z) * Mathf.Rad2Deg;

        Vector3 moveDirection = Quaternion.Euler(0f, desiredAngle, 0f) * Vector3.forward;

        controller.Move(finalMoveVector * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            inAirTimer = 0f;
            finalMoveVector.y = -stickToGroundForce;

            HandleJump();
        }
        else
        {
            inAirTimer += Time.deltaTime;
            finalMoveVector += Physics.gravity * gravityMultiplier * Time.deltaTime;
        }
    }
    #endregion

    #region Initialize Methods
    private void GetReferences()
    {
        controller = GetComponent<CharacterController>();
        camController = GetComponent<CameraController>();
        yawTransform = camController.transform.GetChild(0);
        headBob = new HeadBob(headBobData, moveBackwardsSpeedPercent, moveSideSpeedPercent);
    }

    private void InitVariables()
    {
        lockMovement = false;

        controller.center = new Vector3(0f, controller.height / 2f + controller.skinWidth, 0f);

        initCenter = controller.center;
        initHeight = controller.height;

        crouchHeight = initHeight * crouchPercent;
        crouchCenter = (crouchHeight / 2f + controller.skinWidth) * Vector3.up;

        crouchStandHeightDifference = initHeight - crouchHeight;

        initCamHeight = yawTransform.localPosition.y;
        crouchCamHeight = initCamHeight - crouchStandHeightDifference;

        finalRayLenght = rayLength + controller.center.y;

        isControllerGrounded = true;
        previouslyGrounded = true;

        inAirTimer = 0f;
        headBob.CurrentStateHeight = initCamHeight;
        walkSprintSpeedDifference = sprintSpeed - walkSpeed;

        isDecreasingStamina = false;
    }

    #endregion

    #endregion
}
