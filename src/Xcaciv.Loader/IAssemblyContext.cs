
namespace Xcaciv.Loader;

/// <summary>
/// class for abstracting type loading
/// </summary>
public interface IAssemblyContext : IDisposable
{
    /// <summary>
    /// full assembly file path
    /// </summary>
    string FilePath { get; }
    /// <summary>
    /// full assemblyName of the assembly for reference
    /// </summary>
    string FullAssemblyName { get; }
    /// <summary>
    /// Attempts to create an instance from the current assembly given a class assemblyName.
    /// If the class does not exist in this assembly a null object is returned.
    /// </summary>
    /// <param assemblyName="className"></param>
    /// <returns></returns>
    object? CreateInstance(string className);
    /// <summary>
    /// Attempts to create an instance from the current assembly given a class assemblyName.
    /// If the class does not exist in this assembly a null object is returned.
    /// </summary>
    /// <param assemblyName="className"></param>
    /// <returns></returns>
    T CreateInstance<T>(string className);
    /// <summary>
    /// Attempts to create an instance from the current assembly given its Type and
    /// tries to box it to T
    /// If the class does not exist or cannot be boxed in T a null object is returned.
    /// </summary>
    /// <param assemblyName="className"></param>
    /// <returns></returns>
    T CreateInstance<T>(Type classType);
    /// <summary>
    /// list types from loaded assembly
    /// </summary>
    /// <returns></returns>
    IEnumerable<Type>? GetTypes();
    /// <summary>
    /// list types that implement or extend a base type
    /// </summary>
    /// <param assemblyName="baseType"></param>
    /// <returns></returns>
    IEnumerable<Type>? GetTypes(Type baseType);
    /// <summary>
    /// list types that implement or extend a base type T
    /// </summary>
    /// <returns></returns>
    IEnumerable<Type> GetTypes<T>();
    /// <summary>
    /// return assembly version
    /// </summary>
    /// <returns></returns>
    Version GetVersion();
    /// <summary>
    /// attempt to unload the load context
    /// May or may not be optimistic unloading
    /// </summary>
    /// <returns>true on success</returns>
    bool Unload();
}
