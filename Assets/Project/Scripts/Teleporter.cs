using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField]
    private Transform destination;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }

        other.transform.position = destination.position;
    }
}
