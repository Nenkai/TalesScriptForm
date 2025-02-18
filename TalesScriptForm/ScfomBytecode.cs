using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Syroot.BinaryData;
using Syroot.BinaryData.Memory;

using TalesScriptForm.Instructions;

namespace TalesScriptForm;

public class ScfomBytecode
{
    public List<ScfomInstructionBase> Instructions { get; set; } = [];

    public void Read(BinaryStream bs, uint version)
    {
        while (true)
        {
            long instOffset = bs.Position;

            byte bits = bs.Read1Byte();
            ScfomInstructionType instType = (ScfomInstructionType)(bits & 0b1111);
            byte instSize = (byte)(bits >> 4);

            ScfomInstructionBase inst = ScfomInstructionBase.GetInstructionForType(instType);
            inst.Size = instSize;
            inst.InstOffset = (uint)(bs.Position - 1);
            inst.ReadData(bs, version);

            Debug.Assert(bs.Position == instOffset + instSize, "Instruction was not read in full");
            bs.Position = instOffset + instSize;

            Instructions.Add(inst);
            if (inst.InstructionType == ScfomInstructionType.SCFOM_INST_EXIT)
                break;
        }
    }

    // Note: There's always 8 registers. register 0 seems special in some way.
    // Note 2: 8 tables. Refer to constant type in enums.
    // Note 3: Stack size is 0x800. multiply by 8 or 4 depending on pointer size.
    // Note 4: SCFOM push const can have register index 8, which should be invalid. How is it handled?
    // Note 5: Static data declared in initialize { } blocks are simply shoved into the data section and addressed just like a regular C compiler would, with offsets.
    // Note 6: ToV DE executable has a list of syscalls unreferenced no other games seem to do.

