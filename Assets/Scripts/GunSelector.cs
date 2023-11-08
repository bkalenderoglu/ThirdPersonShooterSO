using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class GunSelector : MonoBehaviour
{
    [SerializeField] private GunType gunType;
    [SerializeField] private Transform gunParent;
    [SerializeField] private List<GunSO> guns;
    [SerializeField] PlayerIK inverseKinematics;


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

        Transform[] allChildren = gunParent.GetComponentsInChildren<Transform>();
        inverseKinematics.leftElbowIK = allChildren.FirstOrDefault(child => child.name == "LeftElbow");
        inverseKinematics.rightElbowIK = allChildren.FirstOrDefault(child => child.name == "RightElbow");
        inverseKinematics.leftHandIK = allChildren.FirstOrDefault(child => child.name == "LeftHand");
        inverseKinematics.rightHandIK = allChildren.FirstOrDefault(child => child.name == "RightHand");

    }
}
