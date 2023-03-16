using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidVisualization : MonoBehaviour
{

    private Rigidbody rb;

    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private Vector2 _walkRandomMinMax;
    [SerializeField] private FootstepPlayer footstepPlayer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        if(footstepPlayer)
            StartCoroutine(Walk());
        
        rb.useGravity = true;

 

    }

    // Update is called once per frame
    void Update()
    {

        var velNormalized = rb.velocity.normalized;
        Vector3 targetDirection = new Vector3(velNormalized.x, 0f, velNormalized.z);

        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            transform.rotation = targetRotation;
        }
    }

    IEnumerator Walk()
    {
        while (true)
        {
            if(rb.position.y <= 1.1f)
            {
                var vel = rb.velocity;
                Vector3 targetDirection = new Vector3(vel.x, 0f, vel.z) / 10f;

                if(targetDirection.magnitude > 0.1f)
                    footstepPlayer.Play();

                rb.AddForce(Vector3.up * _walkSpeed * targetDirection.magnitude, ForceMode.Impulse);
               
            }

            yield return new WaitForFixedUpdate();
        }
       


    }

}
