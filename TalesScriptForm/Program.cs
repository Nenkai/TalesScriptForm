using Syroot.BinaryData;

using System.Text;

namespace TalesScriptForm;

public class Program
{
    public const string Version = "0.1.1";

    static void Main(string[] args)
    {
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine($"- TalesScriptForm {Version} by Nenkai");
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine("- https://github.com/Nenkai");
        Console.WriteLine("- https://twitter.com/Nenkaai");
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine("");

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: TalesScriptForm <path to SCFOMBIN/SCR (v3) or SFM2 (v2) file or directory>");
            return;
        }

        if (Directory.Exists(args[0]))
        {
            foreach (var file in  Directory.GetFiles(args[0]))
            {
                try
                {
                    ProcessFile(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error for '{file}': {ex.Message}");
                }
            }
        }
        else if (File.Exists(args[0]))
        {
            ProcessFile(args[0]);
        }
    }

    private static void ProcessFile(string file)
    {
        var fs = File.OpenRead(file);
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

        Console.WriteLine($"Disassembling '{file}'...");

        scriptForm.Disassemble(fs, file + ".diss");
    }
}
