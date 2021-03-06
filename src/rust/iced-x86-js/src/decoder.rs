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

#[cfg(feature = "instr_info")]
use super::constant_offsets::ConstantOffsets;
use super::instruction::Instruction;
use std::slice;
use wasm_bindgen::prelude::*;

/// Decodes 16/32/64-bit x86 instructions
#[wasm_bindgen]
pub struct Decoder {
	// The decoder has a reference to this vector and the 'static lifetime is really the lifetime of
	// this vector. We can't use another lifetime. This vector and the field are read-only.
	#[allow(dead_code)]
	__data_do_not_use: Vec<u8>,
	decoder: iced_x86::Decoder<'static>,
}

#[wasm_bindgen]
impl Decoder {
	/// Creates a decoder
	///
	/// # Panics
	///
	/// Panics if `bitness` is not one of 16, 32, 64.
	///
	/// # Arguments
	///
	/// * `bitness`: 16, 32 or 64
	/// * `data`: Data to decode
	/// * `options`: Decoder options (a [`DecoderOptions`] flags value), `0` or eg. `DecoderOptions.NoInvalidCheck | DecoderOptions.AmdBranches`
	///
	/// [`DecoderOptions`]: enum.DecoderOptions.html
	///
	/// # Examples
	///
	/// ```
	/// use iced_x86::*;
	///
	/// // xchg [rdx+rsi+16h],ah
	/// // xacquire lock add dword ptr [rax],5Ah
	/// // vmovdqu64 zmm18{k3}{z},zmm11
	/// let bytes = b"\x86\x64\x32\x16\xF0\xF2\x83\x00\x5A\x62\xC1\xFE\xCB\x6F\xD3";
	/// let mut decoder = Decoder::new(64, bytes, DecoderOptions::NONE);
	/// decoder.set_ip(0x1234_5678);
	///
	/// let instr1 = decoder.decode();
	/// assert_eq!(Code::Xchg_rm8_r8, instr1.code());
	/// assert_eq!(Mnemonic::Xchg, instr1.mnemonic());
	/// assert_eq!(4, instr1.len());
	///
	/// let instr2 = decoder.decode();
	/// assert_eq!(Code::Add_rm32_imm8, instr2.code());
	/// assert_eq!(Mnemonic::Add, instr2.mnemonic());
	/// assert_eq!(5, instr2.len());
	///
	/// let instr3 = decoder.decode();
	/// assert_eq!(Code::EVEX_Vmovdqu64_zmm_k1z_zmmm512, instr3.code());
	/// assert_eq!(Mnemonic::Vmovdqu64, instr3.mnemonic());
	/// assert_eq!(6, instr3.len());
	/// ```
	///
	/// It's sometimes useful to decode some invalid instructions, eg. `lock add esi,ecx`.
	/// Pass in [`DecoderOptions.NoInvalidCheck`] to the constructor and the decoder
	/// will decode some invalid encodings.
	///
	/// [`DecoderOptions.NoInvalidCheck`]: enum.DecoderOptions.html#variant.NoInvalidCheck
	///
	/// ```
	/// use iced_x86::*;
	///
	/// // lock add esi,ecx   ; lock not allowed
	/// let bytes = b"\xF0\x01\xCE";
	/// let mut decoder = Decoder::new(64, bytes, DecoderOptions::NONE);
	/// decoder.set_ip(0x1234_5678);
	/// let instr = decoder.decode();
	/// assert_eq!(Code::INVALID, instr.code());
	///
	/// // We want to decode some instructions with invalid encodings
	/// let mut decoder = Decoder::new(64, bytes, DecoderOptions::NO_INVALID_CHECK);
	/// decoder.set_ip(0x1234_5678);
	/// let instr = decoder.decode();
	/// assert_eq!(Code::Add_rm32_r32, instr.code());
	/// assert!(instr.has_lock_prefix());
	/// ```
	#[wasm_bindgen(constructor)]
	pub fn new(bitness: u32, data: Vec<u8>, options: u32 /*flags: DecoderOptions*/) -> Self {
		// Safe, we only read it, we own the data, and store it in the returned value.
		// The decoder also doesn't impl Drop (it can't ref possibly freed data in drop()).
		let decoder_data = unsafe { slice::from_raw_parts(data.as_ptr(), data.len()) };
		let decoder = iced_x86::Decoder::new(bitness, decoder_data, options);
		Decoder { __data_do_not_use: data, decoder }
	}

	/// Gets the low 32 bits of the current `IP`/`EIP`/`RIP` value, see also [`position`].
	/// Enable the `bigint` feature to support `BigInt`.
	///
	/// [`position`]: #method.position
	#[wasm_bindgen(getter)]
	#[cfg(not(feature = "bigint"))]
	pub fn ip_lo(&self) -> u32 {
		self.decoder.ip() as u32
	}

