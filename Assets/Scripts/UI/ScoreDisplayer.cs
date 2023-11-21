using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreDisplayer : MonoBehaviour
{
    [SerializeField] private ScoreCounter scoreCounter;
    private TextMeshProUGUI scoreText;

    private void Awake() {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        scoreText.SetText($"Score: {scoreCounter.GetScore()} / " + $"100");
    }
}
