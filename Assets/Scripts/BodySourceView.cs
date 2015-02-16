using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
	public GameObject BodySourceManager;
	public GUIText GUIRightHand;
	public GUIText GUIDebug;
	public GUIText GUIDebugTwo;
	
	public int accelerationBufferSize;
	public int jointBufferSize;
	
	private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
	private BodySourceManager _BodyManager;
	
	private int jointCount;
	
	private List<Vector3>[] jointStorage = new List<Vector3>[25];
	private List<Vector3>[] localAccelerationCache = new List<Vector3>[25];

	private Vector3[] localAcceleration = new Vector3[25];
	Vector3 globalAcceleration;

	private bool isTracked = false;

	private float nearestBody = 100f;
	private int bodyCountIndex;

	public float zRestriction = 30f;

	void Start ()
	{
		//fill caches
		globalAcceleration.Set(0, 0, 0);
		for (int i = 0; i < localAcceleration.Length; i++) {
			localAcceleration[i] = Vector3.zero;
		}

		//joint position cache
		for (int i = 0; i < 25; i++) {
			jointStorage[i] = new List<Vector3>();
			for (int  u = 0; u < jointBufferSize; u++) {
				jointStorage[i].Add(Vector3.zero);
			}
		}

		//localAcceleration cache
		for (int i = 0; i < 25; i++) {
			localAccelerationCache[i] = new List<Vector3>();
			for (int  u = 0; u < accelerationBufferSize; u++) {
				localAccelerationCache[i].Add(Vector3.zero);
			}
		}
	}
	
	void Update () 
	{
		//reset acceleration
		globalAcceleration.Set(0, 0, 0);
		//reset z of nearest body
		nearestBody = 100f;

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
		}
		
		List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
		
		// First delete untracked bodies
		foreach(ulong trackingId in knownIds)
		{
			if(!trackedIds.Contains(trackingId)) //if no body is tracked
			{
				Destroy(_Bodies[trackingId]);
				_Bodies.Remove(trackingId);

				playerOutOfRange();
			}
		}

		bodyCountIndex = -1;
		for (int i = 0; i < data.Length; i++)
		{
			if (data[i] == null)
			{
				continue;
			}
			
			if(data[i].IsTracked)
			{
				if(!_Bodies.ContainsKey(data[i].TrackingId))
				{
					_Bodies[data[i].TrackingId] = CreateBodyObject(data[i].TrackingId);
				}
				if ((data[i].Joints[Kinect.JointType.SpineBase].Position.Z * 10) < zRestriction) {
					if ((data[i].Joints[Kinect.JointType.SpineBase].Position.Z * 10) < nearestBody) {
						nearestBody = (data[i].Joints[Kinect.JointType.SpineBase].Position.Z * 10);
						bodyCountIndex = i;

						GUIDebug.text = bodyCountIndex.ToString();
						GUIDebugTwo.text = nearestBody.ToString();
					}
				}
				isTracked = true;
			}
		}

		if (bodyCountIndex != -1) {
			RefreshBodyObject(data[bodyCountIndex]);
		}
		else {
			playerOutOfRange();
		}
	}
	
	private GameObject CreateBodyObject(ulong id)
	{
		GameObject body = new GameObject("Body:" + id);
		return body;
	}

	//this is where the party happens
	private void RefreshBodyObject(Kinect.Body body)
	{
		jointCount = 0;
		foreach (Kinect.Joint sourceJoint in body.Joints.Values) {
			Vector3 vectorSourceJoint = GetVector3FromJoint(sourceJoint);




			localAcceleration[jointCount] = vectorSourceJoint - jointStorage[jointCount][0];
			if (!V3Equal(localAcceleration[jointCount], Vector3.zero)) {
				globalAcceleration += V3Abs(localAcceleration[jointCount]);
			}

			if (jointCount == 10) {
				GUIRightHand.text = SmoothAcceleration(false, 10).ToString();
			}

			//joint cache
			jointStorage[jointCount].RemoveAt(0);
			jointStorage[jointCount].Add(vectorSourceJoint);

			//acceleration cache
			localAccelerationCache[jointCount].RemoveAt(0);
			localAccelerationCache[jointCount].Add(localAcceleration[jointCount]);

			jointCount++;
		}
	}

	//stuff to do when the player gets out of range
	private void playerOutOfRange() {
		isTracked = false;
		
		//reset caches when player is out of view
		for (int i = 0; i < 25; i++) {
			for (int  u = 0; u < jointBufferSize; u++) {
				jointStorage[i][u] = Vector3.zero;
			}
		}

		for (int i = 0; i < 25; i++) {
			for (int u = 0; i < accelerationBufferSize; i++) {
				localAccelerationCache[i][u] = Vector3.zero;
			}
		}

		for (int i = 0; i < localAcceleration.Length; i++) {
			localAcceleration[i] = Vector3.zero;
		}
		globalAcceleration = Vector3.zero;
		
		GUIRightHand.text = "joint not tracked";
	}

	//smooth joint position over time
	public Vector3 SmoothJoint (int joint) {
		Vector3 smoothedJoint = jointStorage[joint][jointBufferSize - 1];
		foreach (Vector3 vec in jointStorage[joint]) {
			smoothedJoint += vec;
		}
		
		smoothedJoint = (smoothedJoint/(jointStorage[joint].Count + 1));
		smoothedJoint = new Vector3 ((float) Math.Round (smoothedJoint.x, 2), (float) Math.Round (smoothedJoint.y, 2), (float) Math.Round (smoothedJoint.z, 2));
		
		return smoothedJoint;
	}

	//smooth joint acceleration over time
	public Vector3 SmoothAcceleration (bool absolute, int joint) {
		Vector3 smoothedAcceleration = localAcceleration [joint];
		foreach (Vector3 vec in localAccelerationCache[joint]) {
			smoothedAcceleration += vec;
		}
		
		smoothedAcceleration = (smoothedAcceleration/(localAccelerationCache[joint].Count + 1));
		smoothedAcceleration = new Vector3 ((float) Math.Round (smoothedAcceleration.x, 2), (float) Math.Round (smoothedAcceleration.y, 2), (float) Math.Round (smoothedAcceleration.z, 2));


		if (absolute) {
			return V3Abs(smoothedAcceleration);
		}
		else {
			return smoothedAcceleration;
		}
	}

	//convert kinect vector to unity vector
	private Vector3 GetVector3FromJoint(Kinect.Joint joint)
	{
		return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
	}

	//get the absolute values of a vector
	private Vector3 V3Abs(Vector3 input) {
		input.Set(Mathf.Abs(input[0]), Mathf.Abs(input[1]), Mathf.Abs(input[2]));
		return input;
	}

	//compare vectors
	private bool V3Equal(Vector3 a, Vector3 b){
		return Vector3.SqrMagnitude(a - b) < 0.0001;
	}

	//access acceleration from other scripts
	public Vector3 GetLocalAcceleration(bool absolute, int joint) {
		if (absolute) {
			return (V3Abs(localAcceleration[joint]));
		}
		else {
			return localAcceleration[joint];
		}
	}

	//access joints from other scripts
	public Vector3 GetJoint (int joint) {
		return jointStorage[joint][jointBufferSize - 1];
	}
	
	public bool isBodyTracked () {
		return isTracked;
	}
}