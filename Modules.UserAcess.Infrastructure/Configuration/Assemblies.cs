using System.Reflection;
using Modules.UserAcess.Application.Contracts;

namespace Modules.UserAcess.Infrastructure.Configuration;

internal static class Assemblies
{
    public static readonly Assembly Application = typeof(IUserAccessModule).Assembly;
}