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

#if !NO_INSTR_INFO
using System;

namespace Iced.Intel.InstructionInfoInternal {
	static class OpAccesses {
		// GENERATOR-BEGIN: OpAccesses
		// ⚠️This was generated by GENERATOR!🦹‍♂️
		public static readonly OpAccess[] Op1 = new OpAccess[7] {
			OpAccess.None,
			OpAccess.CondRead,
			OpAccess.NoMemAccess,
			OpAccess.Read,
			OpAccess.Read,
			OpAccess.ReadWrite,
			OpAccess.Write,
		};
		public static readonly OpAccess[] Op2 = new OpAccess[3] {
			OpAccess.None,
			OpAccess.Read,
			OpAccess.ReadWrite,
		};
		// GENERATOR-END: OpAccesses
	}

	// GENERATOR-BEGIN: InstrInfoConstants
	// ⚠️This was generated by GENERATOR!🦹‍♂️
	static class InstrInfoConstants {
		internal const int OpInfo0_Count = 10;
		internal const int OpInfo1_Count = 7;
		internal const int OpInfo2_Count = 3;
		internal const int OpInfo3_Count = 2;
		internal const int OpInfo4_Count = 2;
		internal const int RflagsInfo_Count = 54;
		internal const int DefaultUsedRegisterCollCapacity = 10;
		internal const int DefaultUsedMemoryCollCapacity = 8;
	}
	// GENERATOR-END: InstrInfoConstants

	// GENERATOR-BEGIN: InfoFlags1
	// ⚠️This was generated by GENERATOR!🦹‍♂️
	[Flags]
	enum InfoFlags1 : uint {
		OpInfo0Shift = 0x00000000,
		OpInfo0Mask = 0x0000000F,
		OpInfo1Shift = 0x00000004,
		OpInfo1Mask = 0x00000007,
		OpInfo2Shift = 0x00000007,
		OpInfo2Mask = 0x00000003,
		OpInfo3Shift = 0x00000009,
		OpInfo3Mask = 0x00000001,
		OpInfo4Shift = 0x0000000A,
		OpInfo4Mask = 0x00000001,
		RflagsInfoShift = 0x0000000E,
		RflagsInfoMask = 0x0000003F,
		CodeInfoShift = 0x00000014,
		CodeInfoMask = 0x0000007F,
		SaveRestore = 0x08000000,
		StackInstruction = 0x10000000,
		ProtectedMode = 0x20000000,
		Privileged = 0x40000000,
		NoSegmentRead = 0x80000000,
	}
	// GENERATOR-END: InfoFlags1

	// GENERATOR-BEGIN: InfoFlags2
	// ⚠️This was generated by GENERATOR!🦹‍♂️
	[Flags]
	enum InfoFlags2 : uint {
		EncodingShift = 0x00000000,
		EncodingMask = 0x00000007,
		AVX2_Check = 0x00040000,
		OpMaskRegReadWrite = 0x00080000,
		FlowControlShift = 0x00000014,
		FlowControlMask = 0x0000000F,
		CpuidFeatureInternalShift = 0x00000018,
		CpuidFeatureInternalMask = 0x000000FF,
	}
	// GENERATOR-END: InfoFlags2

	// GENERATOR-BEGIN: OpInfo0
	// ⚠️This was generated by GENERATOR!🦹‍♂️
	enum OpInfo0 {
		None,
		CondWrite,
		CondWrite32_ReadWrite64,
		NoMemAccess,
		Read,
		ReadCondWrite,
		ReadWrite,
		Write,
		WriteForce,
		WriteMem_ReadWriteReg,
	}
	// GENERATOR-END: OpInfo0

	// GENERATOR-BEGIN: OpInfo1
	// ⚠️This was generated by GENERATOR!🦹‍♂️
	enum OpInfo1 {
		None,
		CondRead,
		NoMemAccess,
		Read,
		ReadP3,
		ReadWrite,
		Write,
	}
	// GENERATOR-END: OpInfo1

	// GENERATOR-BEGIN: OpInfo2
	// ⚠️This was generated by GENERATOR!🦹‍♂️
	enum OpInfo2 {
		None,
		Read,
		ReadWrite,
	}
	// GENERATOR-END: OpInfo2

	// GENERATOR-BEGIN: OpInfo3
	// ⚠️This was generated by GENERATOR!🦹‍♂️
	enum OpInfo3 {
		None,
		Read,
	}
	// GENERATOR-END: OpInfo3

