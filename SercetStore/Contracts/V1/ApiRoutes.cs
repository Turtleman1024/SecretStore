namespace SecretStore.Contracts.V1
{
    public class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = Root + "/" + Version;

        public static class Entries
        {

            public const string GetPasswordEntries = Base + "/entries";
            public const string GetPasswordEntryById = Base + "/entry/{entryId}";
            public const string CreatePasswordEntry = Base + "/entry";
            public const string UpdatePasswordEntry = Base + "/entry/{entryId}";
            public const string DeletePasswordEntry = Base + "/entry/remove/{entryId}";
            public const string SearchForPasswordEntry = Base + "/entry/search-owner/{searchValue}";
        }
    }
}
