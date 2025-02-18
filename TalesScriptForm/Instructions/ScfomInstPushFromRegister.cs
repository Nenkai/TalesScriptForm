using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesScriptForm.Instructions;

public class ScfomInstPushFromRegister : ScfomInstructionBase
{
    public override ScfomInstructionType InstructionType => ScfomInstructionType.SCFOM_INST_PUSH_FROM_REGISTER;

    public byte RegisterIndex { get; set; }

    public override void ReadData(BinaryStream bs, uint version)
    {
        if (Size > 1)
            RegisterIndex = bs.Read1Byte();
    }
}
