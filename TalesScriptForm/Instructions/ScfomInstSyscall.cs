using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesScriptForm.Instructions;

public class ScfomInstSyscall : ScfomInstructionBase
{
    public override ScfomInstructionType InstructionType => ScfomInstructionType.SCFOM_INST_SYSCALL;

    public byte NumArgs { get; set; }
    public byte Flags { get; set; }

    /// <summary>
    /// For version < 30000
    /// </summary>
    public ushort SyscallNumber { get; set; }

    public override void ReadData(BinaryStream bs, uint version)
    {
        if (version >= 30000)
        {
            byte bits = bs.Read1Byte();
            NumArgs = (byte)(bits & 0b1111);
            Flags = (byte)(bits >> 4);
        }
        else
        {
            NumArgs = bs.Read1Byte();
            SyscallNumber = bs.ReadUInt16();
        }
    }
}
