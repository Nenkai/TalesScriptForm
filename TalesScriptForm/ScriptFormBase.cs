﻿using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesScriptForm;

public class ScriptFormBase
{
    public bool IsBigEndian { get; set; } = false;

    public byte[] FileBytes { get; set; }

    public uint Version { get; set; }
    public uint FileSize { get; set; }
    public uint ScriptBodySize { get; set; }
    public uint CodeOffset { get; set; }
    public uint StringTableOffset { get; set; }
    public uint MainCodeOffset { get; set; }
    public uint DataOffset { get; set; } // Kinda like .data section
    public uint Unk5 { get; set; } // Const entry 5
    public uint Unk6 { get; set; } // Const entry 6
    public uint Unk7 { get; set; } // Const entry 7

    public List<FinalizerFunctionInfo> FinalizersInfo { get; set; } = [];

    public void Disassemble(Stream stream, string outputFileName)
    {
        var bs = new BinaryStream(stream, IsBigEndian ? ByteConverter.Big : ByteConverter.Little);
        bs.Position = CodeOffset;

        using var sw = new StreamWriter(outputFileName);
        sw.WriteLine("/////////////////////////////////////////////////////");
        sw.WriteLine($"// Endian: {(IsBigEndian ? "Big" : "Little")}");
        sw.WriteLine($"// Version: {Version}");
        sw.WriteLine($"// CodeOffset: 0x{CodeOffset:X}");
        sw.WriteLine($"// DataOffset: 0x{CodeOffset:X}");
        sw.WriteLine($"// StringTableOffset: 0x{StringTableOffset:X}");
        sw.WriteLine($"// EntrypointCodeOffset: 0x{CodeOffset + MainCodeOffset:X} (Relative: 0x{MainCodeOffset:X})");
        sw.WriteLine($"// Unk1: 0x{Unk5:X}");
        sw.WriteLine($"// Unk2: 0x{Unk6:X}");
        sw.WriteLine($"// Unk3: 0x{Unk7:X}");

        sw.WriteLine($"// Func Infos ({FinalizersInfo.Count}) :");

        for (int i = 0; i < FinalizersInfo.Count; i++)
            sw.WriteLine($"// - Index {FinalizersInfo[i].Index}, Val:{FinalizersInfo[i].Unk2}, CodeOffset: {CodeOffset + FinalizersInfo[i].CodeOffset:X} (Rel: {FinalizersInfo[i].CodeOffset:X})");

        sw.WriteLine("////////////////////////////////////////////////////");
        sw.WriteLine();

        while (bs.Position < CodeOffset + ScriptBodySize)
        {
            var byteCode = new ScfomBytecode();
            byteCode.Read(bs, Version);

            byteCode.Disassemble(this, sw);
            sw.WriteLine();
        }
    }
}

public class FinalizerFunctionInfo
{
    public short Index; // 0 = Main always.
    public short Unk2 { get; set; }
    public uint CodeOffset { get; set; }

    public void Read(BinaryStream bs)
    {
        Index = bs.ReadInt16();
        Unk2 = bs.ReadInt16();
        CodeOffset = bs.ReadUInt32();
    }
}
