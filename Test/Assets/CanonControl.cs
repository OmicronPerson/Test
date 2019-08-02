using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller))]
public class CanonControl : MonoBehaviour {

    public GameObject bulletPrefab;
    [Space]

    public LayerMask attackLayer;
    Vector3 mouseWorldPos = Vector3.zero;
    public Camera cam;
    bool canAttack = false;

    [Space][Header("Line settings")]
    public LineRenderer lineTrajectory;
    [Range(2,100)]
    public int trajectoryResolution = 10;

    [Space]
    [Header("Gun settings")]
    public Transform gun;
    Controller controller;

    [Range(0,90)]
    public float maxUpAngle = 80f;

    [Range(0, 180)]
    public float maxLRAngle = 20f;

    public float bulletVelocity = 10f;
    float oldAngle = 0;

    [Space]
    [Header("Cursor settings")]
    public Texture2D activeCursor;
    public Texture2D disableCursor;
    public Vector2 cursorHotspot = Vector2.zero;


    private void Awake()
    {
        controller = GetComponent<Controller>();
    }


    void Update () {

        CalcMousePos();

        CalcAngle();
        
        if (canAttack) RotateGun();
        DrawTrajectory();
        if (canAttack) { if (Input.GetButtonDown("Fire1")) Fire(); }
        
        SetCursor();

    }

    void SetCursor()
    {
        Cursor.SetCursor( (canAttack)? activeCursor: disableCursor, cursorHotspot, CursorMode.Auto);
    }

    void CalcMousePos()
    {
        Ray r = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(r,out hit, 1000, attackLayer))
        {
            mouseWorldPos = hit.point;
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
        
    }


    void DrawTrajectory()
    {
        if (canAttack)
        {
            Vector3[] trajectoryPoints = new Vector3[trajectoryResolution];
            trajectoryPoints[0] = gun.position;

            for (int i = 1; i < trajectoryResolution; i++)
            {
                float t = (float)i/ ((float)trajectoryResolution-1f);

                trajectoryPoints[i] = Vector3.Lerp( trajectoryPoints[0], mouseWorldPos, t);

                t *=Vector3.Distance(new Vector3(gun.position.x,0, gun.position.z) , new Vector3(mouseWorldPos.x, 0, mouseWorldPos.z) );

                trajectoryPoints[i].y = -(t * Mathf.Tan(oldAngle*Mathf.Deg2Rad) - ((t * t * Physics.gravity.y) / (2*bulletVelocity*bulletVelocity*Mathf.Cos(oldAngle * Mathf.Deg2Rad) * Mathf.Cos(oldAngle * Mathf.Deg2Rad))));
                
                trajectoryPoints[i].y += trajectoryPoints[0].y;

            }

            lineTrajectory.enabled = true;
            lineTrajectory.positionCount = trajectoryPoints.Length;
            lineTrajectory.SetPositions(trajectoryPoints);
        }
        else
        {
            lineTrajectory.enabled = false;
        }
    }

    float CalcAngle()
    {

        Vector3 startPoint = gun.position;
        float dist = new Vector3(mouseWorldPos.x - gun.position.x, 0, mouseWorldPos.z - gun.position.z).magnitude;
        float height = -mouseWorldPos.y + gun.position.y;
        float gravity = Physics.gravity.y;

        float angle = bulletVelocity * bulletVelocity * bulletVelocity * bulletVelocity - gravity * (gravity * dist * dist + 2 * height * bulletVelocity * bulletVelocity);

        if (angle < 0) return oldAngle;

        angle= bulletVelocity * bulletVelocity + Mathf.Sqrt(angle);

        angle /= gravity * dist;

        angle = Mathf.Atan(angle);
        angle *= Mathf.Rad2Deg;

        oldAngle = angle;

        return angle;
    }

   
    void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, gun.position, gun.rotation);
        bullet.GetComponent<Rigidbody>().useGravity = true;
        bullet.GetComponent<Rigidbody>().velocity = gun.forward * bulletVelocity;


    }

    void RotateGun()
    {
        Vector3 lookDir = (mouseWorldPos - gun.position).normalized;
        float upAngle = -CalcAngle();
        

        if (0> upAngle|| upAngle> maxUpAngle)
        {
            upAngle = Mathf.Clamp(upAngle, 0, maxUpAngle);
            canAttack = false;
        }
        

        float horizontalAngle = Vector3.SignedAngle(transform.forward, new Vector3(lookDir.x, 0, lookDir.z), Vector3.up);

        if (Mathf.Abs(horizontalAngle) > maxLRAngle)
        {
            controller.RotateHorizontal((Mathf.Abs(horizontalAngle) - maxLRAngle)* Mathf.Sign(horizontalAngle));
        }
        
        gun.forward = new Vector3(lookDir.x,0, lookDir.z) ;
        gun.rotation *= Quaternion.Euler(-upAngle, 0,0);

    }

    private void OnDrawGizmosSelected()
    {
        //mouse
        Gizmos.color = (canAttack) ? Color.green : Color.red;
        Gizmos.DrawSphere(mouseWorldPos, 0.5f);
        

        //angles
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(gun.position, gun.position + transform.TransformVector( Quaternion.Euler(0, maxLRAngle, 0) * Vector3.forward * 4f));
        Gizmos.DrawLine(gun.position, gun.position + transform.TransformVector(Quaternion.Euler(0, -maxLRAngle, 0) * Vector3.forward * 4f));
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(gun.position, gun.position + transform.TransformVector(Quaternion.Euler (-maxUpAngle, 0, 0) *Vector3.forward * 4f));
    }
}
