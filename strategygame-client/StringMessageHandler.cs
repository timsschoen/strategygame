using System;
using strategygame_common;
using System.Collections.Generic;

namespace strategygame_client
{
	public class StringMessageHandler
	{
		private LinkedList<IStringMessageListener> ListenerQueue;

		public StringMessageHandler ()
		{
			ListenerQueue = new LinkedList<IStringMessageListener> ();
		}

		public void inputMessage(IStringOutput input)
		{
			notifyMessage (input);
		}


		public void AddListener(IStringMessageListener listener)
		{
			ListenerQueue.AddAfter (new LinkedListNode<IStringMessageListener> (listener));
		}

		public void RemoveListener(IStringMessageListener listener)
		{
			ListenerQueue.Remove (ListenerQueue.Find (listener));
		}

		private void notifyMessage(IStringOutput message)
		{
			foreach(IStringMessageListener listener in ListenerQueue){
				if (listener.type.Contains(message.Type)) {
					listener.Notify (message);
				}
			}
		}
	}
}