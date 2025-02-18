using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesScriptForm.Instructions;

public class ScformInstCast : ScfomInstructionBase
{
    public override ScfomInstructionType InstructionType => ScfomInstructionType.SCFOM_INST_CAST;

    public ScfomDataType Type { get; set; }

    public override void ReadData(BinaryStream bs, uint version)
    {
        Type = (ScfomDataType)bs.Read1Byte();
    }
}
