using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutAndRemoveBehavior : StateMachineBehaviour
{
    public float fadeTime = 0.5f;
    private float timePassed = 0f;
    SpriteRenderer spriteRenderer;
    Color fadeColor;
    GameObject objectToFade;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timePassed = 0f;
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        fadeColor = spriteRenderer.color;
        objectToFade = animator.gameObject;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timePassed += Time.deltaTime;

        float fadeAlpha = fadeColor.a * 1 - (timePassed / fadeTime);

        spriteRenderer.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, fadeAlpha);

        if (timePassed > fadeTime)
        {
            Destroy(objectToFade);
        }
    }
}
