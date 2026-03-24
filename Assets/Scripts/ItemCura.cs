using UnityEngine;

// ============================================================
// SCRIPT: ITEM_CURA (Morango / Coração)
// ============================================================
public class ItemCura : MonoBehaviour
{
    [Header("Configurações de Cura")]
    public int quantidadeCura = 1;
    public AudioClip somCura;
    public GameObject efeitoParticula;

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Tenta encontrar o script da Ruby
        RubyController ruby = other.GetComponent<RubyController>();

        if (ruby != null)
        {
            // 2. VERIFICAÇÃO DE VIDA (Usando a propriedade 'health' que criámos)
            // Nota: Se der erro no 'maxHealth', garante que ele é PUBLIC na Ruby!
            if (ruby.health < ruby.maxHealth)
            {
                // 3. CURA A RUBY
                ruby.ChangeHealth(quantidadeCura);
                
                // 4. FEEDBACK (Som e Partículas)
                if (somCura != null) 
                {
                    ruby.PlaySound(somCura);
                }

                if (efeitoParticula != null) 
                {
                    Instantiate(efeitoParticula, transform.position, Quaternion.identity);
                }
                
                // 5. O ITEM DESAPARECE
                Debug.Log("Ruby curada! Item consumido.");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Ruby já está com a vida cheia! O item fica no chão.");
            }
        }
    }
}