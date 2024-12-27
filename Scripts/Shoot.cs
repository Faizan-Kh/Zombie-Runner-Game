using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    Camera cam;
    [SerializeField]
    InputAction fire;
    bool isFired;
    [SerializeField]
    float forceMagnitude = 1000f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip singleShotClip; 
    [SerializeField] private AudioClip burstShotClip;  
    [SerializeField] private bool isBurstFire = false; 

    private void OnEnable()
    {
        fire.Enable();
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (fire.WasPressedThisFrame())
        {
            isFired = true;
        }
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        GameObject hitGO = Target(out hit);
        if (isFired && hitGO)
        {
            // Start in a separate thread
            StartCoroutine(Fire(hit, hitGO));
        }
    }

    private IEnumerator Fire(RaycastHit hit, GameObject hitGO)
    {
        isFired = false;

        // Play gunfire sound based on whether it's burst fire or single shot
        if (isBurstFire)
        {
            // Play burst shot sound
            if (burstShotClip && audioSource)
            {
                audioSource.PlayOneShot(burstShotClip);
            }
        }
        else
        {
            // Play single shot sound
            if (singleShotClip && audioSource)
            {
                audioSource.PlayOneShot(singleShotClip);
            }
        }

        if (hitGO.CompareTag("Enemy"))
        {
            Enemy enemy = hitGO.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.Damage(hit.point, cam.transform.position, forceMagnitude);
            }
        }

        yield return null;
    }

    private GameObject Target(out RaycastHit hit)
    {
        bool isHit;
        Vector2 screenPos = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = cam.ScreenPointToRay(screenPos);
        isHit = Physics.Raycast(ray, out hit, Mathf.Infinity);
        if (isHit)
        {
            return hit.transform.root.gameObject;
        }
        else
        {
            return null;
        }
    }
}
