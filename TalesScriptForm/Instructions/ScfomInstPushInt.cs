using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesScriptForm.Instructions;

public class ScfomInstPushInt : ScfomInstructionBase
{
    public override ScfomInstructionType InstructionType => ScfomInstructionType.SCFOM_INST_PUSH_INT;

    public long Value { get; set; }

    public override void ReadData(BinaryStream bs, uint version)
    {
        switch (Size)
        {
            case 2:
                Value = bs.ReadSByte();
                break;

            case 3:
                Value = bs.ReadInt16();
                break;

            case 5:
                Value = bs.ReadInt32();
                break;

            case 9:
                Value = bs.ReadInt64();
                break;
        }
    }
}
