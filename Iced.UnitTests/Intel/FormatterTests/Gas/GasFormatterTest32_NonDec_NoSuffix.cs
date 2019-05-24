/*
Copyright (C) 2018-2019 de4dot@gmail.com

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

#if !NO_GAS_FORMATTER && !NO_FORMATTER && !NO_ENCODER
using System.Collections.Generic;
using Iced.Intel;
using Iced.UnitTests.Intel.DecoderTests;
using Xunit;

namespace Iced.UnitTests.Intel.FormatterTests.Gas {
	public sealed class GasFormatterTest32_NonDec_NoSuffix : FormatterTest {
		[Theory]
		[MemberData(nameof(Format_Data))]
		void Format(int index, Instruction info, string formattedString) => FormatBase(index, info, formattedString, GasFormatterFactory.Create_NoSuffix());
		public static IEnumerable<object[]> Format_Data => GetFormatData(NonDecodedInstructions.Infos32, "Gas", nameof(GasFormatterTest32_NonDec_NoSuffix));
	}
}
#endif