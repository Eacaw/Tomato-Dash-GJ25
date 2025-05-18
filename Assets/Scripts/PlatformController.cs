using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public float speed = 8f;
    public bool isInitialPlatform = false;

    void Update()
    {
        transform.parent.transform.Translate(Vector3.back * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DestroyTrigger"))
        {
            // TODO Use object pooling
            Destroy(transform.parent.gameObject);
        }
    }
}
