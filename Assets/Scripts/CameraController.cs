using UnityEngine;

public class CameraController : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] float moveSpeed = .1f;
	[SerializeField] float fastSpeedMultiplier = 2f;
	[SerializeField] float moveTime = 10f;
	[SerializeField] float zoomSpeed = 10f;
	[SerializeField] float zoomTime = 10f;
	[SerializeField] float rotateSpeed;
	[SerializeField] float rotateTime;

	float actualMoveSpeed;

	[Header("Constraints")]
	[SerializeField][Range(0f, 90f)] float verticalAngle;
	[SerializeField] float minDistance = 3;
	[SerializeField] float maxDistance = 15;

	float scrollWheel;
	Vector2 axes, mouse;
	float zoom;

	public Vector3 newPosition, newCameraPosition;
	public Quaternion newRotation;

	Transform cam;
	CelestialBody anchorBody;

	private void Awake()
	{
		cam = transform.GetChild(0);
	}

	private void Start()
	{
		newPosition = transform.position;
		newCameraPosition = cam.localPosition;
		newRotation = transform.rotation;
	}

	private void FixedUpdate()
	{
		HandleInput();
		HandleMouseInput();
		MoveCamera();
		ZoomCamera();
		RotateCamera();
	}

	public void SetAnchorBody(CelestialBody body)
	{
		anchorBody = body;
		float bodySize = Length.ConvertToWorld(body.radius);
		minDistance = bodySize + 1 / (Mathf.Sqrt(bodySize) * 20f * (.01f / bodySize));
		maxDistance = bodySize + 1 / (Mathf.Sqrt(bodySize) * 40f * (.01f / bodySize));
	}

	void HandleMouseInput()
	{
		mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		scrollWheel = Input.mouseScrollDelta.y;
		if (Input.GetMouseButton(0))
		{
			newPosition += moveSpeed * transform.TransformVector(new Vector3(-mouse.x, 0, -mouse.y));
		}
		if (Input.GetMouseButton(2))
		{
			newRotation *= Quaternion.Euler(rotateSpeed * Vector3.up * (-mouse.x));
		}
	}

	void HandleInput()
	{
		axes = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		newRotation *= Quaternion.Euler(Input.GetAxis("Rotate") * rotateSpeed * Vector3.up);
		if (Input.GetButton("Sprint"))
		{
			actualMoveSpeed = moveSpeed * fastSpeedMultiplier;
		}
		else
		{
			actualMoveSpeed = moveSpeed;
		}
	}

	void MoveCamera()
	{
		if (anchorBody != null)
		{
			newPosition = anchorBody.transform.position;
			transform.position = newPosition;
		}
		else
		{
			Vector3 move = actualMoveSpeed * new Vector3(axes.x, 0, axes.y);
			newPosition += transform.TransformVector(move);
			transform.position = Vector3.Lerp(transform.position, newPosition, Time.fixedDeltaTime * moveTime);
		}
	}

	void ZoomCamera()
	{
		//Vector3 move = zoomSpeed * -scrollWheel * cam.worldToLocalMatrix.MultiplyVector(transform.forward);
		//newCameraPosition += move;
		//float distance = newCameraPosition.y;
		zoom += -scrollWheel * zoomSpeed;
		zoom = Mathf.Clamp01(zoom);
		float distance = minDistance + (maxDistance-minDistance) * zoom;
		distance = Mathf.Clamp(distance, minDistance, maxDistance);
		newCameraPosition.y = distance * Mathf.Sin(verticalAngle * Mathf.Deg2Rad);
		newCameraPosition.z = -distance * Mathf.Cos(verticalAngle * Mathf.Deg2Rad);
		cam.localPosition = Vector3.Lerp(cam.localPosition, newCameraPosition, Time.fixedDeltaTime * zoomTime);
		cam.localRotation = Quaternion.Euler(verticalAngle, 0, 0);
	}

	void RotateCamera()
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.fixedDeltaTime * rotateTime);
	}



	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position, .25f);
	}
}