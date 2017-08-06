using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float speed = 10f;
    protected Vector3 targetPosition;

	// Update is called once per frame
	void LateUpdate ()
    {
        if (target != null) targetPosition = target.position;
        Vector3 cameraPosition = transform.position;
        cameraPosition = Vector3.Lerp(cameraPosition, targetPosition, Time.deltaTime * speed);
        cameraPosition.z = transform.position.z;
        transform.position = cameraPosition;

    }
}
