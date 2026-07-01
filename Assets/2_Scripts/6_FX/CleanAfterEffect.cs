using UnityEngine;

public class CleanAfterEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 5f); // Destroy the GameObject after 5 seconds
    }


}
