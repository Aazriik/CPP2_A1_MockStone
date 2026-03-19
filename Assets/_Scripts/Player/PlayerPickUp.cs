using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    public int resourceCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            resourceCount += 1;
            Destroy(other.gameObject);
        }
    }
}