using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MatricsMC
{
    public class MinecraftAuthenticationResult
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; } = "";

        public string MicrosoftAccessToken { get; set; } = "";

        public string MinecraftAccessToken { get; set; } = "";

        public string MicrosoftUserId { get; set; } = "";

        public string MinecraftUuid { get; set; } = "";

        public string MinecraftUsername { get; set; } = "";

        public bool OwnsMinecraft { get; set; }
    }

    public static class MicrosoftMinecraftAuthService
    {
        private const string ClientId =
            "00000000402b5328";

        private const string RedirectUri =
            "https://login.live.com/oauth20_desktop.srf";

        private const string Scope =
            "service::user.auth.xboxlive.com::MBI_SSL";

        private static readonly HttpClient Http =
            new HttpClient();

        public static string GetLoginUrl()
        {
            return
                "https://login.live.com/oauth20_authorize.srf" +
                "?client_id=" +
                Uri.EscapeDataString(ClientId) +
                "&response_type=code" +
                "&redirect_uri=" +
                Uri.EscapeDataString(RedirectUri) +
                "&scope=" +
                Uri.EscapeDataString(Scope);
        }

        public static void OpenLoginPage()
        {
            Process.Start(
                new ProcessStartInfo
                {
                    FileName = GetLoginUrl(),
                    UseShellExecute = true
                });
        }

        public static async Task<MinecraftAuthenticationResult>
            AuthenticateFromRedirectUrlAsync(
                string redirectUrl)
        {
            try
            {
                string? code =
                    ExtractQueryParameter(
                        redirectUrl,
                        "code");

                if (string.IsNullOrWhiteSpace(code))
                {
                    string? error =
                        ExtractQueryParameter(
                            redirectUrl,
                            "error_description");

                    return Fail(
                        string.IsNullOrWhiteSpace(error)
                            ? "Microsoft did not return an authorization code."
                            : Uri.UnescapeDataString(error));
                }

                TokenResponse microsoftToken =
                    await ExchangeAuthorizationCodeAsync(
                        code);

                if (string.IsNullOrWhiteSpace(
                    microsoftToken.AccessToken))
                {
                    return Fail(
                        "Microsoft did not return an access token.");
                }

                XboxTokenResponse xbox =
                    await AuthenticateXboxLiveAsync(
                        microsoftToken.AccessToken);

                if (string.IsNullOrWhiteSpace(
                    xbox.Token))
                {
                    return Fail(
                        "Xbox Live did not return an authentication token.");
                }

                string userHash =
                    ExtractUserHash(
                        xbox.DisplayClaims);

                if (string.IsNullOrWhiteSpace(userHash))
                {
                    return Fail(
                        "Xbox Live did not return a user hash.");
                }

                XboxTokenResponse xsts =
                    await AuthenticateXstsAsync(
                        xbox.Token);

                if (string.IsNullOrWhiteSpace(
                    xsts.Token))
                {
                    return Fail(
                        "Xbox XSTS did not return a token.");
                }

                string xstsUserHash =
                    ExtractUserHash(
                        xsts.DisplayClaims);

                if (!string.Equals(
                    userHash,
                    xstsUserHash,
                    StringComparison.Ordinal))
                {
                    return Fail(
                        "Xbox Live returned an unexpected user identity.");
                }

                MinecraftLoginResponse minecraft =
                    await LoginToMinecraftAsync(
                        userHash,
                        xsts.Token);

                if (string.IsNullOrWhiteSpace(
                    minecraft.AccessToken))
                {
                    return Fail(
                        "Minecraft Services did not return a Minecraft access token.");
                }

                bool ownsMinecraft =
                    await CheckMinecraftOwnershipAsync(
                        minecraft.AccessToken);

                if (!ownsMinecraft)
                {
                    return Fail(
                        "This Microsoft account does not have a Minecraft entitlement.");
                }

                MinecraftProfileResponse profile =
                    await GetMinecraftProfileAsync(
                        minecraft.AccessToken);

                if (string.IsNullOrWhiteSpace(
                    profile.Id) ||
                    string.IsNullOrWhiteSpace(
                    profile.Name))
                {
                    return Fail(
                        "Minecraft Services did not return a Minecraft profile.");
                }

                return new MinecraftAuthenticationResult
                {
                    Success = true,
                    MicrosoftAccessToken =
                        microsoftToken.AccessToken,
                    MinecraftAccessToken =
                        minecraft.AccessToken,
                    MicrosoftUserId =
                        microsoftToken.UserId,
                    MinecraftUuid =
                        profile.Id,
                    MinecraftUsername =
                        profile.Name,
                    OwnsMinecraft = true
                };
            }
            catch (Exception ex)
            {
                return Fail(
                    "Microsoft/Minecraft authentication failed.\n\n" +
                    ex.Message);
            }
        }

        private static async Task<TokenResponse>
            ExchangeAuthorizationCodeAsync(
                string code)
        {
            Dictionary<string, string> values =
                new()
                {
                    ["client_id"] = ClientId,
                    ["code"] = code,
                    ["grant_type"] = "authorization_code",
                    ["redirect_uri"] = RedirectUri,
                    ["scope"] = Scope
                };

            using FormUrlEncodedContent content =
                new(values);

            using HttpResponseMessage response =
                await Http.PostAsync(
                    "https://login.live.com/oauth20_token.srf",
                    content);

            string json =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Microsoft token request failed ({(int)response.StatusCode}).\n\n{json}");
            }

            TokenResponse? result =
                JsonSerializer.Deserialize<TokenResponse>(
                    json,
                    JsonOptions());

            return result ?? new TokenResponse();
        }

        private static async Task<XboxTokenResponse>
            AuthenticateXboxLiveAsync(
                string microsoftAccessToken)
        {
            object body =
                new
                {
                    Properties = new
                    {
                        AuthMethod = "RPS",
                        SiteName = "user.auth.xboxlive.com",
                        RpsTicket =
                            "d=" + microsoftAccessToken
                    },
                    RelyingParty =
                        "http://auth.xboxlive.com",
                    TokenType = "JWT"
                };

            return await PostJsonAsync<XboxTokenResponse>(
                "https://user.auth.xboxlive.com/user/authenticate",
                body);
        }

        private static async Task<XboxTokenResponse>
            AuthenticateXstsAsync(
                string xboxToken)
        {
            object body =
                new
                {
                    Properties = new
                    {
                        SandboxId = "RETAIL",
                        UserTokens = new[]
                        {
                            xboxToken
                        }
                    },
                    RelyingParty =
                        "rp://api.minecraftservices.com/",
                    TokenType = "JWT"
                };

            return await PostJsonAsync<XboxTokenResponse>(
                "https://xsts.auth.xboxlive.com/xsts/authorize",
                body);
        }

        private static async Task<MinecraftLoginResponse>
            LoginToMinecraftAsync(
                string userHash,
                string xstsToken)
        {
            object body =
                new
                {
                    identityToken =
                        $"XBL3.0 x={userHash};{xstsToken}"
                };

            return await PostJsonAsync<MinecraftLoginResponse>(
                "https://api.minecraftservices.com/authentication/login_with_xbox",
                body);
        }

        private static async Task<bool>
            CheckMinecraftOwnershipAsync(
                string minecraftAccessToken)
        {
            using HttpRequestMessage request =
                new(
                    HttpMethod.Get,
                    "https://api.minecraftservices.com/entitlements/mcstore");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    minecraftAccessToken);

            using HttpResponseMessage response =
                await Http.SendAsync(request);

            string json =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return false;

            using JsonDocument document =
                JsonDocument.Parse(json);

            if (!document.RootElement.TryGetProperty(
                "items",
                out JsonElement items))
            {
                return false;
            }

            if (items.ValueKind != JsonValueKind.Array)
                return false;

            foreach (JsonElement item in items.EnumerateArray())
            {
                if (!item.TryGetProperty(
                    "name",
                    out JsonElement name))
                {
                    continue;
                }

                string entitlementName =
                    name.GetString() ?? "";

                if (entitlementName.Equals(
                        "product_minecraft",
                        StringComparison.OrdinalIgnoreCase) ||
                    entitlementName.Equals(
                        "game_minecraft",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static async Task<MinecraftProfileResponse>
            GetMinecraftProfileAsync(
                string minecraftAccessToken)
        {
            using HttpRequestMessage request =
                new(
                    HttpMethod.Get,
                    "https://api.minecraftservices.com/minecraft/profile");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    minecraftAccessToken);

            using HttpResponseMessage response =
                await Http.SendAsync(request);

            string json =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Minecraft profile request failed ({(int)response.StatusCode}).\n\n{json}");
            }

            MinecraftProfileResponse? profile =
                JsonSerializer.Deserialize<MinecraftProfileResponse>(
                    json,
                    JsonOptions());

            return profile ?? new MinecraftProfileResponse();
        }

        private static async Task<T>
            PostJsonAsync<T>(
                string url,
                object body)
        {
            string json =
                JsonSerializer.Serialize(body);

            using StringContent content =
                new(
                    json,
                    Encoding.UTF8,
                    "application/json");

            using HttpResponseMessage response =
                await Http.PostAsync(
                    url,
                    content);

            string responseJson =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Authentication request failed ({(int)response.StatusCode}).\n\n{responseJson}");
            }

            T? result =
                JsonSerializer.Deserialize<T>(
                    responseJson,
                    JsonOptions());

            return result
                ?? throw new InvalidOperationException(
                    "The authentication server returned an empty response.");
        }

        private static string ExtractUserHash(
            Dictionary<string, List<Dictionary<string, string>>> claims)
        {
            if (!claims.TryGetValue(
                "xui",
                out List<Dictionary<string, string>>? users))
            {
                return "";
            }

            if (users.Count == 0)
                return "";

            if (!users[0].TryGetValue(
                "uhs",
                out string? hash))
            {
                return "";
            }

            return hash;
        }

        private static string? ExtractQueryParameter(
            string url,
            string parameter)
        {
            try
            {
                Uri uri =
                    new(url);

                string query =
                    uri.Query.TrimStart('?');

                foreach (string pair in query.Split('&'))
                {
                    if (string.IsNullOrWhiteSpace(pair))
                        continue;

                    string[] parts =
                        pair.Split(
                            '=',
                            2);

                    if (parts.Length != 2)
                        continue;

                    if (!parts[0].Equals(
                        parameter,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    return Uri.UnescapeDataString(
                        parts[1].Replace(
                            "+",
                            " "));
                }
            }
            catch
            {
                // The caller receives an authentication error.
            }

            return null;
        }

        private static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private static MinecraftAuthenticationResult Fail(
            string message)
        {
            return new MinecraftAuthenticationResult
            {
                Success = false,
                ErrorMessage = message
            };
        }

        private class TokenResponse
        {
            public string AccessToken { get; set; } = "";

            public string RefreshToken { get; set; } = "";

            public string UserId { get; set; } = "";

            public string TokenType { get; set; } = "";

            public int ExpiresIn { get; set; }
        }

        private class XboxTokenResponse
        {
            public string Token { get; set; } = "";

            public Dictionary<string, List<Dictionary<string, string>>>
                DisplayClaims { get; set; } = new();
        }

        private class MinecraftLoginResponse
        {
            public string AccessToken { get; set; } = "";

            public string TokenType { get; set; } = "";

            public int ExpiresIn { get; set; }
        }

        private class MinecraftProfileResponse
        {
            public string Id { get; set; } = "";

            public string Name { get; set; } = "";
        }
    }
}