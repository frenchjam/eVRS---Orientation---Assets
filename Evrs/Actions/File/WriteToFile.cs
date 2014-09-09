// Author: Xavier Martinez
// (c) Copyright SpaceApplications Services. All rights reserved.

using UnityEngine;
using System.IO;
using HutongGames.PlayMaker.Actions;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Files")]
    [Tooltip("Writes string to a File")]
    public class WriteToFile : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The file name")]
        public FsmString filePath;

        [RequiredField]
        [Tooltip("The text")]
        public FsmString text;

        [Tooltip("Newline after each insertions ?")]
        public FsmBool newline;
        [Tooltip("Delimiter between insertions")]
        public FsmString delimiter;

        public FsmEvent TokenWriteEvent;
        public FsmEvent failureEvent;

        public bool everyFrame;

        public override void Reset()
        {
            filePath = null;
            text = null;

        }

        public override void OnEnter()
        {

            OnUpdate();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            // test 
            if (Write())
            {
                Fsm.Event(TokenWriteEvent);
            }
            else
            {
                Fsm.Event(failureEvent);
            }
        }

        private bool Write()
        {
            if (string.IsNullOrEmpty(filePath.Value))
            {
                return false;
            }

            // Create an instance of StreamWriter to write text to a file.
            StreamWriter sw = new StreamWriter(filePath.Value, true); // append
            if (newline.Value)
            {
                sw.WriteLine(text.Value + delimiter);
            }
            else
            {
                sw.Write(text.Value + delimiter);
            }
            sw.Close();
            return true;
        }

    }
}

