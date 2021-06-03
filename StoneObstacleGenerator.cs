using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneObstacleGenerator : MonoBehaviour
{
    [SerializeField]private Transform m_endpos1;
    [SerializeField]private Transform m_endpos2;
    [SerializeField] private List<GameObject> Stones = new List<GameObject>();

    [SerializeField]private int SpawnMinimum;
    [SerializeField]private int spawnmaximum;

    private ObjectPool pool; 
    private void Awake()
    {
        pool = new ObjectPool(Stones, 6, true, gameObject.transform);
    }
    private void OnEnable()
    {
        Randomize();
    }

    private void Randomize()
    {
        int spawnamount = Random.Range(SpawnMinimum, spawnmaximum);

        for (int i = 0; i < spawnamount; i++)
        {
            Vector3 Newpos = new Vector3(Random.Range(m_endpos1.position.x, m_endpos2.position.x), gameObject.transform.position.y, Random.Range(m_endpos1.position.z, m_endpos2.position.z));

            GameObject stone = pool.GetObjectFromPool();
            stone.SetActive(true);
            stone.transform.position = Newpos;
        }
    }

    private void OnDisable()
    {
        pool.SetAllObjectsOff();
    }
}
 