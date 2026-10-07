using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MatricsMC
{
    public class MatricsAccount
    {
        public string Id { get; set; } =
            Guid.NewGuid().ToString();

        public string Name { get; set; } = "";

        public string Type { get; set; } = "Offline";

        public string MinecraftUuid { get; set; } = "";

        public string MicrosoftUserId { get; set; } = "";

        public bool IsLoaded { get; set; }

        public bool IsOfficial
        {
            get
            {
                return Type.Equals(
                    "Microsoft",
                    StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    public class AccountData
    {
        public List<MatricsAccount> Accounts { get; set; } =
            new();

        public string LoadedAccountId { get; set; } = "";
    }

    public static class AccountManager
    {
        private static readonly string MatricsDirectory =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                ".matricsmc");

        private static readonly string AccountsFile =
            Path.Combine(
                MatricsDirectory,
                "accounts.json");

        public static AccountData Data { get; private set; } =
            new();

        public static MatricsAccount? LoadedAccount
        {
            get
            {
                if (string.IsNullOrWhiteSpace(
                    Data.LoadedAccountId))
                {
                    return null;
                }

                return Data.Accounts.FirstOrDefault(
                    account =>
                        account.Id ==
                        Data.LoadedAccountId);
            }
        }

        public static bool HasOfficialAccount
        {
            get
            {
                return Data.Accounts.Any(
                    account => account.IsOfficial);
            }
        }

        public static void Initialize()
        {
            Directory.CreateDirectory(
                MatricsDirectory);

            Load();

            SetLoadedState();
        }

        public static void Load()
        {
            try
            {
                if (!File.Exists(AccountsFile))
                {
                    Data = new AccountData();
                    return;
                }

                string json =
                    File.ReadAllText(
                        AccountsFile);

                AccountData? loaded =
                    JsonSerializer.Deserialize<AccountData>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                Data =
                    loaded ??
                    new AccountData();

                if (Data.Accounts == null)
                    Data.Accounts =
                        new List<MatricsAccount>();
            }
            catch
            {
                Data = new AccountData();
            }

            SetLoadedState();
        }

        public static void Save()
        {
            Directory.CreateDirectory(
                MatricsDirectory);

            string json =
                JsonSerializer.Serialize(
                    Data,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

            File.WriteAllText(
                AccountsFile,
                json);
        }

        public static MatricsAccount AddMicrosoftAccount(
            string username,
            string minecraftUuid,
            string microsoftUserId)
        {
            username =
                username.Trim();

            MatricsAccount? existing =
                Data.Accounts.FirstOrDefault(
                    account =>
                        account.IsOfficial &&
                        (
                            (
                                !string.IsNullOrWhiteSpace(
                                    minecraftUuid) &&
                                account.MinecraftUuid.Equals(
                                    minecraftUuid,
                                    StringComparison.OrdinalIgnoreCase)
                            )
                            ||
                            account.Name.Equals(
                                username,
                                StringComparison.OrdinalIgnoreCase)
                        ));

            if (existing != null)
            {
                existing.Name =
                    username;

                existing.MinecraftUuid =
                    minecraftUuid;

                existing.MicrosoftUserId =
                    microsoftUserId;

                existing.Type =
                    "Microsoft";

                Save();

                return existing;
            }

            MatricsAccount account =
                new()
                {
                    Id =
                        Guid.NewGuid().ToString(),

                    Name =
                        username,

                    Type =
                        "Microsoft",

                    MinecraftUuid =
                        minecraftUuid,

                    MicrosoftUserId =
                        microsoftUserId,

                    IsLoaded =
                        false
                };

            Data.Accounts.Add(
                account);

            Save();

            return account;
        }

        public static MatricsAccount AddOfflineAccount(
            string username)
        {
            if (!HasOfficialAccount)
            {
                throw new InvalidOperationException(
                    "Offline profiles are locked until a legitimate Minecraft account has been connected.");
            }

            username =
                username.Trim();

            MatricsAccount? existing =
                Data.Accounts.FirstOrDefault(
                    account =>
                        !account.IsOfficial &&
                        account.Name.Equals(
                            username,
                            StringComparison.OrdinalIgnoreCase));

            if (existing != null)
                return existing;

            MatricsAccount account =
                new()
                {
                    Id =
                        Guid.NewGuid().ToString(),

                    Name =
                        username,

                    Type =
                        "Offline",

                    IsLoaded =
                        false
                };

            Data.Accounts.Add(
                account);

            Save();

            return account;
        }

        public static bool RemoveAccount(
            string accountId)
        {
            MatricsAccount? account =
                Data.Accounts.FirstOrDefault(
                    a => a.Id == accountId);

            if (account == null)
                return false;

            bool wasLoaded =
                Data.LoadedAccountId ==
                accountId;

            Data.Accounts.Remove(
                account);

            if (wasLoaded)
            {
                Data.LoadedAccountId = "";

                MatricsAccount? next =
                    Data.Accounts.FirstOrDefault();

                if (next != null)
                {
                    Data.LoadedAccountId =
                        next.Id;
                }
            }

            SetLoadedState();
            Save();

            return true;
        }

        public static bool LoadAccount(
            string accountId)
        {
            MatricsAccount? account =
                Data.Accounts.FirstOrDefault(
                    a => a.Id == accountId);

            if (account == null)
                return false;

            Data.LoadedAccountId =
                account.Id;

            SetLoadedState();
            Save();

            return true;
        }

        public static bool LoadAccount(
            MatricsAccount account)
        {
            if (account == null)
                return false;

            return LoadAccount(
                account.Id);
        }

        private static void SetLoadedState()
        {
            foreach (MatricsAccount account
                in Data.Accounts)
            {
                account.IsLoaded =
                    account.Id ==
                    Data.LoadedAccountId;
            }
        }

        public static string GetAccountsFilePath()
        {
            return AccountsFile;
        }

        public static IReadOnlyList<MatricsAccount>
            GetAccounts()
        {
            return Data.Accounts;
        }
    }
}