using UnityEngine;

public class Gun : WeaponBase
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform muzzlePoint;

    [SerializeField] private AudioClip gunshotSound;

    public void Fire()
    {
        GameObject proj = Instantiate(bullet, muzzlePoint.position, muzzlePoint.rotation);

        Projectile projectileScript = proj.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.SetDirection(muzzlePoint.forward);
        }

        if (gunshotSound != null)
        {
            AudioSource.PlayClipAtPoint(gunshotSound, muzzlePoint.position);
        }
    }

    /* Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}