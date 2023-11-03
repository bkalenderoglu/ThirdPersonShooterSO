using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;

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
    }
}