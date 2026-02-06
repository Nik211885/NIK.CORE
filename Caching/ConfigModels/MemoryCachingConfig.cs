using NIK.CORE.DOMAIN.Caching.Contracts;

namespace NIK.CORE.DOMAIN.Caching.ConfigModels;
/// <summary>
///     Represents configuration settings for the in-memory caching provider.
///     <para>
///     This configuration acts as a marker configuration and does not
///     require any additional settings.
///     </para>
/// </summary>
public class MemoryCachingConfig : ICachingConfig;
