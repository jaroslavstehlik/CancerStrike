using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;



public class CollisionReciever : MonoBehaviour {

    [System.Serializable]
    public class CollisionEvent : UnityEvent<Collision2D> { }
    public CollisionEvent onCollisionEnter;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (onCollisionEnter != null) onCollisionEnter.Invoke(collision);
    }
}
