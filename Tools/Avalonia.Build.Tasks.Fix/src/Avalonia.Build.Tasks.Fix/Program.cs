using Mono.Cecil;
using Mono.Cecil.Cil;

try
{
    var options = Options.Parse(args);
    using var assembly = AssemblyDefinition.ReadAssembly(options.InputPath, new ReaderParameters { ReadSymbols = false });
    var patchPlan = PatchPlan.Create(assembly.MainModule);

    if (options.InspectOnly)
    {
        Console.WriteLine($"Assembly: {assembly.Name.FullName}");
        Console.WriteLine($"Constructor sites: {patchPlan.ConstructorSites.Count}");
        Console.WriteLine($"Dispose insertions required: {patchPlan.RequiredInsertions}");
        Console.WriteLine($"Already patched insertions: {patchPlan.ExistingInsertions}");
        return;
    }

    if (patchPlan.ConstructorSites.Count != PatchConstants.ExpectedConstructorSiteCount ||
        patchPlan.RequiredInsertions != PatchConstants.ExpectedDisposeInsertionCount)
    {
        throw new InvalidOperationException(
            $"Expected {PatchConstants.ExpectedConstructorSiteCount} CecilTypeSystem construction site(s) and " +
            $"{PatchConstants.ExpectedDisposeInsertionCount} Dispose insertion(s) in Avalonia 0.10.22, but found " +
            $"{patchPlan.ConstructorSites.Count} and {patchPlan.RequiredInsertions}. Refusing to patch an unexpected Avalonia.Build.Tasks layout.");
    }

    if (patchPlan.ExistingInsertions == patchPlan.RequiredInsertions)
    {
        File.Copy(options.InputPath, options.OutputPath, overwrite: true);
        Console.WriteLine($"Already patched: copied unchanged assembly to '{options.OutputPath}'.");
        return;
    }

    if (patchPlan.ExistingInsertions != 0)
    {
        throw new InvalidOperationException(
            $"Found a partially patched assembly ({patchPlan.ExistingInsertions} of {patchPlan.RequiredInsertions} expected Dispose call(s)). Refusing to modify it.");
    }

    patchPlan.Apply();
    assembly.Write(options.OutputPath);

    using var verificationAssembly = AssemblyDefinition.ReadAssembly(options.OutputPath, new ReaderParameters { ReadSymbols = false });
    var verificationPlan = PatchPlan.Create(verificationAssembly.MainModule);
    if (verificationPlan.ConstructorSites.Count != PatchConstants.ExpectedConstructorSiteCount ||
        verificationPlan.RequiredInsertions != PatchConstants.ExpectedDisposeInsertionCount ||
        verificationPlan.ExistingInsertions != verificationPlan.RequiredInsertions)
    {
        throw new InvalidOperationException("The output assembly did not contain the expected complete patch.");
    }

    Console.WriteLine($"Patched {PatchConstants.ExpectedConstructorSiteCount} CecilTypeSystem construction site(s) with {PatchConstants.ExpectedDisposeInsertionCount} Dispose call(s) -> '{options.OutputPath}'.");
}
catch (Exception exception)
{
    Console.Error.WriteLine($"Avalonia.Build.Tasks patch failed: {exception.Message}");
    Environment.ExitCode = 1;
}

sealed class Options
{
    public required string InputPath { get; init; }
    public required string OutputPath { get; init; }
    public bool InspectOnly { get; init; }

    public static Options Parse(string[] arguments)
    {
        string? input = null;
        string? output = null;
        var inspectOnly = false;

        for (var index = 0; index < arguments.Length; index++)
        {
            switch (arguments[index])
            {
                case "--input" when index + 1 < arguments.Length:
                    input = arguments[++index];
                    break;
                case "--output" when index + 1 < arguments.Length:
                    output = arguments[++index];
                    break;
                case "--inspect":
                    inspectOnly = true;
                    break;
                default:
                    throw new ArgumentException("Usage: --input <original.dll> --output <patched.dll> [--inspect]");
            }
        }

        if (string.IsNullOrWhiteSpace(input) || !File.Exists(input))
        {
            throw new ArgumentException("--input must name an existing Avalonia.Build.Tasks.dll.");
        }

        if (inspectOnly)
        {
            return new Options { InputPath = Path.GetFullPath(input), OutputPath = string.Empty, InspectOnly = true };
        }

        if (string.IsNullOrWhiteSpace(output))
        {
            throw new ArgumentException("--output is required unless --inspect is specified.");
        }

        var inputPath = Path.GetFullPath(input);
        var outputPath = Path.GetFullPath(output);
        if (string.Equals(inputPath, outputPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("--output must differ from --input; the tool never overwrites its input.");
        }

        var outputDirectory = Path.GetDirectoryName(outputPath);
        if (string.IsNullOrEmpty(outputDirectory) || !Directory.Exists(outputDirectory))
        {
            throw new ArgumentException("The directory containing --output must already exist.");
        }

        return new Options { InputPath = inputPath, OutputPath = outputPath, InspectOnly = false };
    }
}

sealed class PatchPlan
{
    private PatchPlan(MethodReference disposeMethod, List<ConstructorSite> constructorSites)
    {
        DisposeMethod = disposeMethod;
        ConstructorSites = constructorSites;
    }

