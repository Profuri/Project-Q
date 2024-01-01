  using UnityEngine;
using System.Collections;

public class CamHolder : MonoBehaviour {

	public Transform cam_target;
	public float smooth = 0.5f;
	public float rotSpeed = 50f;
	private Vector3 start_angle;

	void Start () 
	{
		start_angle = transform.position; 

	}

	void FixedUpdate () 
	{
		if (cam_target != null) 
		{
			Vector3 desPosition = cam_target.position;
			Vector3 smoothedPosition = Vector3.Lerp(transform.position, desPosition, smooth*Time.deltaTime);
			transform.position = smoothedPosition;	

	
			if (Input.GetKey ("z")) 
			{
				transform.Rotate(new Vector3(0,rotSpeed*Time.deltaTime,0));
			}
			if (Input.GetKey ("c")) 
			{
				transform.Rotate(new Vector3(0,rotSpeed*Time.deltaTime*-1,0));
			}
		}
	}
}