    public void Disassemble(ScriptFormBase file, StreamWriter sw)
    {
        int pointerSize = file.Version >= 31000 ? 8 : 4; // Might be inaccurate if some 3.1 versions are 32bit

        if (Instructions[0].InstOffset == file.CodeOffset + file.MainCodeOffset)
            sw.WriteLine("  // void main() {");
        else
            sw.WriteLine($"  // func_{Instructions[0].InstOffset:X}");

        const int CommentPadRight = 60;

        Stack<(int SyscallNum, int PushIntOffset)> syscallGuesses = new();

        for (int i = 0; i < Instructions.Count; i++)
        {
            ScfomInstructionBase inst = Instructions[i];

            string instStr = $"{inst.InstOffset,5:X}|{i,4}|";
            switch (inst.InstructionType)
            {
                case ScfomInstructionType.SCFOM_INST_SYSCALL:
                    {
                        var syscall = inst as ScfomInstSyscall;
                        if (file.Version >= 30000)
                        {
                            // Syscall number is in stack
                            string name;

                            int lastNum = -1337;
                            int sysCallOffset = 0;
                            if (syscallGuesses.Count > 0)
                            {
                                var (SyscallNum, PushIntOffset) = syscallGuesses.Pop();
                                lastNum = SyscallNum;
                                sysCallOffset = PushIntOffset;
                            }

                            if (lastNum != -1337)
                            {
                                int id = lastNum & 0xFFFFFF;
                                if (file.Version == 31600) // Tales of Xillia
                                {
                                    if (Enum.IsDefined((ScfomSyscall_V31600)id))
                                        name = ((ScfomSyscall_V31600)id).ToString();
                                    else
                                        name = $"syscall_{id}";
                                }
                                else if (file.Version == 30100) // Tales of Vesperia, we have all syscalls for it.
                                    name = ((ScfomSyscall_V30100)(id)).ToString();
                                else
                                    name = $"syscall_{id}";

                                instStr += $"SYSCALL: {name}() ({syscall.Flags}, {syscall.NumArgs} arg(s))".PadRight(CommentPadRight);
                                instStr += $"; id: {id}, guessed syscall from {sysCallOffset:X}";
                            }
                            else
                            {
                                instStr += $"SYSCALL: syscall_?() ({syscall.Flags}".PadRight(CommentPadRight);
                                instStr += $"; could not locate syscall from stack, it may be above";
                            }
                        }
                        else
                        {
                            // Syscall number in instruction
                            int num = syscall.SyscallNumber;
                            string name;
                            if (file.Version == 20100) // Tales of Rebirth, we also have all syscalls
                                name = ((ScfomSyscall_V20100)num).ToString();
                            else
                                name = $"syscall_{num}";

                            instStr += $"SYSCALL: {name}() ({syscall.NumArgs} arg(s))".PadRight(CommentPadRight);
                            instStr += $"; id: {syscall.SyscallNumber}";
                        }

                    }
                    break;
                case ScfomInstructionType.SCFOM_INST_CALL:
                    {
                        var call = inst as ScformInstCall;
                        instStr += $"CALL: {call.NumArgs} arg(s)";
                    }
                    break;
                case ScfomInstructionType.SCFOM_INST_JUMP:
                    {
                        var jump = inst as ScformInstJump;
                        instStr += $"JUMP: Jump To {(file.CodeOffset + jump.JumpOffset):X}";
                    }
                    break;
                case ScfomInstructionType.SCFOM_INST_JUMP_IF:
                    {
                        var jumpIf = inst as ScformInstJumpIf;
                        instStr += $"JUMP_IF {(jumpIf.Flag == 1 ? "TRUE" : "FALSE")}: Jump To {(file.CodeOffset + jumpIf.JumpOffset):X}";
                    }
                    break;
                case ScfomInstructionType.SCFOM_INST_EXIT:
                    instStr += $"EXIT";
                    break;
                case ScfomInstructionType.SCFOM_INST_CAST:
                    {
                        var cast = inst as ScformInstCast;
                        instStr += $"CAST: {GetDataTypeString(cast.Type)}";
                    }
                    break;
                case ScfomInstructionType.SCFOM_INST_PUSH_INT:
                    {
                        var pushInt = inst as ScfomInstPushInt;
                        instStr += $"PUSH_INT: {pushInt.Value} (0x{pushInt.Value:X})";

                        if ((pushInt.Value & 0x10000000) != 0 && pushInt.Value >= 0x10000000 && pushInt.Value <= 0x1000FFFF)
                        {
                            syscallGuesses.Push(((int)pushInt.Value, (int)inst.InstOffset));
                        }
                    }
                    break;
                case ScfomInstructionType.SCFOM_INST_PUSH_CONST:
                    {
                        var pushConst = inst as ScfomInstPushFromConst;
                        switch (pushConst.ConstType)
                        {
                            case ScfomConstantType.CONST_CODE:
                                instStr += $"PUSH_CONST: func_{file.CodeOffset + pushConst.Offset:X}".PadRight(CommentPadRight) + $"; Code, RelativeOffset: {pushConst.Offset:X}";
                                break;

                            case ScfomConstantType.CONST_STRINGS:
                                {
                                    SpanReader sr = new SpanReader(file.FileBytes);
                                    sr.Position = (int)(file.StringTableOffset + pushConst.Offset);

                                    sr.Encoding = file.Version >= 31600 ? 
                                        Encoding.UTF8 : // New swapped to UTF8
                                        Encoding.GetEncoding(932); // Old is Shift-JIS

                                    string str = sr.ReadString0();
                                    instStr += $"PUSH_CONST: \"{str}\"".PadRight(CommentPadRight) + $"; StringTable, RelativeOffset: {pushConst.Offset:X}";
                                }
                                break;
                            default:
                                instStr += $"PUSH_CONST: Type {pushConst.ConstType}".PadRight(CommentPadRight) + $"; RelativeOffset: {pushConst.Offset:X}";
                                break;
                        }
                    }
                    break;
                case ScfomInstructionType.SCFOM_INST_PUSH_FROM_REGISTER:
                    {
                        var pushFromReg = inst as ScfomInstPushFromRegister;
                        instStr += $"PUSH_FROM_REGISTER: Reg[{pushFromReg.RegisterIndex}]";
                    }
                    break;
                case ScfomInstructionType.SCFOM_INST_ASSIGN_POP_TO_REGISTER:
                    {
                        var assignPopToRegister = inst as ScfomInstPushAssignPopFromRegister;
                        instStr += $"ASSIGN_POP_TO_REGISTER: Reg[{assignPopToRegister.RegisterIndex}]";
                    }
                    break;
                case ScfomInstructionType.SCFOM_INST_STACK_SEEK:
                    {
                        var stackSeek = inst as ScfomInstStackSeek;
                        if (stackSeek.SeekOffset < 0)
                            instStr += $"STACK PUSH: {-stackSeek.SeekOffset / pointerSize}";
                        else
                            instStr += $"STACK POP: {stackSeek.SeekOffset / pointerSize}";
                    }
                    break;
                case ScfomInstructionType.SCFOM_INST_CALC:
                    {
                        var calc = inst as ScfomInstCalc;
                        instStr += $"OPERATOR: {GetOperatorSymbol(calc.CalcFlag)}".PadRight(CommentPadRight) +
                            $"; {GetDataTypeString(calc.LeftDataType)}<->{GetDataTypeString(calc.RightDataType)}";
                    }
                    break;
                case ScfomInstructionType.SCFOM_INST_CAST2:
                    var inst12 = inst as ScformInstCast2;
                    instStr += $"CAST2: Source={inst12.SourceTypeMaybe}, TargetMaybe={inst12.TargetTypeMaybe}";
                    break;
            }

            sw.WriteLine(instStr);
            sw.Flush();
        }
    }

