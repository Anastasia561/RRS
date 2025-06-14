using Microsoft.EntityFrameworkCore;
using RRS.Data;
using RRS.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<DatabaseContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    opt.UseSqlServer(connectionString);
});
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IIndividualService, IndividualService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();