using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    [Header("Movement")]
    private Rigidbody m_rb;
    private Animator ani;

    private float m_xAxis;
    [Tooltip("Speed of the Player")]
    [SerializeField] private float m_Speed = 2;
    [SerializeField]private float m_oldspeed;
    [SerializeField] private float m_JumpSpeed = 2;

    [Tooltip("Max Stamina of the Player")]
    [SerializeField] private float m_MaxStamina = 100;
    [Tooltip("Decrease Stamina every X seconds")]
    [SerializeField] private float m_DecreaseStaminaSeconds = 5f;
    [Tooltip("Decrease amount")]
    [SerializeField] private float m_StaminaDecreaseAmount = 20;
    [Tooltip("For Debugging Purposes")]
    [SerializeField] private bool m_TurnOffStamina = false;
    //Current Stamina of this unit
    [SerializeField]private float m_Stamina;
    public float m_StaminaTimer =0;

    [Header("Jumping")]
    [Tooltip("Height the Player Jumps")]
    [SerializeField] private float m_JumpHeight = 10f;
    [Tooltip("Distance needed between Player & Ground")]
    [SerializeField] private float m_JumpRayLength = .6f;
    [Tooltip("Time Delay for Jumping")]
    [SerializeField] private float m_JumpDelay = .25f;
    [Tooltip("Fall multiplier for Falling")]
    [SerializeField] private float m_fallmultiplier = 5;

    //Timer for the jump;
    private float m_JumpTimer = 0f;

    [Header("Physics")]
    [Tooltip("Normal Gravity")]
    [SerializeField] private float m_Gravity = 1;
    [Tooltip("Simulating gravity of Earth")]
    [SerializeField] private float m_GlobalGravity = -9.81f;
    [Tooltip("Linear Drag")]
    [SerializeField] private float m_Lineardrag = 4;

    [Header("Collision")]
    [Tooltip("Collider offset to adjust to the feet of the player")]
    [SerializeField] private Vector3 m_ColliderOffSett;
    [Tooltip("The layer of the ground")]
    [SerializeField] private LayerMask m_GroundLayer;
    [SerializeField] private LayerMask m_WallLayer;
    [Tooltip("Checks if the player is grounded to the ground")]
    [SerializeField] private bool m_IsGrounded;

    [SerializeField] private bool m_HasShield = false;

    [SerializeField] public ParticleSystem m_ParticleDirt;
    [SerializeField] public ParticleSystem m_ParticleShield;
    public bool m_FadeAway = false;
    public bool m_EndScreenWait = false;
    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else if (instance == null)
            instance = this;
    }
    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        ani = GetComponent<Animator>();
        m_Stamina = m_MaxStamina;
        m_oldspeed = m_Speed;
    }
    private void Update()
    {
        if (!m_FadeAway)
        {
            m_IsGrounded = Physics.Raycast(transform.position + m_ColliderOffSett, Vector3.down, m_JumpRayLength, m_GroundLayer);
            if (m_IsGrounded) ani.SetBool("Jump", false);
            else ani.SetBool("Jump", true);
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && !GameManager.instance.gameOver && GameManager.instance.gameStarted)
            {
             
                m_JumpTimer = Time.time + m_JumpDelay;
                ani.SetBool("Jump", true);
                //MapGenerator.instance.mapSpeed 
            }

            if (!GameManager.instance.gameOver) m_xAxis = Input.GetAxisRaw("Horizontal");
            else if (GameManager.instance.gameOver) m_xAxis = 0;

            if (m_StaminaTimer == 0 && GameManager.instance.gameStarted && !GameManager.instance.gameOver)
            {
                m_StaminaTimer = Time.time + m_DecreaseStaminaSeconds;
            }
            else if (m_StaminaTimer <= Time.time && GameManager.instance.gameStarted && !GameManager.instance.gameOver)
            {
                DecreaseStamina(m_StaminaDecreaseAmount);
            }
        }

        if (!GameManager.instance.gameStarted && Input.anyKey && !m_FadeAway && !m_EndScreenWait && !Input.GetKey(KeyCode.Tab))
        {
            GameManager.instance.StartGame();
            ani.SetBool("IsRunning", true);
        }
        else if (GameManager.instance.gameOver && Input.anyKey && !m_FadeAway && !m_EndScreenWait && !Input.GetKey(KeyCode.Tab)) StartCoroutine(UIManager.instance.FadeScreen());

        if (m_IsGrounded && GameManager.instance.gameStarted && !GameManager.instance.gameOver && !m_FadeAway)
        {
            if (!m_ParticleDirt.isPlaying)
            {
            m_ParticleDirt.Play();
            }
            m_Speed = m_oldspeed;
        }
        else
        {
            m_ParticleDirt.Stop();
            
        }

        if (Input.GetKey(KeyCode.Tab) && !GameManager.instance.gameOver)
        {
            UIManager.instance.GuideParent.SetActive(true);
            if (!GameManager.instance.gameStarted)
                UIManager.instance.m_MainMenu.SetActive(false);

        }
        else
        {
            UIManager.instance.GuideParent.SetActive(false);
            if (!GameManager.instance.gameStarted)
                UIManager.instance.m_MainMenu.SetActive(true);
        }

        if (m_HasShield)
        {
            if (!m_ParticleShield.isPlaying)
                m_ParticleShield.Play();
        }
        else
            m_ParticleShield.Stop();
    }
    #region Stamina

    public void IncreaseStamina(int Amount)
    {
        if (!m_TurnOffStamina)
        {
           if (m_Stamina + Amount >= m_MaxStamina)
           {
              m_Stamina = m_MaxStamina;
              
           }
           else
           {
              m_Stamina += Amount;
           }
            UIManager.instance.UpdateStaminaBar(m_Stamina);
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        IncreaseStamina(5);
        Debug.Log("StaminaAdded");
    }
    public void DecreaseStamina(float Amount)   
    {
        if (!m_TurnOffStamina)
        {
           if (m_Stamina - Amount > 0)
            {
               m_Stamina -= Amount;
               UIManager.instance.UpdateStaminaBar(m_Stamina);
            }
           else
           {
                UIManager.instance.UpdateStaminaBar(m_Stamina); 
                GameManager.instance.SetGameOver(false);
           }
            UIManager.instance.UpdateStaminaBar(m_Stamina);
            m_StaminaTimer = 0;
        }
    }
    #endregion
    #region Collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
                if (!m_HasShield)
                {
                 GameManager.instance.SetGameOver(true);
                ani.SetBool("IsRunning", false);
                }
                else
                {
                    m_HasShield = false;
                    collision.gameObject.SetActive(false);
                }
            
        }
    }
    public void AddShieldPowerUP(bool Shield)
    {
        m_HasShield = Shield;
    }
    #endregion
    #region FixedUpdate And Physics
    private void FixedUpdate()
    {
        m_rb.position = new Vector3(m_rb.position.x, m_rb.position.y, m_rb.position.z + -m_xAxis * m_Speed * Time.fixedDeltaTime);
        if (m_JumpTimer > Time.time && m_IsGrounded)
            Jump();
        ModifyPhysics();
    }
    private void Jump()
    {
        m_oldspeed = m_Speed;
        m_Speed = m_JumpSpeed;
        m_rb.velocity = new Vector2(m_rb.velocity.x, 0);
        m_rb.AddForce(Vector2.up * m_JumpHeight, ForceMode.Impulse);
        m_JumpTimer = 0;
    }
    private void ModifyPhysics()
    {

        bool changingDirections = (m_xAxis > 0 && m_rb.velocity.x < 0) || (m_xAxis < 0 && m_rb.velocity.x > 0);

        if (m_IsGrounded)
        {
            if (Mathf.Abs(m_xAxis) < 0.4f || changingDirections)
            {
                m_rb.drag = m_Lineardrag;
            }
            else
            {
                m_rb.drag = 0f;
            }
            m_rb.useGravity = false;
        }
        else
        {
            m_rb.useGravity = true;
            Vector3 Gravity = m_GlobalGravity * m_Gravity * Vector3.up;
            m_rb.drag = m_Lineardrag * .15f;
            if (m_rb.velocity.y < 0)
            {
                Gravity = m_GlobalGravity * m_Gravity * m_fallmultiplier * Vector3.up;
            }
            else if (m_rb.velocity.y > 0 && !Input.GetKey(KeyCode.W)  && !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.UpArrow))
            {
                Gravity = m_GlobalGravity * m_Gravity * (m_fallmultiplier * .5f) * Vector3.up;
            }

            m_rb.AddForce(Gravity, ForceMode.Acceleration);
        }
    }
    #endregion
    #region Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + m_ColliderOffSett, transform.position + m_ColliderOffSett + Vector3.down * m_JumpRayLength);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position - m_ColliderOffSett, transform.position - m_ColliderOffSett + Vector3.down * m_JumpRayLength);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * m_JumpRayLength);
    }
    #endregion
}
