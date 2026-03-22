using UnityEngine;

public class DanoZona : MonoBehaviour
{
    // -1 para espinhos, -2 para água (podes mudar no Inspector)
    public int quantidadeDano = -1; 

    // OnTriggerStay2D é melhor que Enter porque tira vida enquanto ela lá estiver
    void OnTriggerStay2D(Collider2D other)
    {
        // Procuramos o script RubyController no objeto que tocou nos picos
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            // Chamamos a função de mudar a vida na Ruby
            controller.ChangeHealth(quantidadeDano);
        }
    }
}