	/// Gets the high 32 bits of the current `IP`/`EIP`/`RIP` value, see also [`position`].
	/// Enable the `bigint` feature to support `BigInt`.
	///
	/// [`position`]: #method.position
	#[wasm_bindgen(getter)]
	#[cfg(not(feature = "bigint"))]
	pub fn ip_hi(&self) -> u32 {
		(self.decoder.ip() >> 32) as u32
	}

	/// Gets the current `IP`/`EIP`/`RIP` value, see also [`position`]
	///
	/// [`position`]: #method.position
	#[wasm_bindgen(getter)]
	#[cfg(feature = "bigint")]
	pub fn ip(&self) -> u64 {
		self.decoder.ip()
	}

	/// Sets the low 32 bits of the current `IP`/`EIP`/`RIP`, see also [`position`].
	/// Enable the `bigint` feature to support `BigInt`.
	///
	/// [`position`]: #method.set_position
	///
	/// # Arguments
	///
	/// * `lo`: Low 32 bits of the new IP
	#[wasm_bindgen(setter)]
	#[cfg(not(feature = "bigint"))]
	pub fn set_ip_lo(&mut self, lo: u32) {
		let ip = (self.decoder.ip() & !0xFFFF_FFFF) | (lo as u64);
		self.decoder.set_ip(ip);
	}

	/// Sets the high 32 bits of the current `IP`/`EIP`/`RIP`, see also [`position`].
	/// Enable the `bigint` feature to support `BigInt`.
	///
	/// [`position`]: #method.set_position
	///
	/// # Arguments
	///
	/// * `hi`: High 32 bits of the new IP
	#[wasm_bindgen(setter)]
	#[cfg(not(feature = "bigint"))]
	pub fn set_ip_hi(&mut self, hi: u32) {
		let ip = ((hi as u64) << 32) | (self.decoder.ip() as u32 as u64);
		self.decoder.set_ip(ip);
	}

	/// Sets the current `IP`/`EIP`/`RIP`, see also [`position`]
	///
	/// [`position`]: #method.set_position
	///
	/// # Arguments
	///
	/// * `new_value`: New IP
	#[wasm_bindgen(setter)]
	#[cfg(feature = "bigint")]
	pub fn set_ip(&mut self, new_value: u64) {
		self.decoder.set_ip(new_value)
	}

	/// Gets the bitness (16, 32 or 64)
	#[wasm_bindgen(getter)]
	pub fn bitness(&self) -> u32 {
		self.decoder.bitness()
	}

	/// Gets the max value that can be passed to [`position`]. This is the size of the data that gets
	/// decoded to instructions and it's the length of the slice that was passed to the constructor.
	///
	/// [`position`]: #method.set_position
	#[wasm_bindgen(getter)]
	#[wasm_bindgen(js_name = "maxPosition")]
	pub fn max_position(&self) -> usize {
		self.decoder.max_position()
	}

	/// Gets the current data position. This value is always <= [`maxPosition`].
	/// When [`position`] == [`maxPosition`], it's not possible to decode more
	/// instructions and [`canDecode`] returns `false`.
	///
	/// [`maxPosition`]: #method.max_position
	/// [`position`]: #method.position
	/// [`canDecode`]: #method.can_decode
	#[wasm_bindgen(getter)]
	pub fn position(&self) -> usize {
		self.decoder.position()
	}

	/// Sets the current data position, which is the index into the data passed to the constructor.
	/// This value is always <= [`maxPosition`]
	///
	/// [`maxPosition`]: #method.max_position
	///
	/// # Panics
	///
	/// Panics if the new position is invalid.
	///
	/// # Arguments
	///
	/// * `new_pos`: New position and must be <= [`maxPosition`]
	///
	/// # Examples
	///
	/// ```
	/// use iced_x86::*;
	///
	/// // nop and pause
	/// let bytes = b"\x90\xF3\x90";
	/// let mut decoder = Decoder::new(64, bytes, DecoderOptions::NONE);
	/// decoder.set_ip(0x1234_5678);
	///
	/// assert_eq!(0, decoder.position());
	/// assert_eq!(3, decoder.max_position());
	/// let instr = decoder.decode();
	/// assert_eq!(1, decoder.position());
	/// assert_eq!(Code::Nopd, instr.code());
	///
	/// let instr = decoder.decode();
	/// assert_eq!(3, decoder.position());
	/// assert_eq!(Code::Pause, instr.code());
	///
	/// // Start all over again
	/// decoder.set_position(0);
	/// assert_eq!(0, decoder.position());
	/// assert_eq!(Code::Nopd, decoder.decode().code());
	/// assert_eq!(Code::Pause, decoder.decode().code());
	/// assert_eq!(3, decoder.position());
	/// ```
	#[wasm_bindgen(setter)]
	pub fn set_position(&mut self, new_pos: usize) {
		self.decoder.set_position(new_pos)
	}

