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

using System;
using System.IO;
using System.Linq;
using Generator.Constants;
using Generator.Constants.CSharp;
using Generator.Enums;
using Generator.Enums.CSharp;
using Generator.Enums.InstructionInfo;
using Generator.IO;

namespace Generator.InstructionInfo.CSharp {
	[Generator(TargetLanguage.CSharp, GeneratorNames.InstrInfo)]
	sealed class CSharpInstrInfoGenerator : InstrInfoGenerator {
		readonly IdentifierConverter idConverter;
		readonly GeneratorOptions generatorOptions;
		readonly CSharpEnumsGenerator enumGenerator;
		readonly CSharpConstantsGenerator constantsGenerator;

		public CSharpInstrInfoGenerator(GeneratorOptions generatorOptions) {
			idConverter = CSharpIdentifierConverter.Create();
			this.generatorOptions = generatorOptions;
			enumGenerator = new CSharpEnumsGenerator(generatorOptions);
			constantsGenerator = new CSharpConstantsGenerator(generatorOptions);
		}

		protected override void Generate(EnumType enumType) => enumGenerator.Generate(enumType);
		protected override void Generate(ConstantsType constantsType) => constantsGenerator.Generate(constantsType);

		protected override void Generate((InstrInfo info, uint dword1, uint dword2)[] infos) {
			var filename = Path.Combine(CSharpConstants.GetDirectory(generatorOptions, CSharpConstants.InstructionInfoNamespace), "InfoHandlers.g.cs");
			using (var writer = new FileWriter(TargetLanguage.CSharp, FileUtils.OpenWrite(filename))) {
				writer.WriteFileHeader();
				writer.WriteLine($"#if {CSharpConstants.InstructionInfoDefine}");

				writer.WriteLine($"namespace {CSharpConstants.InstructionInfoNamespace} {{");

				writer.Indent();
				writer.WriteLine("static class InfoHandlers {");
				writer.Indent();

				writer.WriteLine($"internal static readonly uint[] Data = new uint[{infos.Length * 2}] {{");
				writer.Indent();
				foreach (var info in infos)
					writer.WriteLine($"0x{info.dword1:X8}, 0x{info.dword2:X8},// {info.info.Code.Name(idConverter)}");
				writer.Unindent();
				writer.WriteLine("};");

				writer.Unindent();
				writer.WriteLine("}");
				writer.Unindent();

				writer.WriteLine("}");
				writer.WriteLine("#endif");
			}
		}

		protected override void Generate(EnumValue[] enumValues, RflagsBits[] read, RflagsBits[] undefined, RflagsBits[] written, RflagsBits[] cleared, RflagsBits[] set, RflagsBits[] modified) {
			var filename = Path.Combine(CSharpConstants.GetDirectory(generatorOptions, CSharpConstants.InstructionInfoNamespace), "RflagsInfoConstants.g.cs");
			using (var writer = new FileWriter(TargetLanguage.CSharp, FileUtils.OpenWrite(filename))) {
				writer.WriteFileHeader();
				writer.WriteLine($"#if {CSharpConstants.InstructionInfoDefine}");

				writer.WriteLine($"namespace {CSharpConstants.InstructionInfoNamespace} {{");

				writer.Indent();
				writer.WriteLine("static class RflagsInfoConstants {");
				writer.Indent();

				var infos = new (RflagsBits[] rflags, string name)[] {
					(read, "read"),
					(undefined, "undefined"),
					(written, "written"),
					(cleared, "cleared"),
					(set, "set"),
					(modified, "modified"),
				};
				foreach (var info in infos) {
					var rflags = info.rflags;
					if (rflags.Length != infos[0].rflags.Length)
						throw new InvalidOperationException();
					var name = idConverter.Field("flags" + info.name.Substring(0, 1).ToUpperInvariant() + info.name.Substring(1));
					writer.WriteLine($"public static readonly ushort[] {name} = new ushort[{rflags.Length}] {{");
					writer.Indent();
					for (int i = 0; i < rflags.Length; i++) {
						var rfl = rflags[i];
						uint value = (uint)rfl;
						if (value > ushort.MaxValue)
							throw new InvalidOperationException();
						writer.WriteLine($"0x{value:X4},// {enumValues[i].Name(idConverter)}");
					}
					writer.Unindent();
					writer.WriteLine("};");
				}

				writer.Unindent();
				writer.WriteLine("}");
				writer.Unindent();

				writer.WriteLine("}");
				writer.WriteLine("#endif");
			}
		}

