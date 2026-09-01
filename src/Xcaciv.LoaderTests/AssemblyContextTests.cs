using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zTestInterfaces;
using Xunit.Abstractions;
using System.Reflection;

namespace Xcaciv.Loader.Tests
{
    public class AssemblyContextTests
    {
        private ITestOutputHelper _testOutput;
        private string simpleDllPath;
        private string dependentDllPath;

        public AssemblyContextTests(ITestOutputHelper output)
        {
            this._testOutput = output;
#if DEBUG
            this._testOutput.WriteLine("Tests in Debug mode");
            const string configuration = "Debug";
#else
            this._testOutput.WriteLine("Tests in Release mode??");
            const string configuration = "Release";
#endif
            this.simpleDllPath = System.IO.Path.Combine("..", "..", "..", "..", "TestAssembly", "bin", configuration, "net8.0", "zTestAssembly.dll");
            this.dependentDllPath = System.IO.Path.Combine("..", "..", "..", "..", "zTestDependentAssembly", "bin", configuration, "net8.0", "zTestDependentAssembly.dll");

            // resolve absolute paths
            this.simpleDllPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.simpleDllPath));
            this.dependentDllPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.dependentDllPath));
        }

        [Fact()]
        public void VerifyPath_Test()
        {
            // Build a path that is genuinely rooted on the current platform (a Windows
            // drive letter is not rooted on Unix, so "C:\..." would resolve relative to
            // the current directory there instead of round-tripping unchanged).
            var root = OperatingSystem.IsWindows() ? "C:" : System.IO.Path.DirectorySeparatorChar.ToString();
            var restrictedPath = System.IO.Path.Combine(root, "some", "folder", "path");
            var filePath = System.IO.Path.Combine(restrictedPath, "subpath");
            var actualpath = Xcaciv.Loader.AssemblyContext.VerifyPath(filePath);

            Xunit.Assert.Equal(filePath, actualpath);
        }

        [Fact()]
        public void LoadAssembly_Test()
        {
            var expectedPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, simpleDllPath));
            var basePath = System.IO.Path.GetDirectoryName(expectedPath) ?? String.Empty;
            var context = new Xcaciv.Loader.AssemblyContext(simpleDllPath, basePathRestriction: basePath);

            Xunit.Assert.Equal(expectedPath, context.FilePath);
        }

        [Fact()]
        public void LoadOutOfRangeAssembly_Test()
        {
            var expectedPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, simpleDllPath));


            Xunit.Assert.Throws<ArgumentOutOfRangeException>(() => new Xcaciv.Loader.AssemblyContext(simpleDllPath));
        }

        [Fact()]
        public void LoadDoesNotExistAssembly_Test()
        {
            var expectedPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "does\\not\\exist.dll"));
            var context = new Xcaciv.Loader.AssemblyContext(expectedPath, basePathRestriction: "*");

            Xunit.Assert.Throws<System.IO.FileNotFoundException>(() => context.CreateInstance("Class1"));
        }


        [Fact()]
        public void GetInstance_OutputTest()
        {
            var actual = String.Empty;

            using (var context = new Xcaciv.Loader.AssemblyContext(simpleDllPath, basePathRestriction: "*"))
            {
                IClass1? class1 = context.CreateInstance("Class1") as IClass1;
                actual = class1?.Stuff("input text here") ?? String.Empty;
                context.Unload();
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Xunit.Assert.Equal("input text here output", actual);
        }

        private string UseFactory(string path)
        {
            using (var context = new AssemblyContext(path, basePathRestriction: "*"))
            {
                IClass1? class1 = context.CreateInstance("Class1") as IClass1;
                return class1?.Stuff("input text here") ?? String.Empty;
            }
        }

        [Fact()]
        public void UsingFactory_FromPath_Unloads()
        {
            var actual = UseFactory(simpleDllPath);

            // collect to demonstrate unload
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Xunit.Assert.Equal("input text here output", actual);
        }

        public string DoManual(string aPath)
        {
            var context = new System.Runtime.Loader.AssemblyLoadContext(null, true);
            var assembly = context.LoadFromAssemblyPath(aPath);
            var location = assembly.Location;

            var classTypeName = assembly.GetTypes().FirstOrDefault(t => typeof(IClass1).IsAssignableFrom(t))?.FullName ?? String.Empty;
            IClass1? class1 = assembly.CreateInstance(classTypeName) as IClass1;
            var actual = class1?.Stuff("input text here") ?? String.Empty;

            context.Unload();

            return actual;
        }

        [Fact()]
        public void ManualTest()
        {
            var actual = DoManual(Xcaciv.Loader.AssemblyContext.VerifyPath(simpleDllPath));

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Xunit.Assert.Equal("input text here output", actual);
        }

        [Fact()]
        public void UsingFactory_WithDependency_Unloads()
        {
            var actual = String.Empty;
            using (var context = new AssemblyContext(dependentDllPath, basePathRestriction: "*"))
            {
                IClass1? class1 = context.CreateInstance("Class1") as IClass1;
                actual = class1?.Stuff("input text here") ?? String.Empty;
            }

            Xunit.Assert.Equal("5,5,8", actual);
        }

        [Fact()]
        public void UsingStrongTypedFactory_Unloads()
        {
            var actual = String.Empty;
            using (var context = new AssemblyContext(dependentDllPath, basePathRestriction: "*"))
            {
                var class1 = context.CreateInstance<IClass1>("Class1");
                actual = class1?.Stuff("input text here") ?? String.Empty;
            }

            Xunit.Assert.Equal("5,5,8", actual);
        }
    }
}