using UnityEngine;

public class InimigoController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float speed = 2.0f;
    public bool vertical; // Marca no Inspector se queres que ele ande para cima/baixo
    public float changeTime = 3.0f; // Tempo até ele mudar de direção

    // Componentes (RB2D mudado para evitar o aviso amarelo)
    Rigidbody2D rb2d;
    Animator animator;
    SpriteRenderer spriteRenderer;

    // Variáveis de controle
    float timer;
    int direction = 1;
    
    void Start()
    {
        // Pegar os componentes do Marshmallow
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        timer = changeTime;

        // Dica da Bestie: O Body Type deve ser Kinematic para ele não ser empurrado!
    }

    void Update()
    {
        // Contagem para mudar de direção
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }
    
    void FixedUpdate()
    {
        Vector2 position = rb2d.position;
        Vector2 movementDirection = Vector2.zero;

        // 1. MOVIMENTO E ANIMAÇÃO
        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            movementDirection.y = direction;
            
            if (animator != null)
            {
                animator.SetFloat("Move X", 0);
                animator.SetFloat("Move Y", direction);
            }
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            movementDirection.x = direction;
            
            if (animator != null)
            {
                animator.SetFloat("Move X", direction);
                animator.SetFloat("Move Y", 0);
            }
        }
        
        rb2d.MovePosition(position);

        // 2. LÓGICA DO FLIP (Olhar para a Direita/Esquerda)
        if (!vertical && spriteRenderer != null)
        {
            // Se direction for 1 (Direita), flipX fica True
            // Se direction for -1 (Esquerda), flipX fica False
            spriteRenderer.flipX = (direction > 0);
        }
    }

    // 3. LÓGICA DE DANO (Quando choca com a Ruby)
    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            // Tira 1 de vida à Ruby através do script dela
            player.ChangeHealth(-1); 
            Debug.Log("💥 O Marshmallow deu um encontrão na Ruby!");
        }
    }
}
