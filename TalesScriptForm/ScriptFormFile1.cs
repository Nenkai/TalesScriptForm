using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace TalesScriptForm;

public class ScriptFormFile1 : ScriptFormBase
{
    public static ScriptFormFile1 Read(Stream stream)
    {
        var bs = new BinaryStream(stream);
        byte[] magic = bs.ReadBytes(4);
        if (!magic.AsSpan().SequenceEqual("SFM_"u8))
            throw new InvalidDataException("Not a SCFOM file.");

        var script = new ScriptFormFile1();
        script.Read(bs);
        return script;
    }

    private void Read(BinaryStream bs)
    {
        Version = bs.ReadUInt32();
        FileSize = bs.ReadUInt32();
        ScriptBodySize = bs.ReadUInt32();
        uint field_0x10 = bs.ReadUInt32();
        CodeOffset = bs.ReadUInt32();

        // Some of this might be wrong, may need to figure out field_0x10 too.
        StringTableOffset = bs.ReadUInt32();
        MainCodeOffset = bs.ReadUInt32();

        // TODO: Read different bytecode.
    }
}
