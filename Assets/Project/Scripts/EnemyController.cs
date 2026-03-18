using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]  //Adding this sentence this class will be used only if the GameObject has a NavMeshAgent
[RequireComponent(typeof(UnityEngine.Animator))]

public class EnemyController : MonoBehaviour
{

    #region Constants
    private const int DAMAGE_POINTS = 10;
    private const int MILLISECONDS_AMONG_ATTACK = 3000;
    #endregion

    #region Inspector Fields
    [SerializeField]
    private int raisePlayer = 5;

    [SerializeField]
    private float waitTime = 5f;

    [SerializeField]
    private float baseWaitPointRadius = 1.0f;

    [SerializeField]
    private float extraRadiusPerEnemy = 0.25f;

    [SerializeField]
    private float maxWaitPointRadius = 3.0f;
        
    [SerializeField]
    private float nearbyEnemyCheckRadius = 3.0f;

    [SerializeField]
    private LayerMask enemyLayer;

    [SerializeField]
    private GameObject spawnEffect;

    #endregion

    #region Private Fields
    private int millisencondsSinceLastAttack = 0;
    private Transform[] waitPoints;
    private GameObject player;
    private Rigidbody playerRigid;
    private NavMeshAgent pathFinder;
    private Animator anim;
    private int currentWaitPointIndex = 0;
    private float waitCounter = 0f;
    
    #region Static State (Global Enemy State)
    private bool isActivated = false;
    private static int aliveEnemies = 0;
    #endregion

    #endregion

    //Delegated method to attack the player
    public delegate void Attack(int damage);
    //Event to attack the player
    public event Attack OnAttack = null;


    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        playerRigid = player.GetComponent<Rigidbody>();
        OnAttack += player.GetComponent<PlayerController>().Injure;
        anim = this.GetComponent<Animator>();
        aliveEnemies++;

    }

    //To assign the waitPoints from GameController
    public void Initialize(Transform[] levelWaitPoints, bool spawnedAtRuntime)
    {

        waitPoints = levelWaitPoints;

        if (waitPoints == null || waitPoints.Length < 2)
        {
            Debug.LogError("Enemy needs at least 2 WaitPoints");
        }

        pathFinder = GetComponent<NavMeshAgent>();
        pathFinder.SetDestination(waitPoints[0].position);

        if (spawnEffect != null && spawnedAtRuntime)
        {
            doSpawnEffect();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated)
        {
            goAgainstPlayer();
        }

        else if (pathFinder.pathPending)
            return;


        //Instead of arriving to the exact WaitPoint, a radius is created
        //to avoid enemies from freezing when some of them are trying to
        //wait at the same WaitPoint
        float distanceToPoint = Vector3.Distance(
            transform.position,
            waitPoints[currentWaitPointIndex].position
        );

        if (distanceToPoint <= GetDynamicWaitPointRadius())
        {
            waitCounter += Time.deltaTime;

            if (waitCounter >= waitTime)
            {
                GoToNextPoint();
                waitCounter = 0f;
            }
        }
    }

    #region Chase and attack behaviour
    private void goAgainstPlayer()
    {
        pathFinder.SetDestination(player.transform.position);
        bool isWalking = pathFinder.remainingDistance > pathFinder.stoppingDistance;
        anim.SetBool("isWalking", isWalking);
        this.millisencondsSinceLastAttack += (int)(Time.deltaTime * 1000);
        /* if (!isWalking) //Force to look at the player when the enemy is not walking
         {
             gameObject.transform.LookAt(player.transform);
         }*/
    }
    private void OnTriggerEnter(Collider other)
    {//OnTriggerEnter
        if (other.gameObject.tag.Equals("Player"))
        {
            ActivateAllExistingEnemies();
        }
    }//OnTriggerEnter

    private static void ActivateAllExistingEnemies()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();

        foreach (EnemyController enemy in enemies)
        {
            enemy.isActivated = true;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {//OnCollisionEnter
        if (this.millisencondsSinceLastAttack >= MILLISECONDS_AMONG_ATTACK &&
            collision.gameObject.tag.Equals("Player"))
        {
            OnAttack(DAMAGE_POINTS);
            playerRigid.AddForce(Vector3.up * raisePlayer, ForceMode.Impulse);
            this.millisencondsSinceLastAttack = 0;
        }
    }//OnCollisionEnter
    #endregion

    #region Patrol behaviour
    void GoToNextPoint()
    {
        currentWaitPointIndex++;

        if (currentWaitPointIndex >= waitPoints.Length)
            currentWaitPointIndex = 0;

        pathFinder.SetDestination(
            waitPoints[currentWaitPointIndex].position
        );
    }

    #region WaitPoint Radius Logic
    private int GetNearbyEnemiesAtWaitPoint()
    {
        Collider[] enemies = Physics.OverlapSphere(
            waitPoints[currentWaitPointIndex].position,
            nearbyEnemyCheckRadius,
            enemyLayer
        );

        return enemies.Length;
    }

    //The radius for enemies to wait grows dinamically, depending
    //on the number of enemies trying to wait on that spot
    private float GetDynamicWaitPointRadius()
    {
        int nearbyEnemies = GetNearbyEnemiesAtWaitPoint();

        float radius =
            baseWaitPointRadius +
            (nearbyEnemies - 1) * extraRadiusPerEnemy;

        return Mathf.Min(radius, maxWaitPointRadius);
    }
    #endregion

    //Useful for debugging
    void OnDrawGizmosSelected()
    {
        if (waitPoints == null || waitPoints.Length == 0) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(
            waitPoints[currentWaitPointIndex].position,
            GetDynamicWaitPointRadius()
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            waitPoints[currentWaitPointIndex].position,
            nearbyEnemyCheckRadius
        );
    }

    #endregion

    #region Death behaviour

    //When the enemy has been killed this service will be used
    void Die()
    {
        isActivated = false;
        anim.SetTrigger("isDead");
        pathFinder.isStopped = true;

        aliveEnemies--;

        if (aliveEnemies <= 0)
        {
            isActivated = false;
        }
    }

    //Remove de GameObjet from playground
    void Destroy()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Spawn Effect
    private void doSpawnEffect()
    {
        GameObject fx = Instantiate(
                spawnEffect,
                transform.position,
                Quaternion.identity
            );

        Destroy(fx, 3f);
    }
    #endregion
}
