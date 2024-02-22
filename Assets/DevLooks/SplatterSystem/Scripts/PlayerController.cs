using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform target;
    public float distance = 3.0f;
    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;
    public float yMinLimit = -20;
    public float yMaxLimit = 80;
    public float speed = 10.0f;

    private float x = 0.0f;
    private float y = 0.0f;
    private Quaternion targetCurrentRotation;

    void Start()
    {
        targetCurrentRotation = target.rotation;
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }


    void LateUpdate()
    {
        if (target)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

            transform.rotation = rotation;
            transform.position = position;

            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Quaternion targetRotation = Quaternion.Euler(0, x, 90f);
            Vector3 movement = new Vector3(0.0f, -moveHorizontal, moveVertical);
            movement = target.transform.TransformDirection(movement);

            target.rotation = targetRotation;
            target.position = target.position + movement * speed * Time.deltaTime;
        }
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
