using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesScriptForm.Instructions;

public class ScformInstCast2 : ScfomInstructionBase
{
    public override ScfomInstructionType InstructionType => ScfomInstructionType.SCFOM_INST_CAST2;

    public ScfomDataType TargetTypeMaybe { get; set; }
    public ScfomDataType SourceTypeMaybe { get; set; }

    public override void ReadData(BinaryStream bs, uint version)
    {
        if (version >= 30000)
        {
            byte bits2 = bs.Read1Byte();
            TargetTypeMaybe = (ScfomDataType)(bits2 & 0b1_1111); // 5 bits

            byte bits3 = bs.Read1Byte();
            SourceTypeMaybe = (ScfomDataType)(bits3 & 0b1_1111); // 5 bits
        }
        else
        {
            byte bits2 = bs.Read1Byte();
            TargetTypeMaybe = (ScfomDataType)(bits2 & 0b1111); // 4 bits
            SourceTypeMaybe = (ScfomDataType)(bits2 >> 4); // 4 bits
        }
    }
}


