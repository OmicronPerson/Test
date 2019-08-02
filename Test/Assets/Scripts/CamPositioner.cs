using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPositioner : MonoBehaviour {

    public Transform objToFollow;
    public Vector3 offset;


    void SetPosAndRot()
    {
        if (objToFollow != null)
        {
            transform.position = objToFollow.position + offset;
            transform.rotation = Quaternion.LookRotation(objToFollow.position - transform.position);
        }
    }
  

    void FixedUpdate ()
    {
        SetPosAndRot();

    }

    private void OnDrawGizmosSelected()
    {
         SetPosAndRot();
    }


}
