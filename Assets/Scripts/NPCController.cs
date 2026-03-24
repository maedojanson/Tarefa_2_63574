using UnityEngine;
using System.Collections;

// ============================================================
// SCRIPT: NPC_CONTROLLER (Versão: Falar com a Tecla E)
// ============================================================
public class NPCController : MonoBehaviour
{
    private bool rubyEstaPerto = false;
    
    // A tua frase perfeita com os ENTERs exatos que pediste!
    private string mensagemMestra = "Olá Ruby! Para" + System.Environment.NewLine + 
                                     "entrares na" + System.Environment.NewLine + 
                                     "casa tens de me" + System.Environment.NewLine + 
                                     "combater clicando" + System.Environment.NewLine + 
                                     "na tecla \"X\"!";

    void Update()
    {
        // VERIFICAÇÃO DA TECLA E:
        // Se a Ruby estiver dentro do círculo do NPC E tu clicares no E...
        if (rubyEstaPerto && Input.GetKeyDown(KeyCode.E))
        {
            ExibirDialogo();
        }
    }

    public void ExibirDialogo()
    {
        if (UIHealthBar.instance != null)
        {
            // Mostra o balão com os Enters
            UIHealthBar.instance.MostrarDialogo(mensagemMestra);
            Debug.Log("NPC: Ruby clicou no E e eu respondi!");
        }
    }

    // --- DETECÇÃO DE PROXIMIDADE ---
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            rubyEstaPerto = true;
            // Dica: Podes colocar um Debug.Log aqui para ver se o sensor funciona
            Debug.Log("NPC: Ruby aproximou-se. Clica no E para falar!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            rubyEstaPerto = false;
            // Se a Ruby se afastar, o balão fecha para não atrapalhar
            if (UIHealthBar.instance != null) UIHealthBar.instance.FecharDialogo();
        }
    }
}
