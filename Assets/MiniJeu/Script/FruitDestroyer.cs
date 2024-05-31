using System;
using UnityEngine;

public class FruitDestroyer : MonoBehaviour
{
    public float bottomScreenY = -10f; // Position Y en bas de l'écran
    public FruitGameManager gameManager => FruitGameManager.i; // Référence au GameManager

    void Update()
    {
        // Si le fruit est en bas de l'écran, détruire le fruit et retirer une vie
        if (transform.position.y < bottomScreenY)
        {
            Fruit fruitScript = gameObject.GetComponent<Fruit>();
            if (fruitScript != null)
            {
                string fruitType = fruitScript.fruitType;
                if (fruitType != "Coco" && fruitType != "Saucisse") // Si ce n'est pas une noix de coco
                {
                    gameManager.FruitHurt(fruitType);
                }
            }

            if (gameObject != null) // Vérifier si l'objet est toujours actif avant de le détruire
            {
                Destroy(gameObject);
            }
        }
    }
}
