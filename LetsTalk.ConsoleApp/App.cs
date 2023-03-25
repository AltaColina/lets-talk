using IdentityModel;
using IdentityModel.Client;
using LetsTalk.Rooms;
using LetsTalk.Security;
using LetsTalk.Services;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace LetsTalk;
internal sealed class App
{
    private static readonly IReadOnlyDictionary<string, string> CommandToContentTypeMap = typeof(LetsTalk.Messaging.MimeType.Image)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .ToDictionary(f => $"/{f.Name}", f => (string)f.GetValue(null)!, StringComparer.InvariantCultureIgnoreCase);

    private readonly HttpClient _identityClient;
    private readonly ILetsTalkHttpClient _webApiClient;
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly ILetsTalkHubClient _hubClient;
    private readonly MessageRecipient _messageRecipient;

    private DiscoveryDocumentResponse? DiscoveryDocument { get; set; }

    private TokenResponse? Token { get; set; }

    private string? UserId { get; set; }

    private string? UserName { get; set; }

    [MemberNotNullWhen(true, nameof(DiscoveryDocument), nameof(Token), nameof(UserId), nameof(UserName))]
    public bool IsAuthenticated { get; private set; }

    public RoomDto? Room { get; private set; }

    [MemberNotNullWhen(true, nameof(DiscoveryDocument), nameof(Token), nameof(UserId), nameof(UserName), nameof(Room))]
    public bool HasJoinedRoom { get; private set; }

    public App(IHttpClientFactory httpClientFactory, IAccessTokenProvider accessTokenProvider, ILetsTalkHttpClient webApiClient, ILetsTalkHubClient hubClient, MessageRecipient messageRecipient)
    {
        _identityClient = httpClientFactory.CreateClient("Identity");
        _webApiClient = webApiClient;
        _accessTokenProvider = accessTokenProvider;
        _hubClient = hubClient;
        _messageRecipient = messageRecipient;
    }

