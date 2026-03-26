using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] private float speed = 20f; //projectile/bullet speed when firing
    [SerializeField] private float lifetime = 5f; //how long the bullet/projectile has before being destroyed

    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
