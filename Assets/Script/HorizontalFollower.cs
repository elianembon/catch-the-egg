using UnityEngine;

public class HorizontalFollower : MonoBehaviour
{
    public Transform target;
    private float fixedY;
    private float fixedZ;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("No egg");
            enabled = false;
            return;
        }

        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, fixedY, fixedZ);
        }
    }
}
