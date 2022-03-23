using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personalManagement.Entidades;
using personalManagement.Servicios;

namespace personalManagement.Controllers
{
    [ApiController]
    [Route("api/autores")] //es mejor asi, si cambia el nombre de la clase no cambia la ruta
    //[Route("api/[controller]")] --> //toma el nombre del controlador
    public class AutoresCotrollers : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IServicio servicio;
        private readonly ServicioScoped _servicioScoped;
        private readonly ServicioSingleton _servicioSingleton;
        private readonly ServicioTransient _servicioTransient;
        private readonly ILogger<AutoresCotrollers> logger;

        public AutoresCotrollers(
            ApplicationDbContext context,
            IServicio servicio, 
            ServicioScoped servicioScoped, 
            ServicioSingleton servicioSingleton, 
            ServicioTransient servicioTransient,
            ILogger<AutoresCotrollers> logger //log, el parametro es para saber desde donde se genera el log
            ) 
        {
            _context = context;
            this.servicio = servicio;
            _servicioScoped = servicioScoped;
            _servicioSingleton = servicioSingleton;
            _servicioTransient = servicioTransient;
            this.logger = logger;
        }

        [HttpGet("GUID")]
        public ActionResult ObtenerGuids() 
        {
            return Ok(new
                {
                AutoresCotrollers_Transient = _servicioTransient.Guid,               
                servicioA_transient = servicio.OptenerTransient(),

                AutoresCotrollers_Scoped = _servicioScoped.Guid,                
                servicioA_scoped = servicio.OptenerScoped(),

                AutoresCotrollers_Singleton = _servicioSingleton.Guid,
                servicioA_singleton = servicio.OptenerSingleton()
            });
        }



        //varias rutas para llegar al mismo recurso (REGLAS DE RUTEO)
        [HttpGet] //api/autores
        [HttpGet("listado")] //api/autores/listado
        [HttpGet("/listado")] //listado
        public async Task<ActionResult<List<Autor>>> Get() 
        {
            logger.LogInformation("Estamos opteniendo los autores");
            servicio.RealizarTarea();
            return await _context.Autores.Include(x=>x.Libros).ToListAsync();
        }

        //api/autores/primero
        //end pint
        [HttpGet("primero")]
        public async Task<ActionResult<Autor>> PrimerAutor() 
        {
            return await _context.Autores.FirstOrDefaultAsync();
        }


        //VARIABLES DE RUTA
        //el mismo nombre de la variable 
        //se envia como parametro en el end point
        //restricciones de tipos 
        //se mandan dos parametros separados por "/"
        //simbolo "?" es para que un parametro sea opcional {param2?}
        //si se pone un "=" despues del parametro, se asigna un valor por defecto 
        [HttpGet("{id:int}/{param2=persona}")] 
        
        public async Task<ActionResult<Autor>> Get(int id, string param2)
        {

            var autor = await _context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null) 
            {
                return NotFound();
            }

            return autor;
        }
        //llegar por parametro string //api/autores/Get?otroNombre=ejemplo&apellido=otroApellido
        [HttpGet("{nombre}")]
        public async Task<ActionResult<Autor>> Get([FromHeader] string nombre , [FromQuery] string otroNombre)
        //especificamos por donde viene el parametro [FromRoute] -- por la ruta
        //FromBody -- cuerpo de la peticion 
        //FromHeader -- cabecera de la peticion
        {
            var autor = await _context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));
            if (autor == null)
            {
                return NotFound();
            }

            return autor;
        }



        [HttpPost]
        //ActionResult define exactamente lo que se quiere retornar 
        //ActionResult<T>
        public async Task<ActionResult> Post(Autor autor) 
        {
            //una validacion para saber si existe 
            var existeAutorConElMismoNombre = await _context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);
            if (existeAutorConElMismoNombre) { 
            return BadRequest($"ya existe un autor con el mismo nombre {autor.Nombre}");
            }

            _context.Add(autor);
            await _context.SaveChangesAsync();
            
            // dentro del ok puedo retornar cualquier cosa 
            return Ok();
        }

        [HttpPut("{id:int}")] // api/autores/{entero}
        public async Task<ActionResult> Put(Autor autor, int id) 
        {
            bool exists = await _context.Autores.AnyAsync(x => x.Id == id);

            if (!exists)
            {
                return NotFound();
            }


            if (autor.Id != id) 
            {
                return BadRequest("El Id del autorn no coincide con el id de la URL");
            }
            _context.Update(autor);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.Autores.AnyAsync(x => x.Id == id);

            if (!exists)
            {
                return NotFound();
            }

            _context.Remove(new Autor() { Id = id });
            await _context.SaveChangesAsync();
            return Ok();

        }
    }
}
