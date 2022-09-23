using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();
    public int nearestDistIndex;
    public bool hasTarget
    {
        get
        {
            if (visibleTargets.Count <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private set { }
    }



    private void Start()
    {
        StartCoroutine("FindTargetswithDelay", .2f);
    }

    IEnumerator FindTargetswithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTarget();
            FindNearestTarget();
        }

    }

    void FindVisibleTarget()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask) && !target.GetComponent<LivingEntity>().dead)
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    void FindNearestTarget()
    {
        if (visibleTargets.Count <= 1)
        {
            nearestDistIndex = 0;
            return;
        }
        float dist = viewRadius;
        for(int i = 0; i < visibleTargets.Count; i++)
        {
            float tmp = Vector3.Distance(transform.position, visibleTargets[i].position);
            if (tmp <= dist)
            {
                dist = tmp;
                nearestDistIndex = i;
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
