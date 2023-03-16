using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMenuController : MonoBehaviour
{

    public BoidLeader PlayerCursor;

    public Rect Bounds;

    public float MoveTimer;
    public float MoveSpeed;

    public Vector3 Target;

    public IEnumerator Move()
    {
        while ((transform.position - Target).magnitude > 0.1f)
        {
            transform.position = transform.position + (Target - transform.position).normalized * MoveSpeed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();

        }
    }

    public IEnumerator TriggerMove()
    {
        while (true)
        {
            yield return new WaitForSeconds(MoveTimer);
            Vector3 v = Vector3.zero;

            v.x = Random.Range(Bounds.xMin, Bounds.xMax);
            v.z = Random.Range(Bounds.yMin, Bounds.yMax);

            Target = v;

            StartCoroutine(Move());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TriggerMove());
    }

}
