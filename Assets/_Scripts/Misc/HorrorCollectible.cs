using UnityEngine;

public class Collectible : MonoBehaviour
{
    public EnemyAI enemyAI;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAI.OnCollectiblePickedUp(transform.position);
            Destroy(gameObject); // Remove collectible
        }
    }
}
