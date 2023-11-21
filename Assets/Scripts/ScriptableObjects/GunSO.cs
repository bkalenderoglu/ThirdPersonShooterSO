using Cinemachine;
using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class GunSO : ScriptableObject {
    public GunType type;
    public string gunName;
    public GameObject modelPrefab;
    public Vector3 spawnPoint;
    public Vector3 spawnRotation;

    public ShootConfigSO shootConfig;
    public TrailConfigSO trailConfig;
    public DamageConfigSO damageConfig;
    public AmmoConfigSO ammoConfig;

    private MonoBehaviour activeMonoBehaviour;
    private GameObject model;
    private Camera camera;

    private float lastShootTime;
    private float initialClickTime;
    private float stopShootingTime;
    private bool lastFrameToShoot;

    private ParticleSystem shootSystem;
    private ObjectPool<TrailRenderer> trailPool;
    private ObjectPool<Bullet> bulletPool;

    public void Spawn(Transform parent, MonoBehaviour activeMonoBehaviour, Camera camera = null) {
        this.activeMonoBehaviour = activeMonoBehaviour;
        this.camera = camera;

        lastShootTime = 0;
        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        if(!shootConfig.isHitScan) {
            bulletPool = new ObjectPool<Bullet>(CreateBullet);
        }

        model = Instantiate(modelPrefab);
        model.transform.SetParent(parent, false);
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRotation);

        shootSystem = model.GetComponentInChildren<ParticleSystem>();
    }

    public void Shoot() {
        if(Time.time - lastShootTime - shootConfig.fireRate > Time.deltaTime) {
            // start clicking
            float lastDuration = Mathf.Clamp(0, (stopShootingTime - initialClickTime), shootConfig.maxSpreadTime);
            float lerpTime = (shootConfig.recoilRecoverySpeed - (Time.time - stopShootingTime)) / shootConfig.recoilRecoverySpeed;
            initialClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
        }

        if(Time.time > shootConfig.fireRate + lastShootTime) {
            lastShootTime = Time.time;
            shootSystem.Play();

            Vector3 spreadAmount = shootConfig.GetSpread(Time.time - initialClickTime);
            model.transform.forward += model.transform.TransformDirection(spreadAmount);

            Vector3 shootDirection = Vector3.zero;

            if(shootConfig.shootType == ShootType.FromGun) {
                shootDirection = shootSystem.transform.forward;
            } else {
                shootDirection = camera.transform.forward + camera.transform.TransformDirection(shootDirection);
            }

            ammoConfig.currentClipAmmo--;

            if(shootConfig.isHitScan) {
                DoHitscanShoot(shootDirection);
            }else {
                DoProjectileShoot(shootDirection);
            }
        }
    }

    public void UpdateCamera(Camera camera) {
        this.camera = camera;
    }

    public bool CanReload() {
        return ammoConfig.CanReload();
    }

    public void EndReload() {
        ammoConfig.Reload();
    }

    public void Tick(bool shoot) {
        model.transform.localRotation = Quaternion.Lerp(model.transform.localRotation,
                                                        Quaternion.Euler(spawnRotation),
                                                        Time.deltaTime * shootConfig.recoilRecoverySpeed);
        if(shoot) {
            lastFrameToShoot = true;
            if(ammoConfig.currentClipAmmo > 0) {
                Shoot();
            }
            
        }
        else if(!shoot && lastFrameToShoot) {
            //stop shooting
            stopShootingTime = Time.time;
            lastFrameToShoot = false;
        }
    }

    public Vector3 GetRaycastOrigin() {
        Vector3 origin = shootSystem.transform.position;

        if(shootConfig.shootType == ShootType.FromCamera) {
            origin = camera.transform.position + camera.transform.forward * Vector3.Distance(camera.transform.position, shootSystem.transform.position);
        }

        return origin;
    }

    public Vector3 GetGunForward() {
        return model.transform.forward;
    }

    private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hitInfo) {
        TrailRenderer instance = trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPoint;
        yield return null; // avoid persistence of position from the previous frame if used

        instance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;
        while(remainingDistance > 0) {
            instance.transform.position = Vector3.Lerp(startPoint, endPoint, Mathf.Clamp01(1 - (remainingDistance / distance)));
            remainingDistance -= trailConfig.simulationSpeed * Time.deltaTime;
            yield return null;
        }

        instance.transform.position = endPoint;

        if(hitInfo.collider != null) {
            HandleBulletImpact(distance, endPoint, hitInfo.normal, hitInfo.collider);
        }

        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);
    }

    private void DoHitscanShoot(Vector3 shootDirection) {
        if(Physics.Raycast(GetRaycastOrigin(), shootDirection, out RaycastHit hitInfo, float.MaxValue, shootConfig.hitLayer)) {
            activeMonoBehaviour.StartCoroutine(PlayTrail(shootSystem.transform.position, hitInfo.point, hitInfo));
        }
        else {
            activeMonoBehaviour.StartCoroutine(PlayTrail(shootSystem.transform.position, (shootDirection * trailConfig.missDistanceThreshold), new RaycastHit()));
        }
    }

    private void DoProjectileShoot(Vector3 shootDirection) {
        Bullet bullet = bulletPool.Get();
        bullet.gameObject.SetActive(true);
        bullet.OnCollision += HandleBulletCollision;

        if(shootConfig.shootType == ShootType.FromCamera && Physics.Raycast(GetRaycastOrigin(), shootDirection, out RaycastHit hit, float.MaxValue, shootConfig.hitLayer)) {
            Vector3 directionToHit = (hit.point - shootSystem.transform.position).normalized;
            model.transform.forward = directionToHit;
            shootDirection = directionToHit;
        }

        bullet.transform.position = shootSystem.transform.position;
        bullet.Spawn(shootDirection * shootConfig.bulletSpawnForce);

        TrailRenderer trail = trailPool.Get();
        if(trail != null) {
            trail.transform.SetParent(bullet.transform, false);
            trail.transform.localPosition = Vector3.zero;
            trail.emitting = true;
            trail.gameObject.SetActive(true);
        }
    }

    private void HandleBulletCollision(Bullet bullet, Collision collision) {
        TrailRenderer trail = bullet.GetComponentInChildren<TrailRenderer>();
        if(trail != null) {
            trail.transform.SetParent(null, true);
            activeMonoBehaviour.StartCoroutine(DelayedDisableTrail(trail));
        }

        bullet.gameObject.SetActive(false);
        bulletPool.Release(bullet);

        if(collision != null) {
            ContactPoint contactPoint = collision.GetContact(0);
            HandleBulletImpact(Vector3.Distance(contactPoint.point, bullet.spawnLocation), contactPoint.point, contactPoint.normal, contactPoint.otherCollider);
        }
    }

    private void HandleBulletImpact(float distanceTraveled, Vector3 hitLocation, Vector3 hitNormal, Collider hitCollider) {
        if(hitCollider.TryGetComponent(out IDamageable damageable)) {
            damageable.TakeDamage(damageConfig.GetDamage(distanceTraveled));
            Debug.Log("damage taken");
        }
    }

    private IEnumerator DelayedDisableTrail(TrailRenderer trail) {
        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        trail.emitting = false;
        trail.gameObject.SetActive(false);
        trailPool.Release(trail);
    }

    private TrailRenderer CreateTrail() {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = trailConfig.Color;
        trail.material = trailConfig.material;
        trail.widthCurve = trailConfig.widthCurve;
        trail.time = trailConfig.duration;
        trail.minVertexDistance = trailConfig.minVertexDistance;

        trail.emitting = false; // emit after spawning trail
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    public Vector3 GetAimDirection() {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, shootConfig.hitLayer)) {
            mouseWorldPosition = raycastHit.point;
        }
        Vector3 worldAimTarget = mouseWorldPosition;
        Vector3 aimDirection = (worldAimTarget - shootSystem.transform.position).normalized;

        return aimDirection;
    }

    private Bullet CreateBullet() {
        return Instantiate(shootConfig.bulletPrefab);
    }
}
