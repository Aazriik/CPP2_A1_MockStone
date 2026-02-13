using UnityEngine;

public class KillZone : MonoBehaviour
{
    [Tooltip("If set, only objects with this tag will be killed.")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(playerTag) && !other.CompareTag(playerTag))
            return;


        var health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Die();
            return;
        }

        Destroy(other.gameObject);
    }
}