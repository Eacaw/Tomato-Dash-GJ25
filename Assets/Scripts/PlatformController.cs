using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public float speed = 8f;

    void Update()
    {
        transform.parent.transform.Translate(Vector3.back * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DestroyTrigger"))
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
