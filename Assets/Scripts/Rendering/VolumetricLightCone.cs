using UnityEngine;

/// <summary>
/// A single reusable volumetric-light cone (Lethal Company style). Builds an open cone
/// mesh procedurally and drives the <c>URP/VolumetricLightCone</c> material, which does the
/// depth fade, noise attenuation and forward-scatter shaping.
///
/// Placement is just the GameObject transform - the beam points down local +Z (the Unity
/// spotlight convention), so an optional child <see cref="Light"/> lines up with it and the
/// two can be kept in sync. Because the mesh renders in the Transparent queue it is already
/// in the camera colour buffer when <c>StylizedFogEdgesFeature</c> runs, so the existing fog
/// and colour-grade composite over it with no extra wiring.
/// </summary>
[ExecuteAlways]
[AddComponentMenu("Rendering/Volumetric Light Cone")]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VolumetricLightCone : MonoBehaviour
{
    private const string ShaderName = "URP/VolumetricLightCone";

    [Header("Cone shape")]
    [Min(0.01f)] [SerializeField] private float range = 6f;
    [Range(2f, 170f)] [SerializeField] private float coneAngle = 35f;
    [Range(3, 96)] [SerializeField] private int radialSegments = 24;
    [Range(1, 32)] [SerializeField] private int lengthSegments = 6;
    [SerializeField] private bool capMouth = true;

    [Header("Appearance")]
    [ColorUsage(true, true)] [SerializeField] private Color color = new Color(1f, 0.92f, 0.72f, 1f);
    [Min(0f)] [SerializeField] private float intensity = 2.5f;
    [Range(0.25f, 8f)] [SerializeField] private float edgeSoftness = 3f;
    [Range(0.01f, 6f)] [SerializeField] private float lengthFade = 1.6f;
    [Range(0.001f, 1f)] [SerializeField] private float tipFade = 0.12f;
    [Range(0.01f, 12f)] [SerializeField] private float depthFadeDistance = 1.5f;
    [Range(0.01f, 12f)] [SerializeField] private float cameraFadeDistance = 1f;

    [Header("Scattering & noise")]
    [Range(-0.95f, 0.95f)] [SerializeField] private float forwardScattering = 0.6f;
    [Range(0f, 6f)] [SerializeField] private float scatterBoost = 1.6f;
    [Min(0f)] [SerializeField] private float noiseScale = 1.5f;
    [Range(0f, 1f)] [SerializeField] private float noiseStrength = 0.45f;
    [SerializeField] private Vector3 noiseSpeed = new Vector3(0.05f, -0.13f, 0.03f);

    [Header("Optional real light")]
    [Tooltip("Match a child spot Light's range / cone angle / colour to this beam every frame.")]
    [SerializeField] private bool syncChildSpotLight = true;
    [SerializeField] private Light spotLight;

    private static readonly int IdColor = Shader.PropertyToID("_Color");
    private static readonly int IdIntensity = Shader.PropertyToID("_Intensity");
    private static readonly int IdEdgeSoftness = Shader.PropertyToID("_EdgeSoftness");
    private static readonly int IdLengthFade = Shader.PropertyToID("_LengthFade");
    private static readonly int IdTipFade = Shader.PropertyToID("_TipFade");
    private static readonly int IdDepthFade = Shader.PropertyToID("_DepthFade");
    private static readonly int IdCameraFade = Shader.PropertyToID("_CameraFade");
    private static readonly int IdScattering = Shader.PropertyToID("_Scattering");
    private static readonly int IdScatterBoost = Shader.PropertyToID("_ScatterBoost");
    private static readonly int IdNoiseScale = Shader.PropertyToID("_NoiseScale");
    private static readonly int IdNoiseStrength = Shader.PropertyToID("_NoiseStrength");
    private static readonly int IdNoiseSpeed = Shader.PropertyToID("_NoiseSpeed");

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    private Material material;
    private bool geometryDirty = true;

    private void OnEnable()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        EnsureMaterial();
        geometryDirty = true;
        Rebuild();
        ApplyMaterial();
    }

    private void OnDisable()
    {
        if (mesh != null)
        {
            DestroyOwned(mesh);
            mesh = null;
        }

        if (material != null)
        {
            DestroyOwned(material);
            material = null;
        }
    }

    private void OnValidate()
    {
        range = Mathf.Max(0.01f, range);
        radialSegments = Mathf.Clamp(radialSegments, 3, 96);
        lengthSegments = Mathf.Clamp(lengthSegments, 1, 32);
        geometryDirty = true;
    }

    private void LateUpdate()
    {
        if (geometryDirty)
        {
            Rebuild();
        }

        ApplyMaterial();
        SyncLight();
    }

    private void EnsureMaterial()
    {
        if (material != null)
        {
            return;
        }

        // Prefer an already-assigned material, otherwise build one from the shader.
        Material existing = meshRenderer != null ? meshRenderer.sharedMaterial : null;
        if (existing != null && existing.shader != null && existing.shader.name == ShaderName)
        {
            material = existing;
            return;
        }

        Shader shader = Shader.Find(ShaderName);
        if (shader == null)
        {
            Debug.LogError($"VolumetricLightCone: shader '{ShaderName}' not found.", this);
            return;
        }

        material = new Material(shader)
        {
            name = "VolumetricLightCone (instance)",
            hideFlags = HideFlags.DontSave,
        };

        if (meshRenderer != null)
        {
            meshRenderer.sharedMaterial = material;
        }
    }

    private void ApplyMaterial()
    {
        if (material == null)
        {
            EnsureMaterial();
            if (material == null)
            {
                return;
            }
        }

        material.SetColor(IdColor, color);
        material.SetFloat(IdIntensity, intensity);
        material.SetFloat(IdEdgeSoftness, edgeSoftness);
        material.SetFloat(IdLengthFade, lengthFade);
        material.SetFloat(IdTipFade, tipFade);
        material.SetFloat(IdDepthFade, depthFadeDistance);
        material.SetFloat(IdCameraFade, cameraFadeDistance);
        material.SetFloat(IdScattering, forwardScattering);
        material.SetFloat(IdScatterBoost, scatterBoost);
        material.SetFloat(IdNoiseScale, noiseScale);
        material.SetFloat(IdNoiseStrength, noiseStrength);
        material.SetVector(IdNoiseSpeed, noiseSpeed);

        if (meshRenderer != null && meshRenderer.sharedMaterial != material)
        {
            meshRenderer.sharedMaterial = material;
        }
    }

    private void SyncLight()
    {
        if (!syncChildSpotLight)
        {
            return;
        }

        if (spotLight == null)
        {
            spotLight = GetComponentInChildren<Light>();
        }

        if (spotLight == null)
        {
            return;
        }

        spotLight.type = LightType.Spot;
        spotLight.range = range;
        spotLight.spotAngle = Mathf.Clamp(coneAngle, 1f, 179f);
        spotLight.color = new Color(color.r, color.g, color.b, 1f);
    }

    private void Rebuild()
    {
        geometryDirty = false;

        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        if (mesh == null)
        {
            mesh = new Mesh { name = "VolumetricLightCone" };
            mesh.hideFlags = HideFlags.DontSave;
        }

        BuildConeMesh(mesh);

        if (meshFilter != null)
        {
            meshFilter.sharedMesh = mesh;
        }
    }

    // Open cone: apex at local origin, opening toward +Z, mouth radius from the half-angle.
    private void BuildConeMesh(Mesh target)
    {
        target.Clear();

        int radial = Mathf.Clamp(radialSegments, 3, 96);
        int rings = Mathf.Clamp(lengthSegments, 1, 32);
        float mouthRadius = Mathf.Tan(Mathf.Deg2Rad * Mathf.Clamp(coneAngle, 2f, 170f) * 0.5f) * range;
        float slope = mouthRadius / Mathf.Max(range, 1e-4f);
        float normalZ = -slope / Mathf.Sqrt(1f + slope * slope);
        float normalR = 1f / Mathf.Sqrt(1f + slope * slope);

        int ringVerts = radial + 1;
        int sideVertCount = ringVerts * (rings + 1);
        bool cap = capMouth;
        int capVertCount = cap ? ringVerts + 1 : 0;

        Vector3[] vertices = new Vector3[sideVertCount + capVertCount];
        Vector3[] normals = new Vector3[vertices.Length];
        Vector2[] uv = new Vector2[vertices.Length];

        for (int ring = 0; ring <= rings; ring++)
        {
            float t = (float)ring / rings;
            float z = t * range;
            float r = t * mouthRadius;

            for (int i = 0; i <= radial; i++)
            {
                float a = (float)i / radial * Mathf.PI * 2f;
                float cos = Mathf.Cos(a);
                float sin = Mathf.Sin(a);

                int v = ring * ringVerts + i;
                vertices[v] = new Vector3(cos * r, sin * r, z);
                normals[v] = new Vector3(cos * normalR, sin * normalR, normalZ);
                uv[v] = new Vector2((float)i / radial, t);
            }
        }

        int sideTriCount = radial * rings * 6;
        int capTriCount = cap ? radial * 3 : 0;
        int[] triangles = new int[sideTriCount + capTriCount];

        int ti = 0;
        for (int ring = 0; ring < rings; ring++)
        {
            for (int i = 0; i < radial; i++)
            {
                int a = ring * ringVerts + i;
                int b = a + 1;
                int c = a + ringVerts;
                int d = c + 1;

                triangles[ti++] = a;
                triangles[ti++] = c;
                triangles[ti++] = b;

                triangles[ti++] = b;
                triangles[ti++] = c;
                triangles[ti++] = d;
            }
        }

        if (cap)
        {
            int centre = sideVertCount + ringVerts;
            vertices[centre] = new Vector3(0f, 0f, range);
            normals[centre] = Vector3.forward;
            uv[centre] = new Vector2(0.5f, 1f);

            for (int i = 0; i <= radial; i++)
            {
                float a = (float)i / radial * Mathf.PI * 2f;
                int v = sideVertCount + i;
                vertices[v] = new Vector3(Mathf.Cos(a) * mouthRadius, Mathf.Sin(a) * mouthRadius, range);
                normals[v] = Vector3.forward;
                uv[v] = new Vector2((float)i / radial, 1f);
            }

            for (int i = 0; i < radial; i++)
            {
                triangles[ti++] = centre;
                triangles[ti++] = sideVertCount + i;
                triangles[ti++] = sideVertCount + i + 1;
            }
        }

        target.vertices = vertices;
        target.normals = normals;
        target.uv = uv;
        target.triangles = triangles;
        target.RecalculateBounds();
    }

    private static void DestroyOwned(Object obj)
    {
        if (obj == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(obj);
        }
        else
        {
            DestroyImmediate(obj);
        }
    }

    private void OnDrawGizmosSelected()
    {
        float mouthRadius = Mathf.Tan(Mathf.Deg2Rad * Mathf.Clamp(coneAngle, 2f, 170f) * 0.5f) * range;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(color.r, color.g, color.b, 0.5f);

        Vector3 tip = Vector3.zero;
        Vector3 end = new Vector3(0f, 0f, range);
        const int spokes = 16;
        Vector3 prev = end + new Vector3(mouthRadius, 0f, 0f);
        for (int i = 1; i <= spokes; i++)
        {
            float a = (float)i / spokes * Mathf.PI * 2f;
            Vector3 p = end + new Vector3(Mathf.Cos(a) * mouthRadius, Mathf.Sin(a) * mouthRadius, 0f);
            Gizmos.DrawLine(prev, p);
            if (i % 4 == 0)
            {
                Gizmos.DrawLine(tip, p);
            }
            prev = p;
        }
    }
}
