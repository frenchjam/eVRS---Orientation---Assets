// Author: Xavier Martinez
// (c) Copyright SpaceApplications Services. All rights reserved.

using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Files")]
    [Tooltip("Reads string from a File")]
    public class ReadFromFile : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The file name")]
        public FsmString filePath;

        [RequiredField]
        [Tooltip("The text")]
        public FsmString text;

        private Queue<string> queueWords = new Queue<string>();

        [Tooltip("Delimiter between insertions")]
        public FsmString delimiter;
		private FsmString delimiterInternal = new FsmString();

		[Tooltip("Number of header lines to skip on open.")]
		public int headerLines;

        public FsmEvent TokenReadEvent;
        public FsmEvent EndOfFileEvent;

        public bool everyFrame;
        bool fileHasBeenRead = false;

		private int hdr;


        public override void Reset()
        {
            filePath = null;
            text = null;
            queueWords.Clear();
        }

        public override void OnEnter()
        {
			if (fileHasBeenRead == false)
            {
                if (Read())
                {
					fileHasBeenRead = true;
					// Skip over the header lines.
					for ( hdr = 0; hdr < headerLines; hdr++ ) {
						if ( !Parse() ) {
							Fsm.Event(EndOfFileEvent);
							Finish();
						}
					}
				}
                else
                {
                    Fsm.Event(EndOfFileEvent);
                }
            }

            OnUpdate();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            if (Parse())
            {
                Fsm.Event(TokenReadEvent);
            }
            else
            {
                Fsm.Event(EndOfFileEvent);
                Finish();
            }
        }

		// Read in all of the lines from the file and place in queue.
        private bool Read()
        {
            if (string.IsNullOrEmpty(filePath.Value))
            {
                return false;
            }

			// Allow user to specify \n in the delimiter field.
			// Here they get translated into newlines.
			// This won't work if the user has specified more than one delimiter.

			// THIS ISN'T WORKING, but it shows the idea.
			// To get the behavior, one has to press alt return in the text field.
			if (delimiter.Value.Equals ("\\n")) {
				delimiterInternal.Value = "\n";
			} else {
				delimiterInternal.Value = delimiter.Value;
			}

            StreamReader sr = new StreamReader(filePath.Value);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] lineWords = line.Split(delimiterInternal.Value.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
				foreach (string word in lineWords)
                {
                    queueWords.Enqueue(word);
                }
            }
            text.Value = queueWords.Peek();
            return true;
        }

		// Retrieve the next line out of the queue, if any.
        private bool Parse()
        {
            if (queueWords.Count == 0)
            {
                return false;
            }
            else
                text.Value = queueWords.Dequeue();
            return true;
        }

    }
}

