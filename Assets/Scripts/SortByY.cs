using UnityEngine;

// ============================================================
// SCRIPT: SORT_BY_Y (Para a Caixa ficar atrás/frente da Ruby)
// ============================================================
[RequireComponent(typeof(SpriteRenderer))]
public class SortByY : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // USAMOS A MESMA LÓGICA DA RUBY
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100f) + 1000;
        }
    }
}
