using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    private void Awake() {
        startButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.Playground);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
    }
}
