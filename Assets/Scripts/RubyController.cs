using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Garante que o Unity adiciona os componentes necessários automaticamente
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class RubyController : MonoBehaviour
{
    // --- SECÇÃO DE CONFIGURAÇÃO DE MOVIMENTO ---
    [Header("Mecânicas de Movimento")]
    [Tooltip("Velocidade máxima de caminhada")]
    public float speed = 4.0f;
    [Tooltip("Suavidade da aceleração e travagem")]
    public float acceleration = 12f;
    
    // --- SECÇÃO DE STATUS ---
    [Header("Mecânicas de Vida")]
    [Tooltip("Quantidade máxima de corações")]
    public int maxHealth = 5;
    [Tooltip("Tempo que a Ruby fica invulnerável após levar dano")]
    public float timeInvincible = 2.0f;
    
    // --- SECÇÃO DE COMBATE ---
    [Header("Mecânicas de Combate")]
    [Tooltip("O prefab da bala que a Ruby dispara")]
    public GameObject projectilePrefab;
    [Tooltip("Força com que a bala é expelida")]
    public float launchForce = 350f;
    [Tooltip("Sistema de partículas que aparece quando a Ruby se magoa")]
    public ParticleSystem hitParticle; 
    [Tooltip("Sistema de partículas para o rasto de poeira (Opcional)")]
    public ParticleSystem footstepParticle;

    // --- SECÇÃO DE ÁUDIO ---
    [Header("Sons e Feedback")]
    public AudioSource audioSource;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip walkSound; // Som de passos (Opcional)

    // --- PROPRIEDADES PÚBLICAS (GETTERS) ---
    // Permite que outros scripts leiam a vida, mas não a alterem diretamente
    public int health { get { return currentHealth; } }
    
    // --- VARIÁVEIS DE ESTADO INTERNO ---
    private int currentHealth;
    private bool isInvincible;
    private float invincibleTimer;
    
    // --- COMPONENTES CACHEADOS ---
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // --- VARIÁVEIS DE CÁLCULO ---
    private Vector2 lookDirection = new Vector2(1, 0);
    private Vector2 move;
    private float stepTimer; // Para controlar o som dos passos

    // ============================================================
    // INICIALIZAÇÃO
    // ============================================================
    void Awake()
    {
        // Guardar as referências dos componentes para performance
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Configurar o AudioSource se ele não estiver atribuído no Inspector
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Definir a vida inicial
        currentHealth = maxHealth;
        
        // CONFIGURAÇÕES FÍSICAS CRÍTICAS PARA ESCALA 60
        // Impede a Ruby de cair com a gravidade
        rb.gravityScale = 0;
        // Detecta colisões continuamente para não atravessar troncos
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        // Mantém o Rigidbody sempre acordado para colisões imediatas
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        // Suaviza o movimento visual entre frames
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        // Impede que a Ruby gire ao bater em quinas
        rb.freezeRotation = true;

        // Inicia na layer correta para interagir com o ambiente
        spriteRenderer.sortingLayerName = "Default";

        // Feedback inicial no console
        Debug.Log("Sistema Ruby Inicializado. Escala: " + transform.localScale.x);
    }

    // ============================================================
    // LOOP DE LÓGICA (INPUT E ANIMAÇÃO)
    // ============================================================
    void Update()
    {
        // 1. CAPTURA DE INPUT (TECLADO/COMANDO)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        move = new Vector2(horizontal, vertical);

        // Atualizar a direção para onde a Ruby está a olhar
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();

            // Emitir partículas de poeira se configuradas
            if (footstepParticle != null && !footstepParticle.isPlaying)
            {
                footstepParticle.Play();
            }
        }
        else if (footstepParticle != null)
        {
            footstepParticle.Stop();
        }

        // 2. GESTÃO DE ANIMAÇÕES
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        // 3. SISTEMA DE INVENCIBILIDADE (PISCAR)
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            // Efeito visual de Flicker usando PingPong na cor (Alpha/Transparência)
            spriteRenderer.color = new Color(1, 1, 1, Mathf.PingPong(Time.time * 25, 1));
            
            if (invincibleTimer < 0)
            {
                isInvincible = false;
                spriteRenderer.color = Color.white; // Volta ao normal
            }
        }

        // 4. MECÂNICA DE DISPARO (TECLA X)
        if (Input.GetKeyDown(KeyCode.X))
        {
            Launch();
        }

        // ============================================================
        // 5. O SEGREDO DA ÁRVORE (SORTEIO DINÂMICO)
        // Multiplicamos por -1 e somamos 1000.
        // O 1000 garante que ela nunca fica atrás do chão (Order 0).
        // Ao subir, o Y aumenta, o SortingOrder diminui -> Fica Atrás.
        // ============================================================
        if (spriteRenderer != null)
        {
            // O uso de RoundToInt evita que a Ruby trema visualmente
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -1.0f) + 1000;
        }

        // 6. SOM DE PASSOS (OPCIONAL)
        if (move.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer < 0)
            {
                // PlaySound(walkSound); // Descomenta se tiveres som de passos
                stepTimer = 0.35f; // Cadência dos passos
            }
        }
    }

    // ============================================================
    // LOOP DE FÍSICA (MOVIMENTAÇÃO)
    // ============================================================
    void FixedUpdate()
    {
        // Cálculo de velocidade alvo com interpolação para suavidade
        Vector2 targetVelocity = move * speed;
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        
        // SENSOR LASER (RAYCAST) PARA SEGURANÇA DE COLISÃO
        // Lança um pequeno laser à frente para garantir que deteta o tronco antes de entrar nele
        RaycastHit2D hit = Physics2D.Raycast(rb.position + Vector2.up * 0.2f, lookDirection, 0.5f, LayerMask.GetMask("Default"));
        if (hit.collider != null)
        {
            // Se estivermos quase a entrar dentro de um tronco, abrandamos
            if (hit.collider.CompareTag("Decoration")) { /* Logica extra se necessário */ }
        }
    }

    // ============================================================
    // SISTEMA DE SAÚDE E DANO
    // ============================================================
    public void ChangeHealth(int amount)
    {
        // Caso a Ruby sofra dano (amount é negativo)
        if (amount < 0)
        {
            if (isInvincible) return; // Proteção contra múltiplos hits rápidos
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            
            // Ativar animação de "Hit"
            animator.SetTrigger("Hit");
            
            // Tocar som de impacto
            PlaySound(hitSound);
            
            // Criar partículas de dor na posição da Ruby (subindo um pouco no Y)
            if(hitParticle != null) 
            {
                ParticleSystem p = Instantiate(hitParticle, rb.position + Vector2.up * 1.5f, Quaternion.identity);
                Destroy(p.gameObject, 2.0f); // Limpeza de memória
            }
            
            Debug.Log("Status: Ruby recebeu dano. HP: " + (currentHealth + amount));
        }
        else if (amount > 0)
        {
            Debug.Log("Status: Ruby foi curada. HP: " + (currentHealth + amount));
        }

        // Atualizar o valor matemático da vida garantindo os limites 0 e Max
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        // ENVIAR INFORMAÇÃO PARA A UI (BARRA ROSA)
        if (UIHealthBar.instance != null)
        {
            float fillRatio = (float)currentHealth / (float)maxHealth;
            UIHealthBar.instance.SetValue(fillRatio);
        }
    }

    // ============================================================
    // MECÂNICA DE ATAQUE
    // ============================================================
    void Launch()
    {
        // Se não houver prefab, não fazemos nada para evitar erros
        if (projectilePrefab == null)
        {
            Debug.LogError("Erro: Projectile Prefab em falta no RubyController!");
            return;
        }

        // Calcular a posição de spawn (Ligeiramente à frente e acima do centro da Ruby)
        // O multiplicador 2.8f é necessário devido à escala 60
        Vector2 spawnPos = rb.position + lookDirection * 2.8f + Vector2.up * 1.5f;
        
        // Criar o objeto no mundo
        GameObject projectileObject = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        
        // Aceder ao script da bala para lhe dar força
        BalaProjetil projectile = projectileObject.GetComponent<BalaProjetil>();
        
        if (projectile != null)
        {
            projectile.Lancar(lookDirection, launchForce);
            
            // Trigger da animação de lançamento
            animator.SetTrigger("Launch");
            
            // Feedback sonoro
            PlaySound(throwSound);
        }
        else
        {
            Debug.LogWarning("O prefab disparado não tem o script BalaProjetil!");
        }
    }

    // ============================================================
    // UTILITÁRIOS
    // ============================================================
    public void PlaySound(AudioClip clip)
    {
        // Toca o som sem interromper outros sons já a tocar
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Função de debug visual para ver os eixos no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, lookDirection * 2f);
    }
}