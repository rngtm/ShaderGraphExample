using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour
{
    [SerializeField] private float m_TimeScale = 1f;
    [SerializeField] private Vector3 m_RotateSpeeds = new Vector3(3f, 4f, 5f);
    [Space]
    [SerializeField] private bool randomize = false;
    [SerializeField] float randomizeInterval = 4f;
    float randomizeTime = 0f;

    public void SetRotation(Vector3 speed)
    {
        m_RotateSpeeds = speed;
    }

    void Update()
    {
        transform.Rotate(m_RotateSpeeds * m_TimeScale * Time.deltaTime);

        if (randomize && Time.time > randomizeTime + randomizeInterval)
        {
            randomizeTime = Time.time;
            float speed = m_RotateSpeeds.magnitude;
            m_RotateSpeeds = new Vector3(Random.value, Random.value, Random.value).normalized * speed;
        }
    }
}
