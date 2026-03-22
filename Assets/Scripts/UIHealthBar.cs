using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIHealthBar : MonoBehaviour
{
    public static UIHealthBar instance { get; private set; }

    [Header("Vida")]
    public Image healthBarImage; 

    [Header("Diálogo")]
    public GameObject dialogBoxPanel;
    public Text dialogTextComponent;
    public float displayDuration = 4.0f;

    private float dialogTimer;
    private bool isDialogActive = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (healthBarImage != null)
        {
            healthBarImage.type = Image.Type.Filled;
            healthBarImage.fillMethod = Image.FillMethod.Horizontal;
        }

        // Começa escondido
        if (dialogBoxPanel != null) dialogBoxPanel.SetActive(false);
    }

    void Update()
    {
        if (isDialogActive)
        {
            dialogTimer -= Time.deltaTime;
            if (dialogTimer <= 0) FecharDialogo();
        }
    }

    public void SetValue(float value)
    {
        if (healthBarImage != null) healthBarImage.fillAmount = Mathf.Clamp01(value);
    }

    public void MostrarDialogo(string texto)
    {
        if (dialogBoxPanel != null && dialogTextComponent != null)
        {
            dialogTextComponent.text = texto;
            dialogBoxPanel.SetActive(true); // LIGA O BALÃO AZUL
            isDialogActive = true;
            dialogTimer = displayDuration;
        }
    }

    public void FecharDialogo()
    {
        if (dialogBoxPanel != null)
        {
            dialogBoxPanel.SetActive(false);
            isDialogActive = false;
        }
    }
}
