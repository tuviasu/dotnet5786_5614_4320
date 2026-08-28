using System;
using System.IO;

namespace DalApi;

/// <summary>
/// Resolves the path to the <c>xml\</c> data directory in a working-directory-agnostic way.
/// <para>
/// The app historically loaded data from the hardcoded relative path <c>..\xml\</c>, which only
/// works when the current working directory is the folder that contains the executable (e.g.
/// <c>app\</c> or <c>bin\</c>). When launched via a batch file from the release root, the CWD is
/// the release root and <c>..\xml\</c> resolves to the wrong place. This resolver tries several
/// candidate locations — relative to the CWD and relative to the executable's base directory —
/// and uses the first one that actually exists, so the app starts correctly no matter how it is
/// launched (direct <c>PL.exe</c>, <c>START_APP.bat</c>, <c>dotnet bin\PL.dll</c>, or the solution root).
/// </para>
/// </summary>
public static class XmlPaths
{
    /// <summary>Candidate <c>xml</c> directories, evaluated in order. First that exists wins.</summary>
    private static readonly string[] s_candidates =
    {
        @"..\xml",                                                              // relative to CWD (legacy: works when run from app\ or bin\)
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "xml"),       // relative to exe dir (app\..\xml  -> release root\xml)
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xml"),             // exe dir\xml (if xml sits next to the exe)
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "xml"), // deeper nesting fallback
    };

    /// <summary>Resolved xml directory as an absolute path with a trailing separator.</summary>
    public static string XmlDir { get; } = ResolveXmlDir();

    /// <summary>Returns the full path to <paramref name="fileName"/> inside the resolved xml directory.</summary>
    public static string ResolveFile(string fileName) => Path.Combine(XmlDir, fileName);

    private static string ResolveXmlDir()
    {
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

        // Nothing found — fall back to the legacy relative path so any downstream
        // error message references the familiar location.
        return @"..\xml\";
    }
}