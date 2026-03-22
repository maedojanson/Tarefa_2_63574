using UnityEngine;

public class ItemCura : MonoBehaviour
{
    public int quantidadeCura = 1;
    public AudioClip somCura;
    public GameObject efeitoParticula;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController ruby = other.GetComponent<RubyController>();

        if (ruby != null)
        {
            if (ruby.health < ruby.maxHealth)
            {
                ruby.ChangeHealth(quantidadeCura);
                
                // Feedback sonoro e visual
                ruby.PlaySound(somCura);
                if(efeitoParticula != null) Instantiate(efeitoParticula, transform.position, Quaternion.identity);
                
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Ruby já está com a vida cheia!");
            }
        }
    }
}