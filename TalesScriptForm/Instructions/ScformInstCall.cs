using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesScriptForm.Instructions;

public class ScformInstCall : ScfomInstructionBase
{
    public override ScfomInstructionType InstructionType => ScfomInstructionType.SCFOM_INST_CALL;

    public byte NumArgs { get; set; }

    public override void ReadData(BinaryStream bs, uint version)
    {
        NumArgs = bs.Read1Byte();
    }
}
