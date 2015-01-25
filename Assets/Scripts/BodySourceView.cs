using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
	public GameObject BodySourceManager;
	public GUIText GUIRightHand;
	public GUIText GUIDebug;
	public GUIText GUIDebugTwo;
	public int frameBufferCount;
	
	private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
	private BodySourceManager _BodyManager;

	Queue<Vector3>[] jointCacheQueues;
	private int jointCount;

	private Vector3[] jointStorage = new Vector3[25];
	private Vector3[] localAcceleration = new Vector3[25];
	public Vector3 globalAcceleration;

	private bool isTracked = false;

	void Start ()
	{
		//joint position cache
		jointCacheQueues = new Queue<Vector3>[25];
		for (int i = 0; i < 25; i++) {
			jointCacheQueues[i] = new Queue<Vector3>();

			for (int  u = 0; u < frameBufferCount; u++) {
				jointCacheQueues[i].Enqueue(Vector3.zero);
			}
		}

		//create acceleration caches
		globalAcceleration.Set(0, 0, 0);
		for (int i = 0; i < localAcceleration.Length; i++) {
			localAcceleration[i] = Vector3.zero;
		}
	}
	
	void Update () 
	{
		//reset acceleration
		globalAcceleration.Set(0, 0, 0);

		if (BodySourceManager == null)
		{
			return;
		}


		_BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
		if (_BodyManager == null)
		{
			return;
		}
		
		Kinect.Body[] data = _BodyManager.GetData();
		if (data == null)
		{
			return;
		}
		
		List<ulong> trackedIds = new List<ulong>();
		int bodyCount = 0;
		foreach(var body in data)
		{
			if (body == null)
			{
				continue;
			}
			
			if(body.IsTracked)
			{
				bodyCount++;
				trackedIds.Add (body.TrackingId);
			}
			//Debug.Log(bodyCount.ToString());
		}
		
		List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
		
		// First delete untracked bodies
		foreach(ulong trackingId in knownIds)
		{
			if(!trackedIds.Contains(trackingId)) //if no body is tracked
			{
				Destroy(_Bodies[trackingId]);
				_Bodies.Remove(trackingId);

				isTracked = false;

				//reset caches when player is out of view
				for (int i = 0; i < 25; i++) {
					for (int  u = 0; u < frameBufferCount; u++) {
						jointCacheQueues[i].Dequeue();
						jointCacheQueues[i].Enqueue(Vector3.zero);
					}
				}
				for (int i = 0; i < localAcceleration.Length; i++) {
					localAcceleration[i] = Vector3.zero;
				}
				globalAcceleration = Vector3.zero;

				GUIRightHand.text = "joint not tracked";
				GUIDebug.text = globalAcceleration.ToString();
			}
		}
		
		foreach(var body in data)
		{
			if (body == null)
			{
				continue;
			}
			
			if(body.IsTracked)
			{
				if(!_Bodies.ContainsKey(body.TrackingId))
				{
					_Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
				}

				isTracked = true;
				RefreshBodyObject(body, _Bodies[body.TrackingId]);
			}
		}
	}
	
	private GameObject CreateBodyObject(ulong id)
	{
		GameObject body = new GameObject("Body:" + id);
		return body;
	}
	
	private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
	{
		jointCount = 0;
		foreach (Kinect.Joint sourceJoint in body.Joints.Values) {
			Vector3 vectorSourceJoint = GetVector3FromJoint(sourceJoint);

			jointStorage[jointCount] = vectorSourceJoint;


			localAcceleration[jointCount] = vectorSourceJoint - jointCacheQueues[jointCount].Peek();
			if (!V3Equal(localAcceleration[jointCount], Vector3.zero)) {
				globalAcceleration += V3Abs(localAcceleration[jointCount]);
			}

			if (jointCount == 10) {
				GUIRightHand.text = localAcceleration[jointCount].ToString();
			}

			//GUIDebugTwo.text = SmoothAcceleration(10).ToString();

			jointCacheQueues[jointCount].Dequeue();
			jointCacheQueues[jointCount].Enqueue(vectorSourceJoint);

			jointCount++;
		}

		GUIDebug.text = globalAcceleration.ToString();
	}

	private Vector3 SmoothJoint (int joint) {
		Vector3 smoothedAcceleration = localAcceleration [joint];
		foreach (Vector3 vec in localAccelerationCache[joint]) {
			smoothedAcceleration += vec;
		}
		
		smoothedAcceleration = (smoothedAcceleration/(localAccelerationCache[joint].Count + 1));
		
		return smoothedAcceleration;
	}

	private Vector3 GetVector3FromJoint(Kinect.Joint joint)
	{
		return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
	}

	private Vector3 V3Abs(Vector3 input) {
		input.Set(Mathf.Abs(input[0]), Mathf.Abs(input[1]), Mathf.Abs(input[2]));
		return input;
	}

	private bool V3Equal(Vector3 a, Vector3 b){
		return Vector3.SqrMagnitude(a - b) < 0.0001;
	}

	public Vector3 GetLocalAcceleration(bool absolute, int joint) {
		if (absolute) {
			return (V3Abs(localAcceleration[joint]));
		}
		else {
			return localAcceleration[joint];
		}
	}

	public Vector3 GetJoint (int joint) {
		return jointStorage[joint];
	}

	public bool isBodyTracked () {
		return isTracked;
	}
}

//-----UNUSED-----//
/*
private List<Vector3>[] localAccelerationCache = new List<Vector3>[25];

//localAcceleration cache
	for (int i = 0; i < 25; i++) {
		localAccelerationCache[i] = new List<Vector3>();

		for (int  u = 0; u < frameBufferCount; u++) {
			localAccelerationCache[i].Add(Vector3.zero);
		}
	}

	localAccelerationCache[jointCount].RemoveAt(0);
	localAccelerationCache[jointCount].Add(localAcceleration[jointCount]);

private Vector3 SmoothAcceleration (int joint) {
	Vector3 smoothedAcceleration = localAcceleration [joint];
	foreach (Vector3 vec in localAccelerationCache[joint]) {
		smoothedAcceleration += vec;
	}
	
	smoothedAcceleration = (smoothedAcceleration/(localAccelerationCache[joint].Count + 1));
	
	return smoothedAcceleration;
}
*/