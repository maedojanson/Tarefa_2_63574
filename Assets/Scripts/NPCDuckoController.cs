using UnityEngine;

// ============================================================================
// SCRIPT: NPCDuckoController
// DESCRIÇÃO: Abre a caixa de texto automaticamente quando a Ruby se aproxima.
// ============================================================================

public class NPCDuckoController : MonoBehaviour
{
    [Header("DIÁLOGO")]
    public GameObject dialogBox;
    public float displayTime = 4.0f;
    private float timerDisplay;

    void Start()
    {
        if (dialogBox != null) dialogBox.SetActive(false);
        timerDisplay = -1.0f;
    }

    void Update()
    {
        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                dialogBox.SetActive(false);
            }
        }
    }

    // FUNÇÃO QUE ATIVA O TEXTO ✅
    public void DisplayDialog()
    {
        timerDisplay = displayTime;
        if (dialogBox != null) dialogBox.SetActive(true);
    }

    // DETECÇÃO AUTOMÁTICA (TRIGGER) ✅
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se quem entrou na zona do Ducko foi o "Player"
        if (other.CompareTag("Player"))
        {
            DisplayDialog();
            Debug.Log("Ruby aproximou-se! Ducko a falar automaticamente.");
        }
    }
}
