using System.Reflection;

namespace LetsTalk.Security;

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

    public static IEnumerable<string> ReadOnly() => Values.Where(v => v.EndsWith(nameof(Room.Read), StringComparison.InvariantCultureIgnoreCase));

    public static IEnumerable<string> WriteOnly() => Values.Where(v => !v.EndsWith(nameof(Room.Read), StringComparison.InvariantCultureIgnoreCase));

    public static class Room
    {
        public const string Create = $"{nameof(Room)}:{nameof(Create)}";
        public const string Read = $"{nameof(Room)}:{nameof(Read)}";
        public const string Update = $"{nameof(Room)}:{nameof(Update)}";
        public const string Delete = $"{nameof(Room)}:{nameof(Delete)}";
        public static class User
        {
            public const string Create = $"{nameof(Room)}:{nameof(User)}:{nameof(Create)}";
            public const string Read = $"{nameof(Room)}:{nameof(User)}:{nameof(Read)}";
            public const string Update = $"{nameof(Room)}:{nameof(User)}:{nameof(Update)}";
            public const string Delete = $"{nameof(Room)}:{nameof(User)}:{nameof(Delete)}";
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

        public static class Room
        {
            public const string Create = $"{nameof(User)}:{nameof(Room)}:{nameof(Create)}";
            public const string Read = $"{nameof(User)}:{nameof(Room)}:{nameof(Read)}";
            public const string Update = $"{nameof(User)}:{nameof(Room)}:{nameof(Update)}";
            public const string Delete = $"{nameof(User)}:{nameof(Room)}:{nameof(Delete)}";
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