using UnityEngine;
using System.Collections;

// ============================================================
// SCRIPT: ENTRADA_CASA_TRIGGER (Versão de Teste Direto)
// ============================================================
[RequireComponent(typeof(BoxCollider2D))]
public class EntradaCasaTrigger : MonoBehaviour
{
    [Header("Mecânica da Vitória")]
    [Tooltip("Arraste aqui o objeto PRETO (EscuridaoPorta)")]
    public Transform portaAbertaTransform;
    [Tooltip("Altura para detetar vitória")]
    public float alturaMinimaDeAbertura = 40.0f;

    private bool jogoAcabou = false;

    void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    // ============================================================
    // TRIGGERS (SENSORES)
    // ============================================================
    void OnTriggerEnter2D(Collider2D other)
    {
        // Se a Ruby tocar na porta, verificamos a vitória
        if (other.CompareTag("Player") && !jogoAcabou)
        {
            // TESTE: Vamos ver se o objeto da porta foi arrastado
            if (portaAbertaTransform == null)
            {
                Debug.LogError("ERRO: Esqueceste-te de arrastar a EscuridaoPorta para o script EntradaCasa!");
                return;
            }

            // Se a porta estiver aberta (Escala Y maior que 40)
            if (portaAbertaTransform.localScale.y >= alturaMinimaDeAbertura)
            {
                ChamarVitoria();
            }
            else 
            {
                Debug.Log("Ruby: A porta ainda está fechada!");
            }
        }
    }

    // ============================================================
    // O FIM DO JOGO
    // ============================================================
    void ChamarVitoria()
    {
        jogoAcabou = true;
        Debug.Log("VITÓRIA DETETADA!");
        
        // Bloquear a Ruby
        RubyController ruby = FindObjectOfType<RubyController>();
        if (ruby != null)
        {
            ruby.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            ruby.enabled = false;
        }

        // CHAMAR A TELA DE PARABÉNS!
        if (UIHealthBar.instance != null)
        {
            UIHealthBar.instance.MostrarTelaDeVitoria();
        }
        else
        {
            Debug.LogError("ERRO: O script UIHealthBar não está ativo ou não tem o WinScreenPanel arrastado!");
        }
    }
}
