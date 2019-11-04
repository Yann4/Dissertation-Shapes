namespace Dissertation.Character
{
	public static class ConversationFunctionLibrary
	{
		public static void RunFunction( ConversationOutput toRun, Data parameters, BaseCharacterController speaker, BaseCharacterController listener )
		{
			switch (toRun)
			{
				case ConversationOutput.None:
					break;
				case ConversationOutput.TransferMoney:
					TransferMoney(speaker, listener, parameters.iVal);
					break;
			}
		}

		private static void TransferMoney(BaseCharacterController speaker, BaseCharacterController listener, int amount)
		{
			if (amount > 0)
			{
				speaker.Inventory.TransferCurrencyTo(listener.Inventory, amount);
			}
			else
			{
				listener.Inventory.TransferCurrencyTo(speaker.Inventory, -amount);
			}
		}
	}
}