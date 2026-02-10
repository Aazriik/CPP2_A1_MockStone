using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class WeaponBase : MonoBehaviour, IInteract
{

    [SerializeField] protected Vector3 localPositionValue = Vector3.zero;
    [SerializeField] protected Vector3 localRotationValue = new Vector3(-180, 0, 0);
    [SerializeField] protected float dropCooldown = 1.0f;
    [SerializeField] protected float dropForce = 2.0f;

    protected Rigidbody rb;
    protected Collider col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        
        if (col == null)
        {
            Debug.LogError("Collider component is missing on " + gameObject.name);
        }
    }

    public void Interact(PlayerController interactor)
    {
        rb.isKinematic = true;
        transform.SetParent(interactor.WeaponAttachPoint);
        transform.localPosition = localPositionValue;
        transform.localRotation = Quaternion.Euler(localRotationValue);
        Physics.IgnoreCollision(col, interactor.Collider, true);
    }

    public void Drop(Collider playerCollider)
    {
        rb.isKinematic = false;
        transform.SetParent(null);
        rb.AddForce(playerCollider.transform.forward * dropForce, ForceMode.Impulse);
        StartCoroutine(DropCooldown(playerCollider));
    }

    IEnumerator DropCooldown(Collider playerCollider)
    {
        yield return new WaitForSeconds(dropCooldown);
        Physics.IgnoreCollision(col, playerCollider, false);
    }
}
