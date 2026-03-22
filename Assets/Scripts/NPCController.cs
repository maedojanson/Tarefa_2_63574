using UnityEngine;
using System.Collections;

// ============================================================
// SCRIPT: NPC_CONTROLLER (Mensagem Específica para a Ruby)
// ============================================================
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class NPCController : MonoBehaviour
{
    [Header("Identidade")]
    public string nomeDoNPC = "Marshmallow Inimigo";

    [Header("Configurações de Interação")]
    [Tooltip("Distância do círculo azul")]
    public float raioDeConversa = 3.5f;
    [Tooltip("Tecla para interagir (Diferente do X de tiro!)")]
    public KeyCode teclaFalar = KeyCode.E;

    // A tua mensagem personalizada com as quebras de linha (\n)
    private string mensagemUnica = "Olá Ruby! Para\nentrares na\ncasa tens de me\ncombater clicando\nna tecla \"E\"!";

    // --- VARIÁVEIS INTERNAS ---
    private bool rubyEstaPerto = false;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D sensor;

    // ============================================================
    // INICIALIZAÇÃO
    // ============================================================
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sensor = GetComponent<CircleCollider2D>();
        
        // Configuração automática do sensor circular
        if (sensor != null)
        {
            sensor.isTrigger = true; 
            sensor.radius = raioDeConversa;
        }
    }

    void Start()
    {
        Debug.Log("Sistema: O " + nomeDoNPC + " está pronto para desafiar a Ruby!");
    }

    // ============================================================
    // LOOP: DETECTAR INPUT
    // ============================================================
    void Update()
    {
        // Se a Ruby estiver na zona e carregar na tecla de falar (E)
        if (rubyEstaPerto && Input.GetKeyDown(teclaFalar))
        {
            FalarComARuby();
        }

        // Sistema de Profundidade (Sorting Order)
        if (spriteRenderer != null)
        {
            // Garante que o NPC e a Ruby se sobrepõem corretamente
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -1.0f) + 1000;
        }
    }

    // ============================================================
    // LÓGICA DE ENVIO DO TEXTO
    // ============================================================
    void FalarComARuby()
    {
        // Acedemos à UI (UIHealthBar) para mostrar o balão azul
        if (UIHealthBar.instance != null)
        {
            // Enviamos a tua mensagem exata
            UIHealthBar.instance.MostrarDialogo(mensagemUnica);
            
            Debug.Log("UI: Mensagem de combate enviada para o ecrã.");
        }
        else
        {
            Debug.LogError("ERRO: Não encontrei o script UIHealthBar no teu Canvas!");
        }
    }

    // ============================================================
    // SENSORES (TRIGGERS)
    // ============================================================
    void OnTriggerEnter2D(Collider2D other)
    {
        // Só deteta se for o Player (Ruby)
        if (other.CompareTag("Player"))
        {
            rubyEstaPerto = true;
            Debug.Log("Interação: Ruby está perto do Marshmallow.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            rubyEstaPerto = false;
            
            // Se a Ruby se afastar, o balão azul fecha-se
            if (UIHealthBar.instance != null)
            {
                UIHealthBar.instance.FecharDialogo();
            }
        }
    }

    // Desenha o círculo de alcance no Editor para ajudar
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, raioDeConversa);
    }
}