	/// Returns `true` if there's at least one more byte to decode. It doesn't verify that the
	/// next instruction is valid, it only checks if there's at least one more byte to read.
	/// See also [`position`] and [`maxPosition`]
	///
	/// It's not required to call this method. If this method returns `false`, then [`decodeOut()`]
	/// and [`decode()`] will return an instruction whose [`code`] == [`Code.INVALID`].
	///
	/// [`position`]: #method.position
	/// [`maxPosition`]: #method.max_position
	/// [`decodeOut()`]: #method.decode_out
	/// [`decode()`]: #method.decode
	/// [`code`]: struct.Instruction.html#method.code
	/// [`Code.INVALID`]: enum.Code.html#variant.INVALID
	///
	/// # Examples
	///
	/// ```
	/// use iced_x86::*;
	///
	/// // nop and an incomplete instruction
	/// let bytes = b"\x90\xF3\x0F";
	/// let mut decoder = Decoder::new(64, bytes, DecoderOptions::NONE);
	/// decoder.set_ip(0x1234_5678);
	///
	/// // 3 bytes left to read
	/// assert!(decoder.can_decode());
	/// let instr = decoder.decode();
	/// assert_eq!(Code::Nopd, instr.code());
	///
	/// // 2 bytes left to read
	/// assert!(decoder.can_decode());
	/// let instr = decoder.decode();
	/// // Not enough bytes left to decode a full instruction
	/// assert_eq!(Code::INVALID, instr.code());
	///
	/// // 0 bytes left to read
	/// assert!(!decoder.can_decode());
	/// ```
	#[wasm_bindgen(getter)]
	#[wasm_bindgen(js_name = "canDecode")]
	pub fn can_decode(&self) -> bool {
		self.decoder.can_decode()
	}

	/// Decodes all instructions and returns an array of [`Instruction`]s
	///
	/// [`Instruction`]: struct.Instruction.html
	#[wasm_bindgen(js_name = "decodeAll")]
	pub fn decode_all(&mut self) -> js_sys::Array {
		//TODO: https://github.com/rustwasm/wasm-bindgen/issues/111
		self.decoder.iter().map(|i| JsValue::from(Instruction(i))).collect()
	}

	/// Decodes at most `count` instructions and returns an array of [`Instruction`]s.
	/// It returns less than `count` instructions if there's nothing left to decode.
	///
	/// [`Instruction`]: struct.Instruction.html
	///
	/// # Arguments
	///
	/// - `count`: Max number of instructions to decode
	#[wasm_bindgen(js_name = "decodeInstructions")]
	pub fn decode_instructions(&mut self, count: usize) -> js_sys::Array {
		//TODO: https://github.com/rustwasm/wasm-bindgen/issues/111
		self.decoder.iter().take(count).map(|i| JsValue::from(Instruction(i))).collect()
	}

	/// This method can be called after calling [`decode()`] and [`decodeOut()`] to check if the
	/// decoded instruction is invalid because there's no more bytes left or because of bad input data.
	///
	/// [`decode()`]: #method.decode
	/// [`decodeOut()`]: #method.decode_out
	#[wasm_bindgen(getter)]
	#[wasm_bindgen(js_name = "invalidNoMoreBytes")]
	pub fn invalid_no_more_bytes(&self) -> bool {
		self.decoder.invalid_no_more_bytes()
	}

	/// Decodes and returns the next instruction, see also [`decodeOut()`]
	/// which avoids copying the decoded instruction to the caller's return variable.
	/// See also [`invalidNoMoreBytes`].
	///
	/// [`decodeOut()`]: #method.decode_out
	/// [`invalidNoMoreBytes`]: #method.invalid_no_more_bytes
	///
	/// # Examples
	///
	/// ```
	/// use iced_x86::*;
	///
	/// // xrelease lock add [rax],ebx
	/// let bytes = b"\xF0\xF3\x01\x18";
	/// let mut decoder = Decoder::new(64, bytes, DecoderOptions::NONE);
	/// decoder.set_ip(0x1234_5678);
	/// let instr = decoder.decode();
	///
	/// assert_eq!(Code::Add_rm32_r32, instr.code());
	/// assert_eq!(Mnemonic::Add, instr.mnemonic());
	/// assert_eq!(4, instr.len());
	/// assert_eq!(2, instr.op_count());
	///
	/// assert_eq!(OpKind::Memory, instr.op0_kind());
	/// assert_eq!(Register::RAX, instr.memory_base());
	/// assert_eq!(Register::None, instr.memory_index());
	/// assert_eq!(1, instr.memory_index_scale());
	/// assert_eq!(0, instr.memory_displacement());
	/// assert_eq!(Register::DS, instr.memory_segment());
	/// assert_eq!(Register::None, instr.segment_prefix());
	/// assert_eq!(MemorySize::UInt32, instr.memory_size());
	///
	/// assert_eq!(OpKind::Register, instr.op1_kind());
	/// assert_eq!(Register::EBX, instr.op1_register());
	///
	/// assert!(instr.has_lock_prefix());
	/// assert!(instr.has_xrelease_prefix());
	/// ```
	pub fn decode(&mut self) -> Instruction {
		Instruction(self.decoder.decode())
	}

