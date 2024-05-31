using System;
using UnityEngine;

public class FruitDestroyer : MonoBehaviour
{
    public float bottomScreenY = -10f; // Position Y en bas de l'�cran
    public FruitGameManager gameManager => FruitGameManager.i; // R�f�rence au GameManager

    void Update()
    {
        // Si le fruit est en bas de l'�cran, d�truire le fruit et retirer une vie
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

            if (gameObject != null) // V�rifier si l'objet est toujours actif avant de le d�truire
            {
                Destroy(gameObject);
            }
        }
    }
}
