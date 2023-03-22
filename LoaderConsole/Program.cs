using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;

var referenceFile = @"..\..\..\..\TargetApp\obj\Debug\net7.0-windows\Reference.txt";
var targetFile = @"..\..\..\..\TargetApp\bin\Debug\net7.0-windows\TargetApp.dll";
//var referenceFile = @"..\..\..\..\TargetLibrary\obj\Debug\net7.0\Reference.txt";
//var targetFile = @"..\..\..\..\TargetLibrary\bin\Debug\net7.0\TargetLibrary.dll";

var references = File.ReadAllLines(Path.GetFullPath(referenceFile))
    .Select(x => new { Name = Path.GetFileNameWithoutExtension(x), FilePath = x })
    .ToDictionary(x => x.Name);

var target = Path.GetFullPath(targetFile);
var targetAssembly = Assembly.LoadFile(target);
var context = AssemblyLoadContext.GetLoadContext(targetAssembly);
context!.Resolving += (_, name) =>
{
    if (references.TryGetValue(name.Name!, out var reference))
    {
        try
        {
            return context.LoadFromAssemblyPath(reference.FilePath);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            throw;
        }
    }

    return null;
};

var assembly = context.LoadFromAssemblyPath(target);
foreach (var type in assembly.ExportedTypes)
{
    Debug.WriteLine($"Type: {type}");
}