	/// Decodes the next instruction. The difference between this method and [`decode()`] is that this
	/// method doesn't need to copy the result to the caller's return variable (saves 32-bytes of copying).
	/// See also [`invalidNoMoreBytes`].
	///
	/// [`decode()`]: #method.decode
	/// [`invalidNoMoreBytes`]: #method.invalid_no_more_bytes
	///
	/// # Arguments
	///
	/// * `instruction`: Updated with the decoded instruction. All fields are initialized (it's an `out` argument)
	///
	/// # Examples
	///
	/// ```
	/// use iced_x86::*;
	///
	/// // xrelease lock add [rax],ebx
	/// let bytes = b"\xF0\xF3\x01\x18";
	/// let mut decoder = Decoder::new(64, bytes, DecoderOptions::NONE);
	/// decoder.set_ip(0x1234_5678);
	/// // or use core::mem::MaybeUninit:
	/// //    let mut instr = unsafe { core::mem::MaybeUninit::uninit().assume_init() };
	/// // to not clear `instr` more than once (`decode_out()` initializes all its fields).
	/// let mut instr = Instruction::default();
	/// decoder.decode_out(&mut instr);
	///
	/// assert_eq!(Code::Add_rm32_r32, instr.code());
	/// assert_eq!(Mnemonic::Add, instr.mnemonic());
	/// assert_eq!(4, instr.len());
	/// assert_eq!(2, instr.op_count());
	///
	/// assert_eq!(OpKind::Memory, instr.op0_kind());
	/// assert_eq!(Register::RAX, instr.memory_base());
	/// assert_eq!(Register::None, instr.memory_index());
	/// assert_eq!(1, instr.memory_index_scale());
	/// assert_eq!(0, instr.memory_displacement());
	/// assert_eq!(Register::DS, instr.memory_segment());
	/// assert_eq!(Register::None, instr.segment_prefix());
	/// assert_eq!(MemorySize::UInt32, instr.memory_size());
	///
	/// assert_eq!(OpKind::Register, instr.op1_kind());
	/// assert_eq!(Register::EBX, instr.op1_register());
	///
	/// assert!(instr.has_lock_prefix());
	/// assert!(instr.has_xrelease_prefix());
	/// ```
	#[wasm_bindgen(js_name = "decodeOut")]
	pub fn decode_out(&mut self, instruction: &mut Instruction) {
		self.decoder.decode_out(&mut instruction.0);
	}
}

#[wasm_bindgen]
#[cfg(feature = "instr_info")]
impl Decoder {
	/// Gets the offsets of the constants (memory displacement and immediate) in the decoded instruction.
	/// The caller can check if there are any relocations at those addresses.
	///
	/// # Arguments
	///
	/// * `instruction`: The latest instruction that was decoded by this decoder
	///
	/// # Examples
	///
	/// ```
	/// use iced_x86::*;
	///
	/// // nop
	/// // xor dword ptr [rax-5AA5EDCCh],5Ah
	/// //                  00  01  02  03  04  05  06
	/// //                \opc\mrm\displacement___\imm
	/// let bytes = b"\x90\x83\xB3\x34\x12\x5A\xA5\x5A";
	/// let mut decoder = Decoder::new(64, bytes, DecoderOptions::NONE);
	/// decoder.set_ip(0x1234_5678);
	/// assert_eq!(Code::Nopd, decoder.decode().code());
	/// let instr = decoder.decode();
	/// let co = decoder.get_constant_offsets(&instr);
	///
	/// assert!(co.has_displacement());
	/// assert_eq!(2, co.displacement_offset());
	/// assert_eq!(4, co.displacement_size());
	/// assert!(co.has_immediate());
	/// assert_eq!(6, co.immediate_offset());
	/// assert_eq!(1, co.immediate_size());
	/// // It's not an instruction with two immediates (e.g. enter)
	/// assert!(!co.has_immediate2());
	/// assert_eq!(0, co.immediate_offset2());
	/// assert_eq!(0, co.immediate_size2());
	/// ```
	#[wasm_bindgen(js_name = "getConstantOffsets")]
	pub fn get_constant_offsets(&self, instruction: &Instruction) -> ConstantOffsets {
		ConstantOffsets(self.decoder.get_constant_offsets(&instruction.0))
	}
}
