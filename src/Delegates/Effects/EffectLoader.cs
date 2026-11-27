using System.Reflection;

namespace CrowdControl.Delegates.Effects;

/// <summary>An effect delegate container.</summary>
public class EffectLoader
{
    /// <summary>Provides a mapping of effect IDs to their respective delegates.</summary>
    /// <remarks>
    /// This should not need to be explicitly filled out, it is done automatically via reflection in the constructor.
    /// Just make sure to add the [Effect] attribute to your classes.
    /// </remarks>
    public readonly Dictionary<string, Effect> Effects = new();

    private readonly CrowdControlMod m_mod;
    private readonly NetworkClient m_client;

    /// <summary>
    /// Automatically loads all effect delegates from the mod assembly.
    /// </summary>
    public EffectLoader(CrowdControlMod mod, NetworkClient client)
    {
        m_mod = mod;
        m_client = client;
        RegisterAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>Instantiates and registers every effect class in the supplied assembly.</summary>
    /// <param name="assembly">The assembly to scan.</param>
    /// <remarks>Effects that fail to load are logged and skipped - one bad effect must not cost us the rest.</remarks>
    private void RegisterAssembly(Assembly assembly)
    {
        //abstract classes are skipped so intermediate base classes can be used to share code between effects
        foreach (Type type in SafeGetTypes(assembly).Where(type => type.IsSubclassOf(typeof(Effect)) && !type.IsAbstract))
        {
            try
            {
                foreach (EffectAttribute attribute in type.GetCustomAttributes<EffectAttribute>())
                {
                    Effect? effect = null;
                    foreach (string id in attribute.IDs)
                    {
                        try
                        {
                            //one instance per class, shared by every ID it declares
                            effect ??= (Effect)Activator.CreateInstance(type, m_mod, m_client);
                            Effects[id] = effect;
                        }
                        catch (Exception e) { m_mod.Logger.LogError(e); }
                    }
                }
            }
            catch (Exception e) { m_mod.Logger.LogError(e); }
        }
    }

    /// <summary>Resolves an incoming request to an effect by its effect code.</summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="effect">The effect that should service the request.</param>
    /// <returns>True if an effect was found, false otherwise.</returns>
    public bool TryResolve(ConnectorLib.JSON.EffectRequest request, out Effect effect)
        => Effects.TryGetValue(request.code ?? string.Empty, out effect);

    /// <summary>Gets the types in an assembly, tolerating the ones that fail to load.</summary>
    private IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try { return assembly.GetTypes(); }
        catch (ReflectionTypeLoadException e)
        {
            m_mod.Logger.LogWarning($"Some types in \"{assembly.GetName().Name}\" could not be loaded, and the effects in them will be unavailable.");
            foreach (Exception? inner in e.LoaderExceptions.Take(5))
                if (inner != null) m_mod.Logger.LogWarning($"  {inner.Message}");
            return e.Types.Where(t => t != null)!;
        }
        catch (Exception e)
        {
            m_mod.Logger.LogError(e);
            return Array.Empty<Type>();
        }
    }
}
