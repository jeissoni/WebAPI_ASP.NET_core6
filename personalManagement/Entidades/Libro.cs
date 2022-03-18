namespace personalManagement.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }    
        public int AutorId { get; set; }

        //propiedad de gavegacion
        public Autor Autor { get; set; }    
    }
}
