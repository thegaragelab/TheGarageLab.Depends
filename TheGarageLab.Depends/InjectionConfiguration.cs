namespace TheGarageLab.Depends
{
    /// <summary>
    /// Represents a single injection configuration for the resolver.
    /// 
    /// Instances of this class can be read from an external file and
    /// passed to a Resolver instance for configuration. This only
    /// supports mapping a type to an implementation, it does not
    /// support manually created singletons or factory functions.
    /// 
    /// The Target and Implementation fields are strings suitable
    /// for passing to the Type.GetType() method. This is the qualified
    /// type name optionally followed by a comma and the name of the
    /// assembly that contains it.
    /// </summary>
    public class InjectionConfiguration
    {
        /// <summary>
        /// The target of the injection. This is the interface or
        /// class which will be injected.
        /// </summary>
        public string Target;

        /// <summary>
        /// The implementation to use for the target. This is the
        /// class which will be created when the target is resolved.
        /// </summary>
        public string Implementation;

        /// <summary>
        /// The lifetime of the instance. Singletons will be created
        /// in a lazily.
        /// </summary>
        public Lifetime Lifetime = Lifetime.Transient;
    }
}
