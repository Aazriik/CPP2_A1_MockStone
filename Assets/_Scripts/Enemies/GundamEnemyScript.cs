using UnityEngine;

public class GundamEnemyScript : MonoBehaviour
{
    public float speed = 2f;
    public float distance = 3f;

    public Vector3 direction = new Vector3(1f, 0f, 1f);

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        direction = direction.normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(startPos, transform.position) >= distance)
        {
            direction *= -1;
        }
    }
}
