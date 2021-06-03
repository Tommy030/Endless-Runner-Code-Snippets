using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupGenerator : MonoBehaviour
{
    [SerializeField] private Transform m_endpos1;
    [SerializeField] private Transform m_endpos2;
    [SerializeField] private GameObject Bird;
    [SerializeField] private GameObject Bunny;

    [SerializeField] private int SpawnBunnyMin =0;
    [SerializeField] private int SpawnBunnyMax =2;

    [SerializeField] private int SpawnBirdMin = 0;
    [SerializeField] private int SpawnBirdMax = 3;

    private ObjectPool BunnyPool;
    private ObjectPool BirdPool;
    private void Awake()
    {
        BunnyPool = new ObjectPool(Bunny, 6, true, gameObject.transform);
        BirdPool = new ObjectPool(Bird, 6, true, gameObject.transform);  
    }
    private void OnEnable()
    {
        Randomize();
    }

    private void Randomize()
    {
        int spawnamount = Random.Range(SpawnBunnyMin, SpawnBunnyMax);

        for (int i = 0; i < spawnamount; i++)
        {
            Vector3 Newpos = new Vector3(Random.Range(m_endpos1.position.x, m_endpos2.position.x), gameObject.transform.position.y, Random.Range(m_endpos1.position.z, m_endpos2.position.z));

            GameObject Powerup = BunnyPool.GetObjectFromPool();
            Powerup.SetActive(true);
            Powerup.transform.position = Newpos;
        }

        int spawna = Random.Range(SpawnBirdMin, SpawnBirdMax);
        for (int i = 0; i < spawna; i++)
        {
            Vector3 Newpos = new Vector3(Random.Range(m_endpos1.position.x, m_endpos2.position.x), Random.Range(m_endpos1.position.y, m_endpos2.position.y), Random.Range(m_endpos1.position.z, m_endpos2.position.z));

            GameObject Powerup = BirdPool.GetObjectFromPool();
            Powerup.SetActive(true);
            Powerup.transform.position = Newpos;
        }
    }

    private void OnDisable()
    {
        BunnyPool.SetAllObjectsOff();
        BirdPool.SetAllObjectsOff();
    }
}
