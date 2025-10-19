using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;  // Nhân vật cần theo dõi

    [Header("Camera Offset & Movement")]
    public Vector3 offset = new Vector3(0f, 1.4f, -3.2f);
    public float smoothSpeed = 10f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;
    public float minYAngle = -25f;
    public float maxYAngle = 55f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        // Lấy input chuột
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, minYAngle, maxYAngle);

        // Tính góc xoay camera
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0f);

        // Vị trí mong muốn của camera (xoay quanh nhân vật)
        Vector3 desiredPosition = target.position + rotation * offset;

        // Di chuyển mượt mà
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Hướng camera nhìn vào thân nhân vật (không quá cao)
        transform.LookAt(target.position + Vector3.up * 1.0f);
    }
}
