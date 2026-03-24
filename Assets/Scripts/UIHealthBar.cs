using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// ============================================================
// SCRIPT: UI_HEALTH_BAR (Versão Definitiva: Vida, Diálogo e Telas)
// ============================================================
public class UIHealthBar : MonoBehaviour
{
    public static UIHealthBar instance { get; private set; }

    [Header("Barra de Vida Rosa")]
    public Image healthBarImage; 

    [Header("Sistema de Diálogo")]
    public GameObject dialogBoxPanel;
    public Text dialogTextComponent;

    [Header("Telas de Finalização (Arraste aqui!)")]
    public GameObject winScreenPanel;
    public GameObject gameOverPanel;

    // --- VARIÁVEIS INTERNAS ---
    private float dialogTimer;
    private bool isDialogActive = false;

    // ============================================================
    // INICIALIZAÇÃO
    // ============================================================
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Esconder tudo o que não deve aparecer ao iniciar
        if (dialogBoxPanel != null) dialogBoxPanel.SetActive(false);
        if (winScreenPanel != null) winScreenPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        if (healthBarImage != null)
            healthBarImage.fillAmount = 1.0f;
            
        Debug.Log("UI: Sistema Completo Reinicializado!");
    }

    void Update()
    {
        if (isDialogActive)
        {
            dialogTimer -= Time.deltaTime;
            if (dialogTimer <= 0)
            {
                FecharDialogo();
            }
        }
    }

    // ============================================================
    // MÉTODOS DE VIDA E DIÁLOGO
    // ============================================================
    public void SetValue(float value)
    {
        if (healthBarImage != null)
            healthBarImage.fillAmount = Mathf.Clamp01(value);
    }

    public void MostrarDialogo(string texto)
    {
        if (dialogBoxPanel != null && dialogTextComponent != null)
        {
            dialogTextComponent.text = texto;
            dialogBoxPanel.SetActive(true);
            isDialogActive = true;
            dialogTimer = 4.0f;
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

    // ============================================================
    // MÉTODOS DE VITÓRIA E DERROTA
    // ============================================================
    public void MostrarTelaDeVitoria()
    {
        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(true);
            // Time.timeScale = 0f; // Opcional: Pausa o jogo ao vencer
            Debug.Log("UI: Parabéns ativado!");
        }
    }

    public void MostrarGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            // Time.timeScale = 0f; // Opcional: Pausa o jogo ao morrer
            Debug.Log("UI: Game Over ativado!");
        }
    }
}
