// Author: Xavier Martinez
// (c) Copyright SpaceApplications Services. All rights reserved.

using UnityEngine;
using SpaceApps.Timeline;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Devices/Serial")]
    [Tooltip("Receive data from the serial port")]
    public class SerialReceive : FsmStateAction
    {
        [Tooltip("Com port to use.")]
        public FsmString port = new FsmString { Value = "COM5" };

        [Tooltip("Data received from the port.")]
        public FsmString data;

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        private Serial m_Serial = new Serial();

        public override void Reset()
        {
            //port = new FsmString { Value = "COM5" };
            //data = new FsmString { Value = "" };
            //everyFrame = false;
            //m_Serial = new Serial(); 
            //m_Serial.PortName = port.Value;
        }

        public override void OnEnter()
        {
            if (m_Serial == null)
                m_Serial = new Serial();
            m_Serial.PortName = port.Value;

            int num = SerialEnumerator.AddListener(m_Serial.PortName);
            if (num == 1) m_Serial.OpenPort();

            Debug.Log(m_Serial.GetLastEntry());
            ReceiveData();
            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnExit()
        {
            int num = SerialEnumerator.RemoveListener(m_Serial.PortName);
            if (num < 1) m_Serial.ClosePort();
        }

        public override void OnUpdate()
        {
            if (everyFrame)
            {
                ReceiveData();
                Debug.Log(m_Serial.GetLastEntry());
            }
        }

        public void ReceiveData()
        {
            //call to actualy send code. Using native stuff, RTX should be later
            data.Value = m_Serial.ReadData();
            //string lastOut = m_Serial.GetLastEntry();
            Debug.Log(m_Serial.GetLastEntry());
        }
    }
}
