using UnityEngine;
using TMPro;

public class CollectibleUI : MonoBehaviour
{
    public TMP_Text collectedTMP;

    private void OnEnable() => CollectiblePickup.OnCollectiblePickedUp += UpdateUI;
    private void OnDisable() => CollectiblePickup.OnCollectiblePickedUp -= UpdateUI;

    private void Start()
    {
        if (SaveManager.Instance != null)
        {
            UpdateUI(SaveManager.Instance.CurrentData.totalCollectedItems, 35);
        }
    }

    private void UpdateUI(int current, int max)
    {
        if (collectedTMP != null)
        {
            collectedTMP.text = $"{current} / {max}";
        }
    }
}