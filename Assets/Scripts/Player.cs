using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public Transform head;						//Tank cannon object transform
	public Transform cannon;					//Cannon where to shoot from
	public GameObject projectile;				//Projectile to instantiate while shooting

	//Timers
	public float lockWaypointTime = 4f;			//Time needed aiming a waypoint to move towards it
	public float lockTargetTime = 8f;			//Time needed aiming a target to shoot at it

	private NavMeshAgent _agent;
	private Vector3 _currentDestination;		//Currently moving towards waypoint
	private float _aimTimer = 0f;				//Current timer

	// Use this for initialization
	void Start () {
		_agent = gameObject.GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {

		//Acquire target, set waypoints
		RaycastHit hit;
		Debug.DrawLine(head.position, head.forward*1000f, Color.red, Time.deltaTime,false);
		if (Physics.Raycast (head.position, head.forward, out hit, 1000f)) {

			//Start time tracking objects seen in the scene for specific actions
			if (hit.transform.tag == "Target") {

				//Track target
				_aimTimer+=Time.deltaTime;
				if (_aimTimer >= lockTargetTime) {
					Shoot();
					_aimTimer = 0f;
				}

			} else if (hit.transform.tag == "Waypoint") {

				//Track waypoint
				_aimTimer+=Time.deltaTime;
				if (_aimTimer >= lockWaypointTime) {
					SetDestination (hit.transform.position);
					_aimTimer = 0f;
				}

			} else {

				//Reset Timer
				_aimTimer = 0f;

			}
		}

		Debug.Log (_aimTimer);
	}

	/// <summary>
	/// Set player destination within the NavMesh
	/// </summary>
	/// <param name="pos">Position.</param>
	void SetDestination(Vector3 pos) {
		if (_currentDestination != pos) {
			_currentDestination = pos;
			_agent.SetDestination(_currentDestination);
			Debug.Log ("New waypoint vector: "+pos.ToString());
		}
	}

	/// <summary>
	/// Shoot Bullet
	/// </summary>
	void Shoot() {
		GameObject proj = (GameObject)Instantiate (projectile, cannon.position, cannon.rotation);
		Rigidbody body = proj.GetComponent<Rigidbody> ();
		body.AddForce (cannon.forward * 1000f);
	}
	
}
