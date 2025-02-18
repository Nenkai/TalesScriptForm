using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesScriptForm.Instructions;

public class ScfomInstCalc : ScfomInstructionBase
{
    public override ScfomInstructionType InstructionType => ScfomInstructionType.SCFOM_INST_CALC;

    public long Value { get; set; }

    public ScfomCalcOperator CalcFlag { get; set; }
    public ScfomDataType LeftDataType { get; set; }
    public ScfomDataType RightDataType { get; set; }

    public override void ReadData(BinaryStream bs, uint version)
    {
        byte bits1 = bs.Read1Byte();
        CalcFlag = (ScfomCalcOperator)(bits1 & 0b11_1111);

        if (version >= 30000)
        {
            byte bits2 = bs.Read1Byte();
            LeftDataType = (ScfomDataType)(bits2 & 0b1_1111); // 5 bits

            byte bits3 = bs.Read1Byte();
            RightDataType = (ScfomDataType)(bits3 & 0b1_1111); // 5 bits
        }
        else
        {
            byte bits2 = bs.Read1Byte();
            LeftDataType = (ScfomDataType)(bits2 & 0b1111); // 4 bits
            RightDataType = (ScfomDataType)(bits2 >> 4); // 4 bits
        }
    }
}
