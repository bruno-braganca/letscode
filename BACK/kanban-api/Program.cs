using kanban_api.Authentication;
using kanban_api.BusinessLayer;
using kanban_api.Context;
using kanban_api.Interfaces;
using kanban_api.Repository;
using kanban_api.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(config =>
{
    #region Adiciona Custom Exception e Log para capturar erros da Business Layer e gravar quando chama PUT e DELETE

    config.Filters.Add(typeof(CustomExceptionFilter));
    config.Filters.Add(typeof(LogFilter));

    #endregion
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Inje��o de depend�ncia da Business Layer

builder.Services.AddTransient<CardsBL>();
builder.Services.AddTransient<LoginBL>();

#endregion

#region Inje��o de depend�ncia do Repository

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

#endregion

#region Inje��o de depend�ncia do token jwt

var tokenConfigurations = new TokenConfigurations();
new ConfigureFromConfigurationOptions<TokenConfigurations>(
    builder.Configuration.GetSection("TokenConfigurations"))
            .Configure(tokenConfigurations);

builder.Services.AddSingleton(tokenConfigurations);

#endregion
#region Adi��o do contexto do Entity Framework e uso de banco de dados em mem�ria

builder.Services.AddDbContext<KanbanContext>(options =>
options.UseInMemoryDatabase("Kanban"));

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
