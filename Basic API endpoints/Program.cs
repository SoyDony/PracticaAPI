var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "root rooooooooot...");
app.MapGet("/hola amigo", () => "Hello dude!!");
app.MapPut("/put", () => $"Hello DONY!!");
app.MapDelete("/delete", () => $"deleteee!!");
app.MapPost("/post", () => $"POST!!");

app.Run();