	// GENERATOR-BEGIN: OpInfo4
	// ⚠️This was generated by GENERATOR!🦹‍♂️
	enum OpInfo4 {
		None,
		Read,
	}
	// GENERATOR-END: OpInfo4

	// GENERATOR-BEGIN: CodeInfo
	// ⚠️This was generated by GENERATOR!🦹‍♂️
	enum CodeInfo {
		None,
		Cdq,
		Cdqe,
		Clzero,
		Cmps,
		Cmpxchg,
		Cmpxchg8b,
		Cpuid,
		Cqo,
		Cwd,
		Cwde,
		Div,
		Encls,
		Enter,
		Ins,
		Invlpga,
		Iret,
		Jrcxz,
		KP1,
		Lahf,
		Lds,
		Leave,
		Llwpcb,
		Loadall386,
		Lods,
		Loop,
		Maskmovq,
		Monitor,
		Montmul,
		Movdir64b,
		Movs,
		Mul,
		Mulx,
		Mwait,
		Mwaitx,
		Outs,
		PcmpXstrY,
		Pconfig,
		Pop_2,
		Pop_2_2,
		Pop_4,
		Pop_4_4,
		Pop_8,
		Pop_8_8,
		Pop_Ev,
		Popa,
		Push_2,
		Push_2_2,
		Push_4,
		Push_4_4,
		Push_8,
		Push_8_8,
		Pusha,
		R_AL_W_AH,
		R_AL_W_AX,
		R_CR0,
		R_EAX_ECX_EDX,
		R_EAX_EDX,
		R_ECX_W_EAX_EDX,
		R_ST0,
		R_ST0_R_ST1,
		R_ST0_RW_ST1,
		R_ST0_ST1,
		R_XMM0,
		Read_Reg8_Op0,
		Read_Reg8_Op1,
		Read_Reg8_Op2,
		Read_Reg16_Op0,
		Read_Reg16_Op1,
		Read_Reg16_Op2,
		RW_AL,
		RW_AX,
		RW_CR0,
		RW_ST0,
		RW_ST0_R_ST1,
		Salc,
		Scas,
		Shift_Ib_MASK1FMOD9,
		Shift_Ib_MASK1FMOD11,
		Shift_Ib_MASK1F,
		Shift_Ib_MASK3F,
		Clear_rflags,
		Clear_reg_regmem,
		Clear_reg_reg_regmem,
		Stos,
		Syscall,
		Umonitor,
		Vmfunc,
		Vmload,
		Vzeroall,
		W_EAX_ECX_EDX,
		W_EAX_EDX,
		W_ST0,
		Xbts,
		Xcrypt,
		Xsha,
		Xstore,
	}
	// GENERATOR-END: CodeInfo

	// GENERATOR-BEGIN: RflagsInfo
	// ⚠️This was generated by GENERATOR!🦹‍♂️
	enum RflagsInfo {
		None,
		C_AC,
		C_c,
		C_cos_S_pz_U_a,
		C_d,
		C_i,
		R_a_W_ac_U_opsz,
		R_ac_W_acpsz_U_o,
		R_acopszid,
		R_acopszidAC,
		R_acpsz,
		R_c,
		R_c_W_acopsz,
		R_c_W_c,
		R_c_W_co,
		R_cz,
		R_d,
		R_d_W_acopsz,
		R_o,
		R_o_W_o,
		R_os,
		R_osz,
		R_p,
		R_s,
		R_z,
		S_AC,
		S_c,
		S_d,
		S_i,
		U_acopsz,
		W_acopsz,
		W_acopszid,
		W_acopszidAC,
		W_acpsz,
		W_aopsz,
		W_c,
		W_c_C_aopsz,
		W_c_U_aops,
		W_co,
		W_co_U_apsz,
		W_copsz_U_a,
		W_cosz_C_ap,
		W_cpz_C_aos,
		W_cs_C_oz_U_ap,
		W_csz_C_o_U_ap,
		W_cz_C_aops,
		W_cz_U_aops,
		W_psz_C_co_U_a,
		W_psz_U_aco,
		W_sz_C_co_U_ap,
		W_z,
		W_z_C_acops,
		W_z_C_co_U_aps,
		W_z_U_acops,
	}
	// GENERATOR-END: RflagsInfo
}
#endif
