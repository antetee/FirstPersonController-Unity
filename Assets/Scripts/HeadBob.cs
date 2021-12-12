using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob
{
    #region Variables
    private HeadBobData data;

    private float xScroll;
    private float yScroll;

    private bool resetted;
    private Vector3 finalOffset;
    private float currentStateHeight = 0f;
    #endregion

    #region Properties
    public Vector3 FinalOffset => finalOffset;
    public bool Resetted => resetted;
    public float CurrentStateHeight
    {
        get => currentStateHeight;
        set => currentStateHeight = value;
    }
    #endregion

    #region Custom Methods
    public HeadBob(HeadBobData _data, float _moveBackwardsMultiplier, float _moveSideMultiplier)
    {
        data = _data;

        data.MoveBackwardsFrequencyMultiplier = _moveBackwardsMultiplier;
        data.MoveSideFrequencyMultiplier = _moveSideMultiplier;

        xScroll = 0f;
        yScroll = 0f;

        resetted = false;
        finalOffset = Vector3.zero;
    }

    public void ScrollHeadBob(bool _running, bool _crouching, Vector2 _input)
    {
        resetted = false;

        float _amplitudeMultiplier;
        float _frequencyMultiplier;
        float _additionalMultiplier;

        _amplitudeMultiplier = _running ? data.runAmplitudeMultiplier : 1f;
        _amplitudeMultiplier = _crouching ? data.crouchAmplitudeMultiplier : _amplitudeMultiplier;

        _frequencyMultiplier = _running ? data.runFrequencyMultiplier : 1f;
        _frequencyMultiplier = _crouching ? data.crouchFrequencyMultiplier : _frequencyMultiplier;

        _additionalMultiplier = _input.y == -1 ? data.MoveBackwardsFrequencyMultiplier : 1f;
        _additionalMultiplier = _input.x != 0 & _input.y == 0 ? data.MoveSideFrequencyMultiplier : _additionalMultiplier;

        float _xValue;
        float _yValue;

        _xValue = data.xCurve.Evaluate(xScroll);
        _yValue = data.yCurve.Evaluate(yScroll);

        finalOffset.x = _xValue * data.xAmplitude * _amplitudeMultiplier * _additionalMultiplier;
        finalOffset.y = _yValue * data.yAmplitude * _amplitudeMultiplier * _additionalMultiplier;
    }

    public void ResetHeadBob()
    {
        resetted = true;

        xScroll = 0f;
        yScroll = 0f;

        finalOffset = Vector3.zero;
    }

    #endregion
}
