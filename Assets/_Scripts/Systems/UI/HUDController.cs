using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image staminaFill;

    [Header("Default Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float maxStamina = 100f;

    public static HUDController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeHUD();
    }

    private void InitializeHUD()
    {
        if (SaveManager.Instance == null)
        {
            UpdateHealth(maxHealth);
            UpdateStamina(maxStamina);
            return;
        }

        GameSaveData data = SaveManager.Instance.CurrentData;

        int startHealth = data.currentHealth == -1 ? maxHealth : data.currentHealth;
        UpdateHealth(startHealth);

        float startStamina = data.currentStamina == -1f ? maxStamina : data.currentStamina;
        UpdateStamina(startStamina);
    }

    public void UpdateHealth(int currentHealth)
    {
        healthFill.fillAmount = (float)currentHealth / maxHealth;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentData.currentHealth = currentHealth;
        }
    }

    public void UpdateStamina(float currentStamina)
    {
        staminaFill.fillAmount = currentStamina / maxStamina;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentData.currentStamina = currentStamina;
        }
    }
}