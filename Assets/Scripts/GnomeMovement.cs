using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GnomeMovement : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 10f;
    public float rotationSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(horizontal * speed, 0f, vertical * speed);
        var lookDir = Quaternion.LookRotation(movement);

        if (movement.magnitude > 0) {
            rb.transform.position += movement;
            rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, lookDir, rotationSpeed);
        }
    }
}
