using System.Threading.Tasks;
using Duende.IdentityServer.Contrib.RedisStore;
using Duende.IdentityServer.Models;

namespace Duende.IdentityServer.Services
{
    /// <summary>
    /// Caching decorator for IProfileService
    /// </summary>
    /// <seealso cref="Duende.IdentityServer.Services.IProfileService" />
    public class CachingProfileService<TProfileService> : IProfileService
    where TProfileService : class, IProfileService
    {
        private readonly TProfileService inner;

        private readonly ICache<IsActiveContextCacheEntry> cache;

        private readonly ProfileServiceCachingOptions<TProfileService> options;

        public CachingProfileService(TProfileService inner, ICache<IsActiveContextCacheEntry> cache, ProfileServiceCachingOptions<TProfileService> options)
        {
            this.inner = inner;
            this.cache = cache;
            this.options = options;
        }

        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await this.inner.GetProfileDataAsync(context);
        }

        /// <summary>
        /// This method gets called whenever identity server needs to determine if the user is valid or active (e.g. if the user's account has been deactivated since they logged in).
        /// (e.g. during token issuance or validation).
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var key = $"{options.KeyPrefix}{options.KeySelector(context)}";

            if (options.ShouldCache(context))
            {
                var entry = await cache.GetOrAddAsync(key, options.Expiration,
                              async () =>
                              {
                                  await inner.IsActiveAsync(context);
                                  return new IsActiveContextCacheEntry { IsActive = context.IsActive };
                              });

                context.IsActive = entry.IsActive;
            }
            else
            {
                await inner.IsActiveAsync(context);
            }
        }
    }

    /// <summary>
    /// Represents cache entry for IsActiveContext
    /// </summary>
    public class IsActiveContextCacheEntry
    {
        public bool IsActive { get; set; }
    }
}