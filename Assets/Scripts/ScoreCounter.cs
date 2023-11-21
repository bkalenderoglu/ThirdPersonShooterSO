using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreCounter : MonoBehaviour
{
    private int playerScore = 0;
    
    private static ScoreCounter instance;
    public static ScoreCounter Instance {
        get {
            if(instance == null) {
                instance = FindObjectOfType<ScoreCounter>();
                if(instance == null) {
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<ScoreCounter>();
                }
            }
            return instance;
        }
    }

    public void AddScore(int score) {
        playerScore += score;
    }

    public int GetScore() {
        return playerScore;
    }
}