    private static string GetDataTypeString(ScfomDataType dataType)
    {
        switch (dataType)
        {
            case ScfomDataType.TYPE_SIZET:
                return "size_t";
            case ScfomDataType.TYPE_USIZET:
                return "usize_t";
            case ScfomDataType.TYPE_FSIZET:
                return "fsize_t";

            case ScfomDataType.TYPE_S8:
                return "s8";
            case ScfomDataType.TYPE_U8:
                return "u8";
            case ScfomDataType.TYPE_1_6:
                break;

            case ScfomDataType.TYPE_2_S16:
                return "s16";
            case ScfomDataType.TYPE_2_U16:
                return "u16";
            case ScfomDataType.TYPE_2_10:
                break;

            case ScfomDataType.TYPE_S32:
                return "s32";
            case ScfomDataType.TYPE_U32:
                return "u32";
            case ScfomDataType.TYPE_F32:
                return "f32";

            case ScfomDataType.TYPE_S64:
                return "s64";
            case ScfomDataType.TYPE_U64:
                return "u64";
            case ScfomDataType.TYPE_F64:
                break;

            case ScfomDataType.TYPE_0_20:
                break;
            case ScfomDataType.TYPE_0_21:
                break;
            case ScfomDataType.TYPE_0_22:
                break;
            case ScfomDataType.TYPE_0_23:
                break;
            default:
                break;
        }

        return dataType.ToString();
    }

    private static string GetOperatorSymbol(ScfomCalcOperator op)
    {
        switch (op)
        {
            case ScfomCalcOperator.OP_ADD:
                return "+";
            case ScfomCalcOperator.OP_SUB:
                return "-";
            case ScfomCalcOperator.OP_MUL:
                return "*";
            case ScfomCalcOperator.OP_DIV:
                return "/";
            case ScfomCalcOperator.OP_MOD:
                return "%";
            case ScfomCalcOperator.OP_BITWISE_AND:
                return "&";
            case ScfomCalcOperator.OP_BITWISE_OR:
                return "|";
            case ScfomCalcOperator.OP_BITWISE_XOR:
                return "^";
            case ScfomCalcOperator.OP_LOGICAL_RIGHT_SHIFT:
                return ">>>";
            case ScfomCalcOperator.OP_LOGICAL_LEFT_SHIFT:
                return "<<<";
            case ScfomCalcOperator.OP_ARITHMETIC_RIGHT_SHIFT:
                return ">>";
            case ScfomCalcOperator.OP_ARITHMETIC_LEFT_SHIFT:
                return "<<";
            case ScfomCalcOperator.OP_UNARY_MINUS:
                return "-";
            case ScfomCalcOperator.OP_UNARY_BITWISE_NOT:
                return "~";
            case ScfomCalcOperator.OP_UNARY_LOGICAL_NOT:
                return "!";
            case ScfomCalcOperator.OP_ADD_UNSIGNED:
                return "+";
            case ScfomCalcOperator.OP_BINARY_ASSIGN_PLUS:
                return "+=";
            case ScfomCalcOperator.OP_EQ:
                return "==";
            case ScfomCalcOperator.OP_NEQ:
                return "!=";
            case ScfomCalcOperator.OP_GREATER_EQ_TO:
                return "<=";
            case ScfomCalcOperator.OP_LESSER_EQ_TO:
                return ">=";
            case ScfomCalcOperator.OP_GREATER_THAN:
                return ">";
            case ScfomCalcOperator.OP_LESSER_THAN:
                return "<";
            case ScfomCalcOperator.OP_ASSIGN:
                return "=";
            case ScfomCalcOperator.OP_COPY_ARRAY:
                break;
            case ScfomCalcOperator.OP_ADD_UNK:
                break;
            case ScfomCalcOperator.OP_SUB_UNK:
                break;
            case ScfomCalcOperator.OP_MUL_UNK:
                break;
            case ScfomCalcOperator.OP_DIV_UNK:
                break;
            case ScfomCalcOperator.OP_MOD_UNK:
                break;
            case ScfomCalcOperator.OP_AND_UNK:
                break;
            case ScfomCalcOperator.OP_BITWISE_OR_UNK:
                break;
            case ScfomCalcOperator.OP_BITWISE_XOR_UNK:
                break;
            case ScfomCalcOperator.OP_LOGICAL_RIGHT_SHIFT_UNK:
                break;
            case ScfomCalcOperator.OP_LOGICAL_LEFT_SHIFT_UNK:
                break;
            case ScfomCalcOperator.OP_ARITHMETIC_RIGHT_SHIFT_UNK:
                break;
            case ScfomCalcOperator.OP_ARITHMETIC_LEFT_SHIFT_UNK:
                break;
            case ScfomCalcOperator.OP_COMPARE:
                return "COMPARE";
            case ScfomCalcOperator.OP_38:
                break;
        }

        return $"TODO ({op})";
    }
}