using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class GunSelector : MonoBehaviour
{
    [SerializeField] private GunType gunType;
    [SerializeField] private Transform gunParent;
    [SerializeField] private List<GunSO> guns;
    [SerializeField] PlayerIK inverseKinematics;
    public Camera activeCamera;


    [Space]
    [Header("Runtime Filled")]
    public GunSO activeGun;

    private void Start() {
        GunSO gun = guns.Find(gun => gun.type == gunType);

        if(gun == null) {
            Debug.LogError($"No GunSO found for this type: {gun}");
            return;
        }

        SetupGun(gun);

    }

    private void SetupGun(GunSO gun) {
        activeGun = gun;
        gun.Spawn(gunParent, this, activeCamera);

        inverseKinematics.SetGunStyle(activeGun.type == GunType.Pistol);
        inverseKinematics.Setup(gunParent);
    }
}
