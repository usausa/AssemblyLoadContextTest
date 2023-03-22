using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace LoaderWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var referenceFile = @"..\..\..\..\TargetApp\obj\Debug\net7.0-windows\Reference.txt";
            var targetFile = @"..\..\..\..\TargetApp\bin\Debug\net7.0-windows\TargetApp.dll";

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
        }
    }
}
