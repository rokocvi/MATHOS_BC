using BootcampApp.Repository.Interfaces;
using BootcampApp.Repository;
using BootcampApp.Service.Interfaces;
using BootcampApp.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Dodaj servise PRIJED Build()
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Ako imaš connection string u appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Registracija repozitorija i servisa
            builder.Services.AddScoped<IBookRepository>(provider => new BookRepository(connectionString));
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
            builder.Services.AddScoped<IAuthorService, AuthorService>();

            var app = builder.Build();

            // Konfiguracija middlewarea
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
