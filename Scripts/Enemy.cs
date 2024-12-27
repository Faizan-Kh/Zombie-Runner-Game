using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
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
    [SerializeField]
    float maxHealth = 4f;
    [SerializeField]
    private UnityEngine.UI.Image healthBar; // Reference to the health bar image

    float currentHealth = 0f;
    bool isAlive = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalPos = transform.position;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            Navigate();
        }
        else
        {
            agent.ResetPath();
        }
    }

    private void Navigate()
    {
        if (Detect())
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
            //Setting the direction previously facing and rotating
            Vector3 direction = -originalPos - transform.position;
            direction.y = 0;
            direction.Normalize();

            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 50f * Time.deltaTime);

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

    public void Damage(Vector3 hitPoint, Vector3 camPosition, float forceMagnitude)
    {
        // Reduce health
        currentHealth -= 1f;

        // Update the health bar's fill amount
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }

        // Check if the enemy is still alive
        if (currentHealth > 0f)
        {
            // Optionally trigger a hit reaction animation (e.g., flinch)
            animator.SetTrigger("isHit");
        }
        else if (isAlive) // Ensure death logic is only triggered once
        {
            // Trigger death behavior
            isAlive = false;
            animator.enabled = false;

            // Handle ragdoll effect
            Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
            Rigidbody hitRb = rbs.OrderBy(rigidbody => Vector3.Distance(rigidbody.position, hitPoint)).FirstOrDefault();

            // Calculate the force direction
            Vector3 forceDirection = transform.position - camPosition;
            forceDirection.y = 0;
            forceDirection.Normalize();

            Vector3 force = forceDirection * forceMagnitude;
            hitRb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);

            // Additional cleanup or effects on death can go here
        }
    }


}
