//using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DroneTech.Models;
using FluentValidation;
using FluentValidation.Results;

namespace DroneTech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DroneController: ControllerBase
    {
        private IValidator<Drone> _validator;

        public DroneController(IValidator<Drone> validator) 
        {
            _validator = validator;
        }
        private static List<Drone> drons = new List<Drone> {
            new Drone { NumberOfSerie = "Raptor01", Weigth = 500, Model = Model.peso_pesado, Batery = 100, Status = Status.INACTIVO },
            new Drone { NumberOfSerie = "Raptor02", Weigth = 450, Model = Model.peso_crucero, Batery = 20, Status = Status.CARGANDO },
            new Drone { NumberOfSerie = "Raptor03", Weigth = 400, Model = Model.peso_medio, Batery = 100, Status = Status.INACTIVO }
        };

        [HttpGet]
        public ActionResult<List<Drone>> Get()
        {
            return Ok(drons);
        }

        [HttpGet]
        [Route("GetAvailable")]
        public ActionResult<List<Drone>> GetAvailable()
        {
            var drones = drons.FindAll(x => x.Status == Status.INACTIVO);
            return Ok(drones);
        }

        [HttpGet]
        [Route("{NumberOfSerie}")]
        public ActionResult<Drone> Get(string NumberOfSerie)
        {
            var dron = drons.Find(x => x.NumberOfSerie == NumberOfSerie);
            return dron == null ? NotFound() : Ok(dron);
        }

        [HttpGet]
        [Route("GetBattery/{NumberOfSerie}")]
        public ActionResult<Drone> GetBattery(string NumberOfSerie)
        {
            var dron = drons.Find(x => x.NumberOfSerie == NumberOfSerie);
            return dron == null ? NotFound() : Ok(dron.Batery);
        }

        [HttpGet]
        [Route("GetWeigth/{NumberOfSerie}")]
        public ActionResult<Drone> GetWeigth(string NumberOfSerie)
        {
            var dron = drons.Find(x => x.NumberOfSerie == NumberOfSerie);
            return dron == null ? NotFound() : Ok(dron.Medicines.Sum(item => item.Weigth));
        }

        [HttpPost]
        public ActionResult Post(Drone dron)
        {
            var existDron = drons.Find(x => x.NumberOfSerie == dron.NumberOfSerie);
            if (existDron != null)
            {
                return Conflict("Ya existe un dron con este NumeberOfSerie");
            }
            else
            {
                ValidationResult result = _validator.Validate(dron);
                if(!result.IsValid){
                   return BadRequest(result.Errors);
                }
                drons.Add(dron);
                var resourceUrl = Request.Path.ToString() + '/' + dron.NumberOfSerie;
                return Created(resourceUrl, dron);
            }
        }

        [HttpPut]
        public ActionResult Put(Drone dron)
        {
            var existDrone = drons.Find(x => x.NumberOfSerie == dron.NumberOfSerie);
            if (existDrone == null)
            {
                return BadRequest("Cannot update a nont existing term.");
            } 
            else
            {
                ValidationResult result = _validator.Validate(dron);
                if(!result.IsValid){
                   return BadRequest(result.Errors);
                }
                existDrone.NumberOfSerie = dron.NumberOfSerie;
                existDrone.Weigth = dron.Weigth;
                existDrone.Model = dron.Model;
                existDrone.Batery = dron.Batery;
                existDrone.Status = dron.Status;
                if(dron.Medicines != null) existDrone.Medicines = dron.Medicines;
                else existDrone.Medicines = new List<Medicine>();
                return Ok();
            }
        }
        
        [HttpPut]
        [Route("PutMedicine/{NumberOfSerie}")]
        public ActionResult PutMedicine(string NumberOfSerie, List<Medicine> Medicines)
        {
            var existDrone = drons.Find(x => x.NumberOfSerie == NumberOfSerie);
            if (existDrone == null)
            {
                return BadRequest("No existe el dron " + NumberOfSerie);
            } 
            else
            {
                if(existDrone.Status == Status.INACTIVO || existDrone.Status == Status.CARGADO){
                    var oldInventary = existDrone.Medicines;
                    existDrone.Medicines.AddRange(Medicines);
                    
                    ValidationResult result = _validator.Validate(existDrone);
                    if(!result.IsValid){
                        existDrone.Medicines = oldInventary;
                        return BadRequest(result.Errors);
                    }
                    //update
                    return Ok();
                }  
                else{
                    return BadRequest("Este dron no puede cargar medicinas.");
                }                
            }
        }


        [HttpDelete]
        [Route("{NumberOfSerie}")]
        public ActionResult Delete(string NumberOfSerie)
        {
            var dron = drons.Find(x => x.NumberOfSerie == NumberOfSerie);
            if (dron == null)
            {
                return NotFound();
            }
            else
            {
                drons.Remove(dron);
                return NoContent();
            }
        }
    }
}