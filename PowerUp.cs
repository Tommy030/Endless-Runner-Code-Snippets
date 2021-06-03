using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Tooltip("Can be Minus to be made into a debuff")]
    [SerializeField] private int m_IncreaseStaminaAmount = 20;
    [Tooltip("Can Be Minus to Slow down Map Speed")]
    [SerializeField] private int m_IncreaseMapSpeed = 5;
    [Tooltip("Duration of the Powerup")]
    [SerializeField] private int m_PowerUpDuration = 10;
    [Tooltip("Shield PowerUP")]
    [SerializeField] private bool m_ShieldPower = false;

    [Range(0, 1)]
    [SerializeField]public float m_Odds = 0.3f;
    private void Awake()
    {
        if (m_Odds < Random.value) gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerMovement>() != null)
        {       
            other.gameObject.GetComponent<PlayerMovement>().IncreaseStamina(m_IncreaseStaminaAmount);
            GameManager.instance.AddNewPowerUpTimer(m_PowerUpDuration,m_IncreaseMapSpeed);

            if (m_ShieldPower)
                other.gameObject.GetComponent<PlayerMovement>().AddShieldPowerUP(m_ShieldPower);
            gameObject.SetActive(false);
        }
    }
}