    public MethodReference DisposeMethod { get; }
    public List<ConstructorSite> ConstructorSites { get; }
    public int RequiredInsertions => ConstructorSites.Sum(site => site.Returns.Count);
    public int ExistingInsertions => ConstructorSites.Sum(site => site.Returns.Count(@return => HasDisposeImmediatelyBefore(@return, site.Local, DisposeMethod)));

    public static PatchPlan Create(ModuleDefinition module)
    {
        var cecilType = AllTypes(module)
            .SingleOrDefault(type => type.FullName == PatchConstants.CecilTypeSystemName)
            ?? throw new InvalidOperationException($"Could not find '{PatchConstants.CecilTypeSystemName}'.");

        var disposeMethod = cecilType.Methods
            .SingleOrDefault(method => method.Name == "Dispose" && !method.HasParameters)
            ?? throw new InvalidOperationException($"Could not find parameterless Dispose on '{PatchConstants.CecilTypeSystemName}'.");

        var constructorSites = new List<ConstructorSite>();
        foreach (var method in AllTypes(module).SelectMany(type => type.Methods).Where(method => method.HasBody))
        {
            foreach (var instruction in method.Body.Instructions.Where(IsCecilTypeSystemConstructor).ToList())
            {
                constructorSites.Add(ConstructorSite.Create(method, instruction));
            }
        }

        if (constructorSites.Count == 0)
        {
            throw new InvalidOperationException($"No construction sites for '{PatchConstants.CecilTypeSystemName}' were found.");
        }

        return new PatchPlan(disposeMethod, constructorSites);
    }

    public void Apply()
    {
        foreach (var site in ConstructorSites)
        {
            var processor = site.Method.Body.GetILProcessor();
            foreach (var @return in site.Returns)
            {
                processor.InsertBefore(@return, processor.Create(OpCodes.Ldloc, site.Local));
                processor.InsertBefore(@return, processor.Create(OpCodes.Callvirt, DisposeMethod));
            }
        }
    }

    private static bool IsCecilTypeSystemConstructor(Instruction instruction) =>
        instruction.OpCode == OpCodes.Newobj &&
        instruction.Operand is MethodReference constructor &&
        constructor.DeclaringType.FullName == PatchConstants.CecilTypeSystemName;

    private static bool HasDisposeImmediatelyBefore(Instruction @return, VariableDefinition local, MethodReference disposeMethod)
    {
        var call = @return.Previous;
        var load = call?.Previous;
        return call?.OpCode == OpCodes.Callvirt &&
               call.Operand is MethodReference calledMethod &&
               calledMethod.FullName == disposeMethod.FullName &&
               IsLoadOfLocal(load, local);
    }

    private static bool IsLoadOfLocal(Instruction? instruction, VariableDefinition local) =>
        instruction?.OpCode.Code switch
        {
            Code.Ldloc_0 => local.Index == 0,
            Code.Ldloc_1 => local.Index == 1,
            Code.Ldloc_2 => local.Index == 2,
            Code.Ldloc_3 => local.Index == 3,
            Code.Ldloc or Code.Ldloc_S => ReferenceEquals(instruction.Operand, local),
            _ => false
        };

    private static IEnumerable<TypeDefinition> AllTypes(ModuleDefinition module) => module.Types.SelectMany(Flatten);

    private static IEnumerable<TypeDefinition> Flatten(TypeDefinition type)
    {
        yield return type;
        foreach (var nestedType in type.NestedTypes.SelectMany(Flatten))
        {
            yield return nestedType;
        }
    }
}

sealed class ConstructorSite
{
    private ConstructorSite(MethodDefinition method, VariableDefinition local, List<Instruction> returns)
    {
        Method = method;
        Local = local;
        Returns = returns;
    }

    public MethodDefinition Method { get; }
    public VariableDefinition Local { get; }
    public List<Instruction> Returns { get; }

    public static ConstructorSite Create(MethodDefinition method, Instruction constructor)
    {
        var store = constructor.Next
            ?? throw new InvalidOperationException($"{method.FullName}: CecilTypeSystem constructor is the final instruction.");
        var local = GetStoredLocal(method.Body, store)
            ?? throw new InvalidOperationException($"{method.FullName}: CecilTypeSystem constructor must be followed by stloc, found '{store}'.");
        var returns = method.Body.Instructions.Where(instruction => instruction.OpCode == OpCodes.Ret).ToList();
        if (returns.Count == 0)
        {
            throw new InvalidOperationException($"{method.FullName}: CecilTypeSystem construction method has no return instruction.");
        }

        return new ConstructorSite(method, local, returns);
    }

    private static VariableDefinition? GetStoredLocal(MethodBody body, Instruction instruction) =>
        instruction.OpCode.Code switch
        {
            Code.Stloc_0 => LocalAt(body, 0),
            Code.Stloc_1 => LocalAt(body, 1),
            Code.Stloc_2 => LocalAt(body, 2),
            Code.Stloc_3 => LocalAt(body, 3),
            Code.Stloc or Code.Stloc_S => instruction.Operand as VariableDefinition,
            _ => null
        };

    private static VariableDefinition? LocalAt(MethodBody body, int index) =>
        index < body.Variables.Count ? body.Variables[index] : null;
}
