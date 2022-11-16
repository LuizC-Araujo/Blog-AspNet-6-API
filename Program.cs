using Blog.Data;
using Blog.Services;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddDbContext<BlogDataContext>();
builder.Services.AddTransient<TokenService>(); // sempre cria ym novo
// builder.Services.AddScoped(); // transação - reaproveita
// builder.Services.AddSingleton(); // 1 por app

var app = builder.Build();

app.MapControllers();

app.Run();
