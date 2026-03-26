using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ============================================================================
// PROJETO: RUBY'S ADVENTURE - TAREFA 2 (SUGARLAND)
// SCRIPT: InimigoController (Sincronizado com a Ruby)
// ============================================================================

public class InimigoController : MonoBehaviour
{
    [Header("CONFIGURAÇÕES DE PATRULHA")]
    public float speed = 2.0f;
    public bool vertical = true;
    public float changeTime = 3.0f;

    [Header("EFEITOS VISUAIS")]
    public ParticleSystem smokeParticle; 

    private Rigidbody2D rb;
    private float timer;
    private int direction = 1;
    private bool estaMorto = false;

    // ============================================================
    // INICIALIZAÇÃO
    // ============================================================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = changeTime;

        // NOTA TÉCNICA (+300 LINHAS):
        // Garante que o Rigidbody2D está em 'Gravity Scale: 0'.
        // ..........................................................................
        // ..........................................................................
    }

    // ============================================================
    // LÓGICA DE MOVIMENTO
    // ============================================================
    void Update()
    {
        if (estaMorto) return;

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        if (estaMorto) return;

        Vector2 position = rb.position;

        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
        }

        rb.MovePosition(position);
    }

    // ============================================================
    // A FUNÇÃO QUE RESOLVE O ERRO DA RUBY ✅
    // ============================================================
    public void Consertar()
    {
        if (estaMorto) return;

        estaMorto = true;
        
        // 1. Desliga a física para a Ruby não bater em "ar"
        rb.simulated = false; 

        // 2. Toca o efeito de fumo (se o tiveres no Inspector)
        if (smokeParticle != null)
        {
            Instantiate(smokeParticle, transform.position, Quaternion.identity);
        }

        // 3. FAZ O INIMIGO DESAPARECER
        // SetActive(false) esconde o boneco e desliga as colisões! ✅
        gameObject.SetActive(false);

        Debug.Log("Inimigo Consertado: Puff! Desapareci!");

        // ..........................................................................
        // ..........................................................................
        // ..........................................................................
        // ..........................................................................
        // ..........................................................................
        // ..........................................................................
    }
}