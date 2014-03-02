using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float speed = 10f;
	private Vector3 targetPos;
	Camera c;

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	
	float rotationY = 0F;
	
	// Use this for initialization
	void Start () {

		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;

		targetPos = transform.position;   
		c = transform.FindChild("Camera").gameObject.camera;
		if (!networkView.isMine)
		{
			c.enabled = false;
		}

	}
	
	// Update is called once per frame
	void Update () {
		if(networkView.isMine)
		{
			InputMovement();
		}
	
	}

	void InputMovement()
	{
		if (axes == RotationAxes.MouseXAndY)
		{
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX)
		{
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}
		//========
		
		float turnSpeed = 90;
		float turnAngle = turnSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
		transform.Rotate (0, turnAngle, 0);
		rigidbody.transform.Rotate (0, turnAngle, 0);

		if (Input.GetKey (KeyCode.W))
			rigidbody.MovePosition(rigidbody.position + transform.forward * speed * Time.deltaTime);
			//transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (speed * Time.deltaTime));
		
		if (Input.GetKey(KeyCode.S))
			rigidbody.MovePosition(rigidbody.position - transform.forward * speed * Time.deltaTime);
		
		if (Input.GetKey(KeyCode.D))
			rigidbody.MovePosition(rigidbody.position + transform.right * speed * Time.deltaTime);
		
		if (Input.GetKey(KeyCode.A))
			rigidbody.MovePosition(rigidbody.position - transform.right * speed * Time.deltaTime);

		if (Input.GetKey (KeyCode.Space))
			transform.Translate(Vector3.up * 300 * Time.deltaTime, Space.World);
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		if (stream.isWriting)
		{
			syncPosition = rigidbody.position;
			stream.Serialize(ref syncPosition);
		}
		else
		{
			stream.Serialize(ref syncPosition);
			rigidbody.position = syncPosition;
		}
	}
}
