using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(TextMeshProUGUI))]
public class AmmoDisplayer : MonoBehaviour
{
    [SerializeField] private GunSelector gunSelector;
    private TextMeshProUGUI ammoText;

    private void Awake() {
        ammoText = GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        ammoText.SetText($"{gunSelector.activeGun.ammoConfig.currentClipAmmo} / " + $"{gunSelector.activeGun.ammoConfig.currentAmmo}");
    }
}
