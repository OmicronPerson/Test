using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour {

    [Range(0,50f)]
    public float speed = 1f;
    [Space]
    [Range(1, 50f)]
    public float levitationHeight = 5f;
    public LayerMask groundLayer;

    Rigidbody rb;
    
    
    void Awake ()
    {
        rb = GetComponent<Rigidbody>();
        

    }


    public void RotateHorizontal(float rotPower)
    {
        transform.Rotate(new Vector3(0, rotPower, 0), Space.Self);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, Vector3.down * levitationHeight + transform.position);
    }

    void FixedUpdate () {

        Vector3 input = Vector3.forward * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal");

        Ray r = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if(Physics.Raycast(r, out hit, 1000, groundLayer))
            input += Vector3.up * (  hit.point.y - transform.position.y + levitationHeight);


        input *= speed;

        rb.AddForce(input - rb.velocity, ForceMode.VelocityChange);
    }
}
