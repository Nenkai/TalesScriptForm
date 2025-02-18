using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace TalesScriptForm;

public class ScriptFormFile2 : ScriptFormBase
{
    public static ScriptFormFile2 Read(Stream stream)
    {
        var bs = new BinaryStream(stream);
        byte[] magic = bs.ReadBytes(4);
        if (!magic.AsSpan().SequenceEqual("SFM2"u8))
            throw new InvalidDataException("Not a SCFOM file.");

        var script = new ScriptFormFile2();
        script.Read(bs);
        return script;
    }

    private void Read(BinaryStream bs)
    {
        Version = bs.ReadUInt32();
        FileSize = bs.ReadUInt32();
        ScriptBodySize = bs.ReadUInt32();
        uint field_0x10 = bs.ReadUInt32();
        uint field_0x14 = bs.ReadUInt32();
        CodeOffset = bs.ReadUInt32();
        StringTableOffset = bs.ReadUInt32();
        DataOffset = bs.ReadUInt32();
        MainCodeOffset = bs.ReadUInt32();
        uint finalizersInfoSectionSize = bs.ReadUInt32();
        uint finalizersInfoSectionOffset = bs.ReadUInt32();
        uint field_0x2C = bs.ReadUInt32();
        uint field_0x30 = bs.ReadUInt32();
        Unk5 = bs.ReadUInt32();
        Unk6 = bs.ReadUInt32();
        Unk7 = bs.ReadUInt32();

        bs.Position = finalizersInfoSectionOffset;
        for (int i = 0; i < finalizersInfoSectionSize / 0x08; i++)
        {
            var info = new FinalizerFunctionInfo();
            info.Read(bs);
            FinalizersInfo.Add(info);
        }

        bs.Position = 0;
        FileBytes = bs.ReadBytes((int)FileSize);
    }
}
