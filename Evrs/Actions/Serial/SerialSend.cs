// Author: Xavier Martinez
// (c) Copyright SpaceApplications Services. All rights reserved.

using UnityEngine;
using SpaceApps.Timeline;
using System.IO.Ports;
using System;
using System.Text;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Devices/Serial")]
    [Tooltip("Send data through serial port")]
    public class SerialSend : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Com port to use.")]
        public FsmString port = new FsmString { Value = "COM5" };

        [RequiredField]
        [Tooltip("Data (number) to send to the port.")]
        public FsmString data = new FsmString { Value = "255" };

        [Tooltip("Toggles linking with other signals.")]
        public FsmBool link = new FsmBool { Value = false };

        public enum TransmissionType { Byte, Decimal, Hex, String, MSI_Commands };

        public TransmissionType MessageType = TransmissionType.String;

        [Tooltip("Repeat every frame")]
        public bool everyFrame = false;

        [NonSerialized]
        private Serial m_Serial = null;

        public override void Reset()
        {
            //Debug.Log("Reset" + this.Name);
            port = new FsmString { Value = "COM5" };
            data = new FsmString { Value = "16" };
            link = new FsmBool { Value = false };
            MessageType = TransmissionType.String;
            everyFrame = false;
            //Create();
            //m_Serial.CurrentTransmissionType = Serial.TransmissionType.
        }

        void Create()
        {
            if (m_Serial != null)
            {
                SerialEnumerator.Destroy(m_Serial);
            }
            
            m_Serial = SerialEnumerator.Create(port.Value);

            m_Serial.PortName = port.Value;
            m_Serial.DataBits = 8;
            m_Serial.BaudRate = 115200;
            m_Serial.Parity = Parity.None;
            m_Serial.StopBits = StopBits.One;

            switch (MessageType)
            {
                case TransmissionType.Byte:
                    m_Serial.SetMessageToByte();
                    break;
                case TransmissionType.Decimal:
                    m_Serial.SetMessageToDecimal();
                    break;
                case TransmissionType.Hex:
                    m_Serial.SetMessageToHex();
                    break;
                case TransmissionType.String:
                case TransmissionType.MSI_Commands:
                    m_Serial.SetMessageToString();
                    break;
            }
            SerialEnumerator.Connect(m_Serial);
        }

        void Destroy()
        {
            SerialEnumerator.Destroy(m_Serial);
            m_Serial = null;
        }

        public override void OnEnter()
        {
            Create();
            SendData();

            if (!everyFrame)
            {
                Finish();
            }
            //Debug.Log("OnEnter Exit");
        }

        public override void OnExit()
        {
            //Debug.Log("OnExit" + this.Name);
            Destroy();
        }

        public override void OnUpdate()
        {
            //Debug.Log("OnUpdate" + this.Name);
            if (everyFrame)
            {
                SendData();
            }
        }

        public void SendData()
        {
            //call to actualy send code. Using native stuff, RTX should be later
            if (MessageType == TransmissionType.MSI_Commands)
                m_Serial.WriteData("ET_REM "+data.Value+"\n");
            else
                m_Serial.WriteData(data.Value);
           // string lastOut = m_Serial.GetLastEntry();
           // Debug.Log("DataSent " + port + " : " + lastOut);
        }
    }
}
