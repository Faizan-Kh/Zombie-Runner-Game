using UnityEngine;

public class FootstepController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] walkClips;
    [SerializeField] private AudioClip[] runClips;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.3f;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

    private float stepTimer;

    private void Update()
    {
        if (characterController.isGrounded && characterController.velocity.magnitude > 0.1f)
        {
            stepTimer += Time.deltaTime;

            bool isRunning = Input.GetKey(runKey);
            float currentStepInterval = isRunning ? runStepInterval : walkStepInterval;

            if (stepTimer >= currentStepInterval)
            {
                PlayFootstep(isRunning);
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f; // Reset timer if not moving or airborne
        }
    }

    private void PlayFootstep(bool isRunning)
    {
        AudioClip[] currentClips = isRunning ? runClips : walkClips;

        if (currentClips.Length > 0)
        {
            int randomIndex = Random.Range(0, currentClips.Length);
            audioSource.PlayOneShot(currentClips[randomIndex]);
        }
    }
}
