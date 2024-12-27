using System;
using UnityEngine;
using UnityEngine.AI;

public class Navigate : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent agent;
    [SerializeField]
    GameObject destinationObject;
    [SerializeField]
    Animator animator;
    [SerializeField]
    float detectDist, runDist, attackDist;
    [SerializeField]
    float distance;
    Vector3 originalPos;
    [SerializeField]
    Transform rayPoint;
    bool isDetected;
    [SerializeField]
    LayerMask playerLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Detect())
        {
            isDetected = true;
        }
        //Debug.Log("Dtected? " + isDetected);
        
        if (isDetected && CalculateDistance(transform.position, destinationObject.transform.position) < detectDist)
        {
            agent.SetDestination(destinationObject.transform.position);
            animator.SetBool("isWalking", true);
            if (CalculateDistance(transform.position, agent.destination) < runDist)
            {
                animator.SetBool("isRunning", true);
                if (CalculateDistance(transform.position, agent.destination) < attackDist)
                {
                    agent.ResetPath();
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isAttacking", true);
                }
                else
                {
                    animator.SetBool("isAttacking", false);
                    animator.SetBool("isWalking", true);
                }
            }
            else
            {
                animator.SetBool("isRunning", false);
            }
        }
        else
        {
            isDetected = false;
            agent.ResetPath();
            agent.SetDestination(originalPos);
            if (CalculateDistance(transform.position, originalPos) < 0.25)
            {
                animator.SetBool("isWalking", false);
                agent.ResetPath();
            }
        }
    }

    private float CalculateDistance(Vector3 from, Vector3 to)
    {
        distance = Vector3.Distance(from, to);
        return distance;
    }

    private bool Detect()
    {
        Ray ray = new Ray(rayPoint.position, rayPoint.forward);
        Debug.DrawRay(rayPoint.position, rayPoint.forward * detectDist, Color.red);
        bool isHit = Physics.Raycast(ray, detectDist, playerLayer);
        return isHit;
    }

}
