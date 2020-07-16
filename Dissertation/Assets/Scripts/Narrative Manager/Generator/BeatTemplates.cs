using UnityEngine;

namespace Dissertation.Narrative.Generator
{
	[CreateAssetMenu(fileName = "BeatTemplates.asset", menuName = "Dissertation/Scriptables/Narrative/Beat Templates")]
	public class BeatTemplates : ScriptableObject
	{
		public Token[] Tokens = new Token[0];

		public TextAsset GetGraph(string name)
		{
			foreach(Token token in Tokens)
			{
				if(token.Name == name)
				{
					return token.Graph;
				}
			}

			return null;
		}
	}
}