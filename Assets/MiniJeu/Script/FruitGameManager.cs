using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;

public class FruitGameManager : MonoBehaviour
{
    public static FruitGameManager i;

    private void Awake()
    {
        if (i != null)
            Destroy(i);
        i = this;
    }

    public Transform spawnPoint;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;
    private float bottomScreenY = -10f; // Position Y en bas de l'écran
    [SerializeField] private float randomXPositionRange = 1.5f;

    public List<PrefFruit> prefFruit = new List<PrefFruit>();

    public float score = 0;
    public int lives = 3;
    private bool gameIsOver = false;

    public float initialUpwardForce = 5f;
    public Vector2 gravity = new Vector2(0, -9.81f);
    public float randomAngleRange = 60f;
    //public float speedIncreaseRate = 0.1f;
    private float currentUpwardForce; //Force de l'objet appliquer vers le haut

    public float currentSpawnRate; //Fréquence d'apparition actuel
    public float maxSpawnRate = 0.2f; //Vitesse max pour le spawn Rate

    public float combo = 0; //Nombre de combo
    public float actualScoreMultiplier = 1.0f; // Multiplicateur initial pour le score
    public float scoreMultiplier = 0.4f; //Multiplicateur ajouter à chaque combo
    public float decreasePercentage = 0.01f;

    // Doit être compris entre 0 et 100
    [System.Serializable]
    public struct PrefFruit
    {
        public GameObject Pref;
        [Range(0, 100)] public int SpawnRate;
    }

    public Image Heart1;
    public Image Heart2;
    public Image Heart3;

    public UnityEvent OnGameStart;
    public UnityEvent OnGameEnd;

    private List<GameObject> spawnedFruit = new List<GameObject>();

    void Start()
    {
        //InitializeGame();
        UpdateGameOverUI(true);
        //Physics2D.bounceThreshold = 0; // Désactiver la collision continue pour permettre le rebondissement des fruits
    }

    void InitializeGame()
    {
        score = 0;
        lives = 3;
        gameIsOver = false;
        UpdateScore();
        UpdateLives();
        UpdateGameOverUI(false);

        Heart1.enabled = true;
        Heart2.enabled = true;
        Heart3.enabled = true;

        currentUpwardForce = initialUpwardForce;
        currentSpawnRate = 1.5f;

        Physics2D.gravity = gravity;
        StartCoroutine(SpawnFruitsRoutine());

        OnGameStart?.Invoke();
    }

    IEnumerator SpawnFruitsRoutine()
    {
        while (!gameIsOver)
        {
            SpawnFruit();
            yield return new WaitForSeconds(currentSpawnRate);
        }
    }

    void Update()
    {
        if (!gameIsOver && Input.GetKeyDown(KeyCode.G))
        {
            ToggleGravity();
        }
    }

    void ToggleGravity()
    {
        Physics2D.gravity = -Physics2D.gravity;
    }

    void UpdateScore()
    {
        string roundedScore = Mathf.RoundToInt(score).ToString();

        scoreText.text = "Score: " + roundedScore;
        comboText.text = "x" + actualScoreMultiplier;
    }

    void UpdateLives()
    {
        if (lives == 2)
        {
            Heart1.enabled = false;
        }
        else if (lives == 1)
        {
            Heart2.enabled = false;
        }
        else if (lives == 0)
        {
            Heart3.enabled = false;
        }
    }

    public void UpdateGameOverUI(bool active)
    {
        gameOverText.gameObject.SetActive(active);
        restartButton.gameObject.SetActive(active);
    }

    public void DeactiveGameOverText(bool active) => gameOverText.gameObject.SetActive(active);

