using System;
using System.Collections.Generic;

namespace LuaBinding
{
	public static class LuaParser
	{
		enum TokenType
		{
			Comment,
			Identifier,
			Operator,
			String,
			Number
		};

		struct Token
		{
			//TokenType Type;
			//int Start, End, Line;
			//string Data;
		};

		static List<Token> GetTokens(string lua, bool gmodsyntax = false)
		{
			throw new NotImplementedException();
		}
	}
}

