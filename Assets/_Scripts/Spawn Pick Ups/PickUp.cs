using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickupItem : MonoBehaviour
{
    public int value = 1; // how much this item gives

    // Assign one of these in the inspector (UI Text or TextMeshPro). If left null,
    // the script will try to find a GameObject named "Collected" at runtime.
    public Text collectedText;
    public TMP_Text collectedTMP;

    private static int collectedCount = 0;

    private void Start()
    {
        UpdateCollectedText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collectedCount += value;
            Debug.Log("Picked up item! Collected: " + collectedCount);
            UpdateCollectedText();

            Destroy(gameObject);
        }
    }

    private void UpdateCollectedText()
    {
        // Update the UI text to show the current collected count out of 35
        string text = collectedCount + "/35";

        if (collectedText != null)
            collectedText.text = text;

        if (collectedTMP != null)
            collectedTMP.text = text;

        // If no reference assigned, try to find a GameObject named "Collected" and use its text component
        if (collectedText == null && collectedTMP == null)
        {
            var go = GameObject.Find("Collected");
            if (go != null)
            {
                var t = go.GetComponent<Text>();
                if (t != null)
                {
                    collectedText = t;
                    collectedText.text = text;
                    return;
                }

                var tt = go.GetComponent<TMP_Text>();
                if (tt != null)
                {
                    collectedTMP = tt;
                    collectedTMP.text = text;
                    return;
                }
            }
        }
    }
}