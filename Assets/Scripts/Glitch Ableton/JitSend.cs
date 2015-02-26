// mu (myu) Max-Unity Interoperability Toolkit
// Ivica Ico Bukvic <ico@vt.edu> <http://ico.bukvic.net>
// Ji-Sun Kim <hideaway@vt.edu>
// Keith Wooldridge <kawoold@vt.edu>
// With thanks to Denis Gracanin

// Virginia Tech Department of Music
// DISIS Interactive Sound & Intermedia Studio
// Collaborative for Creative Technologies in the Arts and Design

// Copyright DISIS 2008.
// mu is distributed under the GPL license (http://www.gnu.org/licenses/gpl.html)

using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class JitSend : MonoBehaviour {

	public int portNo = 32000;
	public string addr = "127.0.0.1";
	public int packetTreshold = 100;

	private TcpClient client;
	private NetworkStream netStream;
	private Queue updateSend;

	private int msgCounter;
	private float connectionAttempt;
	private bool connection;

	public GameObject BodySourceView;
	private BodySourceView _BodyView;
	private Vector3 sourceJoint;
	private float sourcePosition;

	public float grenzwert = 0.3f;
	//main synth note on/off, pitch, volume, bg synth volume, hit sample on/off, globalAcceleration
	private float[] toSend = {0, 0, 0, 0, 0, 0};

	public int minDistance = 15;
	public int maxDistance = 35;


	void Update() {
		if (_BodyView.isBodyTracked()) {
			toSend[0] = 1;

			float handDifference = _BodyView.handDifference();
			handDifference = Math.Abs(handDifference);
			handDifference /= 2;
			handDifference += 73f; //add octaves and stuff

			toSend[1] = (float) Mathf.Round(handDifference);


			float handVolume = _BodyView.GetJoint(0).y * -1 + _BodyView.handHeight();
			if (handVolume > 0) {
				handVolume /= 10;
				handVolume += 0.4f;

				handVolume = Mathf.Clamp (handVolume, 0.4f, 0.8f);
			}
			else {
				handVolume = 0.4f;
			}

			toSend[2] = (float) Math.Round(handVolume, 2);
		}
		else {
			toSend[0] = 0;
			toSend[1] = 0;
			toSend[2] = 0;
		}



		//if no player is in view, set to 0.5
		if (!_BodyView.isBodyTracked()) {
			sourcePosition = 0.5f;
		}
		else {
			//prepare volume of background music
			sourcePosition = _BodyView.SmoothJoint(0).z;
			
			//define boundaries
			if (sourcePosition < minDistance) {
				sourcePosition = 15;
			}
			else if (sourcePosition > maxDistance) {
				sourcePosition = maxDistance;
			}

			//break it down to a range from 0 to 1
			sourcePosition -= minDistance;
			sourcePosition *= (100/(maxDistance - minDistance));
			sourcePosition /= 100;

			//ableton scales the volume exponentially, so we are doing some tricks here
			sourcePosition /= 2;
			sourcePosition = (1 - sourcePosition);

			//at least 0.4
			if (sourcePosition < 0.6) {
				sourcePosition = 0.6f;
			}
		}

		toSend[3] = sourcePosition;


		/*if(_BodyView.detectAcceleration(10)) {
			toSend[4] = 1f;
		}
		else {
			toSend[4] = 0f;
		}*/
		toSend [4] = 0f;

		toSend[5] = (float) Mathf.Round(_BodyView.GetGlobalAcceleration());


		write(toSend);





		if (updateSend.Count != 0) {
			try {
				if (!connection) {
					connectionAttempt += Time.deltaTime;
					if (connectionAttempt > 1.0f) {
						connectionAttempt = 0.0f;
						try {
							client.Connect(addr, portNo);
							connection = true;
						}
						catch(Exception e) {
							connection = false;
						}
					}
					if (connection) {
						netStream = client.GetStream();
					}
					else if (msgCounter > packetTreshold) {
						updateSend.Clear();
						msgCounter = 0;
					}
				}
				if (connection) {
					string toWrite = (string)updateSend.Peek();
					byte[] output;
					output = Encoding.ASCII.GetBytes(toWrite);
					netStream.Write(output, 0, toWrite.Length);

					updateSend.Dequeue();
				}
			}
			//Called when netStream has been closed.
			catch (Exception e) {
				netStream.Close();
				client.Close();
				client = new TcpClient();
				connection = false;
				try {
					client.Connect(addr, portNo);
					connection = true;
				}
				catch(Exception se) {
					connection = false;
				}
				if (connection) netStream = client.GetStream();
			}
		}
	}

	// Use this for initialization
	void Start () {
		_BodyView = BodySourceView.GetComponent<BodySourceView>();

		client = new TcpClient();
		updateSend = new Queue();
		msgCounter = 0;
		connectionAttempt = 0;
		connection = false;
		if (packetTreshold == 0) packetTreshold = 1;
	}

	private void write(float[] var) {
		string toWrite = "";
		for (int i = 0; i < var.Length; i++){
			toWrite += var[i] + " ";
		}
		toWrite += ";\n";

		updateSend.Enqueue(toWrite);
		msgCounter++;
	}
}