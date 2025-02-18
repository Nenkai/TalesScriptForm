using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesScriptForm.Instructions;

public class ScfomInstPushAssignPopFromRegister : ScfomInstructionBase
{
    public override ScfomInstructionType InstructionType => ScfomInstructionType.SCFOM_INST_ASSIGN_POP_TO_REGISTER;

    public byte RegisterIndex { get; set; }

    public override void ReadData(BinaryStream bs, uint version)
    {
        if (Size > 1)
        {
            byte bits = bs.Read1Byte();
            RegisterIndex = (byte)(bits & 0b11111);
        }
    }
}
