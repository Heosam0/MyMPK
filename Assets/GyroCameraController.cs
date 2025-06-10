using UnityEngine;

public class GyroCameraController : MonoBehaviour
{
    [SerializeField] private GameObject button;
    private bool gyroEnabled = false;
    private Gyroscope gyro;

    void Start()
    {
        

        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
        }
        else
        {
            button.SetActive(false);
        }
    }

    public void SwitchState()
    {
          gyroEnabled = !gyroEnabled;
    }

    void Update()
    {
        if (SystemInfo.supportsGyroscope && gyroEnabled)
        {
            Input.gyro.enabled = true;
            transform.rotation = Quaternion.Euler(90, 0, 0) * GyroToUnity(Input.gyro.attitude);
        }
    }
    private static Quaternion GyroToUnity(Quaternion q) => new Quaternion(q.x, q.y, -q.z, -q.w);
}