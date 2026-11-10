using UnityEngine;

public class GearSpinRB : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 20f;

    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        rb.AddTorque(Vector3.up * speed, ForceMode.Impulse);

    }
}
