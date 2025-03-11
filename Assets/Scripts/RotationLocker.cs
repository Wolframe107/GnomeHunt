using UnityEngine;

public class RotationLocker : MonoBehaviour
{
    public bool lockXRotation = true;
    public bool lockYRotation = true;
    public bool lockZRotation = true;

    // Define the target rotation in world space
    private Vector3 targetRotationEuler = new Vector3(90, 0, 0);

    void LateUpdate()
    {
        // Get the current rotation in world space

        if(transform.rotation.x == 0) targetRotationEuler = new Vector3(0, 0, 0);
        
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // Lock each axis based on the target rotation in world space
        float x = lockXRotation ? targetRotationEuler.x : currentRotation.x;
        float y = lockYRotation ? targetRotationEuler.y : currentRotation.y;
        float z = lockZRotation ? targetRotationEuler.z : currentRotation.z;

        // Apply the locked rotation
        transform.rotation = Quaternion.Euler(x, y, z);
    }
}
