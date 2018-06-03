using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPlayerController : MonoBehaviour {

	public float moveSpeed;
	public float rotateSpeed; 	
	public float maxUpDownViewAngle;
	public GameObject eye;

	private Rigidbody mRigidbody;

	void Start () {
		Init ();
	}

	protected void Init() {
		mRigidbody = GetComponent<Rigidbody> ();
	}
		
	void FixedUpdate () {
		// 1. 得到模型坐标系上的位移
		Vector3 movementInput = new Vector3 (
			Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")
		);
		Vector3 movementInModelSpace = movementInput * moveSpeed * Time.deltaTime;
		// 2. 移动模型
		PerformMovementInModelSpace (movementInModelSpace);
		// 3. 从输入中得到视角的旋转角度
		Vector3 rotationInput = new Vector3 (
			Input.GetAxis("CameraRotationY"), Input.GetAxis("CameraRotationX"), 0f
		);
		Vector3 rotation = rotationInput * rotateSpeed;
		// 4. 旋转视角
		PerformRotationofVisualAngle (rotation);
	}

	private void PerformRotationofVisualAngle (Vector3 rotation){
		PerformRotationofVisualAngleInHorizontal (rotation);
		PerformRotationofVisualAngleInVertical (rotation, eye);
	}

	private void PerformRotationofVisualAngleInHorizontal(Vector3 rotation){
		Quaternion horizontalRotation = Quaternion.Euler (0f, rotation.y, 0f);
		mRigidbody.MoveRotation (mRigidbody.rotation * horizontalRotation);
	}

	private void PerformRotationofVisualAngleInVertical(Vector3 rotation, GameObject eye){
		float theUpDownAngle = eye.transform.rotation.eulerAngles.x;
		if (theUpDownAngle - rotation.x < maxUpDownViewAngle 
			|| theUpDownAngle - rotation.x > 360f - maxUpDownViewAngle)
			eye.transform.Rotate(-rotation.x, 0f, 0f);
	}

	private void PerformMovementInModelSpace(Vector3 movement){
		// 1. 将模型坐标系的位移转换为世界坐标系上的位移
		Vector3 movementInWorldSpace = transform.rotation * movement;
		// 2. 移动 Player
		Vector3 position = transform.position + movementInWorldSpace;
		mRigidbody.MovePosition (position);
	}
}
