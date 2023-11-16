using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Config", order = 1)]
public class ShootConfigSO : ScriptableObject {
    public bool isHitScan = true;
    public Bullet bulletPrefab;
    public float bulletSpawnForce = 1000;
    public LayerMask hitLayer;
    public float fireRate = 0.25f;
    [Header("Simple Spread")]
    public Vector3 spread = new Vector3(0.1f, 0.1f, 0.1f);
    public SpreadType spreadType = SpreadType.Simple;
    public ShootType shootType = ShootType.FromGun;

    public float recoilRecoverySpeed = 1f;
    public float maxSpreadTime = 1f;

    public Vector3 GetSpread(float shootTime = 0) {

        Vector3 spread = Vector3.zero;

        if(spreadType == SpreadType.Simple) {
            spread = Vector3.Lerp(Vector3.zero, new Vector3(Random.Range(spread.x, spread.x),
                    Random.Range(spread.y, spread.y),
                    Random.Range(spread.z, spread.z)), Mathf.Clamp01(shootTime / maxSpreadTime));
        }
        return spread;
    }
}


