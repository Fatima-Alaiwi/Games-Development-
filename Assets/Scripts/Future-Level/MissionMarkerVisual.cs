using UnityEngine;

public class MissionMarkerVisual : MonoBehaviour
{
    public bool buildMarkerOnAwake = true;
    public Color markerColor = Color.cyan;
    public float markerRadius = 2.2f;
    public float markerHeight = 3f;
    public Transform rotatingVisual;
    public Light markerLight;
    public float rotationSpeed = 90f;
    public float pulseSpeed = 2f;
    public float minScale = 0.9f;
    public float maxScale = 1.15f;
    public float minLightIntensity = 1.5f;
    public float maxLightIntensity = 4f;

    private Vector3 _baseScale;
    private Material _markerMaterial;

    private void Awake()
    {
        if (buildMarkerOnAwake)
        {
            BuildMarkerIfNeeded();
        }

        if (rotatingVisual == null)
        {
            rotatingVisual = transform;
        }

        _baseScale = rotatingVisual.localScale;
    }

    private void Update()
    {
        if (rotatingVisual != null)
        {
            rotatingVisual.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

            float scale = Mathf.Lerp(minScale, maxScale, GetPulse());
            rotatingVisual.localScale = _baseScale * scale;
        }

        if (markerLight != null)
        {
            markerLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, GetPulse());
        }
    }

    private float GetPulse()
    {
        return (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
    }

    private void BuildMarkerIfNeeded()
    {
        _markerMaterial = CreateMarkerMaterial();

        if (rotatingVisual == null)
        {
            GameObject visualRoot = new GameObject("Marker Visual");
            visualRoot.transform.SetParent(transform, false);
            rotatingVisual = visualRoot.transform;

            CreateCylinder(visualRoot.transform, "Marker Beam", new Vector3(markerRadius, markerHeight * 0.5f, markerRadius), new Vector3(0f, markerHeight * 0.5f, 0f));
            CreateRing(visualRoot.transform, "Marker Ring Bottom", 0.05f);
            CreateRing(visualRoot.transform, "Marker Ring Top", markerHeight);
        }

        if (markerLight == null)
        {
            GameObject lightObject = new GameObject("Marker Light");
            lightObject.transform.SetParent(transform, false);
            lightObject.transform.localPosition = new Vector3(0f, markerHeight * 0.5f, 0f);
            markerLight = lightObject.AddComponent<Light>();
            markerLight.type = LightType.Point;
            markerLight.color = markerColor;
            markerLight.range = markerRadius * 4f;
            markerLight.intensity = maxLightIntensity;
        }

        Collider existingCollider = GetComponent<Collider>();
        if (existingCollider == null)
        {
            SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = markerRadius;
            trigger.center = new Vector3(0f, markerHeight * 0.5f, 0f);
        }
    }

    private Material CreateMarkerMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        Material material = new Material(shader);
        material.color = new Color(markerColor.r, markerColor.g, markerColor.b, 0.35f);
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", markerColor * 2.5f);

        if (material.HasProperty("_Surface"))
        {
            material.SetFloat("_Surface", 1f);
        }

        if (material.HasProperty("_Mode"))
        {
            material.SetFloat("_Mode", 3f);
        }

        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.EnableKeyword("_ALPHABLEND_ON");

        material.renderQueue = 3000;
        return material;
    }

    private void CreateCylinder(Transform parent, string objectName, Vector3 localScale, Vector3 localPosition)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.name = objectName;
        cylinder.transform.SetParent(parent, false);
        cylinder.transform.localPosition = localPosition;
        cylinder.transform.localScale = localScale;

        Collider collider = cylinder.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        Renderer renderer = cylinder.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = _markerMaterial;
        }
    }

    private void CreateRing(Transform parent, string objectName, float height)
    {
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = objectName;
        ring.transform.SetParent(parent, false);
        ring.transform.localPosition = new Vector3(0f, height, 0f);
        ring.transform.localScale = new Vector3(markerRadius, 0.03f, markerRadius);

        Collider collider = ring.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        Renderer renderer = ring.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = _markerMaterial;
        }
    }
}
