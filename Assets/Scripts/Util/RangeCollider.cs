using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class RangeCollider : MonoBehaviour
{
    public UnityAction<Collider> onTriggerEnter_Action;
    public UnityAction<Collider> onTriggerStay_Action;
    public UnityAction<Collider> onTriggerExit_Action;

    void Start(){}
    void Update(){}

    private void OnTriggerEnter(Collider collider)
    {
        onTriggerEnter_Action?.Invoke(collider);
    }
    private void OnTriggerStay(Collider collider)
    {
        onTriggerStay_Action?.Invoke(collider);
    }
    private void OnTriggerExit(Collider collider)
    {
        onTriggerExit_Action?.Invoke(collider);
    }
}
