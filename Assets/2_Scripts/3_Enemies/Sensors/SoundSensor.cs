using UnityEngine;

public class SoundSensor : Sensor
{
    private Transform _target;

    public override bool Detect(out Transform target)
    {
        target = _target;
        return _target != null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSound"))
        {
            _target = other.transform.root;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerSound"))
        {
            _target = null;
        }
    }
}