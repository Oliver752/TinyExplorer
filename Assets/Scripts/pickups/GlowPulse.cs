using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    [Header("Pulse")]
    public float speed = 3f;
    public float minIntensity = 0f;
    public float maxIntensity = 5f;

    [Header("Color")]
    public Color glowColor = Color.green;

    private Renderer[] renderers;
    private Material[] mats;

    void Awake()
    {
        // Grab ALL renderers in this object hierarchy
        renderers = GetComponentsInChildren<Renderer>(true);
    }

    void Start()
    {
        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogWarning($"GlowPulse: No Renderer found on {name} (or children).");
            enabled = false;
            return;
        }

        // Collect material instances (renderer.materials creates instances)
        // This way we affect every material on every renderer.
        int total = 0;
        foreach (var r in renderers) total += r.materials.Length;

        mats = new Material[total];
        int idx = 0;

        foreach (var r in renderers)
        {
            var ms = r.materials;
            for (int i = 0; i < ms.Length; i++)
            {
                mats[idx++] = ms[i];

                // Enable emission if supported
                if (ms[i].HasProperty("_EmissionColor"))
                {
                    ms[i].EnableKeyword("_EMISSION");

                    // Force a visible base emission color (green),
                    // so even "black emission" materials will glow.
                    ms[i].SetColor("_EmissionColor", glowColor * minIntensity);
                }
            }
        }
    }

    void Update()
    {
        if (mats == null || mats.Length == 0) return;

        float emission = Mathf.PingPong(Time.time * speed, maxIntensity - minIntensity) + minIntensity;

        for (int i = 0; i < mats.Length; i++)
        {
            var m = mats[i];
            if (m == null) continue;

            if (m.HasProperty("_EmissionColor"))
            {
                m.SetColor("_EmissionColor", glowColor * emission);
            }
        }
    }
}
