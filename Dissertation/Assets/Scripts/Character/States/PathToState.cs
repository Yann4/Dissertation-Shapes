using Dissertation.Pathfinding;
using Dissertation.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Character.AI
{
	public class PathToState : State
	{
		public class PathToConfig : StateConfig
		{
			public Vector3 Target;
			public Func<bool> IsTargetValid;

			public PathToConfig(AgentController owner, Vector3 target, Func<bool> isTargetValid = null)
				: base(States.PathTo, StatePriority.Normal, owner)
			{
				Target = target;

				if (isTargetValid == null)
				{
					IsTargetValid = () => true;
				}
				else
				{
					IsTargetValid = isTargetValid;
				}
			}
		}

		private PathToConfig _config;
		private List<Node> _path;
		private int _currentPathIndex = 0;
		private Node _targetNode = null;

		public PathToState(PathToConfig config) : base(config)
		{
			_config = config;
			_path = Pathfinder.GetPath(Config.Owner.transform.position, _config.Target);
			_targetNode = Pathfinder.NearestNode(Config.Owner.transform.position);
		}

		public override void OnEnable()
		{
			base.OnEnable();

			if (_currentPathIndex < _path.Count)
			{
				Node nearest = Pathfinder.NearestNode(Config.Owner.transform.position);
				//This means that since the last time we've run the path has become invalid & we should recalculate it
				if ( (_currentPathIndex == 0 && nearest != _path[0]) || (_currentPathIndex > 0 && nearest != _path[_currentPathIndex - 1]) )
				{
					_path = Pathfinder.GetPath(Config.Owner.transform.position, _config.Target);
					_currentPathIndex = 0;
				}

				if (_path[_currentPathIndex].Platform == Positional.GetCurrentPlatform(Config.Owner.transform))
				{
					Config.Owner.PushState(new MoveToState.MoveToConfig(_path[_currentPathIndex].Position, Config.Owner));
				}
				else
				{
					Config.Owner.PushState(new TraverseState.TraverseStateConfig(Config.Owner, _path[_currentPathIndex].Platform));
				}

				_currentPathIndex++;
			}
		}

		protected override bool IsValid()
		{
			return _currentPathIndex < _path.Count && _config.IsTargetValid();
		}
	}
}