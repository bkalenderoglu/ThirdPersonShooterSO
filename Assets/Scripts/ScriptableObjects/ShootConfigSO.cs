using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Config", order = 1)]
public class ShootConfigSO : ScriptableObject {
    public LayerMask hitLayer;
    public float fireRate = 0.25f;
    public Vector3 spread = new Vector3(0.1f, 0.1f, 0.1f);
}


