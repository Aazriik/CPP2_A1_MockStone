using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectibleUI : MonoBehaviour
{
    // Assign one of these in the inspector (UI Text or TextMeshPro). If left null,
    // the script will try to find a GameObject named "Collected" at runtime.
    public Text collectedText;
    public TMP_Text collectedTMP;

    private void Awake()
    {
        // Fallback search if references are missing in the Inspector
        if (collectedText == null && collectedTMP == null)
        {
            GameObject go = GameObject.Find("Collected");
            if (go != null)
            {
                collectedText = go.GetComponent<Text>();
                collectedTMP = go.GetComponent<TMP_Text>();
            }
        }
    }

    private void OnEnable()
    {
        // Subscribe to the pickup event
        PickupItem.OnCollectiblePickedUp += UpdateUI;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks when the UI is destroyed
        PickupItem.OnCollectiblePickedUp -= UpdateUI;
    }

    private void Start()
    {
        // Ensure the UI displays the correct amount immediately upon loading
        if (SaveManager.Instance != null)
        {
            UpdateUI(SaveManager.Instance.CurrentData.totalCollectedItems, 35);
        }
        else
        {
            Debug.LogWarning("SaveManager is missing. The UI will default to 0.");
        }
    }

    private void UpdateUI(int current, int max)
    {
        string text = current + "/" + max;

        if (collectedText != null)
        {
            collectedText.text = text;
        }

        if (collectedTMP != null)
        {
            collectedTMP.text = text;
        }
    }
}