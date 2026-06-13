using Unity.Netcode;
using UnityEngine;

public class NetworkGrabbableObject : NetworkBehaviour
{
    private Rigidbody _rigidbody;
    private Transform _holdPoint;
    private GameObject _jointObject;
    private Rigidbody _jointRigidbody;
    private ConfigurableJoint _joint;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody component is missing on this GameObject.");
        }
    }

    /**
    * Method to grab the object, creating a joint to the hold point.
    * @param holdPoint The transform where the object will be held.
    */
    public void Grab(Transform holdPoint)
    {

        this._holdPoint = holdPoint;

        _jointObject = new GameObject("AttachmentPoint");
        _jointObject.transform.position = holdPoint.position;

        _jointRigidbody = _jointObject.AddComponent<Rigidbody>();
        _jointRigidbody.isKinematic = true;

        _joint = _jointObject.AddComponent<ConfigurableJoint>();
        _joint.connectedBody = _rigidbody;
        _joint.configuredInWorldSpace = true;

        _joint.xDrive = CreateJointDrive();
        _joint.yDrive = CreateJointDrive();
        _joint.zDrive = CreateJointDrive();

        _joint.rotationDriveMode = RotationDriveMode.Slerp;
        _joint.slerpDrive = CreateJointDrive();
    }

    /*
    * Helper method to create a JointDrive with predefined settings.
    * @return A configured JointDrive instance.
    */
    private JointDrive CreateJointDrive()
    {
        JointDrive drive = new JointDrive();
        drive.positionSpring = 500f;   // fuerza
        drive.positionDamper = 10f;    // suavidad
        drive.maximumForce = Mathf.Infinity;
        return drive;
    }

    /*
    * Method to drop the object, destroying the joint and attachment point.
    */
    public void Drop()
    {
        _holdPoint = null;
        if (_joint != null)
        {
            Destroy(_joint);
            _joint = null;
        }
        if (_jointObject != null)
        {
            Destroy(_jointObject);
            _jointObject = null;
        }
    }

    private void FixedUpdate()
    {
        if (_joint != null && _holdPoint != null)
        {
            _jointObject.transform.position = _holdPoint.position;
            _jointObject.transform.rotation = _holdPoint.rotation;
        }
    }
}
