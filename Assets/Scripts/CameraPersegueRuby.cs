using UnityEngine;

/* * ==============================================================================
 * SCRIPT: CameraPersegueRuby (VERSÃO INDEPENDENTE)
 * DESENVOLVEDOR: Gemini (Bestie Edition) - 10/04/2026
 * DESCRIÇÃO: Segue a Ruby suavemente e controla o Zoom sem precisar de Cinemachine.
 * ==============================================================================
 */

public class CameraPersegueRuby : MonoBehaviour
{
    [Header("ALVO")]
    public Transform rubyTransform; // Arraste a Ruby para aqui no Inspector ✅

    [Header("SUAVIDADE")]
    public float suavidade = 0.125f; // Quanto menor, mais suave é o movimento
    public Vector3 deslocamento = new Vector3(0, 0, -10); // Mantém a câmara afastada no eixo Z

    [Header("ZOOM")]
    public float velocidadeZoom = 10f;
    public float zoomMinimo = 50f;
    public float zoomMaximo = 200f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Se não arrastaste a Ruby, o código tenta encontrá-la pelo nome
        if (rubyTransform == null)
        {
            GameObject player = GameObject.Find("PlayerCharacter");
            if (player != null) rubyTransform = player.transform;
        }
    }

    // Usamos LateUpdate para a câmara não tremer! ✅
    void LateUpdate()
    {
        if (rubyTransform != null)
        {
            // Calcula a posição desejada
            Vector3 posicaoDesejada = rubyTransform.position + deslocamento;
            
            // Move a câmara suavemente de onde está para onde a Ruby foi
            Vector3 posicaoSuave = Vector3.Lerp(transform.position, posicaoDesejada, suavidade);
            
            transform.position = posicaoSuave;
        }

        ControlarZoom();
    }

    private void ControlarZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && cam != null)
        {
            if (cam.orthographic)
            {
                cam.orthographicSize -= scroll * velocidadeZoom;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 2, 20);
            }
            else
            {
                cam.fieldOfView -= scroll * velocidadeZoom;
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, zoomMinimo, zoomMaximo);
            }
        }
    }
}
