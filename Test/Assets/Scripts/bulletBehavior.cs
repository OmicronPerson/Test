using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class bulletBehavior : MonoBehaviour {

    public LayerMask attackMask;
    public float castRadius = 0.5f;
    public GameObject afterLife;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update ()
    {

        CastSphere();

        transform.forward = rb.velocity;

    }

    void CastSphere()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, castRadius, attackMask);

        if (colls.Length>0)
        {
            foreach (var coll in colls)
            {
                //if (coll.GetComponent<Damagable>()) coll.GetComponent<Damagable>().GetDmg(dmg);
            }

            if(afterLife!=null) Instantiate(afterLife, transform.position, transform.rotation);

            Destroy(this.gameObject);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, castRadius);
    }
}
