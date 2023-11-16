using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class PlayerIK : MonoBehaviour
{
    private Animator animator;
    public Transform leftHandIK;
    public Transform rightHandIK;
    public Transform leftElbowIK;
    public Transform rightElbowIK;

    [Range(0, 1f)]
    public float handIKAmount = 1f;
    [Range(0, 1f)]
    public float elbowIKAmount = 1f;


    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex) {
        if (leftHandIK != null) {
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handIKAmount);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handIKAmount);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIK.rotation);
        }

        if(rightHandIK != null) {
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handIKAmount);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handIKAmount);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIK.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIK.rotation);
        }

        if(leftElbowIK != null) {
            animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowIK.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, elbowIKAmount);
        }

        if(rightElbowIK != null) {
            animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowIK.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, elbowIKAmount);
        }
    }

    public void SetGunStyle(bool isPistol) {
        animator.SetBool("Pistol", isPistol);
        animator.SetBool("Machinegun", !isPistol);
    }

    public void Setup(Transform gunParent) {
        Transform[] allChildren = gunParent.GetComponentsInChildren<Transform>();
        leftElbowIK = allChildren.FirstOrDefault(child => child.name == "LeftElbow");
        rightElbowIK = allChildren.FirstOrDefault(child => child.name == "RightElbow");
        leftHandIK = allChildren.FirstOrDefault(child => child.name == "LeftHand");
        rightHandIK = allChildren.FirstOrDefault(child => child.name == "RightHand");
    }
}
