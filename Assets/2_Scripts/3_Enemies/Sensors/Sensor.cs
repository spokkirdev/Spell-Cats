using UnityEngine;

public abstract class Sensor : MonoBehaviour
{
    public abstract bool Detect(out Transform target);
}
