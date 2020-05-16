using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public Transform raycastPoint;
    public GameController gameController;
    public float sightDistance = 30f;
    public float hearDistance = 15f;
    public Transform[] patrolPoints;
    private NavMeshAgent agent;
    private Animator animator;
    [SerializeField]
    private Vector3 personalLastSighting;

    private bool playerShoutRun;
    private bool patrol = true;
    private int destPoint = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetFloat("Blend", 0.5f);
        GotoNextPoint();
    }

    void Update()
    {
        playerShoutRun = gameController.playerShoutRun;

        if(patrol && !PlayerInSight())
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                GotoNextPoint();
        }
        if(PlayerInSight())
        {
            patrol = false;
            agent.isStopped = true;
            StopAllCoroutines();
            transform.LookAt(player.position);
            animator.SetBool("Shoot", true);
        }
        else
        {
            patrol = true;
            agent.isStopped = false;
            animator.SetBool("Shoot", false);
        }
    }
    
    void GotoNextPoint() 
    {
        // Returns if no points have been set up
        if (patrolPoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = patrolPoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % patrolPoints.Length;
    }

    private void OnTriggerStay(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            
            if(playerShoutRun)
            {
                if(CalculatePathLength() < hearDistance)
                {
                    Debug.Log("Andar aaya");
                    personalLastSighting = player.position;
                    StartCoroutine(Investigate(personalLastSighting));
                }
            }
        }   
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            patrol = true;
            animator.SetBool("Shoot", false);
        }
    }

    IEnumerator Investigate(Vector3 position)
    {
        patrol = false;
        agent.acceleration = 6f;
        yield return new WaitForSeconds(1f);
        agent.SetDestination(position);
        yield return new WaitForSeconds(3f);
    }

    float CalculatePathLength()
    {
        NavMeshPath path = new NavMeshPath();

        float pathLength = 0f;

        agent.CalculatePath(player.position, path);

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            pathLength += Vector3.Distance(path.corners[i], path.corners[i+1]);
        }

        return pathLength;
    }

    public bool PlayerInSight()
    {
        float fieldOfView = 110f;

        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        RaycastHit hit;

        if(Physics.Raycast(raycastPoint.position, direction, out hit, sightDistance))
        {
            if(angle < 0.5f * fieldOfView && hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}
