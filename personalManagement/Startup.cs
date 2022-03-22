using Microsoft.EntityFrameworkCore;
using personalManagement.Controllers;
using personalManagement.Servicios;
using System.Text.Json.Serialization;

namespace personalManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //aca configuramos los serivicios
        //los servicion son la resolucion de una dependencia configurada 
        //en el sistema de inyeccion de dependencias
        //resuleva toda las dependencias de nuestras clases 
        public void ConfigureServices(IServiceCollection services) 
        {
            //instruccion para que no haga un ciclo infinito al cargar 
            //las llaves foraneas
            services.AddControllers().AddJsonOptions(x => 
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


            //se encarga de configurar la aplicacion dbContext como un servicio 
            //cada vez que el ApplicationDbContext aparezca como una dependencia de una clase resuleva esto
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
                Configuration.GetConnectionString("DefaultConnection")));

            //cuando una clase requiera un Iservicio, pasarle una instancia del la clase ServicioA
            //services.AddTransient<IServicio, ServicioA>();

            //configurando una clase como un servicio
            //services.AddTransient<ServicioA>();

            //se nos da una nueva instancia de la clase
            //bueno para funciones que ejecuten y ya, sin mantener datos para reutilizar 
            //ejecutar funciones y retornar datos -- buen candidato para un servicio trancitorio
            //services.AddTransient


            //diferente instancia por peticion que se haga
            //no se comparte la instancia por peticion
            // --> muy bueno para los dbContext, siempre con los mismos datos
            
            //services.AddScoped<IServicio, ServicioB>();


            //siempre la misma instancia para todas las peticiones 
            //si se hace una peticion 
            //--> para una cache, mantener una informacion en memoria 

            services.AddSingleton<IServicio, ServicioB>();

            services.AddTransient<ServicioTransient>();
            services.AddScoped<ServicioScoped>();
            services.AddSingleton<ServicioSingleton>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) 
        {

            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { 
                endpoints.MapControllers(); 
            });

        }
    }
}
