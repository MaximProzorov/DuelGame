using System.Security.Claims;

namespace Game.Infrastructure
{
    public static class ClaimsPrincipalExtentions
    {
        public static Guid GetPlayerId(this ClaimsPrincipal principal) =>
            Guid.Parse(principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
    }
}