    void SpawnFruit()
    {
        if (gameIsOver)
            return;

        // Déterminer la position aléatoire du spawnPoint
        GameObject fruitPrefab = null;

        // Génère un nombre aléatoire entre 0 et 100
        int randomNumber = UnityEngine.Random.Range(0, 100);
        //Debug.Log("Random Number: " + randomNumber);

        foreach (var fruit in prefFruit)
        {
            if(fruit.SpawnRate >= randomNumber)
            {
                fruitPrefab = fruit.Pref;
                break;
            }
        }

        if (fruitPrefab == null)
        {
            Debug.Log(
                "Aucune prefabs trouvé pour le nombre donné. Vérifier que les spawnRates définis sont bon. " +
                "randomNumber : {randomNumber}, spawnRates : {spawnRates}"
                );
        }

        Vector3 fruitSpawnPoint = spawnPoint.position;
        fruitSpawnPoint.x += UnityEngine.Random.Range(-randomXPositionRange, randomXPositionRange);

        GameObject fruitInstance = Instantiate(fruitPrefab, fruitSpawnPoint, Quaternion.identity);
        spawnedFruit.Add(fruitInstance);

        //rotation aléatoire avant de spawn
        fruitInstance.transform.Rotate(Vector3.forward * UnityEngine.Random.Range(0, 180));

        Rigidbody2D fruitRb = fruitInstance.GetComponent<Rigidbody2D>();
        if (fruitRb != null)
        {
            //on donne une légére rotation à l'objet pour qu'il tourne dans les airs (décoratif)
            float randomTorque = UnityEngine.Random.Range(-0.5f, 0f);
            fruitRb.AddTorque(randomTorque, ForceMode2D.Impulse);

            //on choisis un angle aléatoire pour l'apparition
            float randomAngle = UnityEngine.Random.Range(-randomAngleRange / 2f, randomAngleRange / 2f);
            Vector2 upwardDirection = Quaternion.Euler(0, 0, randomAngle) * Vector2.up;
            fruitRb.AddForce(upwardDirection * currentUpwardForce, ForceMode2D.Impulse);
        }

        // Ajout du script FruitDestroyer pour gérer la destruction du fruit
        FruitDestroyer destroyer = fruitInstance.AddComponent<FruitDestroyer>();
        destroyer.bottomScreenY = bottomScreenY;

        // Augmenter la difficulté au fil du temps
        if (currentSpawnRate >= maxSpawnRate)
        {
            // Diminuer le multiplicateur de vitesse de 1%
            
            float decreaseAmount = currentSpawnRate * decreasePercentage;
            currentSpawnRate -= decreaseAmount;
        }
    }

    //public void FruitScoring(string fruitType)
    //{
    //    switch (fruitType)
    //    {
    //        case "Banana":
    //            score += 1;
    //            IncreaseCombo();
    //            break;
    //        case "Strawberry":
    //            score += 2;
    //            IncreaseCombo();
    //            break;
    //        case "Saucisse":
    //            score += 5;
    //            IncreaseCombo();
    //            break;
    //    }

    //    UpdateScore();
    //}

    public void FruitClick(Fruit fruit)
    {
        if (gameIsOver)
            return;

        if (fruit.makeDamage)
        {
            lives--;
            ResetCombo();
        }
        else
        {
            score += fruit.scoreValue * actualScoreMultiplier;
            IncreaseCombo();
        }

        UpdateScore();
        UpdateLives();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void FruitHurt(string fruitType)
    {
        if (gameIsOver)
            return;

        if (fruitType != "Coco" || fruitType != "Saucisse")
        {
            lives--;
            ResetCombo();
            UpdateLives();
        }

        if (lives <= 0)
        {
            GameOver();
        }
    }
    
    void GameOver()
    {
        OnGameEnd?.Invoke();
        gameIsOver = true;

        gameOverText.text = $"Vous avez gagné {Mathf.RoundToInt(score)} <sprite=0>";
        UserBehaviour.i.AddBanana(Mathf.RoundToInt(score));
        scoreText.text = "Score: 0";
        comboText.text = "x0";

        UpdateGameOverUI(true);
        
        foreach (var fruit in spawnedFruit)
        {
            if(fruit != null)
                Destroy(fruit);
        }
        //Debug.Log("Game Over! Score: " + score);
        // Ajoutez ici le code pour afficher un écran de fin de jeu ou redémarrer le jeu.
    }

    public void RestartGame()
    {
        InitializeGame();
    }

    void ResetCombo()
    {
        combo = 0;
        actualScoreMultiplier = 1.0f;
    }

    void IncreaseCombo()
    {
        combo++;
        if (combo % 5 == 0)
        {
            actualScoreMultiplier += scoreMultiplier;
        }
    }
}
