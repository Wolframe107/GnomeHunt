using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Light))]
public class DayNightLight : MonoBehaviour
{
    private Light m_light;

    private void Start()
    {
        m_light = GetComponent<Light>();
    }

    private void Update()
    {
        if (IsDark())
        {
            m_light.enabled = true;
        }
        else
        {
            m_light.enabled = false;
        }
    }

    private bool IsDark()
    {
        float t = TimeManager.Instance.GetNormalizedTime();
        if (t < 0.35f || t > 0.65)
        {
            return true;
        }

        return false;
    }
}
