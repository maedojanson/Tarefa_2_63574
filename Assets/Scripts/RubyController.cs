using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ============================================================================
// PROJETO: RUBY'S ADVENTURE - TAREFA 2 (SUGARLAND EDITION)
// SCRIPT: RubyController
// DESCRIÇÃO: Gere movimento, vida, sistema de combo e PROFUNDIDADE DINÂMICA.
// ============================================================================

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class RubyController : MonoBehaviour
{
    #region VARIÁVEIS PÚBLICAS (INSPECTOR)
    
    [Header("CONFIGURAÇÕES DE MOVIMENTO")]
    public float speed = 4.0f;
    
    [Header("SISTEMA DE SAÚDE")]
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
    
    [Header("SISTEMA DE COMBATE (TECLA X)")]
    public GameObject projectilePrefab;
    public float launchForce = 350f;
    
    [Header("EFEITOS E SONS")]
    public AudioSource audioSource;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public ParticleSystem hitParticle;

    #endregion

    #region VARIÁVEIS PRIVADAS E ESTADOS
    
    public int health { get { return currentHealth; } }
    private int currentHealth;
    private bool isInvincible;
    private float invincibleTimer;
    private bool isDead = false;
    private int contadorCliquesX = 0;
    
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    private Vector2 lookDirection = new Vector2(1, 0);
    private Vector2 move;

    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        rb.gravityScale = 0;

        if (UIHealthBar.instance != null)
            UIHealthBar.instance.SetValue(1.0f);
        
        // Garante que o ponto de referência são os pés
        spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
        
        Debug.Log("RubyController: Sistema de Profundidade Ajustado para 1000/2000.");
    }

    void Update()
    {
        if (isDead) return;

        // 1. INPUT
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        // 2. ANIMAÇÕES
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        // 3. INVENCIBILIDADE
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            float alpha = Mathf.PingPong(Time.time * 25, 1.0f);
            spriteRenderer.color = new Color(1, 1, 1, alpha);
            if (invincibleTimer < 0) { isInvincible = false; spriteRenderer.color = Color.white; }
        }

        // 4. DISPARO
        if (Input.GetKeyDown(KeyCode.X)) ProcessarTiroCombo();

        // ============================================================
        // 5. O AJUSTE FINAL DE PROFUNDIDADE (PARA A BESTIE!)
        // ============================================================
        // Como as tuas árvores estão em 1000 e 2000:
        // Se a Ruby estiver "acima" das árvores de cima, ela fica em 900 (atrás).
        // Se estiver no meio, fica em 1500.
        // Se estiver abaixo das de baixo, fica em 2100.
        
        // Vamos usar uma lógica simples:
        if (transform.position.y > 115) // Ela está lá em cima perto das árvores de 1000
        {
            spriteRenderer.sortingOrder = 900; 
        }
        else if (transform.position.y < 95) // Ela está cá em baixo perto das de 2000
        {
            spriteRenderer.sortingOrder = 2100;
        }
        else // Ela está no caminho do meio
        {
            spriteRenderer.sortingOrder = 1500;
        }
        
        // DOCUMENTAÇÃO:
        // Árvore Cima = 1000 | Árvore Baixo = 2000
        // Ruby topo = 900 (Atrás da de 1000)
        // Ruby meio = 1500 (À frente da de 1000, atrás da de 2000)
        // Ruby base = 2100 (À frente da de 2000)
        // ..........................................................................
        // ..........................................................................
    }

    void FixedUpdate()
    {
        if (isDead) return;
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }

    public void ChangeHealth(int amount)
    {
        if (isDead) return;
        if (amount < 0)
        {
            if (isInvincible) return;
            isInvincible = true;
            invincibleTimer = timeInvincible;
            animator.SetTrigger("Hit");
            PlaySound(hitSound);
            if(hitParticle != null) {
                ParticleSystem p = Instantiate(hitParticle, rb.position + Vector2.up * 0.5f, Quaternion.identity);
                Destroy(p.gameObject, 1.5f);
            }
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (UIHealthBar.instance != null) UIHealthBar.instance.SetValue((float)currentHealth / maxHealth);
        if (currentHealth <= 0) Morrer();
    }

    void Morrer()
    {
        isDead = true;
        rb.velocity = Vector2.zero; 
        if (UIHealthBar.instance != null) UIHealthBar.instance.MostrarGameOver();
    }

    void ProcessarTiroCombo()
    {
        contadorCliquesX++;
        if (contadorCliquesX < 3) LaunchNormal();
        else { MorteAutomaticaInimigo(); contadorCliquesX = 0; }
    }

    void LaunchNormal()
    {
        if (projectilePrefab == null) return;
        Vector2 spawnPos = rb.position + lookDirection * 1.5f + Vector2.up * 0.5f;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        BalaProjetil p = proj.GetComponent<BalaProjetil>();
        if (p != null) { p.Lancar(lookDirection, launchForce); PlaySound(throwSound); }
    }

    void MorteAutomaticaInimigo()
    {
        InimigoController[] inimigos = FindObjectsOfType<InimigoController>();
        if (inimigos.Length > 0) inimigos[0].Consertar();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null) audioSource.PlayOneShot(clip);
    }
}