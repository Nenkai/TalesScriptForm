# TalesScriptForm

A disassembler for SCFOM files found in Tales of games, under the `SCFOMBIN` (v3) or `SFM2` magics.

Not complete, syscalls are mostly unknown among other bits, but should give a basic overview of what's going on. Script format is pretty close to C. Static data goes into a static data section in binaries.

Tales of Rebirth has an uncompiled script file lying around.

## Note

Scfom can be used alongside CScript (which has [already been](https://delroth.net/posts/reverse-engineering-script-interpreter/) reverse-engineered), for instance both Scfom and CScript are available in Tales of Vesperia.

A 010 editor template is available [here](https://github.com/Nenkai/010GameTemplates/blob/main/Bandai%20Namco/Tales%20of/SCFOMBIN_SFM2_ScriptForm.bt), note that it may be slightly behind compared to this repo.
