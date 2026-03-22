using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class InimigoMarshmallow : MonoBehaviour
{
    [Header("Configuração de Vida")]
    public int maxVida = 3;
    public float forcaSalto = 6.0f;
    
    [Header("Efeitos Visuais")]
    [Tooltip("Partículas que saem em CADA tiro")]
    public GameObject particulaDanoPrefab;
    [Tooltip("Partículas que saem quando ele MORRE")]
    public GameObject particulaMortePrefab;

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip somDano;
    public AudioClip somMorte;

    private int vidaAtual;
    private Rigidbody2D rb2d;
    private Animator animator;
    private bool estaMorto = false;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        vidaAtual = maxVida;
        rb2d.gravityScale = 0;
        rb2d.freezeRotation = true;
    }

    public void TomarDano(int quantidade, Vector2 direcaoDano)
    {
        if (estaMorto) return;

        vidaAtual -= quantidade;

        // 1. PARTÍCULAS DE DANO (Sprinkles em cada tiro!)
        if (particulaDanoPrefab != null)
        {
            GameObject p = Instantiate(particulaDanoPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            p.transform.localScale = Vector3.one * 2.0f;
            Destroy(p, 1.5f);
        }

        // 2. ANIMAÇÃO E SOM
        if (animator != null) animator.SetTrigger("Hit");
        if (somDano != null) audioSource.PlayOneShot(somDano);
        
        // 3. MOVIMENTO (Knockback)
        rb2d.velocity = Vector2.zero;
        rb2d.AddForce(direcaoDano * forcaSalto, ForceMode2D.Impulse);

        if (vidaAtual <= 0) Morrer();
    }

    private void Morrer()
    {
        if (estaMorto) return;
        estaMorto = true;

        // PARTÍCULAS DE VITÓRIA (O Z-Z-Z ou Explosão Final)
        if (particulaMortePrefab != null)
        {
            GameObject p = Instantiate(particulaMortePrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            p.transform.localScale = Vector3.one * 3.5f;
            Destroy(p, 3.0f);
        }

        if (somMorte != null) AudioSource.PlayClipAtPoint(somMorte, transform.position);

        // Animação final
        if (animator != null) animator.SetTrigger("Fixed");

        // Limpeza
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false; // Ele some mas as partículas ficam!
        Destroy(gameObject, 0.5f);
    }
}
