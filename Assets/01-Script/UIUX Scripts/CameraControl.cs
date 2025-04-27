using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform target;
    public float rotationSpeed = 5f;
    private float currentRotationX = 0f;

    void Update()
    {
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y");  

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            horizontal = touch.deltaPosition.x * 0.1f;
        }

        transform.RotateAround(target.position, Vector3.up, horizontal * rotationSpeed);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }

    public void SetRotationSpeed(float Speed)
    {
        rotationSpeed = Speed;
    }
}
