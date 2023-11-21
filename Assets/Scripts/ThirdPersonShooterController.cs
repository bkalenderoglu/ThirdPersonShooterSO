using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform debugTransform;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private GunSelector gunSelector;
    [SerializeField] private bool autoReload = true;
    [SerializeField] private PlayerIK inverseKinematics;
    [SerializeField] private Animator animator;
    [SerializeField] private Image crosshair;
    [SerializeField] private Transform aimTarget;
    private bool isReloading;

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

        if(gunSelector.activeGun != null) {
            gunSelector.activeGun.Tick(Mouse.current.leftButton.isPressed);
        }

        if(starterAssetsInputs.reload) {
            Debug.Log("reload");
        }

        if(ShouldManualReload() || ShouldAutoReload()) {
            isReloading = true;
            animator.SetTrigger("Reload");
            inverseKinematics.handIKAmount = 0.25f;
            inverseKinematics.elbowIKAmount = 0.25f;
        }

        UpdateCrosshair();
    }


    private void UpdateCrosshair() {
        Vector3 gunTipPoint = gunSelector.activeGun.GetRaycastOrigin();
        Vector3 forward;

        if(gunSelector.activeGun.shootConfig.shootType == ShootType.FromGun) {
            forward = gunSelector.activeGun.GetGunForward();
        } else {
            forward = gunSelector.activeCamera.transform.forward;
        }

        Vector3 hitPoint = gunTipPoint + forward * 10;

        if(Physics.Raycast(gunTipPoint, forward, out RaycastHit hit, float.MaxValue, gunSelector.activeGun.shootConfig.hitLayer)) {
           hitPoint = hit.point;
        }

        aimTarget.transform.position = hitPoint;

        if(gunSelector.activeGun.shootConfig.shootType == ShootType.FromGun) {
            Vector3 screenSpaceLocation = gunSelector.activeCamera.WorldToScreenPoint(hitPoint);
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(
               (RectTransform) crosshair.transform.parent,
                screenSpaceLocation,
                null,
                out Vector2 localPoint)) {
                crosshair.rectTransform.anchoredPosition = localPoint;
            }
            else {
                crosshair.rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }

    private bool ShouldManualReload() {
        return !isReloading
            && starterAssetsInputs.reload
            && gunSelector.activeGun.CanReload();
    }

    private bool ShouldAutoReload() {
        return !isReloading
            && autoReload
            && gunSelector.activeGun.ammoConfig.currentClipAmmo == 0
            && gunSelector.activeGun.CanReload();
    }

    private void EndReload() {
        gunSelector.activeGun.EndReload();
        inverseKinematics.handIKAmount = 1f;
        inverseKinematics.elbowIKAmount = 1f;
        isReloading = false;
    }
}
