using UnityEngine;

public class TorchFlicker : MonoBehaviour
{
    public Light torchLight;
    public float flickerSpeed = 0.1f;
    public float intensityMin = 0.5f;
    public float intensityMax = 1.5f;

    private void Start()
    {
        if (torchLight == null)
        {
            torchLight = GetComponent<Light>();
        }
    }

    private void Update()
    {
        float randomValue = Random.Range(intensityMin, intensityMax);
        torchLight.intensity = randomValue;
    }
}