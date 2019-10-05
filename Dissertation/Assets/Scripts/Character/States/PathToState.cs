using Dissertation.Pathfinding;
using Dissertation.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Character.AI
{
	public class PathToState : State
	{
		public class PathToConfig : StateConfig
		{
			public Vector3 Target;

			public PathToConfig(AgentController owner, Vector3 target)
				: base(States.PathTo, StatePriority.Normal, owner)
			{
				Target = target;
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
					Config.Owner.PushState(new MoveToState.MoveToConfig(_path[_currentPathIndex].Position, Config.Owner, 2.0f));
				}
				else
				{
					Config.Owner.PushState(new TraverseState.TraverseStateConfig(Config.Owner, _path[_currentPathIndex].Platform, 2.0f));
				}

				_currentPathIndex++;
			}
		}


		protected override bool IsValid()
		{
			return _currentPathIndex < _path.Count;
		}
	}
}