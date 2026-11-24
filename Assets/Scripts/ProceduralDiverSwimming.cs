using UnityEngine;

public class ProceduralDiverSwimming : MonoBehaviour
{
    [System.Serializable]
    private class BoneMotion
    {
        [SerializeField] private Transform _bone;
        [SerializeField] private Quaternion _restRotation = Quaternion.identity;
        [SerializeField] private Vector3 _axis = Vector3.right;
        [SerializeField] private float _phaseOffset;
        [SerializeField] private float _amplitudeMultiplier = 1f;

        public void Apply(float phase, float amplitude)
        {
            if (_bone == null) return;
            float angle = Mathf.Sin(phase + _phaseOffset) * amplitude * _amplitudeMultiplier;
            _bone.localRotation = Quaternion.AngleAxis(angle, _axis) * _restRotation;
        }
    }

    [Header("Existing Rig")]
    [SerializeField] private Rigidbody _body;
    [SerializeField] private Transform _hips;
    [SerializeField] private Quaternion _hipsRestRotation = Quaternion.identity;
    [SerializeField] private Vector3 _hipsRestPosition;
    [SerializeField] private Vector3 _pitchAxis = Vector3.right;
    [SerializeField] private Vector3 _bobAxis = Vector3.up;
    [SerializeField] private BoneMotion[] _motions;

    [Header("Swimming Motion")]
    [SerializeField, Min(0.1f)] private float _fullSwimSpeed = 4f;
    [SerializeField, Min(0f)] private float _idleKickAngle = 3f;
    [SerializeField, Min(0f)] private float _swimKickAngle = 18f;
    [SerializeField, Min(0f)] private float _idleFrequency = 0.45f;
    [SerializeField, Min(0f)] private float _swimFrequency = 1.8f;
    [SerializeField, Min(0f)] private float _blendSpeed = 5f;
    [SerializeField, Range(0f, 45f)] private float _maximumPitch = 25f;
    [SerializeField, Min(0f)] private float _bobDistance = 0.04f;

    private float _phase;
    private float _swimAmount;
    private float _pitch;

    private void LateUpdate()
    {
        if (_body == null) return;
        AnimateStep(Time.deltaTime, _body.linearVelocity);
    }

    // Also permits deterministic animation checks without simulating input or physics.
    public void AnimateStep(float deltaTime, Vector3 velocity)
    {
        float amount = Mathf.Clamp01(velocity.magnitude / _fullSwimSpeed);
        float blend = 1f - Mathf.Exp(-_blendSpeed * Mathf.Max(0f, deltaTime));
        _swimAmount = Mathf.Lerp(_swimAmount, amount, blend);
        float targetPitch = -Mathf.Clamp(velocity.y / _fullSwimSpeed, -1f, 1f) * _maximumPitch;
        _pitch = Mathf.Lerp(_pitch, targetPitch, blend);
        _phase = Mathf.Repeat(_phase + deltaTime * Mathf.Lerp(_idleFrequency, _swimFrequency, _swimAmount) * Mathf.PI * 2f, Mathf.PI * 2f);

        if (_hips != null)
        {
            _hips.localRotation = Quaternion.AngleAxis(_pitch, _pitchAxis) * _hipsRestRotation;
            _hips.localPosition = _hipsRestPosition + _bobAxis * (Mathf.Sin(_phase * 0.5f) * _bobDistance);
        }
        float amplitude = Mathf.Lerp(_idleKickAngle, _swimKickAngle, _swimAmount);
        if (_motions == null) return;
        foreach (var motion in _motions) if (motion != null) motion.Apply(_phase, amplitude);
    }

    private void OnDisable()
    {
        if (_hips != null)
        {
            _hips.localRotation = _hipsRestRotation;
            _hips.localPosition = _hipsRestPosition;
        }
        if (_motions != null) foreach (var motion in _motions) if (motion != null) motion.Apply(0f, 0f);
        _phase = 0f;
        _swimAmount = 0f;
        _pitch = 0f;
    }
}
