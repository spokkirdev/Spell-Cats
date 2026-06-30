using Unity.Netcode;
using UnityEngine;

public class NetworkGrabbableObject : NetworkBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject deathEffectPrefab;
    private Rigidbody _rigidbody;
    private Transform _holdPoint;
    private GameObject _jointObject;
    private Rigidbody _jointRigidbody;
    private ConfigurableJoint _joint;

    private GameObject _playerHoldingObject;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody component is missing on this GameObject.");
        }
    }

    public override void OnNetworkDespawn()
    {
        if (_playerHoldingObject != null)
        {
            _playerHoldingObject.GetComponent<NetworkPlayerPickUpDrop>().ForceDrop(); // Ensure the player drops the object when it despawns
        }

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        
    }

    /**
    * Method to grab the object, creating a joint to the hold point.
    * @param holdPoint The transform where the object will be held.
    * @param playerHoldingObject The player GameObject that is holding the object.
    */
    public void Grab(Transform holdPoint, GameObject playerHoldingObject)
    {
        RequestOwnershipRpc(playerHoldingObject.GetComponent<NetworkObject>().OwnerClientId, this.GetComponent<NetworkObject>().NetworkObjectId);
        this._holdPoint = holdPoint;
        this._playerHoldingObject = playerHoldingObject;

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
        _playerHoldingObject = null;
    }

    private void FixedUpdate()
    {
        if (_joint != null && _holdPoint != null)
        {
            _jointObject.transform.position = _holdPoint.position;
            _jointObject.transform.rotation = _holdPoint.rotation;
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RequestOwnershipRpc(ulong requestingClientId, ulong objectNetworkId)
    {
        if (!IsServer) return;

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectNetworkId, out NetworkObject netObj))
        {
            netObj.ChangeOwnership(requestingClientId);
        }
        else
        {
            Debug.LogWarning($"No NetworkObject found with ID {objectNetworkId}");
        }
    }
}
