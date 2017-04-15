namespace TheGarageLab.Depends
{
    /// <summary>
    /// Object lifetime
    /// </summary>
    public enum Lifetime
    {
        /// <summary>
        /// A transient object. A new instance is created on demand for every request.
        /// </summary>
        Transient,
        /// <summary>
        /// A singleton instance. A single instance is used for all requests.
        /// </summary>
        Singleton
    }
}
