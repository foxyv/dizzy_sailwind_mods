using Mono.Cecil;
var path = @"D:\SteamLibrary\steamapps\common\Sailwind\Sailwind_Data\Managed\Assembly-CSharp.dll";
var resolver = new DefaultAssemblyResolver();
resolver.AddSearchDirectory(Path.GetDirectoryName(path)!);
var asm = AssemblyDefinition.ReadAssembly(path, new ReaderParameters { AssemblyResolver = resolver });
foreach (var t in asm.MainModule.Types) {
  foreach (var m in t.Methods.Where(m => m.HasBody && m.Body.Instructions.Any(i => i.Operand is MethodReference mr && mr.Name == "ExtraFixedUpdate"))) {
    Console.WriteLine(t.Name + "." + m.Name + " -> ExtraFixedUpdate");
  }
}
