using Autofac;
using Autofac.Extensions.DependencyInjection;
using BootcampApp.Repository;
using BootcampApp.Service;
using BootcampApp.Service.Common;
using BootcampApp.Repository.Common;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ? Kaži ASP.NET Core-u da koristi Autofac kao DI container
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // ?? Registracija modula/servisa se radi u ConfigureContainer
            builder.Host.ConfigureContainer<ContainerBuilder>(container =>
            {
                // Repozitoriji
                container.Register(c => new BookRepository(connectionString)).As<IBookRepository>().InstancePerLifetimeScope();
                container.RegisterType<AuthorRepository>().As<IAuthorRepository>().InstancePerLifetimeScope();

                // Servisi
                container.RegisterType<BookService>().As<IBookService>().InstancePerLifetimeScope();
                container.RegisterType<AuthorService>().As<IAuthorService>().InstancePerLifetimeScope();
            });

            // Ostale ASP.NET usluge
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Middleware
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
