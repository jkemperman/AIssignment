﻿namespace ISGPAI.Game.Entities
{
	/// <summary>
	/// Creeper state when the creeper is nearby the adventurer.
	/// </summary>
	internal class CreeperSeeking : State<Creeper>
	{
		private Entity _target;

		public CreeperSeeking(Creeper agent, Entity target)
			: base(agent)
		{
			this._target = target;
		}

		public override void Enter()
		{
			throw new System.NotImplementedException();
		}

		public override void Update(double elapsed)
		{
			throw new System.NotImplementedException();
		}

		public override void Exit()
		{
			throw new System.NotImplementedException();
		}
	}
}
