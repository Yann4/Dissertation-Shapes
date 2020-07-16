using UnityEngine;

namespace Dissertation.Narrative.Generator
{
	public struct Token
	{
		[SerializeField] private string _name;
		[SerializeField] private TextAsset _graph;
		[SerializeField] private Parameter[] _parameters;

		public string Name { get { return _name; } }
		public TextAsset Graph { get { return _graph; } }
		public Parameter[] Parameters { get { return _parameters; } }

		public Token(string name, Parameter[] parameters)
		{
			_name = name;
			_graph = null;
			_parameters = parameters;

			Debug.Assert(char.IsUpper(_name[0]), "If a token has a lower case first character it must reference a graph");
		}

		public Token(string name, TextAsset graph, Parameter[] parameters)
		{
			_name = name;
			_graph = graph;
			_parameters = parameters;

			Debug.Assert(!char.IsUpper(_name[0]) && _graph != null, "If a token has a lower case first character it must reference a graph");
		}
	}
}