using System;
using strategygame_common;
using System.Collections.Generic;

namespace strategygame_server
{
	public class StringMessageHandler
	{
		public LinkedList<IStringOutput> MessageQueue;

		public StringMessageHandler ()
		{
			MessageQueue = new LinkedList<IStringOutput>();
		}

		public void inputMessage(IStringOutput input)
		{
			MessageQueue.AddAfter (new LinkedListNode<IStringOutput> (input));
		}
			


	}
}

