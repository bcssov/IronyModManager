internal static class PatchConstants
{
    public const string CecilTypeSystemName = "XamlX.TypeSystem.CecilTypeSystem";

    // Established with --inspect against the unmodified Avalonia 0.10.22 task assembly.
    public const int ExpectedConstructorSiteCount = 1;
    public const int ExpectedDisposeInsertionCount = 3;
}
