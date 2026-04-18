using System.Collections.Generic;

namespace Assets.Scripts.Game.Characters.Core.Player.Outputs
{
	public enum AnimBool
	{
		IsGrounded,
		Sprinting,
		Crouching,
		Holstered,
		SecondaryModifierActive,
		Sliding,
	}

	public enum AnimFloat
	{
		Speed,
	}

	public enum AnimInt
	{
		UpperBodyMode,
	}

	public enum AnimTrigger
	{
		Roll,
		LightAttack,
		HeavyAttack,
		Skill1,
		Skill2,
		Skill3,
		ContextInteract,
		ContextGrabOrFire,
		FundamentalBlockPrimary,
		KickOffJump,
	}

	public struct SetBoolCmd
	{
		public AnimBool Param;
		public bool Value;

		public SetBoolCmd(AnimBool param, bool value)
		{
			Param = param;
			Value = value;
		}
	}

	public struct SetFloatCmd
	{
		public AnimFloat Param;
		public float Value;

		public SetFloatCmd(AnimFloat param, float value)
		{
			Param = param;
			Value = value;
		}
	}

	public struct SetIntCmd
	{
		public AnimInt Param;
		public int Value;

		public SetIntCmd(AnimInt param, int value)
		{
			Param = param;
			Value = value;
		}
	}

	public struct TriggerCmd
	{
		public AnimTrigger Param;

		public TriggerCmd(AnimTrigger param)
		{
			Param = param;
		}
	}

	public sealed class AnimationCommands
	{
		private readonly List<SetBoolCmd> _bools = new();
		private readonly List<SetFloatCmd> _floats = new();
		private readonly List<SetIntCmd> _ints = new();
		private readonly List<TriggerCmd> _triggers = new();
		public IReadOnlyList<SetBoolCmd> Bools => _bools;
		public IReadOnlyList<SetFloatCmd> Floats => _floats;
		public IReadOnlyList<SetIntCmd> Ints => _ints;
		public IReadOnlyList<TriggerCmd> Triggers => _triggers;

		public void AddBool(AnimBool param, bool value) => _bools.Add(new SetBoolCmd(param, value));

		public void AddFloat(AnimFloat param, float value) =>
			_floats.Add(new SetFloatCmd(param, value));

		public void AddInt(AnimInt param, int value) => _ints.Add(new SetIntCmd(param, value));

		public void AddTrigger(AnimTrigger param) => _triggers.Add(new TriggerCmd(param));

		public void Clear()
		{
			_bools.Clear();
			_floats.Clear();
			_triggers.Clear();
			_ints.Clear();
		}
	}
}
