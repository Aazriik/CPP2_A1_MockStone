using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public int value = 1; // how much this item gives

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Picked up item!");

            Destroy(gameObject);
        }
    }
}
