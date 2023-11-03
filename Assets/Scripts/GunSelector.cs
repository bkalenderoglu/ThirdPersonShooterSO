using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GunSelector : MonoBehaviour
{
    [SerializeField] private GunType gunType;
    [SerializeField] private Transform gunParent;
    [SerializeField] private List<GunSO> guns;

    [Space]
    [Header("Runtime Filled")]
    public GunSO activeGun;

    private void Start() {
        GunSO gun = guns.Find(gun => gun.type == gunType);

        if(gun == null) {
            Debug.LogError($"No GunSO found for this type: {gun}");
            return;
        }

        activeGun = gun;
        gun.Spawn(gunParent, this);
    }
}
