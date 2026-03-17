using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Criamos uma variável de velocidade para ser mais fácil de ajustar no Unity
    public float speed = 3.0f;

    void Start()
    {
        // Ainda não precisamos de nada aqui
    }

    void Update()
    {
        // 1. Detetar o input do jogador (Horizontal = Esquerda/Direita, Vertical = Cima/Baixo)
        // Estes valores variam entre -1, 0 e 1
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 2. Pegar na posição atual
        Vector2 position = transform.position;

        // 3. Calcular a nova posição com base no Input e na Velocidade
        // Time.deltaTime garante que o movimento é suave e igual em todos os PCs
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        // 4. Aplicar a nova posição
        transform.position = position;
    }
}
