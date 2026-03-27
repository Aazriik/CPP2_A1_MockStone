using UnityEngine;
using System;

public class CollectiblePickup : PickupBase
{
    public int scoreValue = 1;
    public static event Action<int, int> OnCollectiblePickedUp;
    private const int MaxCollectibles = 5;

    [SerializeField] private AudioClip pickupSound;

    protected override void Start()
    {
        base.Start();

        if (SaveManager.Instance != null)
        {
            OnCollectiblePickedUp?.Invoke(SaveManager.Instance.CurrentData.totalCollectedItems, MaxCollectibles);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int currentTotal = 0;
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.CurrentData.totalCollectedItems += scoreValue;
                currentTotal = SaveManager.Instance.CurrentData.totalCollectedItems;
            }

            OnCollectiblePickedUp?.Invoke(currentTotal, MaxCollectibles);

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            MarkAsCollectedAndDestroy();
        }
    }
}