using System;

namespace strategygame_common
{
	public interface IStringMessageListener
	{
		void Notify(IStringOutput e);
		OutputType type {get;}
	}
}

