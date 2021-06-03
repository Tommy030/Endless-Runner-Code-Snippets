using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    public static GameManager instance;

    public bool gameOver;
    public int score { private set; get; }
    
    private List<Timer> m_PowerUpTimers = new List<Timer>(); //Timer That keeps track of all powerups picked up
    [HideInInspector] public bool gameStarted;
    public float scoreMultiplier = 1;

    [Tooltip("X= score, Y=New speed, Z= time")]
    [SerializeField] private Vector3[] SpeedUps;
    [SerializeField] private GameObject TreePlayerPrefab;

    private IEnumerator m_Speedupmap;

    [SerializeField] private float m_MapStartSpeed = 10f;
    [SerializeField] private float m_Increaseamount = 1.5f;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);

    }

    private void Update() 
    {
        UpdateTimers();
    }

    public void StartGame()
    {
        gameStarted = true;
        UIManager.instance.m_MainMenu.SetActive(false);
        UIManager.instance.m_HighscoreStart.gameObject.SetActive(false);
        PlayerMovement.instance.m_ParticleDirt.Play();
        StartCoroutine(GameLoop());
    }

    public void RestartGame()
    {
        gameOver = false;
        gameStarted = false;
        MapGenerator.instance.ResetMap();
        score = 0;
        PlayerMovement.instance.transform.position = new Vector3(-3.04f, 0, 0);
        PlayerMovement.instance.IncreaseStamina(int.MaxValue);
        PlayerMovement.instance.m_StaminaTimer = 0;
    }

    #region Powerup
    public void PowerupOdds(float odds,GameObject gam)
    {
        if (odds < Random.value)
        {
            gam.SetActive(true);
            gam.transform.localPosition = new Vector3(5, 1, Random.Range(-2.7f, 2.7f));
        }
    }
    #region PowerUPTimer
    private struct Timer
    {
        public float m_TimeEnd;
        public int SpeedAdded;
    }
    public void AddNewPowerUpTimer(float TimeOFThisPowerupDuration, int SpeedAddedToMap)
    {
        m_PowerUpTimers.Add(new Timer { m_TimeEnd = Time.time + TimeOFThisPowerupDuration, SpeedAdded = SpeedAddedToMap });
        MapGenerator.instance.mapSpeed += SpeedAddedToMap;
    }
    private void UpdateTimers()
    {
        if (m_PowerUpTimers.Count >0)
        {
            for (int i = 0; i < m_PowerUpTimers.Count; i++)
            {
                if (m_PowerUpTimers[i].m_TimeEnd <= Time.time)
                {
                    MapGenerator.instance.mapSpeed -= m_PowerUpTimers[i].SpeedAdded;
                    m_PowerUpTimers.RemoveAt(i);
                }
            }
        }
    }
    #endregion
    #endregion

    private IEnumerator GameLoop()
    {
        m_Speedupmap = SpeedUpMap(m_MapStartSpeed,0);
        StartCoroutine(m_Speedupmap);
        while (!gameOver)
        {
            for (int i = 0; i < SpeedUps.Length; i++)
            {
                if (SpeedUps[i].x == score)
                {
                    StartCoroutine(SpeedUpMap(SpeedUps[i].y, SpeedUps[i].z));
                }
            }

            AddScore(Mathf.RoundToInt(1 * scoreMultiplier));
            yield return new WaitForSeconds(1);
        }
    }

    public void AddScore(int ammount) //Adds Score 
    {
        score += ammount;
        UIManager.instance.UpdateScoreUI(score);
    }

    public void SetGameOver(bool instant)
    {
        if (instant)
        {
            gameOver = true;

            if (PlayerPrefs.GetFloat("Highscore") < score)
            {
                PlayerPrefs.SetFloat("Highscore", score);
            }
            StopAllCoroutines(); //<-- Stops all running Coroutines
            MapGenerator.instance.mapSpeed = 0;
            UIManager.instance.SetLosingEndScreen();
            StartCoroutine(EndScreenWait());
        }
        else if (!instant)
        {
            gameOver = true;
            StartCoroutine(SlowDownMap());
        }
    }
    private IEnumerator EndScreenWait()
    {
        PlayerMovement.instance.m_EndScreenWait = true;
        yield return new WaitForSeconds(.5f);
        PlayerMovement.instance.m_EndScreenWait = false;
        
    }
    private IEnumerator SpeedUpMap(float maxSpeed, float time)
    {
        while (MapGenerator.instance.mapSpeed < maxSpeed)
        {
            Mathf.Clamp(MapGenerator.instance.mapSpeed += .1f, 0, 500);
            yield return new WaitForSeconds(time);

            if (gameOver)
                StopCoroutine(m_Speedupmap);
                
        }
    }

    private IEnumerator SlowDownMap()
    {
        while (MapGenerator.instance.mapSpeed > 0)
        {
            Mathf.Clamp(MapGenerator.instance.mapSpeed--, 0, 500);
            yield return new WaitForSeconds(0.05f);
        }
        UIManager.instance.SetLosingEndScreen();
    }

    public void TreeSound(Vector3 pos)
    {
        Instantiate(TreePlayerPrefab, pos, Quaternion.identity);
    }
}
