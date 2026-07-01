using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float triggerTransitionTime = 0.25f;
    [SerializeField] private float crouchingTriggerScale = 0.5f;
    [SerializeField] private float walkingTriggerScale = 2f;
    [SerializeField] private float runningTriggerScale = 3f;
    [SerializeField] private float standingTriggerScale = 1f;
    [SerializeField] private float levitateTriggerScale = 0f;

    [Header("References")]
    [SerializeField] private GameObject soundTrigger;
    [SerializeField] FPSMovement playerMovement;

    private void Start()
    {
        if (soundTrigger == null)
        {
            Debug.LogError("Sound trigger is not assigned.");
            return;
        }
    }

    private void Update()
    {
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement reference is not assigned.");
            return;
        }
        HandleSoundTrigger();
    }

    private void HandleSoundTrigger()
    {
        float triggerScale = GetCurrentTriggerScale();
        if (triggerScale > 0f)
        {
            soundTrigger.transform.localScale = Vector3.Lerp(soundTrigger.transform.localScale, Vector3.one * triggerScale, Time.deltaTime * triggerTransitionTime);
        }
    }

    private float GetCurrentTriggerScale()
    {
        switch (playerMovement.CurrentState)
        {
            case PlayerState.Walking:
                return walkingTriggerScale;
            case PlayerState.Running:
                return runningTriggerScale;
            case PlayerState.Crouching:
                return crouchingTriggerScale;
            case PlayerState.Levitating:
                return levitateTriggerScale;
            default:
                return standingTriggerScale;
        }
    }

}
