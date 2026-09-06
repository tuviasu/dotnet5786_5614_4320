using System;
using System.IO;

namespace DalApi;

/// <summary>
/// Resolves the path to the <c>xml\</c> data directory in a working-directory-agnostic way.
/// </summary>
public static class XmlPaths
{
    /// <summary>Candidate <c>xml</c> directories, evaluated in order. First that exists wins.</summary>
    private static readonly string[] s_candidates =
    {
        @"..\xml",                                                              // relative to CWD (legacy: works when run from app\ or bin\)
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "xml"),       // relative to exe dir (app\..\xml  -> release root\xml)
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xml"),             // exe dir\xml (if xml sits next to the exe, or was copied to output)
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "xml"), // deeper nesting fallback (bin\Debug\..\..\xml)
    };

    /// <summary>Resolved xml directory as an absolute path with a trailing separator.</summary>
    public static string XmlDir { get; } = ResolveXmlDir();

    /// <summary>Returns the full path to <paramref name="fileName"/> inside the resolved xml directory.</summary>
    public static string ResolveFile(string fileName) => Path.Combine(XmlDir, fileName);

    private static string ResolveXmlDir()
    {
        // 1. Try the predefined candidates first (covers the common launch scenarios).
        foreach (var candidate in s_candidates)
        {
            try
            {
                if (Directory.Exists(candidate))
                    return Path.GetFullPath(candidate) + Path.DirectorySeparatorChar;
            }
            catch
            {
                // ignore candidates that fail to resolve (e.g. invalid path combos)
            }
        }

        // 2. Walk up from the executable's directory looking for an `xml` folder.
        //    This handles the case where SolutionDir was empty during build and the
        //    exe ended up at an unexpected location (e.g. C:\bin\). Without this,
        //    DalConfig's static ctor throws FileNotFoundException -> TypeInitializationException.
        try
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            for (int i = 0; i < 6 && dir != null; i++)
            {
                var candidate = Path.Combine(dir.FullName, "xml");
                if (Directory.Exists(candidate))
                    return candidate + Path.DirectorySeparatorChar;
                dir = dir.Parent;
            }
        }
        catch
        {
            // ignore and fall through to the legacy default
        }

        // 3. Nothing found — fall back to the legacy relative path so any downstream
        //    error message references the familiar location.
        return @"..\xml\";
    }
}