		protected override void Generate((EnumValue cpuidInternal, EnumValue[] cpuidFeatures)[] cpuidFeatures) {
			var header = new byte[(cpuidFeatures.Length + 7) / 8];
			for (int i = 0; i < cpuidFeatures.Length; i++) {
				int len = cpuidFeatures[i].cpuidFeatures.Length;
				if (len < 1 || len > 2)
					throw new InvalidOperationException();
				header[i / 8] |= (byte)((len - 1) << (i % 8));
			}

			using (var writer = new FileWriter(TargetLanguage.CSharp, FileUtils.OpenWrite(Path.Combine(CSharpConstants.GetDirectory(generatorOptions, CSharpConstants.InstructionInfoNamespace), "CpuidFeatureInternalData.g.cs")))) {
				writer.WriteFileHeader();
				writer.WriteLine($"#if {CSharpConstants.InstructionInfoDefine}");
				writer.WriteLine($"namespace {CSharpConstants.InstructionInfoNamespace} {{");
				writer.Indent();
				writer.WriteLine("static partial class CpuidFeatureInternalData {");
				writer.Indent();
				writer.WriteLine("static byte[] GetGetCpuidFeaturesData() =>");
				writer.Indent();
				writer.WriteLine("new byte[] {");
				writer.Indent();

				writer.WriteCommentLine("Header");
				foreach (var b in header) {
					writer.WriteByte(b);
					writer.WriteLine();
				}
				writer.WriteLine();
				foreach (var info in cpuidFeatures) {
					foreach (var f in info.cpuidFeatures) {
						if ((uint)f.Value > byte.MaxValue)
							throw new InvalidOperationException();
						writer.WriteByte((byte)f.Value);
					}
					writer.WriteCommentLine(string.Join(", ", info.cpuidFeatures.Select(a => a.Name(idConverter)).ToArray()));
				}

				writer.Unindent();
				writer.WriteLine("};");
				writer.Unindent();
				writer.Unindent();
				writer.WriteLine("}");
				writer.Unindent();
				writer.WriteLine("}");
				writer.WriteLine("#endif");
			}
		}

		protected override void GenerateCore() => GenerateOpAccesses();

		void GenerateOpAccesses() {
			var filename = Path.Combine(CSharpConstants.GetDirectory(generatorOptions, CSharpConstants.InstructionInfoNamespace), "InfoHandlerFlags.cs");
			new FileUpdater(TargetLanguage.CSharp, "OpAccesses", filename).Generate(writer => GenerateOpAccesses(writer));
		}

		void GenerateOpAccesses(FileWriter writer) {
			var opInfos = InstrInfoTypes.EnumOpInfos;
			// We assume max op count is 5, update the code if not
			if (opInfos.Length != 5)
				throw new InvalidOperationException();
			// InstructionInfoFactory assumes it's 2
			if (opInfos[3].Values.Length != 2)
				throw new InvalidOperationException();
			// InstructionInfoFactory assumes it's 2
			if (opInfos[4].Values.Length != 2)
				throw new InvalidOperationException();

			var indexes = new int[] { 1, 2 };
			var opAccessTypeStr = OpAccessEnum.Instance.Name(idConverter);
			foreach (var index in indexes) {
				var opInfo = opInfos[index];
				writer.WriteLine($"public static readonly {opAccessTypeStr}[] Op{index} = new {opAccessTypeStr}[{opInfo.Values.Length}] {{");
				writer.Indent();
				foreach (var value in opInfo.Values) {
					var v = value;
					if (v.RawName == nameof(OpInfo.ReadP3))
						v = OpAccessEnum.Instance[nameof(OpAccess.Read)];
					writer.WriteLine($"{opAccessTypeStr}.{v.Name(idConverter)},");
				}
				writer.Unindent();
				writer.WriteLine("};");
			}
		}
	}
}
