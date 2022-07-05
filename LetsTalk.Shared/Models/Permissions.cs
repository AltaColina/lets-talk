namespace LetsTalk.Models;

public static class Permissions
{
    public static class Chat
    {
        public const string View = $"{nameof(Permissions)}.{nameof(Chat)}.{nameof(View)}";
        public const string Create = $"{nameof(Permissions)}.{nameof(Chat)}.{nameof(Create)}";
        public const string Edit = $"{nameof(Permissions)}.{nameof(Chat)}.{nameof(Edit)}";
        public const string Delete = $"{nameof(Permissions)}.{nameof(Chat)}.{nameof(Delete)}";
    }

    public static class Role
    {
        public const string View = $"{nameof(Permissions)}.{nameof(Role)}.{nameof(View)}";
        public const string Create = $"{nameof(Permissions)}.{nameof(Role)}.{nameof(Create)}";
        public const string Edit = $"{nameof(Permissions)}.{nameof(Role)}.{nameof(Edit)}";
        public const string Delete = $"{nameof(Permissions)}.{nameof(Role)}.{nameof(Delete)}";
        public static class User
        {
            public const string View = $"{nameof(Permissions)}.{nameof(Role)}.{nameof(User)}.{nameof(View)}";
            public const string Create = $"{nameof(Permissions)}.{nameof(Role)}.{nameof(User)}.{nameof(Create)}";
            public const string Edit = $"{nameof(Permissions)}.{nameof(Role)}.{nameof(User)}.{nameof(Edit)}";
            public const string Delete = $"{nameof(Permissions)}.{nameof(Role)}.{nameof(User)}.{nameof(Delete)}";
        }
    }

    public static class User
    {
        public const string View = $"{nameof(Permissions)}.{nameof(User)}.{nameof(View)}";
        public const string Create = $"{nameof(Permissions)}.{nameof(User)}.{nameof(Create)}";
        public const string Edit = $"{nameof(Permissions)}.{nameof(User)}.{nameof(Edit)}";
        public const string Delete = $"{nameof(Permissions)}.{nameof(User)}.{nameof(Delete)}";
        public static class Role
        {
            public const string View = $"{nameof(Permissions)}.{nameof(User)}.{nameof(Role)}.{nameof(View)}";
            public const string Create = $"{nameof(Permissions)}.{nameof(User)}.{nameof(Role)}.{nameof(Create)}";
            public const string Edit = $"{nameof(Permissions)}.{nameof(User)}.{nameof(Role)}.{nameof(Edit)}";
            public const string Delete = $"{nameof(Permissions)}.{nameof(User)}.{nameof(Role)}.{nameof(Delete)}";
        }
    }
}