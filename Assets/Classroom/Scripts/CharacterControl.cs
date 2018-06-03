using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;
	public GameObject charactor;

    private CharacterController _characterController;
    private Camera _camera;

    void Start ()
    {
		_characterController = GetComponent<CharacterController>();
        _camera = Camera.main;
    }
	
	void Update ()
    {
        Vector3 moveDir = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
        _characterController.SimpleMove(moveDir * _moveSpeed);

        float yRot = Input.GetAxis("CameraHorizontal") * _rotateSpeed;
        float xRot = Input.GetAxis("CameraVertical") * _rotateSpeed;
        transform.Rotate(0, yRot, 0);
		charactor.transform.Rotate(-xRot, 0, 0);
    }
}
