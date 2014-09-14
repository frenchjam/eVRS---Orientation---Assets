// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Wraps the value of Float Variable into a specified range.")]
	public class FloatWrap : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Float variable to wrap.")]
		public FsmFloat floatVariable;

		[RequiredField]
        [Tooltip("The minimum value.")]
		public FsmFloat minValue;

		[RequiredField]
        [Tooltip("The maximum value.")]
		public FsmFloat maxValue;

        [Tooltip("Repeate every frame. Useful if the float variable is changing.")]
		public bool everyFrame;

		private float delta;

		public override void Reset()
		{
			floatVariable = null;
			minValue = null;
			maxValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoWrap();
			
			if (!everyFrame)
			{
			    Finish();
			}
		}

		public override void OnUpdate()
		{
			DoWrap();
		}
		
		void DoWrap()
		{
			delta = maxValue.Value - minValue.Value;
			while (floatVariable.Value > maxValue.Value) floatVariable.Value -= delta;
			while (floatVariable.Value < minValue.Value) floatVariable.Value += delta;

		}
	}
}