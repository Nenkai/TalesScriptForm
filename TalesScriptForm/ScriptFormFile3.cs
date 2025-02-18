using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace TalesScriptForm;

public class ScriptFormFile3 : ScriptFormBase
{
    public static ScriptFormFile3 Read(Stream stream)
    {
        var bs = new BinaryStream(stream);
        byte[] magic = bs.ReadBytes(8);
        if (!magic.AsSpan().SequenceEqual("SCFOMBIN"u8))
            throw new InvalidDataException("Not a SCFOM file.");

        var script = new ScriptFormFile3();
        script.Read(bs);
        return script;
    }

    private void Read(BinaryStream bs)
    {
        using (var seek = bs.TemporarySeek(0x2C, SeekOrigin.Begin))
            bs.ByteConverter = bs.ReadBoolean() ? ByteConverter.Big : ByteConverter.Little;

        Version = bs.ReadUInt32();
        FileSize = bs.ReadUInt32();
        ScriptBodySize = bs.ReadUInt32();
        uint field_0x10 = bs.ReadUInt32();
        uint field_0x14 = bs.ReadUInt32();
        CodeOffset = bs.ReadUInt32();
        StringTableOffset = bs.ReadUInt32();
        DataOffset = bs.ReadUInt32();
        MainCodeOffset = bs.ReadUInt32();
        IsBigEndian = bs.ReadBoolean();
        bs.Position += 3; // TODO: Third byte is 0x10
        uint field_0x30 = bs.ReadUInt32();
        uint field_0x34 = bs.ReadUInt32();
        uint field_0x38 = bs.ReadUInt32();
        uint field_0x3C = bs.ReadUInt32();
        uint functionInfoSectionSize = bs.ReadUInt32();
        uint functionInfoSectionOffset = bs.ReadUInt32();
        uint field_0x48 = bs.ReadUInt32(); // Size for Unk5?
        Unk5 = bs.ReadUInt32();
        Unk6 = bs.ReadUInt32();
        Unk7 = bs.ReadUInt32();

        bs.Position = functionInfoSectionOffset;
        for (int i = 0; i < functionInfoSectionSize / 0x08; i++)
        {
            var info = new FinalizerFunctionInfo();
            info.Read(bs);
            FinalizersInfo.Add(info);
        }

        bs.Position = 0;
        FileBytes = bs.ReadBytes((int)FileSize);
    }
}

