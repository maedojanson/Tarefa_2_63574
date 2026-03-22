using UnityEngine;
using System.Collections;

// ============================================================
// SCRIPT: BALA PROJETIL (Versão com Efeitos Independentes)
// ============================================================
[RequireComponent(typeof(Rigidbody2D))]
public class BalaProjetil : MonoBehaviour
{
    // --- CONFIGURAÇÃO DE STATUS ---
    [Header("Status do Projétil")]
    public int dano = 1;
    public float tempoDeVida = 3.0f;
    
    // --- CONFIGURAÇÃO DE EFEITOS ---
    [Header("Efeitos Visuais de Impacto")]
    [Tooltip("Arraste aqui o Prefab ParticlesSpriteAtlasSprinkles")]
    public GameObject efeitoExplosao;
    [Tooltip("Tamanho das partículas para a Escala 60")]
    public float escalaParticula = 3.0f;

    [Header("Efeitos Sonoros")]
    public AudioClip somImpacto;

    // --- VARIÁVEIS INTERNAS ---
    private Rigidbody2D rb;
    private bool jaBateu = false;

    // ============================================================
    // INICIALIZAÇÃO
    // ============================================================
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // CONFIGURAÇÕES DE FÍSICA PARA ESCALA 60
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        // Destruição automática por tempo
        Destroy(gameObject, tempoDeVida);
    }

    // ============================================================
    // LANÇAMENTO (Chamado pela Ruby)
    // ============================================================
    public void Lancar(Vector2 direcao, float forca)
    {
        // Aplicar velocidade ajustada à escala
        rb.velocity = direcao * (forca / 35f);

        // Rodar a bala para a direção certa
        float angle = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // ============================================================
    // DETECÇÃO DE IMPACTO
    // ============================================================
    void OnTriggerEnter2D(Collider2D other)
    {
        // Se já bateu em algo neste frame, ignora o resto
        if (jaBateu) return;

        // 1. VERIFICAR SE É O INIMIGO
        InimigoMarshmallow inimigo = other.GetComponent<InimigoMarshmallow>();
        if (inimigo != null)
        {
            // Envia o dano para o inimigo
            inimigo.TomarDano(dano, rb.velocity.normalized);
            Explodir();
            return;
        }

        // 2. VERIFICAR SE É AMBIENTE (Paredes/Árvores)
        if (other.CompareTag("Wall") || other.name.Contains("Tree") || other.name.Contains("Decoration"))
        {
            Explodir();
        }
    }

    // ============================================================
    // SISTEMA DE EXPLOSÃO (Onde as partículas nascem!)
    // ============================================================
    void Explodir()
    {
        jaBateu = true;

        // PARTE A: AS PARTÍCULAS (O SEU SPRINKLES)
        if (efeitoExplosao != null)
        {
            // INSTANCIAR NO MUNDO (Sair de 'baixo' da bala para não morrer com ela)
            GameObject p = Instantiate(efeitoExplosao, transform.position, Quaternion.identity);
            
            // Forçar a escala para ser bem visível na escala 60
            p.transform.localScale = Vector3.one * escalaParticula;
            
            // Garantir que as partículas se limpam sozinhas
            Destroy(p, 2.0f);
            
            Debug.Log("Sistema: Partículas Sprinkles criadas no impacto.");
        }
        else
        {
            Debug.LogWarning("Aviso: Falta arrastar o Prefab Sprinkles para a Bala!");
        }

        // PARTE B: SOM
        if (somImpacto != null)
        {
            // Tocar som na posição exata para não ser cortado
            AudioSource.PlayClipAtPoint(somImpacto, transform.position, 1.0f);
        }

        // PARTE C: DESTRUIÇÃO DA BALA
        // Destruímos a bala, mas as partículas (p) continuam vivas no mapa!
        Destroy(gameObject);
    }
}