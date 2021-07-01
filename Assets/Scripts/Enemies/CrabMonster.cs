using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabMonster : Enemy
{


    protected override IEnumerator Melee()
    {
        //Time last attacked for delay
        timeLastAttacked = Time.time;

        int atkIndex = SelectAttack();
        float wait;

        switch (atkIndex)
        {
            case 0:

                //Set attack animation
                anim.SetInteger("AttackIndex", atkIndex);
                anim.SetTrigger("Attack");

                //Legth of 1 anim frame (24fps 1sec / 24frames) x the number of frames needed to wait
                wait = 0.042f * onOffFrames[atkIndex].x;

                yield return new WaitForSeconds(wait);

                SetMeleeColliders(0, true);

                wait = 0.042f * onOffFrames[atkIndex].y;
                yield return new WaitForSeconds(wait);

                SetMeleeColliders(0, false);

                break;

            case 1:

                //Set attack animation
                anim.SetInteger("AttackIndex", atkIndex);
                anim.SetTrigger("Attack");

                //Legth of 1 anim frame (24fps 1sec / 24frames) x the number of frames needed to wait
                wait = 0.042f * onOffFrames[atkIndex].x;

                yield return new WaitForSeconds(wait);

                SetMeleeColliders(0, true);

                wait = 0.042f * onOffFrames[atkIndex].y;
                yield return new WaitForSeconds(wait);

                SetMeleeColliders(0, false);

                break;

            case 2:

                //Set attack animation
                anim.SetInteger("AttackIndex", atkIndex);
                anim.SetTrigger("Attack");

                //Legth of 1 anim frame (24fps 1sec / 24frames) x the number of frames needed to wait
                wait = 0.042f * onOffFrames[atkIndex].x;

                yield return new WaitForSeconds(wait);

                SetMeleeColliders(1, true);

                wait = 0.042f * onOffFrames[atkIndex].y;
                yield return new WaitForSeconds(wait);

                SetMeleeColliders(1, false);

                break;

            case 3:

                //Set attack animation
                anim.SetInteger("AttackIndex", atkIndex);
                anim.SetTrigger("Attack");

                //Legth of 1 anim frame (24fps 1sec / 24frames) x the number of frames needed to wait
                wait = 0.042f * onOffFrames[atkIndex].x;

                yield return new WaitForSeconds(wait);

                SetMeleeColliders(0, true);
                SetMeleeColliders(1, true);

                wait = 0.042f * onOffFrames[atkIndex].y;
                yield return new WaitForSeconds(wait);

                SetMeleeColliders(0, false);

                break;

            case 4:

                //Set attack animation
                anim.SetInteger("AttackIndex", atkIndex);
                anim.SetTrigger("Attack");

                //Legth of 1 anim frame (24fps 1sec / 24frames) x the number of frames needed to wait
                wait = 0.042f * onOffFrames[atkIndex].x;

                yield return new WaitForSeconds(wait);

                SetMeleeColliders(2, true);

                wait = 0.042f * onOffFrames[atkIndex].y;
                yield return new WaitForSeconds(wait);

                SetMeleeColliders(2, false);

                break;
        }
       
    }

    int SelectAttack()
    {
        int a = Random.Range(0, 4);

        return a;
    }
}
