using System;

namespace strategygame_common
{
	public interface IStringOutput
	{
		string Message { get; set; }
		OutputType Type { get; set; }
	}
}

