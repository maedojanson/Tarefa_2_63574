using UnityEngine;

public class BalaProjetil : MonoBehaviour
{
    private Rigidbody2D rb;

    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    public void Lancar(Vector2 direcao, float forca)
    {
        rb.AddForce(direcao * forca);
        Destroy(gameObject, 3.0f);
    }

    // Como pediste IS TRIGGER na bala, usamos esta função:
    void OnTriggerEnter2D(Collider2D other)
    {
        // Se bater no inimigo (que NÃO é trigger)
        InimigoController inimigo = other.GetComponent<InimigoController>();
        
        if (inimigo != null)
        {
            inimigo.Consertar();
            Destroy(gameObject);
        }
        
        // Se bater numa parede ou objeto estático
        if (other.gameObject.isStatic)
        {
            Destroy(gameObject);
        }
    }
}