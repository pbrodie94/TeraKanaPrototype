using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Titan : Enemy
{
    [Header("Titan Combat")] 
    [SerializeField] private float lightAttackProbability = 65;
    [SerializeField] private float mediumAttackProbability = 30;
    [SerializeField] private float heavyAttackProbability = 5;

    protected override IEnumerator Melee()
    {
        attacking = true;
        
        int atkIndex = SelectAttack();
        float wait;

        switch (atkIndex)
        {
            case 0:
                //Light attack
                
                //Set animator
                anim.SetInteger(AttackIndex, atkIndex);
                anim.SetTrigger(Attack);
                
                //Set waiting time before turning on the hit boxes
                wait = frameInterval * onOffFrames[atkIndex].x;
                yield return new WaitForSeconds(wait);
                
                //Turn on the melee hit boxes for the attack
                SetMeleeColliders(1, true);
                
                //Wait for the duration of the damage portion of the attack
                wait = frameInterval * onOffFrames[atkIndex].y;
                yield return new WaitForSeconds(wait);
                
                //Turn off the hit boxes
                SetMeleeColliders(1, false);
                
                break;
            
            case 1:
                //Medium attack

            case 2:
                //Heavy attack
                
                //Set animator
                anim.SetInteger(AttackIndex, atkIndex);
                anim.SetTrigger(Attack);
                
                //Set waiting time before turning on the hit boxes
                wait = frameInterval * onOffFrames[atkIndex].x;
                yield return new WaitForSeconds(wait);
                
                //Turn on the melee hit boxes for the attack
                SetMeleeColliders(0, true);
                SetMeleeColliders(1, true);
                
                //Wait for the duration of the damage portion of the attack
                wait = frameInterval * onOffFrames[atkIndex].y;
                yield return new WaitForSeconds(wait);
                
                //Turn off the hit boxes
                SetMeleeColliders(0, false);
                SetMeleeColliders(1, false);

                break;
        }
        
        //Time last attacked for delay
        timeLastAttacked = Time.time;

        attacking = false;
    }

    int SelectAttack()
    {
        float randGen = Random.Range(0, 100);

        if (randGen <= heavyAttackProbability)
        {
            return 2;
        }
        
        if (randGen <= mediumAttackProbability)
        {
            return 1;
        }

        return 0;
    }
}
