using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stackoverflow.Data;
using Stackoverflow.Repositories.Posts;
using Stackoverflow.Services.Posts;

var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("DefaultConnection");

// EF DbContext och Dapper connection
// behöver öka timeouten för komplexa queries timar ut utan index
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(cs, sqlOptions => sqlOptions.CommandTimeout(600)));
builder.Services.AddScoped<SqlConnection>(_ => new SqlConnection(cs));
// Repos
builder.Services.AddScoped<EfPostRepository>();
builder.Services.AddScoped<DapperPostRepository>();

// Services
builder.Services.AddScoped<EfPostService>(sp =>
    new EfPostService(sp.GetRequiredService<EfPostRepository>()));
builder.Services.AddScoped<DapperPostService>(sp =>
    new DapperPostService(sp.GetRequiredService<DapperPostRepository>()));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();


