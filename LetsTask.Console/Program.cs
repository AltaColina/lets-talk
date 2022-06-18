// See https://aka.ms/new-console-template for more information

using Grpc.Net.Client;
using LetsTalk;
using LetsTalkClient = LetsTalk.LetsTalk.LetsTalkClient;

var options = new GrpcChannelOptions
{
    HttpHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    }
};

using var channel = GrpcChannel.ForAddress("http://localhost:5219", options);

var client = new LetsTalkClient(channel);

// Register.
var person = default(Person);
while (person is null)
{
    Console.WriteLine("Please enter a unique username.");
    var username = Console.ReadLine();
    var registerResponse = await client.RegisterAsync(new RegisterRequest { Username = username });
    person = registerResponse.Person;
    if(person is null)
        Console.WriteLine($"User '{username}' already exists.");
}

Task.Run(async () =>
{
    var call = client.Join(person);
    while (await call.ResponseStream.MoveNext(CancellationToken.None))
    {
        var message = call.ResponseStream.Current;
        Console.WriteLine($"{message.Person.Username}: {message.Text}");
    }
})
    .ConfigureAwait(false)
    .GetAwaiter();

while (Console.ReadLine() is string line && line != "exit")
    await client.SendAsync(new Message { Person = person, Text = line });

await client.LeaveAsync(person);
