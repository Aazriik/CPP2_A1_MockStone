// Mockstone
using UnityEngine;

public class Interactable : MonoBehaviour, IInteract
{
    public void Interact(PlayerController interactor)
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}
