using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Transform _holdPoint;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody component is missing on this GameObject.");
        }
    }

    public void Grab(Transform holdPoint)
    {
        this._holdPoint = holdPoint;
        _rigidbody.useGravity = false;
    }

    public void Drop()
    {
        _holdPoint = null;
        _rigidbody.useGravity = true;
    }

    private void FixedUpdate()
    {
        if (_holdPoint != null)
        {
            Vector3 lerpPosition = Vector3.Lerp(transform.position, _holdPoint.position, Time.fixedDeltaTime * 10f);
            _rigidbody.MovePosition(lerpPosition);
            _rigidbody.MoveRotation(_holdPoint.rotation);
        }
    }
}
