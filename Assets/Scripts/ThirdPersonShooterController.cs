using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private GunSelector gunSelector;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private bool previousRightClickState = false;

    private void Awake() {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update() {

        bool currentRightClickState = starterAssetsInputs.aim;
        bool aimState = aimVirtualCamera.gameObject.activeSelf;

        if(currentRightClickState && !previousRightClickState) {
            aimVirtualCamera.gameObject.SetActive(!aimState);
            if(!aimState) {
                thirdPersonController.SetSensitivity(aimSensitivity);
            }
            else {
                thirdPersonController.SetSensitivity(normalSensitivity);
            }
        }
        previousRightClickState = currentRightClickState;

        if(Mouse.current.leftButton.isPressed && gunSelector.activeGun != null) {
            gunSelector.activeGun.Shoot();
        }
    }
}
