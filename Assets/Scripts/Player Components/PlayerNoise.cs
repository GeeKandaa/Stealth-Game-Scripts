using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNoise : MonoBehaviour
{

    private bool showSphere;
    private float startTime;
    public void MakeNoise(float VolumeModifier, Vector3 position)
    {
        Utils_Noise UN = new Utils_Noise();
        //RaycastHit hit;

        int FloorLayer = 1 << LayerMask.NameToLayer("Floor");
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit floor, transform.localScale.y, FloorLayer))
        {
            //debug - for showing footsteps sound range
            showSphere = true;
            debugSphereRadius = UN.FloorNoise[floor.transform.tag] * VolumeModifier;

            LayerMask EnemyLayer = 1 << LayerMask.NameToLayer("Enemy");
            Collider[] enemyHit = Physics.OverlapSphere(transform.position, UN.FloorNoise[floor.transform.tag] * VolumeModifier, EnemyLayer);
            foreach (Collider enemy in enemyHit)
            {
                enemy.GetComponent<PathfinderComponent>().InvestigateSound(transform.position);
            }
        }
    }

    //Debug - Shows footsteps sound range
    private float debugSphereRadius = 0;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (showSphere == true)
        {
            startTime = Time.time;
            showSphere = false;
        }
        else
        {
            if (Time.time - startTime < 2f)
            {
                Gizmos.DrawWireSphere(transform.position, debugSphereRadius);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, 1);
            }
        }
    }
}
