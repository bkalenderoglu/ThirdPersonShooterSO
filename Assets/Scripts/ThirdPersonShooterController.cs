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
    [SerializeField] private bool autoReload = false;
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

        if(ShouldManualReload() || ShouldAutoReload()) {
            isReloading = true;
            animator.SetTrigger("Reload");
            //inverseKinematics.HandIKAmount = 0.25f;
            //inverseKinematics.ElbowIKAmount = 0.25f;
        }

        UpdateCrosshair();

        //Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        //Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        //if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue)) {
        //    debugTransform.position = raycastHit.point;
        //}
        //else {
        //    debugTransform.position = mainCamera.transform.position + mainCamera.transform.forward * 200f;

        //}
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
            && Keyboard.current.rKey.wasReleasedThisFrame
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
        //inverseKinematics.HandIKAmount = 1f;
        //inverseKinematics.ElbowIKAmount = 1f;
        isReloading = false;
    }
}
