// Author: Xavier Martinez
// (c) Copyright SpaceApplications Services. All rights reserved.

using UnityEngine;
using SpaceApps.Timeline;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Devices/Parallel")]
    [Tooltip("Send data through Parallel port")]
    public class ParallelSend : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Com port to use.")]
        public FsmString port = new FsmString { Value = "COM5" };

        [RequiredField]
        [Tooltip("Data (number) to send to the port.")]
        public FsmString data = new FsmString { Value = "255" };

        [Tooltip("Toggles linking with other signals.")]
        public FsmBool link = new FsmBool { Value = false };

        [Tooltip("Repeat every frame")]
        public bool everyFrame = false;

        public Serial m_Serial = new Serial();

        public override void Reset()
        {
            // port = new FsmString { Value = "COM5" };
            //data = new FsmString { Value = "16" };
            //link = new FsmBool { Value = false };
            //everyFrame = false;
            //m_Serial = new Serial(); 
            //m_Serial.PortName = port.Value;
            //m_Serial.DataBits = int.Parse(data.Value);
        }

        public override void OnEnter()
        {
            m_Serial = new Serial();
            m_Serial.PortName = port.Value;

            int num = SerialEnumerator.AddListener(m_Serial.PortName);
            if (num == 1) m_Serial.OpenPort();

            Debug.Log(m_Serial.GetLastEntry());
            SendData();
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
                SendData();
            }
        }

        public void SendData()
        {
            //call to actualy send code. Using native stuff, RTX should be later
            m_Serial.WriteData(data.Value);
            string lastOut = m_Serial.GetLastEntry();
            Debug.Log(lastOut);
        }
    }
}
