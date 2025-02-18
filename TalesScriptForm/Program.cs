using Syroot.BinaryData;

using System.Text;

namespace TalesScriptForm;

public class Program
{
    static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: TalesScriptForm <path to SCFOMBIN/SCR (v3) or SFM2 (v2) file>");
            return;
        }

        var fs = File.OpenRead(args[0]);
        byte[] magic = fs.ReadBytes(8);
        fs.Position = 0;

        ScriptFormBase scriptForm;
        if (magic.AsSpan().SequenceEqual("SCFOMBIN"u8))
        {
            scriptForm = ScriptFormFile3.Read(fs);
        }
        else if (magic.AsSpan(0, 4).SequenceEqual("SFM2"u8))
        {
            scriptForm = ScriptFormFile2.Read(fs);
        }
        else
        {
            throw new InvalidDataException("Not a SCFOM file.");
        }

        scriptForm.Disassemble(fs, args[0] + ".diss");
    }
}
