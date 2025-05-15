using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStopperTimer : MonoBehaviour
{
    private Animator animator;
    // Default delay in seconds if you want to trigger via the public method without parameters.
    public float defaultPauseDelay = 2.0f;

    // Store the original speed to resume after pausing.
    private float originalSpeed;

    void Start()
    {
        // Get the Animator component attached to the same GameObject.
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            originalSpeed = animator.speed;
        }
        else
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    /// <summary>
    /// Pauses the animation for the specified delay (in seconds) and then resumes it.
    /// </summary>
    /// <param name="delay">Time in seconds to pause the animation.</param>
    public void PauseAnimationForDelay(float delay)
    {
        if (animator != null)
        {
            StartCoroutine(PauseAnimationCoroutine(delay));
        }
    }

    // Optional: This method can be used as an animation event trigger.
    public void TriggerPauseAnimation()
    {
        PauseAnimationForDelay(defaultPauseDelay);
    }

    /// <summary>
    /// Coroutine that pauses the animation by setting the animator's speed to 0,
    /// waits for the specified delay, and then resumes the animation by restoring the original speed.
    /// </summary>
    /// <param name="delay">Delay in seconds.</param>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator PauseAnimationCoroutine(float delay)
    {
        // Pause the animation.
        animator.speed = 0f;
        // Wait for the specified delay.
        yield return new WaitForSeconds(delay);
        // Resume the animation.
        animator.speed = originalSpeed;
    }
}