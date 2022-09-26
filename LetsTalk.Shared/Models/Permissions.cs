namespace LetsTalk.Models;

public static class Permissions
{
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