using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using UnityEngine;

namespace Assets.Scripts.Game.Characters.Unity.Player.Anim
{
	public sealed class UnityAnimatorAdapter
	{
		private readonly Animator _anim;

		// Bools
		private readonly int _isGrounded = Animator.StringToHash("IsGrounded");

		private readonly int _sprinting = Animator.StringToHash("Sprinting");
		private readonly int _crouching = Animator.StringToHash("Crouching");
		private readonly int _holstered = Animator.StringToHash("Holstered");
		private readonly int _sliding = Animator.StringToHash("Sliding");

		// Floats
		private readonly int _speed = Animator.StringToHash("Speed");

		// Triggers
		private readonly int _roll = Animator.StringToHash("Roll");

		private readonly int _kickOffJump = Animator.StringToHash("KickOffJump");

		// Ints
		private readonly int _upperBodyMode = Animator.StringToHash("UpperBodyMode");

		public UnityAnimatorAdapter(Animator anim) => _anim = anim;

		public void Apply(AnimationCommands cmds)
		{
			foreach (var b in cmds.Bools)
			{
				switch (b.Param)
				{
					case AnimBool.IsGrounded:
						_anim.SetBool(_isGrounded, b.Value);
						break;

					case AnimBool.Sprinting:
						_anim.SetBool(_sprinting, b.Value);
						break;

					case AnimBool.Crouching:
						_anim.SetBool(_crouching, b.Value);
						break;

					case AnimBool.Holstered:
						_anim.SetBool(_holstered, b.Value);
						break;

					case AnimBool.Sliding:
						_anim.SetBool(_sliding, b.Value);
						break;
				}
			}

			foreach (var f in cmds.Floats)
			{
				if (f.Param == AnimFloat.Speed)
					_anim.SetFloat(_speed, f.Value);
			}

			foreach (var i in cmds.Ints)
			{
				if (i.Param == AnimInt.UpperBodyMode)
					_anim.SetInteger(_upperBodyMode, i.Value);
			}

			foreach (var t in cmds.Triggers)
			{
				switch (t.Param)
				{
					case AnimTrigger.Roll:
						_anim.SetTrigger(_roll);
						break;

					case AnimTrigger.KickOffJump:
						_anim.SetTrigger(_kickOffJump);
						break;
				}
			}
		}
	}
}
