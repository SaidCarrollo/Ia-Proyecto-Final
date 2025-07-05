using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float speed = 10f;
    public float lookSpeed = 2f;

    float yaw;
    float pitch;

    void Update()
    {
        // Movimiento
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float y = 0;

        if (Input.GetKey(KeyCode.E)) y += 1;
        if (Input.GetKey(KeyCode.Q)) y -= 1;

        Vector3 dir = transform.right * x + transform.forward * z + transform.up * y;
        transform.position += dir * speed * Time.deltaTime;

        // Rotación con el mouse
        if (Input.GetMouseButton(1)) // botón derecho
        {
            yaw += lookSpeed * Input.GetAxis("Mouse X");
            pitch -= lookSpeed * Input.GetAxis("Mouse Y");
            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }
    }
}
