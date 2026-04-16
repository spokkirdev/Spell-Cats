using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickUpDrop : MonoBehaviour
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

    private void OnEnable()
    {
        pickUpAction.action.performed += PickObject;
        pickUpAction.action.canceled += DropObject;
    }

    private void OnDisable()
    {
        pickUpAction.action.performed -= PickObject;
        pickUpAction.action.canceled -= DropObject;
    }

    private void Update()
    {
        if (_isHoldingObject && _heldObject != null)
        {
            // Keep the held object at the hold point
            _heldObject.transform.position = holdPoint.position;
            _heldObject.transform.rotation = holdPoint.rotation;
        }
    }

    private void PickObject(InputAction.CallbackContext context)
    {
        if (_isHoldingObject) return;

        Ray ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickUpRange, pickUpLayerMask))
        {
            if (hit.transform.TryGetComponent(out GrabbableObject grabbableObject))
            {
                Debug.Log("Picked up: " + hit.collider.gameObject.name);
                grabbableObject.Grab(holdPoint);
                _heldObject = hit.collider.gameObject;
                _isHoldingObject = true;
            }
            /*
            _heldObject = hit.collider.gameObject;
            _heldObject.GetComponent<Rigidbody>().isKinematic = true; // Make the object kinematic while held
            _isHoldingObject = true;
            */
        }
    }

    private void DropObject(InputAction.CallbackContext context)
    {
        if (!_isHoldingObject) return;

        Debug.Log("Dropped: " + _heldObject.name);
        _heldObject.GetComponent<GrabbableObject>().Drop();
        _isHoldingObject = false;
        _heldObject = null;
    }


}
