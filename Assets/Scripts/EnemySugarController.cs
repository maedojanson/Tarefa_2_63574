using UnityEngine;

// ============================================================================
// SCRIPT: EnemySugarController
// DESCRIÇÃO: Movimento, dano, conserto, sons e SISTEMA DE FUMAÇA COM ESCALA.
// ============================================================================

[RequireComponent(typeof(AudioSource))]
public class EnemySugarController : MonoBehaviour
{
    [Header("CONFIGURAÇÕES DE MOVIMENTO")]
    public float velocidade = 5.0f;
    public float amplitude = 5.0f;
    
    [Header("EFEITOS E SONS")]
    public AudioClip somPatrulha;      
    public AudioClip somConsertado;    
    public ParticleSystem fumacaPrefab; 
    
    private Animator animator;
    private AudioSource audioSource;
    private Vector3 posicaoInicial;
    private int direcao = 1;
    private bool estaConsertado = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        posicaoInicial = transform.position;

        if (somPatrulha != null)
        {
            audioSource.clip = somPatrulha;
            audioSource.loop = true;
            audioSource.playOnAwake = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (estaConsertado) return;

        float novoY = transform.position.y + (direcao * velocidade * Time.deltaTime);
        if (novoY >= posicaoInicial.y + amplitude) direcao = -1;
        else if (novoY <= posicaoInicial.y - amplitude) direcao = 1;

        transform.position = new Vector3(transform.position.x, novoY, transform.position.z);
        if (animator != null) animator.SetFloat("MoveY", (float)direcao);
    }

    public void Consertar()
    {
        if (estaConsertado) return;
        estaConsertado = true;
        
        // 1. SONS
        audioSource.Stop();
        if (somConsertado != null) audioSource.PlayOneShot(somConsertado);

        // 2. FUMAÇA AJUSTADA PARA A TUA ESCALA (39!) ✅
        if (fumacaPrefab != null)
        {
            // Criamos a fumaça
            ParticleSystem p = Instantiate(fumacaPrefab, transform.position + Vector3.up * 5f, Quaternion.identity);
            
            // FORÇAMOS A FUMAÇA A FICAR GIGANTE IGUAL AO ROBÔ ✅
            // Como o teu robô é scale 39, a fumaça também precisa de ser grande!
            p.transform.localScale = transform.localScale; 
            
            p.Play();
            Destroy(p.gameObject, 2.0f);
        }

        // 3. ANIMAÇÃO E FÍSICA
        if (animator != null) animator.SetTrigger("Fixed");
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !estaConsertado)
        {
            RubyController player = other.gameObject.GetComponent<RubyController>();
            if (player != null) player.ChangeHealth(-1);
        }

        if (other.gameObject.CompareTag("Projectile"))
        {
            Consertar();
            Destroy(other.gameObject);
        }
    }
}
