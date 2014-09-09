using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Text;
using SpaceApps.Timeline;

[System.Serializable]
public class EEGTalker : MonoBehaviour {

	public string port = "COM5";
	private Serial m_Serial = null;
	private Hashtable table = new Hashtable();

	// Use this for initialization
	void Start () {

		Debug.Log ( "EEG Marker Start() called." );

		// Create a table of markers referred to by keywords.
		// Keywords are converted to unique 1-byte codes that 
		//  can be sent to the EEG recorder as event markers.

		// I learned about how to make Enum lists a little too late.
		// Perhaps that would have been a good solution as well.

		table.Clear ();
		table.Add ("FlasherStarted", "1");
		table.Add ("FlasherFinished", "2");

		table.Add ("Distractor-ON", "3");
		table.Add ("Distractor-OFF", "4");
		table.Add ("Reference-ON", "5");
		table.Add ("Reference-OFF", "6");

		table.Add ("Query-Reference", "20");
		table.Add ("Query-Other", "21");
		table.Add ("Query-Surprise", "22");
		table.Add ("ResponsePressed", "29");

		table.Add ("Probe-A-Neutral", "10");
		table.Add ("Probe-A-Same", "11");
		table.Add ("Probe-A-Different", "12");

		table.Add ("Probe-B-Neutral", "13");
		table.Add ("Probe-B-Same", "14");
		table.Add ("Probe-B-Different", "15");

		table.Add ("Head-Rotate-START", "30");
		table.Add ("Head-Rotate-FINISH", "31");

		if (m_Serial != null)
		{
			SerialEnumerator.Destroy(m_Serial);
		}
		
		m_Serial = SerialEnumerator.Create( port );
		 
		m_Serial.PortName = port;
		m_Serial.DataBits = 8;
		m_Serial.BaudRate = 115200;
		m_Serial.Parity = Parity.None;
		m_Serial.StopBits = StopBits.One;
		m_Serial.SetMessageToByte();

	} 
	
	// Update is called once per frame
	void Update () {}

	// Convert a keyword to a byte code and send it out on the serial port.
	// This will be explicitly called by other methods.
	public void SendMarker( string marker_name ) {
		if (table.ContainsKey (marker_name)) {
			string marker = (string)table [marker_name];
			m_Serial.WriteData (marker);
			Debug.Log ( "EEG Marker "+marker_name+" ("+marker+" ) sent." ); 
		} else {
			Debug.LogWarning ( "EEG Marker "+marker_name+" not recognized." );
			m_Serial.WriteData ("255");
		}
	}
}
