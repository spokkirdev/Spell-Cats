using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class NetworkPlayerPickUpDrop : NetworkBehaviour
{
    [Header("Pick Up Settings")]
    [SerializeField] private float pickUpRange = 3f;
    [SerializeField] private Transform holdPoint;


    [Header("References")]
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private InputActionReference pickUpAction;

    private bool _isHoldingObject;
    private GameObject _heldObject;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        pickUpAction.action.performed += PickObject;
        pickUpAction.action.canceled += DropObject;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        pickUpAction.action.performed -= PickObject;
        pickUpAction.action.canceled -= DropObject;
    }

    /*
    * Method to pick up an object, performing a raycast and checking for a GrabbableObject.
    * @param context The input action context from the performed event.
    */
    private void PickObject(InputAction.CallbackContext context)
    {
        Debug.Log("Pick action performed");
        if (_isHoldingObject) return;

        Ray ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickUpRange, pickUpLayerMask))
        {
            if (hit.transform.TryGetComponent(out NetworkGrabbableObject grabbableObject))
            {
                Debug.Log("Picked up: " + hit.collider.gameObject.name);
                grabbableObject.Grab(holdPoint);
                _heldObject = hit.collider.gameObject;
                _isHoldingObject = true;
            }
        }
    }

    /*
    * Method to drop the object, checking if currently holding one and calling its Drop method.
    * @param context The input action context from the canceled event.
    */
    private void DropObject(InputAction.CallbackContext context)
    {
        Debug.Log("Drop action performed");
        if (!_isHoldingObject) return;
        Debug.Log("Dropped: " + _heldObject.name);
        _heldObject.GetComponent<NetworkGrabbableObject>().Drop();
        _isHoldingObject = false;
        _heldObject = null;
    }

    public bool ForceDrop()
    {
        if (!_isHoldingObject || _heldObject == null)
            return false;

        Debug.Log("Force Dropped: " + _heldObject.name);

        var grabbable = _heldObject.GetComponent<NetworkGrabbableObject>();
        if (grabbable != null)
        {
            grabbable.Drop();
        }

        _isHoldingObject = false;
        _heldObject = null;

        return true;
    }
}
