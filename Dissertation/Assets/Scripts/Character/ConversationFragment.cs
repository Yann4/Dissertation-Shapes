using System.IO;

namespace Dissertation.Character
{
	public class ConversationFragment
	{
		public bool IsPlayer;
		public string[] ToSay;

		public ConversationFragment[] NextFragments;

		public ConversationFragment(bool isPlayer, int SentenceOptions)
		{
			IsPlayer = isPlayer;

			ToSay = new string[SentenceOptions];
			for (int idx = 0; idx < SentenceOptions; idx++)
			{
				ToSay[idx] = string.Empty;
			}
		}

		public ConversationFragment(BinaryReader reader)
		{
			IsPlayer = reader.ReadBoolean();

			int count = reader.ReadInt32();
			ToSay = new string[count];

			for (int idx = 0; idx < count; idx++)
			{
				ToSay[idx] = reader.ReadString();
			}
		}

		public void Serialise(BinaryWriter writer)
		{
			writer.Write(IsPlayer);
			writer.Write(ToSay.Length);

			foreach (string text in ToSay)
			{
				writer.Write(text);
			}
		}
	}
}