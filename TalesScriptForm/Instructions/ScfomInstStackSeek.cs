using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesScriptForm.Instructions;

public class ScfomInstStackSeek : ScfomInstructionBase
{
    public override ScfomInstructionType InstructionType => ScfomInstructionType.SCFOM_INST_STACK_SEEK;

    public int SeekOffset { get; set; }

    public override void ReadData(BinaryStream bs, uint version)
    {
        switch (Size)
        {
            case 2:
                SeekOffset = bs.ReadSByte();
                break;

            case 3:
                SeekOffset = bs.ReadInt16();
                break;

            case 5:
                SeekOffset = bs.ReadInt32();
                break;
        }
    }
}
