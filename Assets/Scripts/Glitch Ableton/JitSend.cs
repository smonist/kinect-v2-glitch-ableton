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


	public GUIText GUIDebugTwo;

	public GameObject BodySourceView;
	private BodySourceView _BodyView;
	private Vector3 sourceJoint;
	private float sourcePosition;

	public float grenzwert = 0.3f;
	private float[] toSend = {0, 0, 0};


	void Update() {
		sourceJoint = _BodyView.GetLocalAcceleration(true, 10);

		if (sourceJoint.x > grenzwert | sourceJoint.y > grenzwert | sourceJoint.z > grenzwert) {
			toSend[0] = 1;
			toSend[1] = sourceJoint.x + sourceJoint.y + sourceJoint.z;
		}
		else {
			toSend[0] = 0;
			toSend[1] = 0;
		}


		//prepare volume of background music
		sourcePosition = _BodyView.GetJoint(0).z;

		//define boundaries
		if (sourcePosition < 15 && sourcePosition > 1) {
			sourcePosition = 15;
		}
		else if (sourcePosition > 35) {
			sourcePosition = 35;
		}

		//if no player is in view, set to 0.4
		if (sourcePosition == 0) {
			sourcePosition = 0.4f;
		}
		else {
			//break it down to a range from 0 to 1
			sourcePosition -= 15;
			sourcePosition *= 5;
			sourcePosition /= 100;

			//ableton scales the volume exponentially, so we are doing some tricks here
			sourcePosition /= 2;
			sourcePosition = (1 - sourcePosition);

			//at least 0.4
			if (sourcePosition < 0.4) {
				sourcePosition = 0.4f;
			}
		}

		toSend[2] = sourcePosition;


		//round them all!
		toSend[1] = (float) Math.Round(toSend[1], 2);
		toSend[2] = (float) Math.Round(toSend[2], 2);

		write(toSend);
		GUIDebugTwo.text = toSend[2].ToString("F2");






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