    public async Task LoginAsync()
    {
        DiscoveryDocument = await _identityClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Policy = { ValidateIssuerName = false }
        });
        if (DiscoveryDocument.IsError)
            throw DiscoveryDocument.Exception ?? new InvalidOperationException(DiscoveryDocument.Error);

        var userName = default(string);
        while (userName is null)
        {
            Console.Write("Username: ");
            userName = Console.ReadLine()!;
            if (!RegexValidation.UserName.IsMatch(userName))
            {
                Console.WriteLine("Invalid username");
                userName = null!;
                continue;
            }


            var loginResponse = await _identityClient.RequestBackchannelAuthenticationAsync(new BackchannelAuthenticationRequest
            {
                Address = DiscoveryDocument.BackchannelAuthenticationEndpoint,
                ClientId = "console",
                ClientSecret = "secret",
                Scope = "openid profile letstalk",
                LoginHint = userName,
                //IdTokenHint = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkYyNjZCQzA3NTFBNjIyNDkzMzFDMzI4QUQ1RkIwMkJGIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo1MDAxIiwibmJmIjoxNjM4NDc3MDE2LCJpYXQiOjE2Mzg0NzcwMTYsImV4cCI6MTYzODQ3NzMxNiwiYXVkIjoiY2liYSIsImFtciI6WyJwd2QiXSwiYXRfaGFzaCI6ImE1angwelVQZ2twczBVS1J5VjBUWmciLCJzaWQiOiIzQTJDQTJDNjdBNTAwQ0I2REY1QzEyRUZDMzlCQTI2MiIsInN1YiI6IjgxODcyNyIsImF1dGhfdGltZSI6MTYzODQ3NzAwOCwiaWRwIjoibG9jYWwifQ.GAIHXYgEtXw5NasR0zPMW3jSKBuWujzwwnXJnfHdulKX-I3r47N0iqHm5v5V0xfLYdrmntjLgmdm0DSvdXswtZ1dh96DqS1zVm6yQ2V0zsA2u8uOt1RG8qtjd5z4Gb_wTvks4rbUiwi008FOZfRuqbMJJDSscy_YdEJqyQahdzkcUnWZwdbY8L2RUTxlAAWQxktpIbaFnxfr8PFQpyTcyQyw0b7xmYd9ogR7JyOff7IJIHPDur0wbRdpI1FDE_VVCgoze8GVAbVxXPtj4CtWHAv07MJxa9SdA_N-lBcrZ3PHTKQ5t1gFXwdQvp3togUJl33mJSru3lqfK36pn8y8ow",
                BindingMessage = Guid.NewGuid().ToString("N")[..10],
                RequestedExpiry = 200
            });

            if (loginResponse.IsError)
                throw loginResponse.Exception ?? new InvalidOperationException(loginResponse.Error);

            var openBrowser = true;
            do
            {
                Token = await _identityClient.RequestBackchannelAuthenticationTokenAsync(new BackchannelAuthenticationTokenRequest
                {
                    Address = DiscoveryDocument.TokenEndpoint,
                    ClientId = "console",
                    ClientSecret = "secret",
                    AuthenticationRequestId = loginResponse.AuthenticationRequestId
                });

                if (Token.IsError)
                {
                    if (Token.Error is OidcConstants.TokenErrors.AuthorizationPending or OidcConstants.TokenErrors.SlowDown)
                    {
                        if (openBrowser)
                        {
                            openBrowser = false;
                            OpenBrowserAsync(_identityClient.BaseAddress + "Account/Login?ReturnUrl=%2Fciba%2Fall").ConfigureAwait(false).GetAwaiter();
                        }

                        Console.WriteLine($"{Token.Error}... Waiting {loginResponse.Interval!.Value} seconds until polling again.");
                        await Task.Delay(loginResponse.Interval!.Value * 1000);
                        Token = null;
                    }
                    else
                    {
                        throw Token.Exception ?? new InvalidOperationException(Token.Error);
                    }
                }
            }
            while (Token is null);
            var token = new JsonWebTokenHandler().ReadJsonWebToken(Token.AccessToken);
            UserId = token.Subject;
            UserName = token.GetClaim(JwtClaimTypes.Name).Value;
            IsAuthenticated = true;
            ((AccessTokenProvider)_accessTokenProvider).AccessToken = Token.AccessToken;
        }

        static Task OpenBrowserAsync(string url)
        {
            using var process = CreateProcess(url);

            return process.WaitForExitAsync();

            static Process CreateProcess(string url)
            {

                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    return Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true })!;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return Process.Start("open", url);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
    }

    public Task<string> PingWebApi()
    {
        if (!IsAuthenticated)
            throw new InvalidOperationException("Not authenticated");
        return _webApiClient.PingAsync();
    }

    public Task ConnectToHubAsync()
    {
        if (!IsAuthenticated)
            throw new InvalidOperationException("Not authenticated");
        return _hubClient.ConnectAsync();
    }

    public Task DisconnectFromHubAsync()
    {
        if (!_hubClient.IsConnected)
            throw new InvalidOperationException("Not connected to the hub");
        return _hubClient.ConnectAsync();
    }

    public async Task JoinChatRoomAsync()
    {
        if (HasJoinedRoom)
            throw new InvalidOperationException("Already joined a room");
        var rooms = (await _hubClient.GetRoomsWithUserAsync()).Rooms;
        Room = null;
        while (Room is null)
        {
            Console.WriteLine("Select a channel to listen to by typing its number.");
            int i = 0;
            for (; i < rooms.Count; ++i)
                Console.WriteLine($"{i}: {rooms[i].Name}");
            Console.WriteLine($"{rooms.Count}: Join another channel.");
            var roomIndex = Console.ReadLine();
            if (!Int32.TryParse(roomIndex, out int number))
            {
                Console.WriteLine("Input must be a number.");
            }
            else if (number < 0 || number > rooms.Count)
            {
                Console.WriteLine($"Must be a number between {0} and {rooms.Count}.");
            }
            else if (number == rooms.Count)
            {
                var allRooms = (await _hubClient.GetRoomsWithoutUserAsync()).Rooms;
                Console.WriteLine("Available rooms:");
                foreach (var item in allRooms)
                    Console.WriteLine($"- {item.Name} (id: {item.Id})");
                Console.WriteLine("Select room to join. Type '/back' to go return to previous menu.");
                Console.Write("Room Id: ");
                var roomId = Console.ReadLine()!;
                if (roomId != "/back")
                {
                    var newRoom = await _webApiClient.GetRoomAsync(roomId);
                    rooms.Add(newRoom);
                    await _hubClient.JoinRoomAsync(roomId);
                }
            }
            else
            {
                Room = rooms[number];
            }
        }
        HasJoinedRoom = true;
        var users = (await _hubClient.GetUsersLoggedInRoomAsync(Room.Id)).Users;
        users.RemoveAll(user => user.Id == UserId);
        Console.WriteLine($"Listening to room {Room.Name} (id: {Room.Id}). Logged users:");
        foreach (var item in users)
            Console.WriteLine($"- {item.Name} (id: {item.Id})");
    }

    public async Task SendAndReceiveMessagesAsync()
    {
        if (!HasJoinedRoom)
            throw new InvalidOperationException("Not in a room");

        static void Recipient_MessageReceived(object? sender, string message) => Console.WriteLine(message);
        _messageRecipient.AddMessageListener(Room.Id);
        _messageRecipient.MessageReceived += Recipient_MessageReceived;
        // Send messages.
        Console.WriteLine("You can start writing messages. Type '/leave' to leave the current room.");
        while (Console.ReadLine() is string message && message != "/leave")
        {
            var (left, top) = Console.GetCursorPosition();
            if (!message.StartsWith("/"))
            {
                Console.SetCursorPosition(left, top - 1);
                await _hubClient.SendContentMessageAsync(Room.Id, MediaTypeNames.Text.Plain, Encoding.UTF8.GetBytes(message));
            }
            else if (message.IndexOf(' ') is var index && index >= 0)
            {
                var command = message[..index];
                if (CommandToContentTypeMap.TryGetValue(command, out var contentType))
                {
                    var content = message[(index + 1)..].Trim('"');
                    await _hubClient.SendContentMessageAsync(Room.Id, contentType, File.ReadAllBytes(content));
                }
                else
                {
                    Console.WriteLine("invalid command");
                }
            }
            else
            {
                Console.WriteLine("invalid message");
            }
        }

        _messageRecipient.MessageReceived -= Recipient_MessageReceived;
    }
}
