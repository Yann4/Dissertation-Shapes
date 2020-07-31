using UnityEngine;

namespace Dissertation.Narrative.Generator
{
	[System.Serializable]
	public struct Token : System.IComparable<Token>
	{
		[SerializeField] private string _name;
		[SerializeField] private TextAsset _graph;
		[SerializeField] private Parameter[] _parameters;

		public string Name { get { return _name; } }
		public TextAsset Graph { get { return _graph; } }
		public Parameter[] Parameters { get { return _parameters; } }
		public bool CanDecompose { get { return Graph == null; } }

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

		public int CompareTo(Token other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return 1;
			}

			int result = Name.CompareTo(other.Name);
			if (result == 0)
			{
				if (Parameters.Length == other.Parameters.Length) //Compare all the params
				{
					for (int idx = 0; idx < Parameters.Length; idx++)
					{
						result = Parameters[idx].CompareTo(other.Parameters[idx]);
						if (result != 0)
						{
							break;
						}
					}

					if (result == 0) //if still equal, compare the graph
					{
						if (!(Graph == null && other.Graph == null))
						{
							if (Graph == null || other.Graph == null)
							{
								result = Graph == null ? -1 : 1;
							}
							else
							{
								result = Graph.name.CompareTo(other.Graph.name);
							}
						}
					}
					else
					{
						result = Parameters.Length > other.Parameters.Length ? 1 : -1;
					}
				}
			}

			return result;
		}
	}
}