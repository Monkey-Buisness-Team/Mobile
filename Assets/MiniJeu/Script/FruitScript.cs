using UnityEngine;

public class Fruit : MonoBehaviour
{
    public string fruitType; // "Coco", "Banana", or "Strawberry"
    public ParticleSystem explosionParticle; // Ajout d'une particule d'explosion

    private FruitGameManager gameManager => FruitGameManager.i;

    private Rigidbody2D rb;

    private void Start()
    {
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Fruit"), LayerMask.NameToLayer("Fruit"));

        rb = GetComponent<Rigidbody2D>();
    }

    private void OnMouseDown()
    {
        if (gameManager != null)
        {
            
            
            gameManager.FruitClick(fruitType);
            

            if (explosionParticle != null)
            {
                // Joue la particule d'explosion
                Instantiate(explosionParticle, transform.position, Quaternion.identity);
            }
        }

        Destroy(gameObject); // Détruit le fruit lorsqu'il est touché.
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Inverser la direction de déplacement du fruit
            rb.velocity = Vector2.Reflect(rb.velocity, collision.contacts[0].normal);
            //Debug.Log("Wall");
        }
    }
}
