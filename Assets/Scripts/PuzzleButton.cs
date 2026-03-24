using UnityEngine;
using System.Collections;

// ============================================================
// SCRIPT: PUZZLE_BUTTON (Versão com Diálogo Formatado)
// ============================================================
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PuzzleButton : MonoBehaviour
{
    [Header("Configurações do Mecanismo")]
    public Color corAtivado = new Color(0.5f, 1f, 0.5f, 1f); // Verde
    public Color corOriginal = Color.white;
    public AudioClip somMecanismo;
    
    [Header("Animação da Porta (Escuridão)")]
    [Tooltip("Arraste aqui o objeto 'EscuridaoPorta' que está na moldura")]
    public Transform portaAbertaTransform;
    
    [Tooltip("Velocidade (Escala 60 pede valores altos como 80 ou 100)")]
    public float velocidadeAbertura = 80.0f;
    
    [Tooltip("Escala Final (Usa o valor Y original da porta, ex: 45)")]
    public float escalaFinalY = 45.0f;

    // --- VARIÁVEIS INTERNAS ---
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool estaPressionado = false;
    private int contadorObjetos = 0;
    private Coroutine animacaoPorta;

    // ============================================================
    // INICIALIZAÇÃO
    // ============================================================
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        
        // Garante que o botão é um sensor (Trigger)
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null) col.isTrigger = true; 
    }

    void Start()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = corOriginal;
            // Botão fica no "chão" (-10) para não tapar a Ruby
            spriteRenderer.sortingOrder = -10;
        }

        // A escuridão começa com escala 0 (fechada)
        if (portaAbertaTransform != null)
        {
            portaAbertaTransform.localScale = new Vector3(portaAbertaTransform.localScale.x, 0f, 1f);
        }
    }

    // --- DETECÇÃO ---
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Caixa") || other.name.Contains("Square"))
        {
            contadorObjetos++;
            if (!estaPressionado) AtivarMecanismo();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Caixa") || other.name.Contains("Square"))
        {
            contadorObjetos--;
            if (contadorObjetos <= 0)
            {
                contadorObjetos = 0;
                DesativarMecanismo();
            }
        }
    }

    // ============================================================
    // AÇÕES DO MECANISMO
    // ============================================================
    void AtivarMecanismo()
    {
        estaPressionado = true;
        if (spriteRenderer != null) spriteRenderer.color = corAtivado;
        if (somMecanismo != null) audioSource.PlayOneShot(somMecanismo);
        
        // Iniciar animação da porta
        if (portaAbertaTransform != null)
        {
            if (animacaoPorta != null) StopCoroutine(animacaoPorta);
            animacaoPorta = StartCoroutine(AnimaPorta(escalaFinalY));
        }

        // --- MENSAGEM COM QUEBRA DE LINHA (\n) ---
        if (UIHealthBar.instance != null)
        {
            // O "\n" faz o texto saltar para a linha de baixo exatamente ali!
            UIHealthBar.instance.MostrarDialogo("O mecanismo da\ncasa foi ativado!");
        }
    }

    void DesativarMecanismo()
    {
        estaPressionado = false;
        if (spriteRenderer != null) spriteRenderer.color = corOriginal;
        
        if (portaAbertaTransform != null)
        {
            if (animacaoPorta != null) StopCoroutine(animacaoPorta);
            animacaoPorta = StartCoroutine(AnimaPorta(0f));
        }
    }

    // COROUTINE DE ANIMAÇÃO
    IEnumerator AnimaPorta(float targetScaleY)
    {
        Vector3 currentScale = portaAbertaTransform.localScale;
        
        while (Mathf.Abs(currentScale.y - targetScaleY) > 0.1f)
        {
            float newScaleY = Mathf.MoveTowards(currentScale.y, targetScaleY, velocidadeAbertura * Time.deltaTime);
            portaAbertaTransform.localScale = new Vector3(currentScale.x, newScaleY, 1f);
            
            currentScale = portaAbertaTransform.localScale;
            yield return null; 
        }
        
        portaAbertaTransform.localScale = new Vector3(currentScale.x, targetScaleY, 1f);
    }
}