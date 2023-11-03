using UnityEngine;

[CreateAssetMenu(fileName = "Trail Config", menuName = "Guns/Trail Config", order = 2)]
public class TrailConfigSO : ScriptableObject {
    // Trail Renderer
    public Material material;
    public AnimationCurve widthCurve;
    public float duration = 0.5f;
    public float minVertexDistance = 0.1f;
    public Gradient Color;

    public float missDistanceThreshold = 100f; //how far it shoot
    // move speed from guns point to hit point/miss loc
    public float simulationSpeed = 100f;
}
