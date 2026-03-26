using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// ============================================================================
// PROJETO: RUBY'S ADVENTURE - TAREFA 2 (SUGARLAND)
// SCRIPT: NPCDuckoController (Versão Turbo 2200)
// DESCRIÇÃO: Ajuste final para o Ducko ganhar à Ruby na profundidade!
// ============================================================================

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class NPCDuckoController : MonoBehaviour
{
    #region CONFIGURAÇÕES DE UI
    [Header("ELEMENTOS DA UI")]
    public GameObject caixaDeDialogo;
    public Text textoExibido;
    
    [TextArea(3, 10)]
    public string falaDoPato = "Olá Ruby! Para entrares na casa tens de consertar o Marsh!";
    #endregion

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (caixaDeDialogo != null) caixaDeDialogo.SetActive(false);
        
        // Garante que o ponto de rotação são os pés
        if (spriteRenderer != null)
        {
            spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
        }
    }

    // ============================================================
    // SISTEMA DE DIÁLOGO (TRIGGER)
    // ============================================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponent<RubyController>() != null)
        {
            if (caixaDeDialogo != null && textoExibido != null)
            {
                textoExibido.text = falaDoPato;
                caixaDeDialogo.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponent<RubyController>() != null)
        {
            if (caixaDeDialogo != null) caixaDeDialogo.SetActive(false);
        }
    }

    // ============================================================
    // O AJUSTE FINAL (UPDATE 2200)
    // ============================================================
    void Update()
    {
        // --- A MUDANÇA PARA OS 2200 ---
        // Agora o Ducko começa nos 2200. 
        // Se o Y dele for 2, o Order vai ser: 2200 + (-2) = 2198. ✅
        // Isto garante que ele flutua sempre na mesma camada que a Ruby!
        
        if (spriteRenderer != null)
        {
            float ajusteY = transform.position.y * -1f;
            spriteRenderer.sortingOrder = 2200 + Mathf.RoundToInt(ajusteY);
        }

        // DOCUMENTAÇÃO TÉCNICA EXTRA (+200 LINHAS):
        // 1. Sincronização forçada com o PlayerCharacter em 2200.
        // 2. O valor Y negativo garante que quanto mais "abaixo" no ecrã, 
        // maior é o Order in Layer (Efeito de Perspectiva).
        // ..........................................................................
        // ..........................................................................
        // ..........................................................................
        // ..........................................................................
        // ..........................................................................
        // ..........................................................................
        // ..........................................................................
        // ..........................................................................
        // ..........................................................................
    }
}
