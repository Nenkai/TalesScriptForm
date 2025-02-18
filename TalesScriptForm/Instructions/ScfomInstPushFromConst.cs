using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesScriptForm.Instructions;

public class ScfomInstPushFromConst : ScfomInstructionBase
{
    public override ScfomInstructionType InstructionType => ScfomInstructionType.SCFOM_INST_PUSH_CONST;

    public ScfomConstantType ConstType { get; set; }
    public int Offset { get; set; }

    public override void ReadData(BinaryStream bs, uint version)
    {
        byte bits = bs.Read1Byte();

        if (version >= 30000)
            ConstType = (ScfomConstantType)(bits & 0b11111);
        else
            ConstType = (ScfomConstantType)(bits & 0b1111);

        // FIXME: Sometimes const type is 8, which should be completely unpossible?
        // How is this handled?

        switch (Size)
            {
                case 3:
                    Offset = bs.ReadSByte();
                    break;

                case 4:
                    Offset = bs.ReadInt16();
                    break;

                case 6:
                    Offset = bs.ReadInt32();
                    break;
            }
    }
}
