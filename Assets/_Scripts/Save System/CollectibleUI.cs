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
            // First try to find the object under a parent named "UI" (path: "UI/Collected")
            GameObject go = GameObject.Find("UI/Collected");

            // If that fails, try locating a parent named "UI" and search its children
            if (go == null)
            {
                GameObject uiRoot = GameObject.Find("UI");
                if (uiRoot != null)
                {
                    var child = uiRoot.transform.Find("Collected");
                    if (child != null) go = child.gameObject;
                }
            }

            // Final fallback: search globally for "Collected"
            if (go == null) go = GameObject.Find("Collected");

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