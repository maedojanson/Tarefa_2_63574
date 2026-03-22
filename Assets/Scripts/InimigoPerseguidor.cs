using UnityEngine;

public class InimigoPerseguidor : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 3.0f; 
    public float distanceToAttack = 20.0f; 

    // Referências
    Rigidbody2D rb2d;
    Animator animator;
    SpriteRenderer spriteRenderer;
    Transform rubyTransform;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // MÉTODO INFALÍVEL: Procura qualquer objeto que tenha o script RubyController
        RubyController rubyScript = Object.FindFirstObjectByType<RubyController>();
        
        if (rubyScript != null)
        {
            rubyTransform = rubyScript.transform;
            Debug.Log("🎯 BESTIE, ACHEI A RUBY! Vou atrás dela!");
        }
        else
        {
            Debug.LogError("🚨 ERRO: Não encontrei a Ruby! Garante que ela tem o script 'RubyController' ligado a ela.");
        }
    }

    void Update()
    {
        // Se não encontrou a Ruby, não faz nada
        if (rubyTransform == null) return;

        // Calcula a distância real (2D)
        float distance = Vector2.Distance(transform.position, rubyTransform.position);
        
        Vector2 moveDirection = Vector2.zero;

        // Se estiver perto o suficiente, PERSEGUE!
        if (distance < distanceToAttack)
        {
            // Calcula para onde ir
            moveDirection = ((Vector2)rubyTransform.position - (Vector2)transform.position).normalized;

            // Move o Marshmallow usando a física
            rb2d.MovePosition(rb2d.position + moveDirection * speed * Time.deltaTime);
        }

        // --- ATUALIZAÇÃO DO ANIMATOR ---
        if (animator != null)
        {
            animator.SetFloat("Speed", moveDirection.magnitude);
            if (moveDirection.magnitude > 0.1f)
            {
                animator.SetFloat("Move X", moveDirection.x);
                animator.SetFloat("Move Y", moveDirection.y);
            }
        }

        // --- FLIP DO SPRITE ---
        if (spriteRenderer != null && moveDirection.x != 0)
        {
            spriteRenderer.flipX = (moveDirection.x > 0);
        }
    }

    // Lógica de Dano
    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController ruby = other.gameObject.GetComponent<RubyController>();
        if (ruby != null)
        {
            ruby.ChangeHealth(-1);
            Debug.Log("💥 En encontrão no Marshmallow!");
        }
    }
}