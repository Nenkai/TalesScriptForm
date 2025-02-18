using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TalesScriptForm.Instructions;

namespace TalesScriptForm;

public abstract class ScfomInstructionBase
{
    public uint InstOffset { get; set; }

    public byte Size { get; set; }
    public virtual ScfomInstructionType InstructionType { get; set; }

    public void ReadOpcode(BinaryStream bs)
    {
        byte bits = bs.Read1Byte();
        Size = (byte)(bits & 0b1111);
        InstructionType = (ScfomInstructionType)(bits >> 4);
    }

    public static ScfomInstructionBase GetInstructionForType(ScfomInstructionType type)
    {
        return type switch
        {
            ScfomInstructionType.SCFOM_INST_SYSCALL => new ScfomInstSyscall(),
            ScfomInstructionType.SCFOM_INST_CALL => new ScformInstCall(),
            ScfomInstructionType.SCFOM_INST_JUMP => new ScformInstJump(),
            ScfomInstructionType.SCFOM_INST_JUMP_IF => new ScformInstJumpIf(),
            ScfomInstructionType.SCFOM_INST_EXIT => new ScformInstExit(),
            ScfomInstructionType.SCFOM_INST_CAST => new ScformInstCast(),
            ScfomInstructionType.SCFOM_INST_PUSH_INT => new ScfomInstPushInt(),
            ScfomInstructionType.SCFOM_INST_PUSH_CONST => new ScfomInstPushFromConst(),
            ScfomInstructionType.SCFOM_INST_PUSH_FROM_REGISTER => new ScfomInstPushFromRegister(),
            ScfomInstructionType.SCFOM_INST_ASSIGN_POP_TO_REGISTER => new ScfomInstPushAssignPopFromRegister(),
            ScfomInstructionType.SCFOM_INST_STACK_SEEK => new ScfomInstStackSeek(),
            ScfomInstructionType.SCFOM_INST_CALC => new ScfomInstCalc(),
            ScfomInstructionType.SCFOM_INST_CAST2 => new ScformInstCast2(),
            _ => throw new NotImplementedException($"Instruction type {type} not implemented."),
        };
    }

    public abstract void ReadData(BinaryStream bs, uint version);
}
