using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelterApiJo.Models;

namespace ShelterApiJo.Controllers
{
    [Route("queries")]
    [ApiController]
    public class QueriesController : ControllerBase
    {
        private readonly AnimalshelterContext _context;

        public QueriesController(AnimalshelterContext context)
        {
            _context = context;
        }

       
        [HttpGet("allSheltersWithAnimals")]
        public async Task<IActionResult> AllSheltersWithAnimals()
        {
            try
            {
                var sheltersWithAnimals = await _context.Shelters
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.City,
                        s.Capacity,
                        s.RegTime,
                        Animals = _context.Animals.Where(a => a.ShelterId == s.Id).ToList()
                    })
                    .ToListAsync();

                return Ok(new
                {
                    message = "Menhelyek és a hozzájuk tartozó állatok sikeresen lekérdezve.",
                    result = sheltersWithAnimals
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = $"Hiba történt a lekérdezés során: {ex.Message}",
                    result = (object?)null
                });
            }
        }

        
        [HttpGet("allAnimalsWithShelter")]
        public async Task<IActionResult> AllAnimalsWithShelter()
        {
            try
            {
                var result = await _context.Animals
                    .Join(
                        _context.Shelters,
                        animal => animal.ShelterId,
                        shelter => shelter.Id,
                        (animal, shelter) => new
                        {
                            animal.Id,
                            animal.Name,
                            animal.Species,
                            animal.Age,
                            animal.Weight,
                            animal.RegTime,
                            animal.ShelterId,
                            ShelterName = shelter.Name
                        })
                    .ToListAsync();

                return Ok(new
                {
                    message = "Állatok a menhely nevével sikeresen lekérdezve.",
                    result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = $"Hiba történt a lekérdezés során: {ex.Message}",
                    result = (object?)null
                });
            }
        }

   
        [HttpGet("animalsOfShelter")]
        public async Task<IActionResult> AnimalsOfShelter([FromQuery] Guid shelterId)
        {
            try
            {
                var shelter = await _context.Shelters
                    .Where(s => s.Id == shelterId)
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.City,
                        s.Capacity,
                        s.RegTime,
                        Animals = _context.Animals.Where(a => a.ShelterId == s.Id).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (shelter == null)
                {
                    return NotFound(new
                    {
                        message = "A megadott azonosítóval nem található menhely.",
                        result = (object?)null
                    });
                }

                return Ok(new
                {
                    message = "A menhely adatai és állatai sikeresen lekérdezve.",
                    result = shelter
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = $"Hiba történt a lekérdezés során: {ex.Message}",
                    result = (object?)null
                });
            }
        }

       
        [HttpGet("animalCountByShelter")]
        public async Task<IActionResult> AnimalCountByShelter()
        {
            try
            {
                var result = await _context.Shelters
                    .Select(s => new
                    {
                        ShelterId = s.Id,
                        ShelterName = s.Name,
                        City = s.City,
                        AnimalCount = _context.Animals.Count(a => a.ShelterId == s.Id)
                    })
                    .OrderByDescending(x => x.AnimalCount)
                    .ToListAsync();

                return Ok(new
                {
                    message = "Menhelyenkénti állatszám sikeresen lekérdezve.",
                    result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = $"Hiba történt a lekérdezés során: {ex.Message}",
                    result = (object?)null
                });
            }
        }
    }
}
