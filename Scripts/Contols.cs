using UnityEngine;
using UnityEngine.InputSystem;

public class Contols : MonoBehaviour
{
    [SerializeField]
    InputAction walk, kill;
    [SerializeField]
    Animator enemyAnimator;

    private void OnEnable()
    {
        walk.Enable();
        kill.Enable();
    }

    private void Update()
    {
        OnWalk();
        if(kill.IsPressed())
        {
            enemyAnimator.enabled = false;
        }
    }

    private void OnWalk()
    {
        if (walk.IsPressed() && enemyAnimator.enabled)
        {
            if (enemyAnimator.GetBool("isWalking"))
            {
                enemyAnimator.SetBool("isWalking", false);
            }
            else
            {
                enemyAnimator.SetBool("isWalking", true);
            }
        }
    }
}
