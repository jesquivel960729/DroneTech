using System;
using System.Globalization;
using FluentValidation;

namespace DroneTech.Models
{
    public enum Status
    {
        INACTIVO, CARGANDO, CARGADO, ENTREGANDO_CARGA, CARGA_ENTREGADA, REGRESANDO
    }

    public enum Model
    {
        peso_ligero, peso_medio, peso_crucero, peso_pesado
    }
    public class Drone
    {
        public string NumberOfSerie { get; set; }
        public float Weigth { get; set; }
        public Model Model { get; set; }
        public int Batery { get; set;}
        public Status Status{ get; set; }
        public List<Medicine> Medicines{ get; set; } = new List<Medicine>();
    }
    
    public class DroneValidator : AbstractValidator<Drone> 
    {
        public DroneValidator() 
        {
            RuleFor(x => x.NumberOfSerie).NotNull().Length(1, 100);
            RuleFor(x => x.Medicines).NotNull();
            RuleFor(x => x.Weigth).NotNull().InclusiveBetween(0,500);
            RuleFor(x => x.Medicines.Sum(a => a.Weigth)).LessThanOrEqualTo(x => x.Weigth);
            RuleFor(x => x.Batery).NotNull().InclusiveBetween(0,100);
            RuleFor(x => x.Status).Equal(Status.CARGANDO).When(x => x.Batery <= 25);
        }
    }

}