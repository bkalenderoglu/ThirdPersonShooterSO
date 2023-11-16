using UnityEngine;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName = "Damage Config", menuName = "Guns/Damage Config", order = 3)]
public class DamageConfigSO : ScriptableObject
{
    public MinMaxCurve damageCurve;

    private void Reset() {
        damageCurve.mode = ParticleSystemCurveMode.Curve;
    }

    public int GetDamage(float distance = 0) {
        return Mathf.CeilToInt(damageCurve.Evaluate(distance, Random.value));
    }
}
