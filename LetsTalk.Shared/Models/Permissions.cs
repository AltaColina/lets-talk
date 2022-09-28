using System.Reflection;

namespace LetsTalk.Models;

public static class Permissions
{
    private static IReadOnlyCollection<string> Values { get; } = new List<string>(GetStaticFieldValues(typeof(Permissions)));

    private static IEnumerable<string> GetStaticFieldValues(Type type, string prefix = "")
    {
        prefix += $"{type.Name}.";

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            yield return (string)field.GetValue(null)!;

        foreach (var nestedType in type.GetNestedTypes())
            foreach (var value in GetStaticFieldValues(nestedType, prefix))
                yield return value;
    }

    public static bool IsPermission(string permission) => Values.Contains(permission, StringComparer.InvariantCultureIgnoreCase);

    public static IEnumerable<string> All() => Values;

    public static IEnumerable<string> ReadOnly() => Values.Where(v => v.EndsWith(nameof(Chat.Read), StringComparison.InvariantCultureIgnoreCase));

    public static IEnumerable<string> WriteOnly() => Values.Where(v => !v.EndsWith(nameof(Chat.Read), StringComparison.InvariantCultureIgnoreCase));

    public static class Chat
    {
        public const string Create = $"{nameof(Chat)}:{nameof(Create)}";
        public const string Read = $"{nameof(Chat)}:{nameof(Read)}";
        public const string Update = $"{nameof(Chat)}:{nameof(Update)}";
        public const string Delete = $"{nameof(Chat)}:{nameof(Delete)}";
        public static class User
        {
            public const string Create = $"{nameof(Chat)}:{nameof(User)}:{nameof(Create)}";
            public const string Read = $"{nameof(Chat)}:{nameof(User)}:{nameof(Read)}";
            public const string Update = $"{nameof(Chat)}:{nameof(User)}:{nameof(Update)}";
            public const string Delete = $"{nameof(Chat)}:{nameof(User)}:{nameof(Delete)}";
        }
    }

    public static class Role
    {
        public const string Create = $"{nameof(Role)}:{nameof(Create)}";
        public const string Read = $"{nameof(Role)}:{nameof(Read)}";
        public const string Update = $"{nameof(Role)}:{nameof(Update)}";
        public const string Delete = $"{nameof(Role)}:{nameof(Delete)}";
        public static class User
        {
            public const string Create = $"{nameof(Role)}:{nameof(User)}:{nameof(Create)}";
            public const string Read = $"{nameof(Role)}:{nameof(User)}:{nameof(Read)}";
            public const string Update = $"{nameof(Role)}:{nameof(User)}:{nameof(Update)}";
            public const string Delete = $"{nameof(Role)}:{nameof(User)}:{nameof(Delete)}";
        }
    }

    public static class User
    {
        public const string Create = $"{nameof(User)}:{nameof(Create)}";
        public const string Read = $"{nameof(User)}:{nameof(Read)}";
        public const string Update = $"{nameof(User)}:{nameof(Update)}";
        public const string Delete = $"{nameof(User)}:{nameof(Delete)}";

        public static class Chat
        {
            public const string Create = $"{nameof(User)}:{nameof(Chat)}:{nameof(Create)}";
            public const string Read = $"{nameof(User)}:{nameof(Chat)}:{nameof(Read)}";
            public const string Update = $"{nameof(User)}:{nameof(Chat)}:{nameof(Update)}";
            public const string Delete = $"{nameof(User)}:{nameof(Chat)}:{nameof(Delete)}";
        }

        public static class Role
        {
            public const string Create = $"{nameof(User)}:{nameof(Role)}:{nameof(Create)}";
            public const string Read = $"{nameof(User)}:{nameof(Role)}:{nameof(Read)}";
            public const string Update = $"{nameof(User)}:{nameof(Role)}:{nameof(Update)}";
            public const string Delete = $"{nameof(User)}:{nameof(Role)}:{nameof(Delete)}";
        }
    }
}