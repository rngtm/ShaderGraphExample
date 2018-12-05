using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SetVelocity : MonoBehaviour
{
    [SerializeField] float m_AngularSpeed = 1f; // 角速度の係数
    [SerializeField] Vector3 m_AngularSpeeds = new Vector3(-0.3f, 0.4f, -0.3f);  // 角速度
    [SerializeField] float m_PositionSpeed = 1f; // 移動速度の係数
    [SerializeField] Vector3 m_PositionSpeeds = new Vector3(-7f, -7.5f, -0.2f); // 移動速度

    void Start()
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.angularVelocity = m_AngularSpeed * m_AngularSpeeds;
        rigidbody.velocity = m_PositionSpeed * m_PositionSpeeds;
        Debug.Log(rigidbody.velocity);

    }
}
