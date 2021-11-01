using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{//a very simple enemy stat manager need to expand on this more later on
    [Header("Assign Level & extra stats")]
    [SerializeField] int enemyLevel;
    [SerializeField] int healthAdd;
    [SerializeField] int defence;

    [Header("Dont touch these")]
    [SerializeField] int maxHealth;
    [SerializeField] int curHealth;
    [SerializeField] int xp;

    NavMeshAgent agent;
    
    LevelSystem levelSystem;

    public uint hitID;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        levelSystem = LevelSystem.instance;
        maxHealth = enemyLevel * (int)(100 * Random.Range(0.75f,1.25f));
        maxHealth += healthAdd;
        curHealth = maxHealth;

        //think of formula here based on level this is placeholder for defence & xp
        defence = enemyLevel * 2;
        xp = (enemyLevel * 100) / 5;

    }


    bool oneRun;
    // Update is called once per frame
    void Update()
    {
        /*
         * 
        timer = 0;
        //player = GameObject.FindGameObjectWithTag("Player").transform;
         * vars
         * 
         *     
    Transform player;
    [SerializeField] GameObject childTrigger;
    bool isAttacking;
    float timer;
    
    /*
        if (!isAttacking)
        {
            agent.SetDestination(player.position);

            if (Vector3.Distance(player.transform.position, transform.position) <= 3f)
            {
                isAttacking = true;
                oneRun = true;
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                if (oneRun)
                {
                    childTrigger.SetActive(true);
                    oneRun = false;
                }

                if (timer >= 3.5f)
                {
                    isAttacking = false;
                    childTrigger.SetActive(false);
                    timer = 0;
                }
            }
        }
        */
    }

    public void TakeDamageFromPlayer(int incDmg)
    {
        incDmg -= defence;
        incDmg = Mathf.Clamp(incDmg, 0, int.MaxValue);
        curHealth -= incDmg;
        if (curHealth <= 0)
        {
            //SendXPToPlayer(xp);
            levelSystem.onXPGainedDelegate.Invoke(enemyLevel, xp);
            Destroy(gameObject);
        }
    }

    public void TakeDamageFromFamiliar(int incDmg)
    {
        incDmg -= defence;
        incDmg = Mathf.Clamp(incDmg, 0, int.MaxValue);
        curHealth -= incDmg;
        if (curHealth <= 0)
        {
            //SendXPToPlayer(xp);
            levelSystem.onXPGainedDelegate.Invoke(enemyLevel, xp);
            Destroy(gameObject);
        }
    }
}
