using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesScriptForm.Instructions;

public class ScformInstJump : ScfomInstructionBase
{
    public override ScfomInstructionType InstructionType => ScfomInstructionType.SCFOM_INST_JUMP;

    public byte Flag { get; set; }
    public int JumpOffset { get; set; }

    public override void ReadData(BinaryStream bs, uint version)
    {
        Flag = bs.Read1Byte();
        JumpOffset = bs.ReadInt32();
    }
}
