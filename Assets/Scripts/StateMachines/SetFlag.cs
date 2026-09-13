using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFlag : StateMachineBehaviour
{
    public string parameterName = "canAct";  
    public bool entryValue = false;          
    public bool exitValue = true;            

    public bool useOnStateEnter = true;      
    public bool useOnStateExit = true;       

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (useOnStateEnter)
        {
            animator.SetBool(parameterName, entryValue);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (useOnStateExit)
        {
            animator.SetBool(parameterName, exitValue);
        }
    }
}
